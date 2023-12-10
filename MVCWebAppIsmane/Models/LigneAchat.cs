using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCWebAppIsmane.Models
{
    public class LigneAchat
    {

     
        public int Id { get; set; }
        public int quantity { get; set; } = 1;

        [ForeignKey(nameof(Achat))]
        public int IdAchat { get; set; }



        [ValidateNever]
        public Achat Achat { get; init; }


        [ForeignKey(nameof(Product))]
        public int IdProduct { get; set; }


        [ValidateNever]
        public Product Product { get; set; }

        public LigneAchat()
        {            
        }

        public LigneAchat(Product product,int quantite)
        {
            Product = product;
            quantity = quantite;
        }


        [NotMapped]
        public double TotalPrice => quantity * Product.Price;


    }
}
