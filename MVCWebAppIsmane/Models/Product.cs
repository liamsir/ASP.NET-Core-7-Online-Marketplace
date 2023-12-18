using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MVCWebAppIsmane.Models
{

    public class Product
    {
        public int Id { get; set; }


        [Required]
        [DisplayName("Product Name")]
        [MinLength(3, ErrorMessage = "Product name should have more than 3 characters")]
        public string Name { get; set; }
        
        
        [DisplayName("Product Description")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Product Description must be between 5 and 100 characters")]               
        public string Description { get; set; }
        

        [Required]
        [DisplayName("Product Price")]
        public double Price { get; set; }


        [Required]
        [DisplayName("Product Price")]
        [Range(10, int.MaxValue, ErrorMessage = "Quantity should be more than 10")]
        [DefaultValue(20)]
        public int QuantityStock { get; set; }



        
        [DisplayName("Product's Image")]
        [ValidateNever]
        public string Poster { get; set; }
        // add category property?


        [ForeignKey(nameof(Category))]
        public int IdCategory { get; set; }


        [ValidateNever]
        public Category Category { get; init; }


        
    }
}
