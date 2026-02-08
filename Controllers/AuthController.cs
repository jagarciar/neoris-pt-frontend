using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using NeorisFrontend.Models;
using NeorisFrontend.Services.Interfaces;

namespace NeorisFrontend.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor con inyección de dependencias
        /// Unity resolverá automáticamente IAuthService
        /// </summary>
        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        // GET: Auth/Login
        public ActionResult Login()
        {
            // Si ya está autenticado, redirigir al home
            if (Session["Token"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginResponse = await _authService.LoginAsync(model);
                    
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Guardar token en sesión
                    Session["Token"] = loginResponse.Token;
                    Session["Username"] = model.Username;
                    
                    TempData["Success"] = "Login exitoso";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al conectar con la API: {ex.Message}";
                return View(model);
            }
        }

        // GET: Auth/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            TempData["Success"] = "Sesión cerrada exitosamente";
            return RedirectToAction("Login");
        }
    }
}
