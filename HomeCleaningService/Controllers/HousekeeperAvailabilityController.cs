using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    public class HousekeeperAvailabilityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
