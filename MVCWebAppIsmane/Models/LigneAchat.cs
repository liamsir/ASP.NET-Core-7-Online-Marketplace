using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCWebAppIsmane.Models
{
    public class LigneAchat
    {
        public int Id { get; set; }
        public int quantity { get; set; }

        [ForeignKey(nameof(Achat))]
        public int IdAchat { get; set; }


        [ValidateNever]
        public Achat Achat { get; init; }


        [ForeignKey(nameof(Product))]
        public int IdProduct { get; set; }


        [ValidateNever]
        public Product Product { get; set; }
    }
}
