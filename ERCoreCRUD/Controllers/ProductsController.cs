using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ERCoreCRUD.Data;
using ERCoreCRUD.Models.db;

namespace ERCoreCRUD.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NorthwindContext _db;

        public ProductsController(NorthwindContext db)
        {
            _db = db;
        }

        private bool ProductsExists(int id)
        {
            return _db.Products.Any(e => e.ProductId == id);
        }

        public async Task<IActionResult> Index()
        {
            var products = _db.Products
                              .Include(p => p.Category)
                              .Include(p => p.Supplier);
            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _db.Products
                                    .Include(p => p.Category) // join table
                                    .Include(p => p.Supplier) // join table
                                    .FirstOrDefaultAsync(m => m.ProductId == id);

            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_db.Suppliers, "SupplierId", "CompanyName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product products)
        {
            if (ModelState.IsValid)
            {
                _db.Add(products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_db.Suppliers, "SupplierId", "CompanyName");
            return View(products);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_db.Suppliers, "SupplierId", "CompanyName");
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ProductId, ProductName, SupplierId, CategoryId, QuantityPerUnit, UnitPrice, UnitInStock, UnitOnOrder, ReorderLevel, Discontinued")] Product products)
        {
            if (id != products.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(products);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductId))
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

            ViewData["CategoryId"] = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_db.Suppliers, "SupplierId", "CompanyName");
            return View(products);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _db.Products
                                    .Include(p => p.Category) // join table
                                    .Include(p => p.Supplier) // join table
                                    .FirstOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _db.Products.FindAsync(id);

            // Check if the product is found
            if (products == null)
            {
                return NotFound();
            }

            _db.Products.Remove(products);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult SearchProducts(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return View("SearchProducts", _db.Products.ToList());
            }
            else
            {
                return View("SearchProducts", _db.Products.Where(p => p.ProductName.Contains(q)).ToList());
            }
        }

    }
}