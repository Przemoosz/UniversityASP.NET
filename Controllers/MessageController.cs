using FirstProject.Data;
using FirstProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Controllers;

[Authorize(Policy = "RequireUser")]
public class MessageController: Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userContext;

    public MessageController(ApplicationDbContext context, UserManager<ApplicationUser> userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<IActionResult> ConversationCreate()
    {
        var userNamesQuery = from user in _userContext.Users select user.UserName;
        var userNames = await userNamesQuery.ToListAsync();
        ViewBag.Users = new SelectList(userNames);
        return View();
    }

}