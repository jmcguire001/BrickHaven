using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BrickHaven.Models;
using BrickHaven.Models.ViewModels;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Security.Claims;
using SQLitePCL;

namespace BrickHaven.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ILegoRepository _repo;

        public HomeController(ILegoRepository temp, ILogger<HomeController> logger, IHostEnvironment hostEnvironment)
        {
            _repo = temp;
        }

        [AllowAnonymous]

        public async Task<IActionResult> Index()
        {
            //var productsQuery = _repo.Products.AsQueryable();

            //if (User.Identity.IsAuthenticated)
            //{
            //    // Assuming you can get the customer ID from the user's claims or related user data
            //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //    var customerId = await _repo.Customers
            //                                    .Where(u => u.Id == userId)
            //                                    .Select(u => u.CustomerId) // Assuming there's a CustomerId property
            //                                    .FirstOrDefaultAsync();

            //    if (customerId != null)
            //    {
            //        // Adjust the query to include products based on customer's orders
            //        var customerProductIds = await _repo.Orders
            //                                               .Where(o => o.CustomerId == customerId)
            //                                               .SelectMany(o => o.LineItems)
            //                                               .Select(li => li.ProductId)
            //                                               .Distinct()
            //                                               .ToListAsync();

            //        // Combine with top-rated products
            //        customerProductIds.AddRange(topRatedProductIds);

            //        productsQuery = productsQuery.Where(p => customerProductIds.Contains(p.ProductId));
            //    }
            //}
            //else
            //{
                // List of top-rated product IDs
                var topRatedProductIds = new List<int> { 27, 33, 34, 37, 24 };

                // Fetching products that match the top-rated product IDs
                var topRatedProducts = await _repo.Products
                                                  .Where(p => topRatedProductIds.Contains(p.ProductId))
                                                  .ToListAsync();

                // Passing the list of top-rated products to the view
                return View(topRatedProducts);
            //}

            //var products = await productsQuery.ToListAsync();

            //return View(products);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult SecureMethod()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Shop(int pageNum, string? legoType, string? legoColor, int pageSize=5) // 'page' means something in dotnet
        {
            // How many items to show per page
            pageNum = pageNum <= 0 ? 1 : pageNum; // If pageNum is 0, set it to 1

            // This variable will hold everything from ProductListViewModel, and then be passed to Index.cshtml
            var shopInfo = new ProductListViewModel
            {
                // This info is for the legos specifically
                Products = _repo.Products
                    .Where(x => (x.Category == legoType || legoType == null) && (x.PrimaryColor == legoColor || legoColor == null)) // If legoType is null, show all legos
                    .OrderBy(x => x.Name)
                    .Skip((pageNum - 1) * pageSize) // NOT SURE WHAT THIS DOES
                    .Take(pageSize), // Only gets a certain number of legos

                // This info is for pagination
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = legoType == null ? _repo.Products.Count() : _repo.Products.Where(x => x.Category == legoType).Count() // If legoType is null, show all legos, otherwise, filter specific legos
                },

                CurrentLegoType = legoType,
                CurrentPageSize = pageSize
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProductDetails(int id)
        {
            // Retrieve product details by calling the method from the repository
            // Product product = await _repo.GetProductByIdAsync(id);
            Product product = _repo.GetProductById(id);

            // Check if product exists
            if (product == null)
            {
                return NotFound(); // Return a 404 Not Found response
            }

            var item_recommendation_Ids = new List<int?>
            {
                product.Recommendation1,
                product.Recommendation2,
                product.Recommendation3,
                product.Recommendation4,
                product.Recommendation5,
            }.Where(id => id.HasValue).Select(id => id.Value);

            var recommendedProducts = await _repo.Products
                .Where(p => item_recommendation_Ids.Contains(p.ProductId))
                .ToListAsync();

            var viewModel = new ProductRecommendationsViewModel
            {
                Product = product,
                RecommendedProducts = recommendedProducts
            };


            // Pass the product to the view
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProductDetails(ProductRecommendationsViewModel productRecs)
        {
            if (ModelState.IsValid)
            {
                // Add the new record; this action comes from ITasksRepository and EFTasksRepository
                _repo.AddToCart(productRecs.Product);
                return View("/Cart", productRecs.Product);
            }
            else
            {
                return View();
            }

        }

        [AllowAnonymous]
        public IActionResult TEST()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId; // Pass UserId to the view through ViewData
                                         // Alternatively, you can pass it as part of a model to the view
            return View();
        }
    }
}