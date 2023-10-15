using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVCWebAppIsmane.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Category Name")]        
        [MinLength(3, ErrorMessage = "Category name should have more than 3 characters")]
        public string Name { get; set; }

        [DisplayName("Category Description")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Category Description must be between 5 and 100 characters")]
        public string Description { get; set; }

        public List<Product> Products { get; set; }
    }
}
