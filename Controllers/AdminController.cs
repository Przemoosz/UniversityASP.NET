using System.Data.Common;
using FirstProject.Data;
using FirstProject.Models;
using FirstProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Controllers;

[Authorize(Policy = "RequireAdmin")]
public class AdminController: Controller
{
    private readonly RoleManager<IdentityRole> _roleContext;
    private readonly UserManager<ApplicationUser> _userContext;
    private readonly ApplicationDbContext _context;

    public AdminController(RoleManager<IdentityRole> roleContext, UserManager<ApplicationUser> usersContext, ApplicationDbContext context)
    {
        _roleContext = roleContext;
        _userContext = usersContext;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Roles()
    {
        var models = from role in _roleContext.Roles select role;
        return View(await models.ToListAsync());
    }

    [HttpGet]
    // [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Users()
    {
        // var user = _userContext.Users.Include()
        
        var users = from user in _userContext.Users select user;
        List<AdminUserDisplayModel> usersAndRoles = new List<AdminUserDisplayModel>(users.Count());
        foreach (var user in users)
        {
            var role = await _userContext.GetRolesAsync(user);
            if (role.Count == 0)
            {
                role = null;
            }
            usersAndRoles.Add(new AdminUserDisplayModel(){User = user,Role = role});
        }
        
        return View(usersAndRoles);
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpGet]
    
    public async Task<IActionResult> Delete(string? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var userToDelete = await _userContext.Users.Where(u => u.Id.Equals(id)).FirstOrDefaultAsync();
        if (userToDelete is null)
        {
            return NotFound();
        }

        var userRoles = await _userContext.GetRolesAsync(userToDelete);
        if (userRoles.Count == 0)
        {
            userRoles = null;
        }
        return View(new AdminUserDisplayModel(){User = userToDelete, Role = userRoles});
    }
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var userToDelete = await _userContext.Users.Where(i => i.Id.Equals(id)).SingleAsync();
        try
        {
            await _userContext.DeleteAsync(userToDelete);
        }
        catch (DbException ex)
        {
            ViewData["DatabaseError"] = "Database Error occured!";
            return RedirectToAction(nameof(Delete), new {id = id});
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Users));
    }
    
}