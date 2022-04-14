using System.ComponentModel.DataAnnotations;
using FirstProject.Models.Abstarct;
using FirstProject.Models.Enums;

namespace FirstProject.Models;

public class StudentModel: Person
{
    
    [Required]
    [Display(Name = "Semester Number")]
    public SemesterNumberEnum SemesterNumber { get; set; }
    
    [Required]
    [Display(Name = "Register Date")]
    [DataType(DataType.Date)]
    public DateTime RegisterDate { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; }
    
    // Many to many relation with Courses
    public ICollection<Course> Courses { get; set; }
}