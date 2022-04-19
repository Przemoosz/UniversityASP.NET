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
            ViewBag.Faculty = new SelectList(_context.Faculty, "FacultyID", "FacultyName");
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
                course.RowVersion = Array.Empty<byte>();
                course.Students = new List<StudentModel>();
                if (ModelState["Faculty"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState["Faculty"].ValidationState = ModelValidationState.Valid;
                    
                }

                if (ModelState["RowVersion"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState["RowVersion"].ValidationState = ModelValidationState.Valid;
                }

                if (ModelState["Students"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState["Students"].ValidationState = ModelValidationState.Valid;
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
                return RedirectToAction("Courses","Faculty", new {facultyId = choosedFaculty.FacultyID });
            }
            ViewBag.Faculty = new SelectList(_context.Faculty, "FacultyID", "FacultyName", course.FacultyID);
            return View(course);
            // return RedirectToAction("Courses","Faculty", new {facultyId = choosedFaculty.FacultyID });
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
        
        // HTTP POST EDIT
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id is null)
            {
                return NotFound();
            }
        
            // TODO 
            // Change Include methods!
            Course? courseToUpdate = await _context.Course.Include(i => i.Faculty)
                .ThenInclude(i => i.University)
                .Include(c => c.Students)
                .Where(c => c.CourseID == id)
                .FirstOrDefaultAsync();
                
            // Faculty? connectedFaculty = await _context.Faculty.Where(f => f.FacultyID == courseToUpdate.FacultyID)
            //     .FirstOrDefaultAsync();
            
            if (courseToUpdate is null)
            {
                Course deletedCourse = new Course();
                await TryUpdateModelAsync(deletedCourse);
                ModelState.AddModelError(string.Empty,"Unable to edit course, because it was already deleted!");
                ViewBag.Faculty = new SelectList(_context.Faculty, "FacultyID", "FacultyName", courseToUpdate.FacultyID);
                return View(deletedCourse);
            }
            
            _context.Entry(courseToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            // ModelState["University"].ValidationState = ModelValidationState.Invalid;
            int i = 0;
            if (await TryUpdateModelAsync<Course>(courseToUpdate, "", c => c.CourseName, c => c.CourseType,
                    c => c.TotalStudents, c=> c.FacultyID))
            {
                try
                {
                    _context.Course.Update(courseToUpdate);
                    await _context.SaveChangesAsync();
                    // Console.WriteLine("Done");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException exception)
                {
                    // Console.WriteLine("Not Done");
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
                        if (databaseValues.CourseName != clientvalues.CourseName)
                        {
                            ModelState.AddModelError("CourseName", $"Current value in database: {databaseValues.CourseName}");
                        }

                        if (databaseValues.CourseType != clientvalues.CourseType)
                        {
                            ModelState.AddModelError("CourseType", $"Current value in database: {databaseValues.CourseType}");
                        }

                        if (databaseValues.TotalStudents != clientvalues.TotalStudents)
                        {
                            ModelState.AddModelError("TotalStudents",$"Current value in database: {databaseValues.TotalStudents}");
                        }

                        if (databaseValues.FacultyID != clientvalues.FacultyID)
                        {
                            Faculty dbFaculty = await _context.Faculty.Where(f => f.FacultyID == databaseValues.FacultyID)
                                .SingleAsync();
                            ModelState.AddModelError("FacultyID", $"Current Faculty in database is: {dbFaculty.FacultyName}");
                        }
                        ModelState.AddModelError(string.Empty,"Concurrency Error occurred! This mean that someone have already made a change to this object and your version is not compatibile with actual version in database." +
                                                              " Click save again to force saving your version, or abort changes and return to list using Back to list ");
                        courseToUpdate.RowVersion = (byte[]) databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                    // var prop = exceptionEntries.CurrentValues;
                    // var orig = exceptionEntries.GetDatabaseValues();
                }
            }
            
            
            ViewBag.Faculty = new SelectList(_context.Faculty, "FacultyID", "FacultyName", courseToUpdate.FacultyID);
            return View(courseToUpdate);
        }
        

        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.Faculty).AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                if(concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurencyError"] = "You are trying to delete object which was modified during your request! Check new value using 'Back to list' or click delete again to force delete'";
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Course course)
        {
            // int id = course.CourseID;
            // course = await _context.Course.Where(f => f.CourseID == id).SingleAsync();
            try
            {
                // Course courseToDelete = await _context.Course.Where(c => c.CourseID == course.CourseID).SingleAsync();
                if (await _context.Course.AnyAsync(m => m.CourseID == course.CourseID))
                {
                    _context.Course.Remove(course);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Console.WriteLine("Catched");
                return RedirectToAction(nameof(Delete), new { id = course.CourseID,  concurrencyError = true});
            }
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseID == id);
        }
    }
}
