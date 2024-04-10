using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BrickHaven.Models;
using BrickHaven.Models.ViewModels;

namespace BrickHaven.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ILegoRepository _repo;

        public HomeController(ILegoRepository temp)
        {
            _repo = temp;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecureMethod()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Shop(int pageNum, string? legoType, int pageSize = 5) // 'page' means something in dotnet
        {
            // How many items to show per page
            int pageSize2 = pageSize; // default pageSize is 5
            pageNum = pageNum <= 0 ? 1 : pageNum; // If pageNum is 0, set it to 1

            // This variable will hold everything from ProductListViewModel, and then be passed to Index.cshtml
            var shopInfo = new ProductListViewModel
            {
                // This info is for the legos specifically
                Products = _repo.Products
                    .Where(x => x.Category == legoType || legoType == null) // If legoType is null, show all legos
                    .OrderBy(x => x.Name)
                    .Skip((pageNum - 1) * pageSize2) // NOT SURE WHAT THIS DOES
                    .Take(pageSize2), // Only gets a certain number of legos

                // This info is for pagination
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize2,
                    TotalItems = legoType == null ? _repo.Products.Count() : _repo.Products.Where(x => x.Category == legoType).Count() // If legoType is null, show all legos, otherwise, filter specific legos
                },

                CurrentLegoType = legoType
            };

            return View(shopInfo);
        }

        [AllowAnonymous]
        public IActionResult NonSecureMethod()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ProductDetails()
        {
            return View();
        }
    }
}