#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
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
            var students = _context.Student.Include(s => s.Courses);
            return View(await students.ToListAsync());
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.Include(s => s.Courses)
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
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Gender,DateOfBirth,SemesterNumber,RegisterDate")] StudentModel student, int[] selectedCourses)
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

                student.Courses = courses;
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
            if (id is null)
            {
                return NotFound();
            }

            var student = await _context.Student.Include(s => s.Courses).Where(s => s.ID == id).FirstOrDefaultAsync();
            if (student is null)
            {
                return NotFound();
            }

            // var attachedCourses = student.Courses;
            // ViewData["attachedCourses"] = attachedCourses;
            var allCoursesQuery = from course in _context.Course select course;
            ViewBag.Courses = await allCoursesQuery.ToListAsync();
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int[] selectedCourses, byte[] rowVersion)
        {

            // TODO
            // Fix this LINQ
            // This should be easier!
            var studentToUpdate = await _context.Student.Include(s => s.Courses)
                .ThenInclude(c => c.Faculty)
                .ThenInclude(c => c.University)
                .Include(s => s.Courses)
                .ThenInclude(c=> c.Faculty)
                .ThenInclude(f => f.Courses)
                .ThenInclude(c => c.Students)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (studentToUpdate is null)
            {
                return NotFound();
            }

            List<Course> attachedCourse = new List<Course>(selectedCourses.Length);
            var allCoursesQuery = from course in _context.Course select course;
            foreach (Course course in allCoursesQuery)
            {
                if (selectedCourses.Contains(course.CourseID))
                {
                    attachedCourse.Add(course);
                }
            }

            studentToUpdate.Courses = attachedCourse;

            // studentToUpdate.Courses = attachedCourse;
            // Person p = await _context.Person.SingleAsync(i => i.ID == id);
            // p.FirstName = "TEST";
            // var update = await TryUpdateModelAsync(p, "", r => r.FirstName, r => r.LastName, r => r.Gender,
            //     r => r.DateOfBirth);
            // var Du = await TryUpdateModelAsync(studentToUpdate, "", s => s.FirstName, s => s.LastName,
            //     s => s.Gender, s => s.DateOfBirth, s => s.SemesterNumber, s => s.Courses, s => s.RegisterDate);
            // _context.Entry(studentToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            // await TryUpdateModelAsync(studentToUpdate, "", s => s.FirstName, s => s.LastName,
            //     s => s.Gender, s => s.DateOfBirth, s => s.SemesterNumber, s => s.Courses, s => s.RegisterDate);
            // ModelState["Faculty"].ValidationState = ModelValidationState.Valid;
            if (await TryUpdateModelAsync(studentToUpdate, "", s => s.FirstName, s => s.LastName,
                    s => s.Gender, s=> s.DateOfBirth, s => s.SemesterNumber, s=>s.RegisterDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    throw;
                }
            }
            // Console.Clear();
            Console.WriteLine("===============");
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return RedirectToAction(nameof(Index));
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
