using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCWebAppIsmane.Models
{
    public class Achat
    {

        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string IdUser { get; set; }

        [ValidateNever]
        public User User { get; init; }
       


        public List<LigneAchat> LigneAchats { get; set; }

        /*
                [NotMapped]
                public double TotalPurchasePrice
                {
                    get { return LigneAchats.Sum(ligne => ligne.TotalPrice); }
                }*/



    }
}
