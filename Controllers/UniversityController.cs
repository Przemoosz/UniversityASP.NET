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
using FirstProject.Models.ViewModels;

namespace FirstProject.Controllers
{
    public class UniversityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UniversityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: University
        public async Task<IActionResult> Index()
        {
            return View(await _context.University.ToListAsync());
        }

        // GET: University?UniversityName=[University Name]
        [HttpGet]
        public async Task<IActionResult> Index(string universityName)
        {
            University selectedUni = await _context.University.Where(u => u.UniversityName == universityName)
                .FirstOrDefaultAsync();
            if (selectedUni is null)
            {
                return NotFound();
            }

            selectedUni = await _context.University.Include(u => u.Faculties).ThenInclude(u => u.Transactions).Include(u => u.Faculties).ThenInclude(u => u.Courses)
                .Where(u => u.UniversityName == universityName).SingleAsync();
            // foreach (var faculty in selectedUni.Faculties)
            // {
            //     faculty.Transactions = new List<Transaction>();
            // }
            int facultiesCount = selectedUni.Faculties.Count;
            // List<University> list = new List<University>(1) {selectedUni};
            ViewData["Rows"] = facultiesCount / 3;
            ViewData["LastRowColumns"] = facultiesCount % 3;
            return View("UniversityView",selectedUni);
        }
            
        // GET: University/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var university = await _context.University
            //     .FirstOrDefaultAsync(m => m.UniversityID == id);
            var university = await _context.University.Include(u => u.Faculties).FirstOrDefaultAsync(u => u.UniversityID == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }
        
        // GET: University/Choose
        [HttpGet]
        public async  Task<IActionResult> Choose()
        {
            var data = from uni in _context.University select new ChooseUniversityModelView(){UniversityName = uni.UniversityName, Employed = uni.Employed, FacultiesCount = uni.Faculties.Count()};
            return View(await data.ToListAsync());
        }

        // GET: University/Create
        public IActionResult Create(bool error = false, string? wrongName = null)
        {
            if (error)
            {
                ViewData["NameError"] = $"University with {wrongName} name already exists!";
            }
            
            return View();
        }

        // POST: University/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UniversityID,UniversityName,CreationDate,Employed,Adress")] University university)
        {
            if (university.Faculties is null)
            {
                university.Faculties = new List<Faculty>();
            }

            var uniExits = await _context.University.Where(u => u.UniversityName == university.UniversityName)
                .FirstOrDefaultAsync();
            if (uniExits is not null)
            {
                Console.WriteLine("Uni exists");
                ModelState.AddModelError("", "University with this name already exists!");
                return RedirectToAction(nameof(Create),new{error = true, wrongName = university.UniversityName});
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(university);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(university);
        }

        // GET: University/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.University.FindAsync(id);
            if (university == null)
            {
                return NotFound();
            }
            return View(university);
        }

        // POST: University/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UniversityID,UniversityName,CreationDate,Employed,Adress")] University university)
        {
            if (id != university.UniversityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(university);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversityExists(university.UniversityID))
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
            return View(university);
        }

        // GET: University/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var university = await _context.University
                .FirstOrDefaultAsync(m => m.UniversityID == id);
            if (university == null)
            {
                return NotFound();
            }

            return View(university);
        }

        // POST: University/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var university = await _context.University.FindAsync(id);
            _context.University.Remove(university);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UniversityExists(int id)
        {
            return _context.University.Any(e => e.UniversityID == id);
        }
    }
}
