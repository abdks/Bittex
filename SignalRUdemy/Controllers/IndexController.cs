using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
