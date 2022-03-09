using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models;

public class Faculty
{
    public int FacultyID { get; set; }
    
    [Required]
    [Display(Name = "Faculty Name")]
    [StringLength(100, MinimumLength = 0)]
    public string FacultyName { get; set; }
    
    [Required]
    [Range(0,200)]
    public int Employed { get; set; }
    
    [Required]
    [DataType(DataType.Currency)]
    public decimal Budget { get; set; }
    
    [Required]
    [Display(Name = "Creation Date")]
    [DataType(DataType.Date)]
    public DateTime CreationDate { get; set; }
    
    public int UniversityID { get; set; }
    
    // One to Many with University
    public University University { get; set; }
    
    // Many to One with Transaction
    
    public ICollection<Transaction>? Transactions { get; set; }

}