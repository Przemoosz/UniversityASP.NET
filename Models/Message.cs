using System.ComponentModel.DataAnnotations;

namespace FirstProject.Models;

public class Message
{
    public int MessageID { get; set; }
    
    public ApplicationUser User { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    
    public string Description { get; set; }
    
    public int MessageBoxID { get; set; }
    
    public MessageBox MessageBox { get; set; }
}