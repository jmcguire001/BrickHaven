using Microsoft.AspNetCore.Mvc;
using BrickHaven.Models;

namespace BrickHaven.Components
{
    public class LegoTypesViewComponent : ViewComponent
    {
        private ILegoRepository _repo;

        // Constructor
        public LegoTypesViewComponent(ILegoRepository temp)
        {
            _repo = temp;
        }

        // This method gets specific legs based on unique lego types
        public IViewComponentResult Invoke()
        {
            // Get the lego type from the URL; store it in the ViewBag
            ViewBag.SelectedLegoType = RouteData?.Values["legoType"]; // RouteData is a dictionary that holds the URL info

            var legoTypes = _repo.Legos
                .Select(x => x.LegoType)
                .Distinct()
                .OrderBy(x => x);

            // Return to the default view
            return View(legoTypes);
        }
    }
}
