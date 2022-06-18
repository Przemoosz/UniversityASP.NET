using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models;

public class Information
{
    public int InformationId { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    
    [MaxLength(150)]
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public int Views { get; set; }
    
    public IEnumerable<InformationBox> InformationBoxes { get; set; }
}