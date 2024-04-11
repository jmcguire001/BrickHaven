﻿using BrickHaven.Models;
using Microsoft.AspNetCore.Mvc;

namespace BrickHaven.Components
{
    public class OrderTypesViewComponent : ViewComponent
    {
        private ILegoRepository _repo;

        public OrderTypesViewComponent(ILegoRepository temp)
        {
            _repo = temp;
        }

        // This method gets specific legs based on unique lego types
        public IViewComponentResult Invoke()
        {
            // Get the lego type from the URL; store it in the ViewBag
            ViewBag.SelectedLegoType = RouteData?.Values["TransactionType"]; // RouteData is a dictionary that holds the URL info

            var transactionTypes = _repo.Orders
                .Select(x => x.TransactionType)
                .Distinct()
                .OrderBy(x => x);

            // Return to the default view
            return View(transactionTypes);
        }
    }
}
