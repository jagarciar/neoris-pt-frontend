using System.Web.Mvc;
using NeorisFrontend.Filters;

namespace NeorisFrontend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Reemplazar HandleErrorAttribute por defecto con nuestro filtro personalizado
            // filters.Add(new HandleErrorAttribute());
            
            // Filtro global para manejo consistente de errores
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
