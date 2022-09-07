using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialAppDemo.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [Column(TypeName ="nvarchar(50)")]
        public string Title { get; set; }
        [Required(ErrorMessage ="Title cannot be null.")]
        [Column(TypeName = "nvarchar(10)")]
        public string Icon { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Expense";

        [NotMapped]
        public string TitleWithIcon 
        {
            get { return this.Icon + " " + this.Title; }
        }

        [NotMapped]
        public string TitleWithIconWithType
        {
            get { return GetExpensesType(); }
        }

        public string GetExpensesType()
        {
            if (this.Type == "Income")
            {
                //return this.Icon + " " + this.Title + " " + "|" + "💲" + this.Type;
                return $"{this.Icon} {this.Title} | \t 💲 {this.Type}";
            }
            else
            {
                return $"{this.Icon} {this.Title} | \t ✂️ {this.Type}";
            }
        }

    }
}
