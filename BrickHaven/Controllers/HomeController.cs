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
        public IActionResult Shop(int pageNum, string? legoType) // 'page' means something in dotnet
        {
            int pageSize = 1; // How many items to show per page
            pageNum = pageNum <= 0 ? 1 : pageNum; // If pageNum is 0, set it to 1

            // This variable will hold everything from LegosListViewModel, and then be passed to Index.cshtml
            var shopInfo = new LegosListViewModel
            {
                // This info is for the legos specifically
                Legos = _repo.Legos
                    .Where(x => x.LegoType == legoType || legoType == null) // If legoType is null, show all legos
                    .OrderBy(x => x.LegoName)
                    .Skip((pageNum - 1) * pageSize) // NOT SURE WHAT THIS DOES
                    .Take(pageSize), // Only gets a certain number of legos

                // This info is for pagination
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = legoType == null ? _repo.Legos.Count() : _repo.Legos.Where(x => x.LegoType == legoType).Count() // If legoType is null, show all legos, otherwise, filter specific legos
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
    }
}