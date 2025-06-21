using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog7311_Part2.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public ProductCategory Category { get; set; }

        [Required(ErrorMessage = "Production date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Production Date")]
        public DateTime ProductionDate { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Required]
        public int FarmerId { get; set; }

        [ForeignKey("FarmerId")]
        public virtual Farmer? Farmer { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public enum ProductCategory
    {
        Vegetables,
        Fruits,
        Dairy,
        Meat,
        Poultry,
        Grains,
        Herbs,
        GreenEnergy,
        Other
    }
}