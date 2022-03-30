using Microsoft.AspNetCore.Identity;

namespace FirstProject.Models.ViewModels;

public class AdminUserDisplayModel
{
    public ApplicationUser User { get; set; }
    public IEnumerable<string>? Role { get; set; }
    
}