using Microsoft.AspNetCore.Mvc;

namespace ParkingManagement.BackendServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
