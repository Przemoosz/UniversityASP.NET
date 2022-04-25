using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirstProject.Models.ViewModels;

public class PermissionViewModel
{
    public Dictionary<string,List<string>> JsonPolicy { get; set; }
    
    public SelectList? Role { get; set; }
    
    public Dictionary<string,List<string>>? SelectedPermissions { get; set; }
}