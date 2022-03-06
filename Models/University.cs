using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Models;

// [Validator]
public class University
{
    public int UniversityID { get; set; }
    
    [Required]
    [Display(Name ="University Name")]
    [StringLength(100,MinimumLength = 0)]
    // [Index("UniveristyName", IsUnique = true)]
    // [Remote("IsExists","Place", ErrorMessage = "Name exists")]
    public string UniversityName { get; set; }
    
    [DataType(DataType.Date)]
    [Display(Name = "Creation Date")]
    public DateTime CreationDate { get; set; }
    
    [Range(0,1000)]
    public int Employed { get; set; }
    
    [Required]
    [StringLength(150)]
    public string Adress { get; set; }

    public ICollection<Faculty>? Faculties { get; set; }
}