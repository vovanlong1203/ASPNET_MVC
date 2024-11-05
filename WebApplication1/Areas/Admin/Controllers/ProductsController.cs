using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Db;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly OnlineShopContext _context;

        public ProductsController(OnlineShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        public IActionResult DeleteGallery(int id)
        {
            var gallery = _context.ProductGaleries.FirstOrDefault(g => g.Id == id);
            if (gallery == null)
                return NotFound();
            string d = Directory.GetCurrentDirectory();
            string fn = d + "\\wwwroot\\images\\products" + gallery.ImageName;

            if (System.IO.File.Exists(fn))
            {
                System.IO.File.Delete(fn);
            }
            _context.Remove(gallery);
            _context.SaveChanges();

            return Redirect("edit/" + gallery.ProductId);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,FullDesc,Discount,ImageName,Qty,Tags,VideoUrl,Price")] Product product, IFormFile? MainImage, IFormFile[]? GalleryImages)
        {
            if (ModelState.IsValid)
            {
                //=========Save Image==============
                if (MainImage != null)
                {
                    product.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(MainImage.FileName);
                    string fn;
                    fn = Directory.GetCurrentDirectory();
                    string ImagePath = Path.Combine(fn + "\\wwwroot\\images\\products\\" + product.ImageName);

                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                        MainImage.CopyTo(stream);
                    }
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                //=================================

                //=======save gallery============

                if (GalleryImages != null)
                {
                    foreach (var item in GalleryImages)
                    {
                        var newGallery = new ProductGalery();
                        newGallery.ProductId = product.Id;

                        newGallery.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(item.FileName);
                        string fn;
                        fn = Directory.GetCurrentDirectory();
                        string ImagePath = Path.Combine(fn+ "\\wwwroot\\images\\products\\" + newGallery.ImageName);

                        using (var stream = new FileStream(ImagePath, FileMode.Create))
                        {
                             item.CopyTo(stream); 
                        }

                        _context.ProductGaleries.Add(newGallery);
                    }
                }
                await _context.SaveChangesAsync();


                //===============================
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["gallery"] = _context.ProductGaleries.Where(x => x.ProductId == product.Id).ToList();

            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,FullDesc,Discount,ImageName,Qty,Tags,VideoUrl,Price")] Product product, IFormFile? MainImage, IFormFile[]? GalleryImages)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Save main image
                    if (MainImage != null)
                    {
                        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/");
                        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(MainImage.FileName);
                        string imagePath = Path.Combine(directoryPath, imageName);

                        string oldImagePath = Path.Combine(directoryPath, product.ImageName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await MainImage.CopyToAsync(stream);
                        }

                        product.ImageName = imageName; // Save only the file name
                    }

                    // Save gallery images
                    if (GalleryImages != null)
                    {
                        foreach (var item in GalleryImages)
                        {
                            string galleryImageName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            string galleryImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/", galleryImageName);

                            using (var stream = new FileStream(galleryImagePath, FileMode.Create))
                            {
                                await item.CopyToAsync(stream);
                            }

                            var galleryItem = new ProductGalery
                            {
                                ProductId = product.Id,
                                ImageName = galleryImageName // Save only the file name
                            };

                            _context.ProductGaleries.Add(galleryItem);
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }


        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {

                // ===================delete image================

                string d = Directory.GetCurrentDirectory();
                string fn = d + "\\wwwroot\\images\\products\\";
                string mainImagePath = fn + product.ImageName;
                if (System.IO.File.Exists(mainImagePath))
                {
                    System.IO.File.Delete(mainImagePath);
                }

                // ============delete image ================
                var galleries = _context.ProductGaleries.Where(x => x.ProductId == id).ToList();
                
                if (galleries != null)
                {
                    foreach (var item in galleries)
                    {
                        string galleryImagePath = fn + item.ImageName;
                        if (System.IO.File.Exists(galleryImagePath))
                        {
                            System.IO.File.Delete(galleryImagePath);
                        }
                    }
                    _context.ProductGaleries.RemoveRange(galleries);
                }


                //================================
                _context.Products.Remove(product);
            }

            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
