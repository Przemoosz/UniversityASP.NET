using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models;

public class Transaction
{
    public int TransactionID { get; set; }
    
    [Required]
    [Display(Name = "Transaction Name")]
    [StringLength(100,MinimumLength = 3)]
    public string TransactionName { get; set; }
    
    [Required]
    public TransactionGroupEnum Group { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime TransactionDate { get; set; }
    
    public int UniversityID { get; set; }
    
    public University University { get; set; }
}