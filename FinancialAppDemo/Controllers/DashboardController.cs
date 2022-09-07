using FinancialAppDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialAppDemo.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // to get last 7 days transactions
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

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

            return View();
        }
    }

    public class LineChartData
    {
        public string Day;
        public double Income;
        public double Expense;
    }
}
