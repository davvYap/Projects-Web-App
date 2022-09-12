using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialAppDemo.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        // specify the foreign key:
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Transaction amount cannot be empty.")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0.")]
        public double Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Details { get; set; }

        [Required(ErrorMessage = "Please enter transaction date.")]
        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryWithIcon
        {
            get { return Category == null ? "" : $"{Category.Icon} {Category.Title}"; }
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get { return $"{((Category == null || Category.Type == "Expense") ? '-' : '+')} {Amount}"; }
        }

        [NotMapped]
        public string? FormattedDate
        {
            get { return this.Date.ToString("dd-MMM-yy (ddd)"); }
        }

    }
}
