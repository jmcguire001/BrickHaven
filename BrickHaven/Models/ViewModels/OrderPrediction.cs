using Microsoft.AspNetCore;

namespace BrickHaven.Models.ViewModels
{
    public class OrderPrediction
    {
        public Order Orders { get; set; }
        public string Prediction { get; set; }

    }
}
