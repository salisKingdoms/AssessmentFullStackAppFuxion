using Microsoft.AspNetCore.Mvc;

namespace CVSalis.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboards()
        {
            return View();
        }
    }
}
