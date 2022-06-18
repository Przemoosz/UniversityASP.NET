namespace FirstProject.Models;

public class InformationBox
{
    public int InformationBoxId { get; set; }
    
    public ApplicationUser ApplicationUser { get; set; }
    
    public IEnumerable<Information> Informations { get; set; }
    
}