using Microsoft.AspNetCore.Mvc;

namespace moviesMVC.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
