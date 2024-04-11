using Microsoft.AspNetCore.Mvc;
using BrickHaven.Models;

namespace BrickHaven.Components
{
    public class RoleFilterViewComponent : ViewComponent
    {
        private LoginDbContext _repo;

        // Constructor
        public RoleFilterViewComponent(LoginDbContext temp)
        {
            _repo = temp;
        }

        // This method gets specific legs based on unique lego types
        public IViewComponentResult Invoke()
        {
            // Get the lego type from the URL; store it in the ViewBag
            ViewBag.SelectedRoleFilter = RouteData?.Values["Selected Role"]; // RouteData is a dictionary that holds the URL info

            var roles = _repo.Roles
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x);

            // Return to the default view
            return View(roles);
        }
    }
}
