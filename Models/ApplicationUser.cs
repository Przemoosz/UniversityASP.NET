using FirstProject.Models.Abstarct;
using Microsoft.AspNetCore.Identity;

namespace FirstProject.Models;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public IEnumerable<Person> Persons { get; set; }
    
    public IEnumerable<Message>? Messages { get; set; }
    
    public IEnumerable<MessageBox>? MessageBoxes { get; set; }
}