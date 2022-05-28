using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirstProject.Data;
using FirstProject.Models;
using FirstProject.Models.Abstarct;
using FirstProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace FirstProject.Controllers;

public class PersonController : Controller
{
    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _usersContext;

    public PersonController(ApplicationDbContext context, UserManager<ApplicationUser> usersContext)
    {
        _context = context;
        _usersContext = usersContext;
    }
    [HttpGet]
    [Authorize(Policy = "RequireUser")]
    public async Task<IActionResult> UserAttach()
    {
        string? userName = HttpContext.User.Identity!.Name;
        if (userName is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 2,
                ErrorName = "Can not receive username",
                ErrorDescription = $"Controller tries to get username from HttpContext.User.Identity ",
                ErrorPlace = "PersonController - UserAttach - GET",
                ErrorSolution = "Check if you are logged in"
            });
        }
        var user = await _usersContext.Users.Include(u => u.Persons).FirstOrDefaultAsync(u => u.UserName.Equals(userName));  
        // var personQuery = from person in _context.Person select person;
        ViewBag.Person = await _context.Person.Include(p => p.User).ToListAsync();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "RequireUser")]
    public async Task<IActionResult> UserAttach(int[] selectedPersons)
    {
        string? userName = HttpContext.User.Identity!.Name;
        if (userName is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 2,
                ErrorName = "Can not receive username",
                ErrorDescription = $"Controller tries to get username from HttpContext.User.Identity ",
                ErrorPlace = "PersonController - UserAttach - POST",
                ErrorSolution = "Check if you are logged in"
            });
        }
        var user = await _usersContext.Users.Include(u => u.Persons).FirstAsync(u => u.UserName.Equals(userName));
        List<Person> selectedPersonsList = new List<Person>(selectedPersons.Length);
        foreach (int personId in selectedPersons)
        {
            var personObject = await _context.Person.Include(p => p.User).FirstOrDefaultAsync(p => p.ID == personId);
            if (personObject is null)
            {
                return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
                {
                    ErrorId = 1,
                    ErrorName = "Selected person does not exists",
                    ErrorDescription = $"You are trying to attach not existing person with id {personId} to user {userName}",
                    ErrorPlace = "StudentController - UserAttach - POST",
                    ErrorSolution = "Check provided id for selected user using debugger"
                });
            }

            if (personObject.User is not null && personObject.User.Id != user.Id)
            {
                return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
                {
                    ErrorId = 3,
                    ErrorName = "Cant attach person, person is already taken",
                    ErrorDescription = $"You are trying to attach a person '{personObject.FullName}' which is already attached to other user {personObject.User.UserName}",
                    ErrorPlace = "StudentController - UserAttach - POST",
                    ErrorSolution = "Detach person from user, and then attach it to this user"
                });
            }
            if (personObject.User is not null && personObject.User.Id == user.Id)
            {
                selectedPersonsList.Add(personObject);
                continue;
            }
            personObject.User = user;
            _context.Update(personObject);
            selectedPersonsList.Add(personObject);
        }

        user.Persons = selectedPersonsList;
        await _usersContext.UpdateAsync(user);
        await _context.SaveChangesAsync();
        ViewBag.Person = await _context.Person.Include(p => p.User).ToListAsync();
        return View(user);
    }
    
    [HttpGet]
    public async Task<IActionResult> DetachPerson(int personId)
    {
        string? username = HttpContext.User.Identity!.Name;
        if (username is null)
        {
            return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
            {
                ErrorId = 2,
                ErrorName = "Can not receive username",
                ErrorDescription = $"Controller tries to get username from HttpContext.User.Identity",
                ErrorPlace = "DetachPerson - UserAttach - GET",
                ErrorSolution = "Check if you are logged in"
            });
        }

        var userObject = await _usersContext.Users.Include(u => u.Persons).FirstAsync(u => u.UserName.Equals(username));
        var userPersonList = userObject.Persons;
        var selectedPerson = await _context.Person.Include(p=> p.User).FirstAsync(p => p.ID == personId);
        List<Person> newPersonList = new List<Person>();
        foreach (Person person in userPersonList)
        {
            if (person.ID == personId)
            {
                continue;
            }
            newPersonList.Add(person);
        }
        userObject.Persons = newPersonList;
        selectedPerson.User = null;
        await _usersContext.UpdateAsync(userObject);
        _context.Update(selectedPerson);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(UserAttach));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int personId)
    {
        var fetchedStudent = await _context.Student.Include(s => s.User).FirstOrDefaultAsync(s => s.ID == personId);
        if (fetchedStudent is not null)
        {
            return View("DetailsStudent", fetchedStudent);
        }
        var fetchedPerson = await _context.Person.Include(s => s.User).FirstOrDefaultAsync(s => s.ID == personId);
        if (fetchedPerson is not null)
        {
            return View(fetchedPerson);
        }
        return RedirectToAction("ErrorPage", "Home", new ErrorPageModelView()
        {
            ErrorId = 4,
            ErrorName = "Person not found in DB",
            ErrorDescription = $"Person with this id {personId} was not found in DB",
            ErrorPlace = "PersonController - Details - GET",
            ErrorSolution = "Type other id or create a person with this id. you can also check Db if person which you are looking for exists"
        });
    }
}