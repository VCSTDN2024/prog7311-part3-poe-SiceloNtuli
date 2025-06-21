using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using Prog7311_Part2.Models.ViewModels;

namespace Prog7311_Part2.Controllers
{
    [Authorize(Policy = "EmployeeOnly")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var farmers = await _context.Farmers
                .Include(f => f.Products)
                .ToListAsync();

            return View(farmers);
        }


        public IActionResult AddFarmer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(RegisterFarmerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already taken");
                    return View(model);
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered");
                    return View(model);
                }

                // Create user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = UserRole.Farmer,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create farmer profile
                var farmer = new Farmer
                {
                    Name = model.FarmerName,
                    Location = model.Location,
                    Description = model.Description,
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now
                };

                _context.Farmers.Add(farmer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> ViewFarmerProducts(int id)
        {
            var farmer = await _context.Farmers
                .Include(f => f.Products)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farmer == null)
            {
                return NotFound();
            }

            ViewBag.FarmerName = farmer.Name;
            return View(farmer.Products);
        }

        public async Task<IActionResult> FilterProducts()
        {
            var farmers = await _context.Farmers.ToListAsync();
            ViewBag.Farmers = farmers;
            ViewBag.Categories = Enum.GetValues(typeof(ProductCategory));

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FilterProducts(int? farmerId, DateTime? startDate, DateTime? endDate, ProductCategory? category)
        {
            var query = _context.Products.AsQueryable();

            if (farmerId.HasValue)
            {
                query = query.Where(p => p.FarmerId == farmerId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate <= endDate.Value);
            }

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            var products = await query.Include(p => p.Farmer).ToListAsync();
            
            var farmers = await _context.Farmers.ToListAsync();
            ViewBag.Farmers = farmers;
            ViewBag.Categories = Enum.GetValues(typeof(ProductCategory));
            
            // Save the filter criteria in ViewBag to maintain form state
            ViewBag.SelectedFarmerId = farmerId;
            ViewBag.SelectedStartDate = startDate;
            ViewBag.SelectedEndDate = endDate;
            ViewBag.SelectedCategory = category;

            return View("FilterResults", products);
        }
    }
} 