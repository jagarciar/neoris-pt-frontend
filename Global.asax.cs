using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NeorisFrontend
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            // Configurar Unity para inyección de dependencias
            // DEBE ser antes de RegisterGlobalFilters para que los filtros también usen DI
            UnityConfig.RegisterComponents();
            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            
            // Log del error para debugging
            System.Diagnostics.Debug.WriteLine("===== APPLICATION ERROR =====");
            System.Diagnostics.Debug.WriteLine($"Error: {exception?.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {exception?.StackTrace}");
            
            if (exception?.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"InnerException: {exception.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"InnerException StackTrace: {exception.InnerException.StackTrace}");
            }
            
            System.Diagnostics.Debug.WriteLine("=============================");
        }
    }
}
