using FinancialAppDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System.Collections.Generic;

namespace FinancialAppDemo.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // to get last 7 days transactions

            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            ViewBag.StartDate = StartDate.ToString("dd-MMM-yy (ddd)");
            ViewBag.EndDate = EndDate.ToString("dd-MMM-yy (ddd)");

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();


            // total income
            double TotalIncome = SelectedTransactions
                .Where(a => a.Category.Type == "Income")
                .Sum(b => b.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C2");

            // total expense
            double TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C2");

            // balance
            double Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C2");

            // Data for Doughnut chart - Expense by category
            ViewBag.DoughnutChart = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Category.CategoryId)
                .Select(z => new
                {
                    categoryTitleWithIcon = z.First().CategoryWithIcon,
                    amount = z.Sum(y => y.Amount),
                    formattedAmount = z.Sum(y => y.Amount).ToString("C0")
                })
                .OrderByDescending(l=>l.amount)
                .ToList();

            // Data for Dashline chart - Income vs Expense
            // Income
            List<LineChartData> IncomeSummary = SelectedTransactions
                .Where(x => x.Category.Type == "Income")
                .GroupBy(y => y.Date)
                .Select(k => new LineChartData()
                {
                    //Day = k.First().Date.ToString("dd-MMM") + ' ' + k.First().Date.DayOfWeek.ToString(),
                    Day = k.First().Date.ToString("dd-MMM (ddd)"),
                    Income = k.Sum(l => l.Amount)
                })
                .ToList();

            // Expense
            List<LineChartData> ExpenseSummary = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Date)
                .Select(k => new LineChartData()
                {
                    //Day = k.First().Date.ToString("dd-MMM") + ' ' + k.First().Date.DayOfWeek.ToString(),
                    Day = k.First().Date.ToString("dd-MMM (ddd)"),
                    Expense = k.Sum(l => l.Amount)
                })
                .ToList();

            //Combine both income and expense as an object
            string[] Last7Days = new string[7];
            for (int i = 0; i < 7; i++)
            {
                Last7Days[i] = StartDate.AddDays(i).ToString("dd-MMM") + ' ' + StartDate.AddDays(i).DayOfWeek.ToString();
                Last7Days[i] = StartDate.AddDays(i).ToString("dd-MMM (ddd)");
            };
            // LinQ method:
            //string[] last7Days = Enumerable.Range(0, 7)
            //    .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
            //    .ToArray();

            //LinQ
            ViewBag.LineChartData = from day in Last7Days
                                    join Income in IncomeSummary on day equals Income.Day into dayIncomeJoined
                                    from Income in dayIncomeJoined.DefaultIfEmpty()
                                    join Expense in ExpenseSummary on day equals Expense.Day into expenseJoined
                                    from Expense in expenseJoined.DefaultIfEmpty()
                                    select new
                                    {
                                        Day = day,
                                        Income = Income == null ? 0 : Income.Income,
                                        Expense = Expense == null ? 0 : Expense.Expense,
                                    };

            // Recent Transaction
            List<Transaction> RecentList = new List<Transaction>();
            var TotalTransaction = await _context.Transactions.OrderByDescending(x => x.Date).ToListAsync();
            for (int i = 0; i < 6; i++) 
            {
                RecentList.Add(TotalTransaction[i]);
            }
            ViewBag.RecentTransaction = RecentList;

            // Highest transaction (using Linq)
            ViewBag.HighestTransaction = await _context.Transactions
                .Where(y => y.Category.Type == "Expense" && y.Date >= StartDate && y.Date <= EndDate)
                .OrderByDescending(x => x.Amount)
                .Include(x => x.Category)
                .Take(6)
                .ToListAsync();

            return View(new DashboardViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(DashboardViewModel dashboardViewModel)
        {
            // to get last 7 days transactions
            //DateTime StartDate = DateTime.Today.AddDays(-6);
            //DateTime EndDate = DateTime.Today;

            DateTime StartDate = dashboardViewModel.StartDate;
            DateTime EndDate = dashboardViewModel.EndDate;

            ViewBag.StartDate = StartDate.ToString("dd-MMM-yy (ddd)");
            ViewBag.EndDate = EndDate.ToString("dd-MMM-yy (ddd)");

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();


            // total income
            double TotalIncome = SelectedTransactions
                .Where(a => a.Category.Type == "Income")
                .Sum(b => b.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C2");

            // total expense
            double TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C2");

            // balance
            double Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C2");

            // Data for Doughnut chart - Expense by category
            ViewBag.DoughnutChart = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Category.CategoryId)
                .Select(z => new
                {
                    categoryTitleWithIcon = z.First().CategoryWithIcon,
                    amount = z.Sum(y => y.Amount),
                    formattedAmount = z.Sum(y => y.Amount).ToString("C0")
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            // Data for Dashline chart - Income vs Expense
            // Income
            List<LineChartData> IncomeSummary = SelectedTransactions
                .Where(x => x.Category.Type == "Income")
                .GroupBy(y => y.Date)
                .Select(k => new LineChartData()
                {
                    //Day = k.First().Date.ToString("dd-MMM") + ' ' + k.First().Date.DayOfWeek.ToString(),
                    Day = k.First().Date.ToString("dd-MMM (ddd)"),
                    Income = k.Sum(l => l.Amount)
                })
                .ToList();

            // Expense
            List<LineChartData> ExpenseSummary = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Date)
                .Select(k => new LineChartData()
                {
                    //Day = k.First().Date.ToString("dd-MMM") + ' ' + k.First().Date.DayOfWeek.ToString(),
                    Day = k.First().Date.ToString("dd-MMM (ddd)"),
                    Expense = k.Sum(l => l.Amount)
                })
                .ToList();

            //Combine both income and expense as an object
            string[] Last7Days = new string[7];
            for (int i = 0; i < 7; i++)
            {
                Last7Days[i] = StartDate.AddDays(i).ToString("dd-MMM (ddd)");
            };
            // LinQ method:
            //string[] last7Days = Enumerable.Range(0, 7)
            //    .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
            //    .ToArray();

            //LinQ
            ViewBag.LineChartData = from day in Last7Days
                                    join Income in IncomeSummary on day equals Income.Day into dayIncomeJoined
                                    from Income in dayIncomeJoined.DefaultIfEmpty()
                                    join Expense in ExpenseSummary on day equals Expense.Day into expenseJoined
                                    from Expense in expenseJoined.DefaultIfEmpty()
                                    select new
                                    {
                                        Day = day,
                                        Income = Income == null ? 0 : Income.Income,
                                        Expense = Expense == null ? 0 : Expense.Expense,
                                    };

            // Recent Transaction
            List<Transaction> RecentList = new List<Transaction>();
            var TotalTransaction = await _context.Transactions.OrderByDescending(x => x.Date).ToListAsync();
            for (int i = 0; i < 6; i++)
            {
                RecentList.Add(TotalTransaction[i]);
            }
            ViewBag.RecentTransaction = RecentList;

            // Highest transaction (using Linq)
            ViewBag.HighestTransaction = await _context.Transactions
                .Where(y => y.Category.Type == "Expense" && y.Date >= StartDate && y.Date <= EndDate)
                .OrderByDescending(x => x.Amount)
                .Include(x => x.Category)
                .Take(6)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> More(DashboardViewModel dashboardViewModel)
        {

            DateTime StartDate = dashboardViewModel.StartDate;
            DateTime EndDate = dashboardViewModel.EndDate;
            int TotalDays = (EndDate - StartDate).Days + 1;

            ViewBag.StartDate = StartDate.ToString("dd-MMM-yy (ddd)");
            ViewBag.EndDate = EndDate.ToString("dd-MMM-yy (ddd)");

            DateTime StartDateMonth = DateTime.Today.AddDays(-150);
            DateTime EndDateMonth = DateTime.Today;

            int StartMonth = StartDateMonth.Month;
            int EndMonth = EndDateMonth.Month;

            ViewBag.StartMonth = StartDateMonth.ToString("MMM-yy");
            ViewBag.EndMonth = EndDateMonth.ToString("MMM-yy");

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            List<Transaction> TotalTransaction = await _context.Transactions
                .Include(x => x.Category)
                .ToListAsync();


            List<Transaction> SelectedTransactionMonth = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date.Month >= StartMonth && y.Date.Month <= EndMonth)
                .ToListAsync();

            double totalIncome = TotalTransaction
                .Where(y => y.Category.Type == "Income")
                .Sum(x => x.Amount);

            ViewBag.TotalIncomeAllTheTime = totalIncome;

            double BalanceRemaining = 0;

            // total income
            double TotalIncome = SelectedTransactions
                .Where(a => a.Category.Type == "Income")
                .Sum(b => b.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C2");

            // total expense
            double TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C2");

            // balance
            double Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C2");

            // Data for Doughnut chart - Expense by category
            ViewBag.DoughnutChartExpense = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Category.CategoryId)
                .Select(z => new
                {
                    categoryTitleWithIcon = z.First().CategoryWithIcon,
                    amount = z.Sum(y => y.Amount),
                    formattedAmount = z.Sum(y => y.Amount).ToString("C0")
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            // Data for Doughnut chart - Income by category
            ViewBag.DoughnutChartIncome = SelectedTransactions
                .Where(x => x.Category.Type == "Income")
                .GroupBy(y => y.Category.CategoryId)
                .Select(z => new
                {
                    categoryTitleWithIcon = z.First().CategoryWithIcon,
                    amount = z.Sum(y => y.Amount),
                    formattedAmount = z.Sum(y => y.Amount).ToString("C0")
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            // Data for Dashline chart - Income vs Expense
            // Income to list
            List<LineChartData> IncomeSummary = SelectedTransactions
                .Where(x => x.Category.Type == "Income")
                .GroupBy(y => y.Date)
                .Select(k => new LineChartData()
                {
                    //Day = k.First().Date.ToString("dd-MMM") + ' ' + k.First().Date.DayOfWeek.ToString(),
                    Day = k.First().Date.ToString("dd-MMM (ddd)"),
                    Income = k.Sum(l => l.Amount),
                })
                .ToList();

            // Expense to list
            List<LineChartData> ExpenseSummary = SelectedTransactions
                .Where(x => x.Category.Type == "Expense")
                .GroupBy(y => y.Date)
                .Select(k => new LineChartData()
                {
                    //Day = k.First().Date.ToString("dd-MMM") + ' ' + k.First().Date.DayOfWeek.ToString(),
                    Day = k.First().Date.ToString("dd-MMM (ddd)"),
                    Expense = k.Sum(l => l.Amount),
                })
                .ToList();

            // Balance to list
            List<LineChartData> BalanceSummary = SelectedTransactions
                .GroupBy(x => x.Date)
                .Select(k => new LineChartData()
                {
                    Day = k.First().Date.ToString("dd-MMM (ddd)"),
                    DailyBalance = totalIncome -= k.Sum(l => l.Amount)
                })
                .ToList();

            // Bar Chart
            List<BarCharData> MonthExpenseBarChart = SelectedTransactionMonth
                .Where(y => y.Category.Type == "Expense")
                .GroupBy(x => x.Date.Month)
                .Select(k => new BarCharData()
                {
                    Month = k.First().Date.Month.ToString(),
                    Amount = k.Sum(l => l.Amount)
                })
                .OrderBy(b => b.Month)
                .ToList();
            ViewBag.ExpenseBarChart = MonthExpenseBarChart;

            ViewBag.AverageExpenseInfo = MonthExpenseBarChart.Average(l => l.Amount).ToString("C0");

            List<BarCharData> MonthIncomeBarChart = SelectedTransactionMonth
                .Where(y => y.Category.Type == "Income")
                .GroupBy(x => x.Date.Month)
                .Select(k => new BarCharData()
                {
                    Month = k.First().Date.Month.ToString(),
                    Amount = k.Sum(l => l.Amount)
                })
                .OrderBy(b => b.Month)
                .ToList();
            ViewBag.IncomeBarChart = MonthIncomeBarChart;

            ViewBag.AverageIncomeInfo = MonthIncomeBarChart.Average(l => l.Amount).ToString("C0");


            // Calculation of total days
            string[] AllSelectedDays = new string[TotalDays];
            for (int i = 0; i < TotalDays; i++)
            {
                AllSelectedDays[i] = StartDate.AddDays(i).ToString("dd-MMM (ddd)");
            };
            // LinQ method:
            //string[] last7Days = Enumerable.Range(0, 7)
            //    .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
            //    .ToArray();


            //LinQ
            ViewBag.LineChartData = from day in AllSelectedDays
                                    join Income in IncomeSummary on day equals Income.Day into dayIncomeJoined
                                    from Income in dayIncomeJoined.DefaultIfEmpty()
                                    join Expense in ExpenseSummary on day equals Expense.Day into expenseJoined
                                    from Expense in expenseJoined.DefaultIfEmpty()
                                    join DailyBalance in BalanceSummary on day equals DailyBalance.Day into balanceJoined
                                    from DailyBalance in balanceJoined.DefaultIfEmpty()
                                    select new
                                    {
                                        Day = day,
                                        Income = Income == null ? 0 : Income.Income,
                                        Expense = Expense == null ? 0 : Expense.Expense,
                                        TotalBalance = DailyBalance == null ? 0 : DailyBalance.DailyBalance
                                    };

            return View(new DashboardViewModel());
        }
    }   

    public class LineChartData
    {
        public string Day;
        public double Income;
        public double Expense;
        public double DailyBalance;
    }

    public class BalanceChartData
    {
        public string Day;  // x-value
        public double Balance; // y-value
    }

    public class BarCharData 
    {
        public string Month;
        public double Amount;
    }

}
