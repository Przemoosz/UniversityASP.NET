using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models.ViewModels
{
    public class ChooseUniversityModelView
    {
        [Display(Name = "University Name")]
        public string UniversityName { get; set; }
        
        public int Employed { get; set; }
        
        [Display(Name = "Total Faculties")]
        public int FacultiesCount { get; set; }
    }
}

