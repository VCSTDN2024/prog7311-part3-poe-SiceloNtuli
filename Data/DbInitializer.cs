using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;

namespace Prog7311_Part2.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            Console.WriteLine("Starting seed logic...");

            context.Database.Migrate();

            // Seed admin user if it doesn't exist
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                Console.WriteLine("Seeding admin user...");

                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@agrienergy.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = UserRole.Employee,
                    CreatedAt = DateTime.Now
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }

            // Farmer & product data
            var farmerData = new[]
            {
                new {
                    Username = "farmer1", Email = "farmer1@gmail.com", Name = "Green Fields Farm", Location = "Cape Town",
                    Description = "Organic vegetables and solar energy integration.", Phone = "021-555-1234",
                    Products = new[] {
                        new { Name = "Organic Tomatoes", Cat = ProductCategory.Vegetables, Desc = "Pesticide-free tomatoes", Price = 25.99M, Img = "https://media.post.rvohealth.io/wp-content/uploads/2020/09/AN313-Tomatoes-732x549-Thumb.jpg" },
                        new { Name = "Mangoes", Cat = ProductCategory.Fruits, Desc = "Juicy mangoes", Price = 50.00M, Img = "https://ichef.bbci.co.uk/images/ic/1920x1080/p06hk0h6.jpg" },
                        new { Name = "Free-Range Eggs", Cat = ProductCategory.Poultry, Desc = "Happy hen eggs", Price = 45.00M, Img = "https://cdn.britannica.com/94/151894-050-F72A5317/Brown-eggs.jpg" }
                    }
                },
                new {
                    Username = "farmer2", Email = "farmer2@gmail.com", Name = "Solar Harvest", Location = "Durban",
                    Description = "Solar-powered citrus farm.", Phone = "031-123-4567",
                    Products = new[] {
                        new { Name = "Oranges", Cat = ProductCategory.Fruits, Desc = "Sweet sun-ripened oranges", Price = 30.00M, Img = "https://images.contentstack.io/v3/assets/bltcedd8dbd5891265b/bltbe582204aeeff242/66707b1b10fde34db2a4a164/facts-about-oranges-1200x675-1.jpg?q=70&width=3840&auto=webp" },
                        new { Name = "Lemons", Cat = ProductCategory.Fruits, Desc = "Tart lemons", Price = 20.00M, Img = "https://foodprint.org/wp-content/uploads/2018/10/AdobeStock_222132175.jpeg" }
                    }
                },
                new {
                    Username = "farmer3", Email = "farmer3@gmail.com", Name = "EcoFarm", Location = "Johannesburg",
                    Description = "Eco-friendly livestock and crops.", Phone = "011-789-2345",
                    Products = new[] {
                        new { Name = "Goat Milk", Cat = ProductCategory.Dairy, Desc = "Fresh goat milk", Price = 60.00M, Img = "https://www.americandairy.com/wp-content/uploads/2016/03/glass-of-milk.jpg" },
                        new { Name = "Carrots", Cat = ProductCategory.Vegetables, Desc = "Crunchy organic carrots", Price = 22.00M, Img = "https://www.hhs1.com/hubfs/carrots%20on%20wood-1.jpg" }
                    }
                },
                new {
                    Username = "farmer4", Email = "farmer4@gmail.com", Name = "Sunrise Poultry", Location = "Bloemfontein",
                    Description = "Sustainable poultry farming.", Phone = "051-321-9999",
                    Products = new[] {
                        new { Name = "Chicken Breasts", Cat = ProductCategory.Meat, Desc = "Lean and fresh", Price = 70.00M, Img = "https://www.allrecipes.com/thmb/Z5s08uvHYI2dg5LAd0vwoA47Ngo=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/240208_simplebakedchickenbreasts_mfs_step1_0181-1-4x3-250b3f145a194aa3bab88e94e3cbae73.jpg" },
                        new { Name = "Duck Eggs", Cat = ProductCategory.Poultry, Desc = "Nutrient-rich duck eggs", Price = 55.00M, Img = "https://images.squarespace-cdn.com/content/v1/58a9fd9ed482e974647ed56b/1597974685134-K3VB9PCVH34VHCZM2OLS/holding+duck+eggs.jpg" }
                    }
                },
                new {
                    Username = "farmer5", Email = "farmer5@gmail.com", Name = "Windy Ridge", Location = "Port Elizabeth",
                    Description = "Wind-powered organic grain farm.", Phone = "041-654-8765",
                    Products = new[] {
                        new { Name = "Quinoa", Cat = ProductCategory.Grains, Desc = "Protein-packed quinoa", Price = 85.00M, Img = "https://alternativedish.com/wp-content/uploads/2023/07/popped-quinoa-500x375.jpg" },
                        new { Name = "Whole Wheat", Cat = ProductCategory.Grains, Desc = "Stone ground wheat", Price = 40.00M, Img = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQaoU3RIiRgRKAmJ1ZW7InowD2Xcvhmk1ncPA&s" }
                    }
                }
            };

            foreach (var data in farmerData)
            {
                if (context.Users.Any(u => u.Username == data.Username))
                {
                    Console.WriteLine($"? Farmer user '{data.Username}' already exists, skipping.");
                    continue;
                }

                Console.WriteLine($" Seeding user '{data.Username}' and related data...");

                var user = new User
                {
                    Username = data.Username,
                    Email = data.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("farmer123"),
                    Role = UserRole.Farmer,
                    CreatedAt = DateTime.Now
                };

                context.Users.Add(user);
                context.SaveChanges();

                var farmer = new Farmer
                {
                    Name = data.Name,
                    Location = data.Location,
                    Description = data.Description,
                    PhoneNumber = data.Phone,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now
                };

                context.Farmers.Add(farmer);
                context.SaveChanges();

                foreach (var prod in data.Products)
                {
                    context.Products.Add(new Product
                    {
                        Name = prod.Name,
                        Category = prod.Cat,
                        Description = prod.Desc,
                        Price = prod.Price,
                        ProductionDate = DateTime.Now.AddDays(-7),
                        FarmerId = farmer.Id,
                        ImageUrl = prod.Img,
                        CreatedAt = DateTime.Now
                    });
                }

                context.SaveChanges();
            }

            Console.WriteLine("Database seeding completed.");
        }
    }
}
