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
    [Range(Double.MinValue, Double.MaxValue)]
    [DataType(DataType.Currency)]
    public double Amount { get; set; }
    
    [Required]
    public DateTime TransactionDate { get; set; }
    
    public int FacultyID { get; set; }
    
    public Faculty Faculty { get; set; }
}