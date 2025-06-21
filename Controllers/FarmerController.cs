using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using System.Security.Claims;

namespace Prog7311_Part2.Controllers
{
    [Authorize(Policy = "FarmerOnly")]
    public class FarmerController : Controller
    {
        private readonly AppDbContext _context;

        public FarmerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return NotFound("Farmer profile not found.");
            }

            return View(farmer);
        }


        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            
            Console.WriteLine("AddProduct POST method called");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

          
            Console.WriteLine($"Product: Name={product.Name}, Category={product.Category}, Price={product.Price}");

            try
            {
                
                ModelState.Remove("Farmer");

                if (ModelState.IsValid)
                {
                    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Console.WriteLine($"User ID claim: {userIdClaim}");

                    if (string.IsNullOrEmpty(userIdClaim))
                    {
                        Console.WriteLine("User ID claim is null or empty, redirecting to login");
                        return RedirectToAction("Login", "Account");
                    }

                    var userId = int.Parse(userIdClaim);
                    var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);
                    Console.WriteLine($"Farmer found: {farmer != null}");

                    if (farmer == null)
                    {
                        Console.WriteLine("Farmer is null, redirecting to login");
                        return RedirectToAction("Login", "Account");
                    }

                    product.FarmerId = farmer.Id;
                    product.CreatedAt = DateTime.Now;
                    product.Farmer = null;

                    Console.WriteLine($"Setting product.FarmerId to {farmer.Id}");

                    _context.Products.Add(product);
                    Console.WriteLine("Product added to context");

                    var saveResult = await _context.SaveChangesAsync();
                    Console.WriteLine($"SaveChangesAsync result: {saveResult}");

                    TempData["SuccessMessage"] = "Product added successfully!";
                    Console.WriteLine("Redirecting to Index action");
                    return RedirectToAction(nameof(Index));
                }

                
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                Console.WriteLine($"Validation errors count: {errors.Count}");
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                return View(product);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                Console.WriteLine($"Error adding product: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", "An error occurred while adding the product. Please try again.");
                return View(product);
            }
        }
        public async Task<IActionResult> ViewProducts()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(farmer.Products);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);

            if (farmer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmer.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Category = product.Category;
                    existingProduct.ProductionDate = product.ProductionDate;
                    existingProduct.ImageUrl = product.ImageUrl;

                    _context.Update(existingProduct);
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
                return RedirectToAction(nameof(ViewProducts));
            }
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}