using System.Web.Mvc;

namespace NeorisFrontend.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/Index (Dashboard)
        public ActionResult Index()
        {
            // Verificar que el usuario esté autenticado
            if (Session["Token"] == null)
            {
                TempData["Error"] = "Debe iniciar sesión para acceder al dashboard";
                return RedirectToAction("Login", "Auth");
            }

            // Pasar datos a la vista
            ViewBag.Username = Session["Username"];
            ViewBag.Token = Session["Token"];

            return View();
        }
    }
}
