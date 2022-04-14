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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FirstProject.Controllers;
using FirstProject.Models.Enums;
namespace FirstProject.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var firstProjectContext = _context.Transaction.Include(t => t.Faculty);
            return View(await firstProjectContext.ToListAsync());
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.Faculty)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Create
        // public async Task<IActionResult> Create()
        // {
        //     ViewData["Faculty"] = new SelectList(_context.Set<Faculty>(), "FacultyID", "FacultyName");
        //     ViewData["FacultyName"] = null;
        //     return View();
        // }


        public async Task<IActionResult> Create(int? facultyID)
        {
            ViewData["FacultyID"] = facultyID;
            if (facultyID is null)
            {
                ViewData["Faculty"] = new SelectList(_context.Set<Faculty>(), "FacultyID", "FacultyName");
                ViewData["FacultyName"] = null;
                return View();
            }
            
            IQueryable<string> transactionFaculty =
                from faculty in _context.Faculty where faculty.FacultyID == facultyID select faculty.FacultyName;
            if (await transactionFaculty.FirstOrDefaultAsync() is null)
            {
                return NotFound();
            }
            ViewData["FacultyName"] = await transactionFaculty.SingleAsync();
            return View();
        }
        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionID,TransactionName,Group,Amount,TransactionDate,FacultyID")] Transaction transaction)
        {
            Console.WriteLine(transaction.FacultyID);
            var selectedFaculty = await _context.Faculty.Where(i => i.FacultyID == transaction.FacultyID).FirstOrDefaultAsync();
            if (selectedFaculty is not null)
            {
                transaction.Faculty = selectedFaculty;
                if (ModelState["Faculty"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState["Faculty"].ValidationState = ModelValidationState.Valid;
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                
                await UpdateFacultyBudget(facultyId: transaction.FacultyID, cost: (decimal) transaction.Amount, income: transaction.Group == TransactionGroupEnum.Income);
                return RedirectToAction(actionName: nameof(FirstProject.Controllers.FacultyController.Transactions),
                    controllerName: nameof(Faculty), new{facultyId = transaction.FacultyID});
            }
            ViewData["FacultyID"] = new SelectList(_context.Set<Faculty>(), "FacultyID", "FacultyName", transaction.FacultyID);
            return View(transaction);
        }

        private async Task UpdateFacultyBudget(int facultyId, decimal cost, bool income)
        {
            Faculty facultyToUpdate =await _context.Faculty.Where(f => f.FacultyID == facultyId).FirstOrDefaultAsync();
            if (facultyToUpdate is null)
            {
                throw new Exception("Faculty does not exists");
            }

            if (income)
            {
                facultyToUpdate.Budget += cost;
            }
            else
            {
                facultyToUpdate.Budget -= cost;
            }
            try
            {
                _context.Update(facultyToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }
        }

        private async Task UpdateFacultyBudget(int facultyID, decimal cost)
        {
            Faculty facultyToUpdate = await _context.Faculty.Where(f => f.FacultyID == facultyID).FirstOrDefaultAsync();
            if (facultyToUpdate is not null)
            {
                // Default removing money when removing Income
                facultyToUpdate.Budget -= cost;
                try
                {
                    _context.Update(facultyToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
                
            }
            else
            {
                throw new Exception();
            }
        }
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create(
        //     [Bind("TransactionID,TransactionName,Group,Amount,TransactionDate,UniversityID")] Transaction transaction, int facultyID)
        // {
        //     
        //     Faculty facultyToConnect;
        // }
        
        
        // GET: Transaction/Edit/5
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var transaction = await _context.Transaction.FindAsync(id);
        //     if (transaction == null)
        //     {
        //         return NotFound();
        //     }
        //     ViewData["FacultyID"] = new SelectList(_context.Set<Faculty>(), "FacultyID", "FacultyName", transaction.FacultyID);
        //     return View(transaction);
        // }

        // TODO
        // POST: Transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("TransactionID,TransactionName,Amount,TransactionDate")] Transaction transaction)
        // {
        //     if (id != transaction.TransactionID)
        //     {
        //         return NotFound();
        //     }
        //     
        //     var facultyToConnect =
        //         await _context.Faculty.Where(f => f.FacultyID == transaction.FacultyID).FirstOrDefaultAsync();
        //     var money = await _context.Transaction.Where(t => t.FacultyID == transaction.FacultyID).SumAsync(i => i.Amount);
        //     Console.WriteLine("=====================================");
        //     Console.WriteLine($"{money}");
        //
        //     double cashDelta =  transaction.Amount - money;
        //     
        //
        //     if (facultyToConnect is not null)
        //     {
        //         transaction.Faculty = facultyToConnect;
        //         ModelState["Faculty"].ValidationState = ModelValidationState.Valid;
        //     }
        //     else
        //     {
        //         return NotFound();
        //     }
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(transaction);
        //             UpdateFacultyBudget(facultyToConnect.FacultyID, (decimal) cashDelta,
        //                 income: transaction.Group == TransactionGroupEnum.Income);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!TransactionExists(transaction.TransactionID))
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
        //     ViewData["FacultyID"] = new SelectList(_context.Set<Faculty>(), "FacultyID", "FacultyName", transaction.FacultyID);
        //     return View(transaction);
        // }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.Faculty)
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            int facultyId = transaction.FacultyID;
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
            if (transaction.Group == TransactionGroupEnum.Income)
            {
                await UpdateFacultyBudget(facultyId, (decimal) transaction.Amount);
            }
            else
            {
                await UpdateFacultyBudget(facultyId, (decimal) ((-1)*transaction.Amount));
            }

            return RedirectToAction(actionName: nameof(FirstProject.Controllers.FacultyController.Transactions),
                controllerName: nameof(Faculty), new{facultyId = facultyId});
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.TransactionID == id);
        }
    }
}
