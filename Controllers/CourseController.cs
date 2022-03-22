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
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FirstProject.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Course
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Course.Include(c => c.Faculty);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Faculty)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Course/Create
        public IActionResult Create()
        {
            ViewData["Faculty"] = new SelectList(_context.Faculty, "FacultyID", "FacultyName");
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,CourseName,TotalStudents,CourseType,FacultyID")] Course course)
        {
            Faculty? choosedFaculty = await _context.Faculty.Where(f => f.FacultyID == course.FacultyID).FirstOrDefaultAsync();
            if (choosedFaculty is not null)
            {
                course.Faculty = choosedFaculty;
                if (ModelState["Faculty"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState["Faculty"].ValidationState = ModelValidationState.Valid;
                }
                
            }
            else
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacultyID"] = new SelectList(_context.Faculty, "FacultyID", "FacultyName", course.FacultyID);
            return View(course);
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewBag.Faculty = new SelectList(_context.Faculty, "FacultyID", "FacultyName", course.FacultyID);
            return View(course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("CourseID,CourseName,TotalStudents,CourseType,FacultyID")] Course course)
        // {
        //     if (id != course.CourseID)
        //     {
        //         return NotFound();
        //     }
        //
        //     Faculty? choosedNewFaculty =
        //         await _context.Faculty.Where(f => f.FacultyID == course.FacultyID).FirstOrDefaultAsync();
        //     if (choosedNewFaculty is not null)
        //     {
        //         course.Faculty = choosedNewFaculty;
        //         if (ModelState["Faculty"].ValidationState == ModelValidationState.Invalid)
        //         {
        //             ModelState["Faculty"].ValidationState = ModelValidationState.Valid;
        //         }
        //     }
        //     else
        //     {
        //         return NotFound();
        //     }
        //     
        //     
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(course);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!CourseExists(course.CourseID))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["FacultyID"] = new SelectList(_context.Faculty, "FacultyID", "FacultyName", course.FacultyID);
        //     return View(course);
        // }
        
        
        
        // HTTP POST EDIT
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id is null)
            {
                return NotFound();
            }

            Course? courseToUpdate = await _context.Course.Include(i => i.Faculty).ThenInclude(i => i.University).Include(f => f.Faculty).ThenInclude(f => f.Transactions).Include(f => f.Faculty).Where(c => c.CourseID == id)
                .FirstOrDefaultAsync();
            // Faculty? connectedFaculty = await _context.Faculty.Where(f => f.FacultyID == courseToUpdate.FacultyID)
            //     .FirstOrDefaultAsync();
            
            if (courseToUpdate is null)
            {
                Course deletedCourse = new Course();
                await TryUpdateModelAsync(deletedCourse);
                ModelState.AddModelError(string.Empty,"Unable to edit course, because it was already deleted!");
                ViewData["FacultyID"] = new SelectList(_context.Faculty, "FacultyID", "FacultyName", courseToUpdate.FacultyID );
                return View(deletedCourse);
            }
            
            _context.Entry(courseToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            // ModelState["University"].ValidationState = ModelValidationState.Invalid;
            int i = 0;
            if (await TryUpdateModelAsync<Course>(courseToUpdate, "", c => c.CourseName, c => c.CourseType,
                    c => c.TotalStudents))
            {
                try
                {
                    _context.Course.Update(courseToUpdate);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Done");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException exception)
                {
                    Console.WriteLine("Not Done");
                    var exceptionEntries  = exception.Entries.Single();
                    var clientvalues = (Course) exceptionEntries.Entity;
                    var databaseEntry = exceptionEntries.GetDatabaseValues();
                    if (databaseEntry is null)
                    {
                        ModelState.AddModelError(string.Empty,"Unable to save changes. Object was deleted!");
                    }
                    else
                    {
                        var databaseValues = (Course) databaseEntry.ToObject();
                        
                    }
                    var prop = exceptionEntries.CurrentValues;
                    var orig = exceptionEntries.GetDatabaseValues();
                }
            }
            
            
            ViewData["FacultyID"] = new SelectList(_context.Faculty, "FacultyID", "FacultyName", courseToUpdate.Faculty.FacultyName);
            return View(courseToUpdate);
        }
        
        

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Faculty)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseID == id);
        }
    }
}
