using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCWebAppIsmane.Models;
using MVCWebAppIsmane.Repositories.IRepositories;

namespace MVCWebAppIsmane.Controllers
{
    public class AchatController : Controller
    {

        private readonly IAchatRepository _achatRepository;
        private readonly ILigneAchatRepository _ligneAchatRepository;
        private readonly IProductRepository _productRepository;



        public AchatController( IAchatRepository achatRepository,
                                ILigneAchatRepository ligneAchatRepository,
                                IProductRepository productRepository
                               )
        {
            _achatRepository = achatRepository;
            _ligneAchatRepository = ligneAchatRepository;
            _productRepository = productRepository;

        }









        /*public IActionResult Index()
        {
            return View();
        }*/


        public  async Task<IActionResult> ViewProducts(List<int> ids)
        {

            var products = await _productRepository.Get(p => ids.Contains(p.Id));
            /*List<LigneAchat> result = new List<LigneAchat>();
            result.Add(new LigneAchat(product, 1));    
*/


            return View("Index",products);
        }


        [HttpPost]
        public async Task<IActionResult> BuyProducts(List<int> productIds, List<int> quantities)
        {
            if (productIds != null && quantities != null && productIds.Count == quantities.Count)
            {
                Achat achat = new Achat();

                await _achatRepository.Create(achat); // Save the new Achat first to generate its Id

                for (int i = 0; i < productIds.Count; i++)
                {
                    int productId = productIds[i];
                    int quantite = quantities[i];

                    Product product = await _productRepository.GetById(productId);
                    if (product != null)
                    {
                        LigneAchat ligneAchat = new LigneAchat(product, quantite);
                        ligneAchat.IdAchat  = achat.Id;

                        await _ligneAchatRepository.Create(ligneAchat);
                        achat.LigneAchats.Add(ligneAchat);
                    }
                }

                await _achatRepository.Update(achat); // Update Achat with LigneAchats references

                return RedirectToAction("Index","Product", _productRepository.GetAll());
            }




            return View("Index","Home");
        }


    }
}
