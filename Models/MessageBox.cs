namespace FirstProject.Models;

public class MessageBox
{
    public int MessageBoxID { get; set; }
    
    public IEnumerable<ApplicationUser> Participants { get; set; }
    
    public IEnumerable<Message> Messages { get; set; }
}