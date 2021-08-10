using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;

namespace Assignment2.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolCommunityAdsContext _context;

        public StudentsController(SchoolCommunityAdsContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(int? id)
        {
            List<Student> students = new List<Student>();
            students = await _context.Students.ToListAsync();

            StudentViewModel viewModel = new StudentViewModel
            {
                Students = students.OrderBy(x => x.LastName) //get full list of studenets

            };
           
            if (id != null)
            {
                List<Community> Communities = new List<Community>();
                List<CommunityMembership> Memberships = await _context.CommunityMemberships.ToListAsync();
                foreach(var mem in Memberships)
                {
                    if (mem.StudentId == id)
                    {
                        Communities.Add(await _context.Communities.FindAsync(mem.CommunityId));
                    }
                }

                viewModel.Communities = Communities.OrderBy(x => x.Title);
            }
            return View(viewModel);
        }

        

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> EditMemberships(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new EditMembershipViewModel
            {
                Communities = await _context.Communities.ToListAsync(),
                Student = student,
                CommunityMemberships = await _context.CommunityMemberships.Where(i => i.StudentId == id).ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Registration(int id, string comid, bool register)
        {
            CommunityMembership mem = new CommunityMembership
            {
                StudentId = id,
                CommunityId = comid
            };
            if (register)
            {
                _context.CommunityMemberships.Add(mem);
            }
            else
            {
                mem = await _context.CommunityMemberships.FindAsync(id, comid);
                _context.Remove(mem);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditMemberships), "Students", new { id = id });
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (id != student.Id)
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
                    if (!StudentExists(student.Id))
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

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
