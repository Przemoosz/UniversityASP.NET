#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstProject.Data;
using FirstProject.Models;
using FirstProject.Models.Abstarct;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FirstProject.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            return View(await _context.Student.ToListAsync());
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = await GetAllCourses();
            return View();
        }

        private async Task<IEnumerable<Course>> GetAllCourses()
        {
            var courses = from course in _context.Course select course;
            return await courses.ToListAsync();
        }
        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Gender,DateOfBirth")] StudentModel student, int[] selectedCourses)
        {
            student.RowVersion = Array.Empty<byte>();
            ModelState["RowVersion"].ValidationState = ModelValidationState.Valid;

            if (selectedCourses is null || selectedCourses.Length == 0)
            {
                student.Courses = new List<Course>(0);
            }
            else
            {
                List<Course> courses = new List<Course>(selectedCourses.Length);
                foreach (var courseId in selectedCourses)
                {
                    var Course = await _context.Course.Where(c => c.CourseID == courseId).FirstOrDefaultAsync();
                    if (Course is not null)
                    {
                        courses.Add(Course);
                    }
                    else
                    {
                        ViewData["Error"] = "Selected courses that do not exists!";
                        return RedirectToAction(nameof(Create));
                    }
                }

                ModelState["Courses"].ValidationState = ModelValidationState.Valid;
            }
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Gender,DateOfBirth")] StudentModel student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.ID == id);
        }
    }
}
