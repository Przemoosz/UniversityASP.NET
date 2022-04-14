using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models.ViewModels;

public class PageInfoDisplayModel
{
    [Display(Name="Total users")]
    public int UserCount { get; set; }
    
    [Display(Name="Admins count")]
    public int Admins { get; set; }
    
    [Display(Name="Users count")]
    public int Users { get; set; }
    
    [Display(Name="Total universities")]
    public int TotalUniversities { get; set; }
    
    [Display(Name="Total faculties")]
    public int TotalFaculties { get; set; }

    [Display(Name="Total transactions")]
    public int TotalTransactions { get; set; }

    [Display(Name="Total courses")]
    public int TotalCourses { get; set; }

}