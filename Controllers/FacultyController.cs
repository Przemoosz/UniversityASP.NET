#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstProject.Data;
using FirstProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FirstProject.Controllers
{
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Faculty
        public async Task<IActionResult> Index()
        {
            return View(await _context.Faculty.ToListAsync());
        }

        // GET: Faculty/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            // var faculty = await _context.Faculty
            //     .FirstOrDefaultAsync(m => m.FacultyID == id);
            var faculty = await _context.Faculty.Include(f => f.University).Include(f => f.Transactions).FirstOrDefaultAsync(f => f.FacultyID == id);
            Console.WriteLine("========================================");
            Console.WriteLine(faculty.Transactions is null);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // GET: Faculty/Create
        public IActionResult Create()
        {
            ViewData["University"] = new SelectList(_context.Set<University>(), "UniversityID", "UniversityName");
            return View();
        }
        
        // Get Faculty/Transactions?FacultyId=[id]
        [HttpGet]
        public async Task<IActionResult> Transactions(int facultyId)
        {
            Faculty selectedFaculty = await _context.Faculty.Where(f => f.FacultyID == facultyId).Include(i => i.Transactions).Include(i => i.University)
                .FirstOrDefaultAsync();
            if (selectedFaculty is null)
            {
                return NotFound();
            }

            ViewData["FacultyName"] = selectedFaculty.FacultyName;
            ViewData["FacultyID"] = selectedFaculty.FacultyID;
            ViewData["UniversityName"] = selectedFaculty.University.UniversityName;
            IQueryable<ICollection<Transaction>> transactionList = from faculty in _context.Faculty
                where faculty.FacultyID == facultyId
                select faculty.Transactions;
            
            return View(await transactionList.FirstOrDefaultAsync());
        }
        
        // POST: Faculty/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacultyID,FacultyName,Employed,Budget,CreationDate,UniversityID")] Faculty faculty)
        {
            University? choosedUniversity = await _context.University.Where(w => w.UniversityID == faculty.UniversityID).FirstOrDefaultAsync();
            if (choosedUniversity is not null)
            {
                faculty.University = choosedUniversity;
                faculty.Transactions = new List<Transaction>();
                // Console.WriteLine("========================================");
                // Console.WriteLine(faculty.Transactions is null);
                ModelState.SetModelValue("University", new ValueProviderResult("", CultureInfo.InvariantCulture));
                if (ModelState["University"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState["University"].ValidationState = ModelValidationState.Valid;
                }
            }
            else
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                // Console.WriteLine("========================================");
                // Console.WriteLine(faculty.Transactions is null);
                _context.Add(faculty);
                await _context.SaveChangesAsync();
            }
            return View(faculty);
        }

        // GET: Faculty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculty.FindAsync(id);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }

        // POST: Faculty/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacultyID,FacultyName,Employed,Budget,CreationDate,UniveristyID")] Faculty faculty)
        {
            if (id != faculty.FacultyID)
            {
                return NotFound();
            }

            if (faculty.Transactions is null)
            {
                faculty.Transactions = new List<Transaction>();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faculty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(faculty.FacultyID))
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
            return View(faculty);
        }

        // GET: Faculty/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculty
                .FirstOrDefaultAsync(m => m.FacultyID == id);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // POST: Faculty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faculty = await _context.Faculty.FindAsync(id);
            _context.Faculty.Remove(faculty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacultyExists(int id)
        {
            return _context.Faculty.Any(e => e.FacultyID == id);
        }
    }
}
