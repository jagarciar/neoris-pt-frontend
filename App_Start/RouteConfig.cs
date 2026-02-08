using System.Web.Mvc;
using System.Web.Routing;

namespace NeorisFrontend
{
    /// <summary>
    /// Provee la configuración de las rutas para la aplicación. En este caso, se define una ruta predeterminada que sigue el patrón "{controller}/{action}/{id}", donde "controller" es el nombre del controlador, "action" es el nombre de la acción dentro del controlador y "id" es un parámetro opcional. La ruta predeterminada se establece para que, si no se especifica ningún controlador o acción en la URL, se dirija a la acción "Login" del controlador "Auth". Esto permite que los usuarios sean redirigidos automáticamente a la página de inicio de sesión cuando acceden a la raíz del sitio web sin especificar una ruta específica.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Configura las rutas para la aplicación. En este método se ignoran las rutas que coincidan con el patrón "{resource}.axd/{*pathInfo}" para evitar que las solicitudes a recursos como archivos de seguimiento o de diagnóstico sean procesadas por el enrutamiento de MVC. Luego, se define una ruta predeterminada que sigue el patrón "{controller}/{action}/{id}", con valores predeterminados para el controlador ("Auth"), la acción ("Login") y un parámetro opcional "id". Esto asegura que, si un usuario accede a la raíz del sitio web sin especificar una ruta, será redirigido automáticamente a la página de inicio de sesión del controlador de autenticación.
        /// </summary>
        /// <param name="routes">Lista de rutas a configurar</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Auth", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
