using System.Data.Common;
using FirstProject.Data;
using FirstProject.Models;
using FirstProject.Models.Enums;
using FirstProject.Models.ViewModels;
using FirstProject.PolicyBasedAuthorization;
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
        // await ConfigurationFile.Load();
        return View(await models.ToListAsync());
    }

    [HttpGet]
    // [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> Users(string? usernameSearchString, UsersRolesEnum? usersRole)
    {
        // var user = _userContext.Users.Include()
        
        var users = from user in _userContext.Users select user;
        if (!string.IsNullOrEmpty(usernameSearchString))
        {
            users = from user in _userContext.Users where user.UserName.Contains(usernameSearchString) select user;
            ViewData["usernameSearchString"] = usernameSearchString;
        }

        List<AdminUserDisplayModel> usersAndRoles = new List<AdminUserDisplayModel>(users.Count());
        if (usersRole is not null)
        {
            foreach (var user in users)
            {
                var role = await _userContext.GetRolesAsync(user);


                if (usersRole is not null)
                {
                    if (checkRole(role, usersRole))
                    {
                        if (role.Count == 0)
                        {
                            role = null;
                        }
                        usersAndRoles.Add(new AdminUserDisplayModel(){User = user,Role = role});
                    }
                }
            }

            ViewData["GroupByRoles"] = usersRole;
            return View(usersAndRoles);
        }
        else
        {
            foreach (var user in users)
            {
                var role = await _userContext.GetRolesAsync(user);
                if (role.Count == 0)
                {
                    role = null;
                }
                usersAndRoles.Add(new AdminUserDisplayModel(){User = user,Role = role});
            }
        }
        return View(usersAndRoles);
    }

    private static bool checkRole(IEnumerable<string> attachedRoles, UsersRolesEnum? role)
    {
        switch (role)
        {
            case UsersRolesEnum.Admin:
                if (attachedRoles.Contains("Admin"))
                {
                    return true;
                }

                return false;
                
            case UsersRolesEnum.User:
                if (attachedRoles.Contains("User"))
                {
                    return true;
                }

                return false;
            case UsersRolesEnum.NoRole:
                if (attachedRoles.Count()==0)
                {
                    return true;
                }

                return false;
            default:
                return false;
        }
        return false;
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

    [HttpGet]
    public async Task<IActionResult> PageInfo()
    {
        string adminUuid = await _roleContext.Roles.Where(x => x.Name.Equals("Admin")).Select(x => x.Id).FirstAsync();
        string userUuid = await _roleContext.Roles.Where(x => x.Name.Equals("User")).Select(x => x.Id).FirstAsync();

        // Console.WriteLine(adminUuid);
        // Console.WriteLine(userUuid);
        PageInfoDisplayModel displayModel = new PageInfoDisplayModel();
        displayModel.UserCount = await _userContext.Users.CountAsync();
        displayModel.Admins= await _context.UserRoles.Where(u => u.RoleId.Equals(adminUuid)).CountAsync();
        displayModel.Users = await _context.UserRoles.Where(u => u.RoleId.Equals(userUuid)).CountAsync();
        displayModel.TotalUniversities = await _context.University.CountAsync();
        displayModel.TotalFaculties = await _context.Faculty.CountAsync();
        displayModel.TotalCourses = await _context.Course.CountAsync();
        displayModel.TotalTransactions = await _context.Transaction.CountAsync();
        
        return View(displayModel);
    }
    
    private string[] _policyArray = new string[3]
    {
        "UniversityCreate",
        "UniversityDelete",
        "UniversityOverview"
    };

    private static Dictionary<string, List<string>> _selectedPermission = new Dictionary<string, List<string>>(3);
    
    [HttpGet]
    public async Task<IActionResult> Permissions()
    {
        // TODO
        var activePolicy = await ConfigurationFile.LoadAsync();
        var roleQuery = from role in _roleContext.Roles select role.Name;
        SelectList policySelectList = new SelectList(_policyArray);
        ViewData["PolicySelectList"] = policySelectList;
        return View(new PermissionViewModel(){
            JsonPolicy = activePolicy,
            Role = new SelectList(await roleQuery.ToListAsync()),
            SelectedPermissions = _selectedPermission
        });
    }

    [HttpGet]
    public async Task<IActionResult> PermissionAdd(string? roleName, string? policyName)
    {
        if (roleName is null || policyName is null)
        {
            return RedirectToAction(nameof(Permissions));
        }
        if (_selectedPermission.ContainsKey(policyName))
        {
            var roleList = _selectedPermission[policyName];
            if (!roleList.Contains(roleName))
            {
                roleList.Add(roleName);
            }
        }
        else
        {
            _selectedPermission.Add(policyName, new List<string>(3){roleName});
        }
        return RedirectToAction(nameof(Permissions));
    }

    [HttpGet]
    public IActionResult PermissionRemoveAll()
    {
        _selectedPermission = new Dictionary<string, List<string>>(3);
        return RedirectToAction(nameof(Permissions));
    }

    [HttpGet]
    public IActionResult PermissionRemove(string? policyName)
    {
        if (policyName is null)
        {
            RedirectToAction(nameof(Permissions));
        }
        if (_selectedPermission.ContainsKey(policyName))
        {
            _selectedPermission.Remove(policyName);
        }
        else
        {
            ViewData["RemoveError"] = "This policy does not exists!";
            return RedirectToAction(nameof(Permissions));
        }
        return RedirectToAction(nameof(Permissions));
    }

    [HttpGet]
    public async Task<IActionResult> PermissionSave()
    {
        await ConfigurationFile.SaveAsync(_selectedPermission);
        ViewData["Saved"] = "Policy have been saved";
        return RedirectToAction(nameof(PermissionRemoveAll));
    }
}