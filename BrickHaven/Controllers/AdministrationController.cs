using BrickHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrickHaven.Models.ViewModels;
using SQLitePCL;

namespace BrickHaven.Controllers
{
    [Authorize(Roles = "Admin")] // Adding another role to this is an OR; place another authorize underneath to be AND
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Customer> _userManager;

        //private readonly UserImporter _userImporter;

        private LoginDbContext _context;
        private readonly ILegoRepository _legoRepository;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<Customer> userManager, LoginDbContext temp, ILegoRepository legoRepository)// , UserImporter userImporter)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            // _userImporter = userImporter;

            _context = temp;
            _legoRepository = legoRepository;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the role already exists
                bool roleExists = await _roleManager.RoleExistsAsync(roleModel?.RoleName);
                if (roleExists)
                {
                    ModelState.AddModelError("", "Role Already Exists");
                }
                else
                {
                    // Create the role
                    // We just need to specify a unique role name to create a new role
                    IdentityRole identityRole = new IdentityRole
                    {
                        Name = roleModel?.RoleName
                    };

                    // Saves the role in the underlying AspNetRoles table
                    IdentityResult result = await _roleManager.CreateAsync(identityRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles", "Administration");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(roleModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();

            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            //First Get the role information from the database
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                // Handle the scenario when the role is not found
                return View("Error");
            }

            //Populate the EditRoleViewModel from the data retrieved from the database
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
                // You can add other properties here if needed
            };

            //Initialize the Users Property to avoid Null Reference Exception while Add the username
            model.Users = new List<string>();

            // Retrieve all the Users
            foreach (var user in _userManager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. 
                // This model object is then passed to the view for display
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    // Handle the scenario when the role is not found
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    role.Name = model.RoleName;
                    // Update other properties if needed

                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles"); // Redirect to the roles list
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;


            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            ViewBag.RollName = role.Name;
            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult? result;


                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    //If IsSelected is true and User is not already in this role, then add the user
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    //If IsSelected is false and User is already in this role, then remove the user
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    //Don't do anything simply continue the loop
                    continue;
                }

                //If you add or remove any user, please check the Succeeded of the IdentityResult
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { roleId = roleId });
                }
            }

            return RedirectToAction("EditRole", new { roleId = roleId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                // Role not found, handle accordingly
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                // Wrap the code in a try/catch block
                try
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        // Role deletion successful
                        return RedirectToAction("ListRoles"); // Redirect to the roles list page
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    // If we reach here, something went wrong, return to the view
                    return View("ListRoles", await _roleManager.Roles.ToListAsync());
                }
                // If the exception is DbUpdateException, we know we are not able to
                // delete the role as there are users in the role being deleted
                catch (DbUpdateException ex)
                {
                    // Log the exception to a file. 
                    ViewBag.Error = ex.Message;

                    // Pass the ErrorTitle and ErrorMessage that you want to show to the user using ViewBag.
                    // The Error view retrieves this data from the ViewBag and displays to the user.
                    ViewBag.ErrorTitle = $"{role.Name} Role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} Role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete";
                    return View("Error");
                    throw;
                }
            }
        }

        public IActionResult ListOrders(string? transactionType, int pageNum = 1, int pageSize = 10)
        {
            var orderList = new ListOrdersViewModel
            {
                Orders = _legoRepository.Orders.OrderBy(o => o.TransactionId).Skip((pageNum - 1) * pageSize).Take(pageSize),
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _legoRepository.Orders.Count() == 0 ? 1 : _legoRepository.Orders.Count()
                },

                CurrentPageSize = pageSize,
                TransactionType = transactionType
            };

            // var users = _userManager.Users;
            return View(orderList);

        }

        [HttpGet]
        public IActionResult ListUsers(string? roleFilter, int pageNum = 1, int pageSize = 10)
        {
            var userList = new ListUsersViewModel
            {
                Orders = _legoRepository.Orders.OrderBy(o => o.TransactionId).Skip((pageNum - 1) * pageSize).Take(pageSize),

                Customers = _context.Users.OrderBy(u => u.UserName).Skip((pageNum - 1) * pageSize).Take(pageSize),
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _context.Users.Count() == 0 ? 1 : _context.Users.Count()
                },

                CurrentPageSize = pageSize,
                Role = roleFilter
            };

            // var users = _userManager.Users;
            return View(userList);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string UserId, int pageNum = 1, int pageSize = 10)
        {
            //First Fetch the User Details by UserId
            var user = await _userManager.FindByIdAsync(UserId);

            var editUserList = new ListUsersViewModel
            {
                Customers = _context.Users.OrderBy(u => u.UserName).Skip((pageNum - 1) * pageSize).Take(pageSize),
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _context.Users.Count()
                },

                CurrentPageSize = pageSize
            };

            //Check if User Exists in the Database
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            //Store all the information in the EditUserViewModel instance
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles
            };

            //Pass the Model to the View
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                //Populate the user instance with the data from EditUserViewModel
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Birthday = model.Birthday;
                user.ResidenceCountry = model.ResidenceCountry;
                user.Gender = model.Gender;

                //UpdateAsync Method will update the user data in the AspNetUsers Identity table
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    //Once user data updated redirect to the ListUsers view
                    return RedirectToAction("ListUsers");
                }
                else
                {
                    //In case any error, stay in the same view and show the model validation error
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            // Fetch the user you want to delete
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                // Handle the case where the user wasn't found
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            // Attempt to delete the user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // Handle a successful delete
                return RedirectToAction("ListUsers");
            }
            else
            {
                // Handle failure
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                // Return to the view with errors
                return View("ListUsers");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string UserId)
        {
            //First Fetch the User Information from the Identity database by user Id
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                //handle if User Not Found in the Database
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            //Store the UserId in the ViewBag which is required while updating the Data
            //Store the UserName in the ViewBag which is used for displaying purpose
            ViewBag.UserId = UserId;
            ViewBag.UserName = user.UserName;

            //Create a List to Hold all the Roles Information
            var model = new List<AllRolesViewModel>();

            //Loop Through Each role and populate the model 
            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                var userRolesViewModel = new AllRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                //Check if the Role is already assigned to this user
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                //Add the userRolesViewModel to the model
                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<AllRolesViewModel> model, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            //fetch the list of roles the specified user belongs to
            var roles = await _userManager.GetRolesAsync(user);

            //Then remove all the assigned roles for this user
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            List<string> RolesToBeAssigned = model.Where(x => x.IsSelected).Select(y => y.RoleName).ToList();

            //If At least 1 Role is assigned, Any Method will return true
            if (RolesToBeAssigned.Any())
            {
                //add a user to multiple roles simultaneously

                result = await _userManager.AddToRolesAsync(user, RolesToBeAssigned);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot Add Selected Roles to User");
                    return View(model);
                }
            }

            return RedirectToAction("ManageUserRoles", new { UserId = UserId });
        }

        [HttpGet]
        public IActionResult ListProducts(string? legoType, int pageNum = 1, int pageSize = 10)
        {
            pageNum = pageNum <= 0 ? 1 : pageNum; // If pageNum is 0, set it to 1

            var productList = new ListProductsViewModel
            {
                Products = _legoRepository.Products.Where(x => (x.Category == legoType || legoType == null)) // If legoType is null, show all legos
                    .OrderBy(p => p.Name).Skip((pageNum - 1) * pageSize).Take(pageSize),
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = legoType == null ? _legoRepository.Products.Count() : _legoRepository.Products.Where(x => x.Category == legoType).Count() // If legoType is null, show all legos, otherwise, filter specific legos
                },

                CurrentPageSize = pageSize,
                CurrentLegoType = legoType
            };

            // var users = _userManager.Users;
            return View(productList);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(string? category, int ProductId, int pageNum = 1, int pageSize = 10)
        {
            //First Fetch the User Details by UserId
            var product = await _legoRepository.Products.FirstOrDefaultAsync(p => p.ProductId == ProductId);

            var editProductList = new ListProductsViewModel
            {
                // Products = _legoRepository.Products.OrderBy(p => p.Name).Skip((pageNum - 1) * pageSize).Take(pageSize),
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _legoRepository.Products.Count()
                },

                CurrentPageSize = pageSize,
                CurrentLegoType = category
            };

            //Check if User Exists in the Database
            if (product == null)
            {
                ViewBag.ErrorMessage = $"{product.Name} cannot be found";
                return View("NotFound");
            }

            //Store all the information in the EditUserViewModel instance
            var model = new EditProductViewModel
            {
                Id = product.ProductId,
                Name = product.Name,
                Year = product.Year,
                NumParts = product.NumParts,
                Price = product.Price,
                ImgLink = product.ImgLink,
                PrimaryColor = product.PrimaryColor,
                SecondaryColor = product.SecondaryColor,
                Description = product.Description,
                Category = product.Category
            };

            //Pass the Model to the View
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductViewModel model)
        {
            var product = await _legoRepository.Products.FirstOrDefaultAsync(p => p.ProductId == model.Id);

            if (product == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                product.ProductId = model.Id;
                product.Name = model.Name;
                product.Year = model.Year;
                product.NumParts = model.NumParts;
                product.Price = model.Price;
                product.ImgLink = model.ImgLink;
                product.PrimaryColor = model.PrimaryColor;
                product.SecondaryColor = model.SecondaryColor;
                product.Description = model.Description;
                product.Category = model.Category;

                await _legoRepository.UpdateProductAsync(product);
                await _legoRepository.SaveChangesAsync();

                //Once user data updated redirect to the ListUsers view
                return RedirectToAction("EditProduct", "Administration", new { category = product.Category, ProductId = product.ProductId });
            }

            return View(model);
        }

        // Method to delete a product
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string? category, Product product)
        {
            if (product == null)
            {
                // Handle the case where the product wasn't found
                ViewBag.ErrorMessage = $"{product.Name} cannot be found";
                return View("NotFound");
            }

            // Attempt to delete the product
            await _legoRepository.DeleteProductAsync(product);
            await _legoRepository.SaveChangesAsync();

            // Handle a successful delete
            return RedirectToAction("ListProducts", new { productCategory = category });
        }

        // GET method for creating a new product
        [HttpGet]
        public IActionResult CreateProduct(string? category)
        {
            return View();
        }

        // POST method for creating a new product
        [HttpPost]
        public async Task<IActionResult> CreateProduct(EditProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create the product
                Product product = new Product
                {
                    Name = model.Name,
                    Year = model.Year,
                    NumParts = model.NumParts,
                    Price = model.Price,
                    ImgLink = model.ImgLink,
                    PrimaryColor = model.PrimaryColor,
                    SecondaryColor = model.SecondaryColor,
                    Description = model.Description,
                    Category = model.Category
                };

                // Add the product to the database
                await _legoRepository.AddProduct(product);
                await _legoRepository.SaveChangesAsync();

                // Redirect to the list of products
                return RedirectToAction("ListProducts", "Administration");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListOrders(string? transactionType, int pageNum = 1, int pageSize = 10)
        {
            var orderList = new ListOrdersViewModel
            {
                Orders = _legoRepository.Orders.OrderBy(o => o.TransactionId).Skip((pageNum - 1) * pageSize).Take(pageSize),
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = _legoRepository.Orders.Count() == 0 ? 1 : _legoRepository.Orders.Count()
                },

                CurrentPageSize = pageSize,
                TransactionType = transactionType
            };

            // var users = _userManager.Users;
            return View(orderList);

         }

        //// Action method to display a view where the user can trigger CSV import
        //[HttpGet]
        //public IActionResult ImportUsersFromCsv()
        //{
        //    return View();
        //}

        //// Action method to handle the CSV import
        //[HttpPost]
        //public async Task<IActionResult> ImportUsersFromCsv(IFormFile file)
        //{
        //    // Ensure a file was provided
        //    if (file == null || file.Length == 0)
        //    {
        //        ModelState.AddModelError("", "Please select a file to import.");
        //        return View();
        //    }

        //    // Check if the file is a CSV file
        //    if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        //    {
        //        ModelState.AddModelError("", "Please select a CSV file.");
        //        return View();
        //    }

        //    try
        //    {
        //        // Get the path to the temporary file on the server
        //        var filePath = Path.GetTempFileName();

        //        // Copy the uploaded file to the temporary file
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // Call the method to import users from the CSV file
        //        await _userImporter.ImportUsersFromCsvAsync(filePath);

        //        // Optionally, delete the temporary file
        //        System.IO.File.Delete(filePath);

        //        // Redirect to a success page or return a success message
        //        return RedirectToAction("Index", "Home");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and display an error message
        //        ModelState.AddModelError("", "An error occurred while importing users from CSV.");
        //        // Log the exception
        //        // Log.Error("An error occurred while importing users from CSV.", ex);
        //        return View();
        //    }
        //}

        //// Action method to display a view where the user can trigger CSV import
        //[HttpGet]
        //public IActionResult ImportUsersFromCsv()
        //{
        //    return View();
        //}

        //// Action method to handle the CSV import
        //[HttpPost]
        //public async Task<IActionResult> ImportUsersFromCsv(IFormFile file)
        //{
        //    // Ensure a file was provided
        //    if (file == null || file.Length == 0)
        //    {
        //        ModelState.AddModelError("", "Please select a file to import.");
        //        return View();
        //    }

        //    // Check if the file is a CSV file
        //    if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        //    {
        //        ModelState.AddModelError("", "Please select a CSV file.");
        //        return View();
        //    }

        //    try
        //    {
        //        // Get the path to the temporary file on the server
        //        var filePath = Path.GetTempFileName();

        //        // Copy the uploaded file to the temporary file
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // Call the method to import users from the CSV file
        //        await _userImporter.ImportUsersFromCsvAsync(filePath);

        //        // Optionally, delete the temporary file
        //        System.IO.File.Delete(filePath);

        //        // Redirect to a success page or return a success message
        //        return RedirectToAction("Index", "Home");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception and display an error message
        //        ModelState.AddModelError("", "An error occurred while importing users from CSV.");
        //        // Log the exception
        //        // Log.Error("An error occurred while importing users from CSV.", ex);
        //        return View();
        //    }
        //}
    }
}