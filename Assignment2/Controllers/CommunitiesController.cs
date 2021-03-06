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
    public class CommunitiesController : Controller
    {
        private readonly SchoolCommunityAdsContext _context;

        public CommunitiesController(SchoolCommunityAdsContext context)
        {
            _context = context;
        }

        public IActionResult Ads(string id)
        {
            return RedirectToAction(nameof(Index), "Ads", new { id = id });
        }

        // GET: Communities
        public async Task<IActionResult> Index(string? id)
        {
            List<Community> communities = new List<Community>();
            communities = await _context.Communities.ToListAsync();

            CommunityViewModel viewModel = new CommunityViewModel()
            {
                Communities = communities.OrderBy(x => x.Title) //get full list of Communities

            };

            if (id != null)
            {
                List<Student> Students = new List<Student>();
                List<CommunityMembership> Memberships = await _context.CommunityMemberships.ToListAsync();
                foreach (var com in Memberships)
                {
                    if (com.CommunityId == id)
                    {
                        Students.Add(await _context.Students.FindAsync(com.StudentId));
                    }
                }

                viewModel.Students = Students.OrderBy(x => x.LastName);
            }
            return View(viewModel);

        }   

        // GET: Communities/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = await _context.Communities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (community == null)
            {
                return NotFound();
            }

            return View(community);
        }

        // GET: Communities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Communities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Budget")] Community community)
        {
            if (ModelState.IsValid)
            {
                _context.Add(community);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(community);
        }

        // GET: Communities/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = await _context.Communities.FindAsync(id);
            if (community == null)
            {
                return NotFound();
            }
            return View(community);
        }

        // POST: Communities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Budget")] Community community)
        {
            if (id != community.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(community);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommunityExists(community.Id))
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
            return View(community);
        }

        // GET: Communities/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DeleteViewModel viewModel = new DeleteViewModel();

            viewModel.Community = await _context.Communities.FirstOrDefaultAsync(m => m.Id == id);
            if (viewModel.Community == null)
            {
                return NotFound();
            }

            var ads = await _context.Ads.Where(x => x.communityId == id).ToListAsync();
            if (ads.Count() == 0)
            {
                viewModel.CanDelete = true;
            }
            else
            {
                viewModel.CanDelete = false;
            }

            return View(viewModel);
        }

        // POST: Communities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var community = await _context.Communities.FindAsync(id);
            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommunityExists(string id)
        {
            return _context.Communities.Any(e => e.Id == id);
        }
    }
}
