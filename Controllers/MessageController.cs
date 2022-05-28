using FirstProject.Data;
using FirstProject.Models;
using FirstProject.Models.ViewModels;
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
        var currentUserName  = HttpContext.User.Identity.Name;
        if (currentUserName is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 2,
                ErrorName = "Can not receive username",
                ErrorDescription = $"Controller tries to get username from HttpContext.User.Identity",
                ErrorPlace = "MessageController - ConversationCreate - POST",
                ErrorSolution = "Check if you are logged in"
            });
        }
        var userNamesQuery = from user in _userContext.Users select user.UserName;
        var userNames = await userNamesQuery.ToListAsync();
        userNames.Remove(currentUserName);
        ViewBag.Users = new SelectList(userNames);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConversationCreate(string username)
    {
        Console.WriteLine(username);
        var user = await  _userContext.Users.Where(u => u.UserName.Equals(username)).FirstOrDefaultAsync();
        if (user is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 5,
                ErrorName = "User not found in DB",
                ErrorDescription = $"User with this username {username} was not found in DB",
                ErrorPlace = "MessageController - ConversationCreate - POST",
                ErrorSolution = @"Select username from select list. Do not type your own username to url adress!"
            });
        }

        var currentUserName  = HttpContext.User.Identity.Name;
        if (currentUserName is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 2,
                ErrorName = "Can not receive username",
                ErrorDescription = $"Controller tries to get username from HttpContext.User.Identity",
                ErrorPlace = "MessageController - ConversationCreate - POST",
                ErrorSolution = "Check if you are logged in"
            });
        }
        var currentUser = await _userContext.Users.Where(u => u.UserName == currentUserName).FirstOrDefaultAsync();
        if (currentUser is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 5,
                ErrorName = "User not found in DB",
                ErrorDescription = $"User not found in DB",
                ErrorPlace = "MessageController - ConversationCreate - POST",
                ErrorSolution = @"Check if you are still logged in!"
            });
        }

        if (currentUserName.Equals(username))
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 6,
                ErrorName = "Typed current User username",
                ErrorDescription = $"Tried to pass the same username as logged user",
                ErrorPlace = "MessageController - ConversationCreate - POST",
                ErrorSolution = @"Do not type your own username to url address! You cant create messagebox to yourself!"
            });
        }

        MessageBox msgBox = new MessageBox()
        {
            Messages = new List<Message>(0),
            Participants = new List<ApplicationUser>(2) { 
                currentUser,
                user
            }
        };
        await _context.MessageBox.AddAsync(msgBox);
        await _context.SaveChangesAsync();
        if (user.MessageBoxes is null)
        {
            user.MessageBoxes = new List<MessageBox>(1) {msgBox};
        }
        else
        {
            var userBoxList = user.MessageBoxes.ToList();
            userBoxList.Add(msgBox);
            user.MessageBoxes = userBoxList;
        }
        await _userContext.UpdateAsync(user);
        
        if (currentUser.MessageBoxes is null)
        {
            currentUser.MessageBoxes = new List<MessageBox>(1) {msgBox};
        }
        else
        {
            var userBoxList = currentUser.MessageBoxes.ToList();
            userBoxList.Add(msgBox);
            currentUser.MessageBoxes = userBoxList;
        }
        await _userContext.UpdateAsync(currentUser);

        var userNames = await _userContext.Users.Select(u => u.UserName).ToListAsync();
        userNames.Remove(currentUserName);
        ViewBag.Users = new SelectList(userNames);
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MessageBox()
    {
        var currentUserName  = HttpContext.User.Identity.Name;
        if (currentUserName is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 2,
                ErrorName = "Can not receive username",
                ErrorDescription = $"Controller tries to get username from HttpContext.User.Identity",
                ErrorPlace = "MessageController - ConversationCreate - POST",
                ErrorSolution = "Check if you are logged in"
            });
        }
        var currentUser = await _userContext.Users.Where(u => u.UserName == currentUserName).Include(u => u.MessageBoxes).FirstOrDefaultAsync();
        if (currentUser is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 5,
                ErrorName = "User not found in DB",
                ErrorDescription = $"User not found in DB",
                ErrorPlace = "MessageController - ConversationCreate - POST",
                ErrorSolution = @"Check if you are still logged in!"
            });
        }
        
        var messageBox = await _context.MessageBox.Where(b => b.Participants.Contains(currentUser)).Include(b => b.Participants).ToListAsync();
        // var query = from box in _context.MessageBox where box.Participants.Contains()
        return View(messageBox);
    }

}