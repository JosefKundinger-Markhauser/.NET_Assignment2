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
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using System.IO;
using Azure;

namespace Assignment2.Controllers
{
    public class AdsController : Controller
    {
        private readonly SchoolCommunityAdsContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public AdsController(SchoolCommunityAdsContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Ads
        public async Task<IActionResult> Index(string id)
        {
            AdsViewModel viewModel = new AdsViewModel
            {
                Ads = await _context.Ads.Where(i => i.communityId == id).ToListAsync(),
                Community = await _context.Communities.FindAsync(id)
            };
            

            return View(viewModel);
        }

        // GET: Ads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .FirstOrDefaultAsync(m => m.AdId == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        // GET: Ads/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile adsFile, string id)
        {
            //Container: ads

            if(adsFile == null)
            {
                return View("Error");
            }

            BlobContainerClient containerClient;
            string containerName = "ads";
            string filename = "";
            string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };

            //check file name extension
            string ext = Path.GetExtension(adsFile.FileName).ToLowerInvariant();
            if(string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return View("Error");
            }
            else
            {
                filename = Path.GetRandomFileName();
            }

            // Get container to hold the blob
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            // Create and store blob ads
            try
            {
                var blockBlob = containerClient.GetBlobClient(filename);
                if(await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using(var memoryStream = new MemoryStream())
                {
                    await adsFile.CopyToAsync(memoryStream);

                    memoryStream.Position = 0;

                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                var image = new Ad();
                image.Url = blockBlob.Uri.AbsoluteUri;
                image.FileName = adsFile.FileName;
                image.communityId = id;

                _context.Ads.Add(image);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            return RedirectToAction("Index", "Ads", new { id = id });
        }

        public async Task<IActionResult> Create(string id)
        {
            return View(await _context.Communities.FindAsync(id));
        }

        // GET: Ads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            return View(ad);
        }

        // POST: Ads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdId,FileName,Url")] Ad ad)
        {
            if (id != ad.AdId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdExists(ad.AdId))
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
            return View(ad);
        }

        // GET: Ads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .FirstOrDefaultAsync(m => m.AdId == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ad = await _context.Ads.FindAsync(id);
            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdExists(int id)
        {
            return _context.Ads.Any(e => e.AdId == id);
        }
    }
}
