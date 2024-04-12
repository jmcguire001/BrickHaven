using BrickHaven.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BrickHaven.Models;

namespace BrickHaven.Pages
{
    public class CartModel : PageModel
    {
        private ILegoRepository _repo;

        public CartModel(ILegoRepository temp)
        {
            _repo = temp;
        }

        public Cart? Cart { get; set; }
        public string ReturnUrl { get; set; } = "/"; 

        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        }
        
        public IActionResult OnPost(int productId, int quantity, string returnUrl)
        {
            Product product = _repo.Products
                .FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(product, quantity);
                HttpContext.Session.SetJson("cart", Cart);
            }

            return RedirectToPage(new { returnUrl = returnUrl });
        }
    }
}