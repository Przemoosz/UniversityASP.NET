using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FirstProject.Data;
using FirstProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Areas.Identity.Pages.Account.Manage;

public class AttachedPerson : PageModel
{
    [TempData]
    public string StatusMessage { get; set; }

    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userContext;
    public AttachedPerson(ApplicationDbContext context, UserManager<ApplicationUser> usersContext)
    {
        _context = context;
        _userContext = usersContext;
    }
    
    public ApplicationUser ApplicationUser { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        string? username = HttpContext.User.Identity.Name;
        if (username is null)
        {
            return NotFound();
        }
        var userObject = await _userContext.Users.Include(u=> u.Persons).FirstOrDefaultAsync(u => u.UserName.Equals(username));
        if (userObject is null)
        {
            return NotFound();
        }
        ApplicationUser = userObject;
        return Page();
    }
}