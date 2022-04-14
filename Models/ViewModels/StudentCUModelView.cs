namespace FirstProject.Models.ViewModels;

public class StudentCUModelView
{
    public StudentModel StudentModel { get; set; }
    
    public IEnumerable<Course> Course { get; set; }
}