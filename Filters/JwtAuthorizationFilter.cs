using System.Web.Mvc;
using System.Web.Routing;
using NeorisFrontend.Services.Interfaces;
using Unity;

namespace NeorisFrontend.Filters
{
    /// <summary>
    /// Filtro de autorización para verificar JWT en la sesión
    /// Evita duplicar código de verificación en cada acción del controlador
    /// Usa Unity para resolver IAuthService
    /// </summary>
    public class JwtAuthorizationFilter : ActionFilterAttribute
    {
        /// <summary>
        /// IAuthService será inyectado por Unity cuando el filtro se use como atributo
        /// </summary>
        [Dependency]
        public IAuthService AuthService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var token = filterContext.HttpContext.Session["Token"] as string;

            // Verificar si existe el token
            if (string.IsNullOrEmpty(token))
            {
                RedirectToLogin(filterContext);
                return;
            }

            // Verificar si el token es válido y no ha expirado
            if (AuthService != null && !AuthService.IsTokenValid(token))
            {
                // Token expirado, limpiar sesión y redirigir
                filterContext.HttpContext.Session.Clear();
                filterContext.Controller.TempData["Error"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
                RedirectToLogin(filterContext);
                return;
            }

            // Token válido, permitir ejecución
            base.OnActionExecuting(filterContext);
        }

        private void RedirectToLogin(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Auth" },
                    { "action", "Login" }
                });
        }
    }
}
