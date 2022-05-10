#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstProject.Data;
using FirstProject.Models;
using FirstProject.Models.Abstarct;
using FirstProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FirstProject.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _usersContext;
        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> usersContext)
        {
            _context = context;
            _usersContext = usersContext;
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

            var student = await _context.Student.Include(s => s.Courses).Include(s => s.User)
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

            var currentUser = await _usersContext.Users.FirstAsync(u => u.Id.Equals("319c52a9-ad1d-4f19-845d-be6635e1cc21"));
            student.User = currentUser;
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
        public async Task<IActionResult> Edit(int? id, int[] selectedCourses, byte[] rowVersion)
        {

            if (id is null)
            {
                return NotFound();
            }
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
            var allCoursesQuery = from course in _context.Course select course;

            if (studentToUpdate is null)
            {
                StudentModel deletedStudent = new StudentModel();
                await TryUpdateModelAsync(deletedStudent);
                deletedStudent.Courses = new List<Course>(0);
                ModelState.AddModelError(string.Empty, "Cant update student because it was already deleted!");
                ViewBag.Courses = await allCoursesQuery.ToListAsync();
                return View(deletedStudent);
            }

            List<Course> attachedCourse = new List<Course>(selectedCourses.Length);
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
            _context.Entry(studentToUpdate).Property("RowVersion").OriginalValue = rowVersion;
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
                    var exceptionEntry = ex.Entries.Single();
                    var clientValeus = (StudentModel) exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry is null)
                    {
                        ModelState.AddModelError(string.Empty,"Student was deleted before saving changes. Unable to edit student!");
                    }
                    else
                    {
                        var databaseValues = (StudentModel) databaseEntry.ToObject();
                        // TODO
                        if (databaseValues.FirstName != clientValeus.FirstName)
                        {
                            ModelState.AddModelError("FirstName", $"Current Value in database is {databaseValues.FirstName}");
                        }

                        if (databaseValues.LastName != clientValeus.LastName)
                        {
                            ModelState.AddModelError("LastName", $"Current Value in database is {databaseValues.LastName}");
                        }

                        if (databaseValues.Gender != clientValeus.Gender)
                        {
                            ModelState.AddModelError("Gender", $"Current Value in database is {databaseValues.Gender.ToString()}");
                        }

                        if (databaseValues.RegisterDate != clientValeus.RegisterDate)
                        {
                            ModelState.AddModelError("RegisterDate", $"Current Value in database is {databaseValues.RegisterDate}");
                        }

                        if (databaseValues.SemesterNumber != clientValeus.SemesterNumber)
                        {
                            ModelState.AddModelError("SemesterNumber",$"Current Value in database is {databaseValues.SemesterNumber}");
                        }

                        if (databaseValues.DateOfBirth != clientValeus.DateOfBirth)
                        {
                            ModelState.AddModelError("DateOfBirth", $"Current Value in database is {databaseValues.DateOfBirth}");
                        }

                        // if (!databaseValues.Courses.Equals(clientValeus.Courses))
                        // {
                        //     StringBuilder builder = new StringBuilder();
                        //     if (databaseValues.Courses  is not null)
                        //     {
                        //         foreach (Course course in databaseValues.Courses)
                        //         {
                        //             builder.Append(course.CourseName);
                        //             builder.Append(" ");
                        //         }
                        //     }
                        //
                        //     string ret = builder.ToString();
                        //
                        //     ModelState.AddModelError("Courses", $"Current Courses in database: {ret}");
                        // }
                        ModelState.AddModelError(string.Empty,"Concurrency Error occurred! This mean that someone have already made a change to this object and your version is not compatible with actual version in database." +
                                                              " Click save again to force saving your version, or abort changes and return to list using Back to list link. ");
                        studentToUpdate.RowVersion = (byte[]) databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            // var allCoursesQuery = from course in _context.Course select course;
            ViewBag.Courses = await allCoursesQuery.ToListAsync();
            return View(studentToUpdate);
            // // Console.Clear();
            // Console.WriteLine("===============");
            // foreach (var modelState in ModelState.Values)
            // {
            //     foreach (var error in modelState.Errors)
            //     {
            //         Console.WriteLine(error.ErrorMessage);
            //     }
            // }    
            // return RedirectToAction(nameof(Index));
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id, bool concurrencyError=false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Courses)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                if (concurrencyError == true) ;
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            if (concurrencyError == true)
            {
                ViewData["ConcurrencyError"] = "You are trying to delete object which was modified during your request! Check new value using 'Back to list' or click delete again to force delete'";
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(StudentModel student)
        {

            try
            {
                if (await _context.Student.AnyAsync(s => s.ID == student.ID))
                {
                    _context.Student.Remove(student);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return RedirectToAction(nameof(Delete), new {id = student.ID, concurrencyError = true});
            }
        }
        
        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.ID == id);
        }
    }
}
