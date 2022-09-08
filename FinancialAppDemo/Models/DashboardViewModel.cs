using System.ComponentModel.DataAnnotations;


namespace FinancialAppDemo.Models
{
    public class DashboardViewModel
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-6);

        public DateTime EndDate { get; set; } = DateTime.Today;
    }
}
