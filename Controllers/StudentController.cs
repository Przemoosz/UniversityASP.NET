#nullable disable
using FirstProject.Data;
using FirstProject.Models;
using FirstProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstProject.Controllers;

public class StudentController: Controller
{
    private readonly ApplicationDbContext _context;

    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Temp method to retrieve index of students
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Student.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var courses = from course in _context.Course select course;
        return View(new StudentCUModelView(){Course = courses});
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Gender,DateOfBirth,SemesterNumber,RegisterDate,Courses")] StudentModel student)
    {
        
        if (ModelState.IsValid)
        {
            Console.WriteLine("Hello There!");
        }
        return RedirectToAction(nameof(Index));
    }
}