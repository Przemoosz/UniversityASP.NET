using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models;

public class Course
{
    public int CourseID { get; set; }
    
    [Required]
    [Display(Name = "Course Name")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Course name can not be longer than 255 letters!")]
    public string CourseName { get; set; }
    
    [Required]
    [Range(5,200,ErrorMessage = "Total students number can not be higher than 200 and lower than 5!")]
    public int TotalStudents { get; set; }
    
    
    
}