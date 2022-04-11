#nullable disable
using FirstProject.Data;
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
}