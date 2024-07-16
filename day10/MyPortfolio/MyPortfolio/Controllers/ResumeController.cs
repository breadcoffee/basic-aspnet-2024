using Microsoft.AspNetCore.Mvc;

namespace MyPortfolio.Controllers
{
    public class ResumeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
