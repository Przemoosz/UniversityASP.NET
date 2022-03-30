using FirstProject.Data;
using FirstProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Controllers;

public class AdminController: Controller
{
    private readonly RoleManager<IdentityRole> _roleContext;
    private readonly UserManager<ApplicationUser> _userContext;

    public AdminController(RoleManager<IdentityRole> roleContext, UserManager<ApplicationUser> usersContext)
    {
        _roleContext = roleContext;
        _userContext = usersContext;
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
        var users = from user in _userContext.Users select user;
        return View(await users.ToListAsync());
    }
    
}