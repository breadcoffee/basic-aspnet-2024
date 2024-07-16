using Microsoft.AspNetCore.Mvc;

namespace MyPortfolio.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
