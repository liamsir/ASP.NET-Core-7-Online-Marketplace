using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCWebAppIsmane.Models
{
    public class Achat
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Client))]
        public int IdClient { get; set; }


        [ValidateNever]
        public Client Client { get; init; }
       


        public List<LigneAchat> LigneAchats { get; set;}
    }
}
