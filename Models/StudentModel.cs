using System.ComponentModel.DataAnnotations;
using FirstProject.Models.Abstarct;

namespace FirstProject.Models;

public class StudentModel: Person
{
    
    [Required]
    [Display(Name = "Semester Number")]
    public SemesterNumberEnum SemesterNumber { get; set; }
    
    [Required]
    [Display(Name = "Register Date")]
    [DataType(DataType.DateTime)]
    public DateTime RegisterDate { get; set; }
    
    // One to many relation with Courses
    public static ICollection<Course> Courses { get; set; }
}