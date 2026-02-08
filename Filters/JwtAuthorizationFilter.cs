using System;
using System.IdentityModel.Tokens.Jwt;
using System.Web.Mvc;
using System.Web.Routing;

namespace NeorisFrontend.Filters
{
    /// <summary>
    /// Filtro de autorización para verificar JWT en la sesión
    /// Evita duplicar código de verificación en cada acción del controlador
    /// </summary>
    public class JwtAuthorizationFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Permite verificar si el token JWT es válido y no ha expirado
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Verdadero si el token es válido, de lo contrario falso</returns>
        private bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();

                // Verificar si el token tiene el formato correcto
                if (!handler.CanReadToken(token))
                {
                    return false;
                }

                var jwtToken = handler.ReadJwtToken(token);

                // Verificar si el token ha expirado
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Permite la ejecución de la acción solo si el token JWT es válido y no ha expirado
        /// </summary>
        /// <param name="filterContext">Contexto del filtro de acción</param>
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
            if (!IsTokenValid(token))
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


        /// <summary>
        /// Permite redirigir al usuario a la página de login si no tiene un token válido o si el token ha expirado
        /// </summary>
        /// <param name="filterContext">Contexto del filtro de acción</param>
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
