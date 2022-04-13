using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models.Abstarct;

public class Person
{
    public int ID { get; set; }
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Required]
    public GenderEnum Gender { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateTime DateOfBirth { get; set; }
    
    [Display(Name="Full Name")]
    public string FullName
    {
        get => FirstName + " " + LastName;
    }
}