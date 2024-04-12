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
using Microsoft.AspNetCore.Identity;
using System;
using BrickHaven.Infrastructure;

namespace BrickHaven.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ILegoRepository _repo;
        private readonly UserManager<Customer> _userManager;
        private LoginDbContext _context;
        
        public HomeController(UserManager<Customer> userManager,  ILegoRepository temp, LoginDbContext context)
        {
            _userManager = userManager;
            _repo = temp;
            _context = context;
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
            {
                // List of top-rated product IDs
                var topRatedProductIds = new List<int> { 27, 33, 34, 37, 24 };

                // Fetching products that match the top-rated product IDs
                var topRatedProducts = await _repo.Products
                                                  .Where(p => topRatedProductIds.Contains(p.ProductId))
                                                  .ToListAsync();

                // Passing the list of top-rated products to the view
                return View(topRatedProducts);
        }
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
                .Skip((pageNum - 1) * pageSize) // calculates which items to show for the specific page by skipping all the items on the previous pages
                .Take(pageSize), // Only gets a certain number of legos

                // This info is for pagination
                PaginationInfo = new PaginationInfo
                {
                    // Dynamically calculate the total items (basically number of page buttons needed) by the following conditions
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = (legoType == null && legoColor == null) ?
                        _repo.Products.Count() :
                        (legoType == null && legoColor != null) ?
                        _repo.Products.Where(x => x.PrimaryColor == legoColor).Count() :
                        (legoType != null && legoColor == null) ?
                        _repo.Products.Where(x => x.Category == legoType).Count() :
                        _repo.Products.Where(x => x.Category == legoType && x.PrimaryColor == legoColor).Count()

        },
                // Store in external state and pass into View
                CurrentLegoCategory = legoType,
                CurrentLegoColor = legoColor,
                CurrentPageSize = pageSize
            };

            return View(shopInfo);
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
        public async Task<IActionResult> ProductDetails(int id, string imglink)
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddOrder()
        {
            // Fetch list of customers
            var customers = _context.Users.ToList(); // Assuming _context is your DbContext

            // Retrieve the current user's ID
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var cart = HttpContext.Session.GetJson<Cart>("cart");

            float? amount = 0;

            foreach (var line in cart.Lines)
            {
                float? priceItem = line.Product.Price * line.Quantity;
                amount += priceItem;
            }

            var viewModel = new AddOrderViewModel
            {
                UserId = user.Id,
                Date = DateTime.Now,
                Weekday = DateTime.Now.DayOfWeek.ToString(),
                Time = DateTime.Now.Hour,
                TransactionCountry = user.ResidenceCountry,
                Amount = amount
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddOrder(AddOrderViewModel? viewModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the current user's ID
                var user = await _userManager.GetUserAsync(HttpContext.User);

                // Create a new order
                var order = new Order
                {
                    UserId = user.Id,
                    Date = viewModel.Date,
                    Weekday = viewModel.Weekday,
                    Time = viewModel.Time,
                    CardType = viewModel.CardType,
                    EntryMode = viewModel.EntryMode,
                    Amount = viewModel.Amount,
                    TransactionType = viewModel.TransactionType,
                    TransactionCountry = viewModel.TransactionCountry,
                    ShippingAddress = viewModel.BillingAddress,
                    Bank = viewModel.Bank
                };

                // Add the order to the database
                await _repo.AddOrder(order);
                await _context.SaveChangesAsync();

                int transactionId = order.TransactionId;

                // Store the transactionId in TempData
                TempData["TransactionId"] = transactionId;

                return RedirectToAction("ConfirmedOrder");
            }

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ConfirmedOrder()
        {
            // Retrieve cart from session storage
            Cart cart = HttpContext.Session.GetJson<Cart>("cart");

            return View(cart);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmedOrder(Cart cart)
        {
            int? transactionId = TempData["TransactionId"] as int?;
            cart = HttpContext.Session.GetJson<Cart>("cart");

            if (ModelState.IsValid)
            {
                // Loop through each item in the cart and add it to the database
                foreach (var line in cart.Lines)
                {
                    var lineItem = new LineItem
                    {
                        TransactionId = transactionId, // Set the TransactionId from the parameter
                        ProductId = line.Product.ProductId,
                        Quantity = line.Quantity
                    };

                    await _repo.AddLineItem(lineItem);
                }

                // Save changes to the database after all line items have been added
                await _context.SaveChangesAsync();

                // Clear the cart after confirming the order
                cart.Clear();

                // Redirect to the shop or wherever appropriate
                return RedirectToAction("Shop", "Home");
            }

            // If ModelState is not valid, return the view with the cart
            return View("ConfirmedOrder", cart);
        }


    }
}