using Microsoft.AspNetCore.Mvc;
using BrickHaven.Models;

namespace BrickHaven.Components
{
    public class LegoColorsViewComponent : ViewComponent
    {
        private ILegoRepository _repo;

        // Constructor
        public LegoColorsViewComponent(ILegoRepository temp)
        {
            _repo = temp;
        }

        // This method gets specific legs based on unique lego types
        public IViewComponentResult Invoke()
        {
            // Get the lego type from the URL; store it in the ViewBag
            ViewBag.SelectedLegoColor = RouteData?.Values["Primary Color"]; // RouteData is a dictionary that holds the URL info

            var legoColors = _repo.Products
                .Select(x => x.PrimaryColor)
                .Distinct()
                .OrderBy(x => x);

            // Return to the default view
            return View(legoColors);
        }
    }
}
