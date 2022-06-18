using FirstProject.Data;
using FirstProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Controllers;

[Authorize(Policy = "RequireAdmin")]
public class InformationController: Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleContext;
    private readonly UserManager<ApplicationUser> _userContext;

    public InformationController(ApplicationDbContext context, RoleManager<IdentityRole> roleContext, UserManager<ApplicationUser> userContext)
    {
        _context = context;
        _userContext = userContext;
        _roleContext = roleContext;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var roleQuery = from role in _roleContext.Roles select role;
        IEnumerable<IdentityRole> roles = await roleQuery.ToListAsync();
        ViewBag.Roles = roles;
        // SelectList roleSelectList = new SelectList(roles);
        // ViewBag.roleSelectList = roleSelectList;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title, Description")] Information information, string[] selectedRoles)
    {
        if (ModelState.IsValid)
        {
            
        }
        return NotFound();
    }
}