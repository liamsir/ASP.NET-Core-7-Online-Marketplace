using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCWebAppIsmane.Models
{
    public class Achat
    {

        public int Id { get; set; }

        [ForeignKey(nameof(Client))]
        public int IdClient { get; set; } = 1;

        [ValidateNever]
        public Client Client { get; init; }
       


        public List<LigneAchat> LigneAchats { get; set;}


        [NotMapped]
        public double TotalPurchasePrice
        {
            get { return LigneAchats.Sum(ligne => ligne.TotalPrice); }
        }



    }
}
