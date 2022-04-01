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
    public async Task<IActionResult> Details(string id, bool success=false, bool idError=false)
    {
        var selectedUser = await _userContext.Users.Where(u => u.Id.Equals(id)).FirstOrDefaultAsync();
        if (selectedUser is null)
        {
            return NotFound();
        }

        var attachedRole = await _userContext.GetRolesAsync(selectedUser);
        var allRoles = from role in _roleContext.Roles select role;
        List<AttachedRolesData> attachedRolesModel = new List<AttachedRolesData>();
        foreach (var role in allRoles)
        {
            if (attachedRole.Contains(role.Name))
            {
                attachedRolesModel.Add(new AttachedRolesData(){Name = role.Name,Attached = true});
            }
            else
            {
                attachedRolesModel.Add(new AttachedRolesData(){Name = role.Name,Attached = false});
            }
            // Console.WriteLine(role.Name);
        }

        if (idError)
        {
            ViewData["Error"] = "You are trying to edit user with id that does not exists!";
        }
        if(success) ViewData["Success"] = "User updated successfully!";
        
        ViewBag.Roles = attachedRolesModel;
        return View(new AdminUserDisplayModel(){User = selectedUser,Role = attachedRole});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Details")]
    public async Task<IActionResult> AssignRoles(string? id, string[] selectedRoles)
    {
        if (id is null)
        {
            // That is impossible XD
            // ViewData["Error"] = "You are trying to edit user with id that does not exists!";
            return RedirectToAction(nameof(Details), new {id = id, idError=true});
        }

        var selectedUser = await _userContext.Users.Where(u => u.Id.Equals(id)).FirstOrDefaultAsync();
        var roleQuery = from role in _roleContext.Roles select role.Name;
        IEnumerable<string> allRoles = await roleQuery.ToListAsync();
        if (selectedUser is null)
        {
            // ViewData["Error"] = "You are trying to edit user with id that does not exists!";
            return RedirectToAction(nameof(Details), new {id = id, idError=true});
        }
        // if (selectedRoles.Length == 0)
        // {
        //     return RedirectToAction(nameof(Details), new {id = id});
        // }

        if (ModelState.IsValid)
        {
            var userRoles = await _userContext.GetRolesAsync(selectedUser);
            foreach (var role in selectedRoles)
            {
                if (!userRoles.Contains(role))
                {
                    await _userContext.AddToRoleAsync(selectedUser, role);
                }
            }

            foreach (var role in allRoles)
            {
                if (!selectedRoles.Contains(role) && userRoles.Contains(role))
                { 
                    await _userContext.RemoveFromRoleAsync(selectedUser, role);
                }
            }
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Details), new {id = id, success=true});
        }
        return RedirectToAction(nameof(Details), new {id = id});
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

        if (userRoles is not null && userRoles.Contains("Admin"))
        {
            return View("AccessDenied", userToDelete);
        }
        
        return View(new AdminUserDisplayModel(){User = userToDelete, Role = userRoles});
    }
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var userToDelete = await _userContext.Users.Where(i => i.Id.Equals(id)).SingleAsync();
        // Second protection against deleting admin user
        var roles = await _userContext.GetRolesAsync(userToDelete);
        if (roles is not null && roles.Contains("Admin"))
        {
           return View("AccessDenied", userToDelete);
        }
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