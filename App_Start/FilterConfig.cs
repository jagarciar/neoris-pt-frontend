using System.Web.Mvc;
using NeorisFrontend.Filters;

namespace NeorisFrontend
{
    /// <summary>
    /// Provee la configuración de los filtros globales para la aplicación. En este caso, se registra un filtro personalizado de manejo de excepciones (GlobalExceptionFilter) que se aplicará a todas las acciones del controlador en la aplicación. Esto permite manejar de manera consistente los errores que puedan ocurrir durante la ejecución de las acciones, proporcionando una experiencia de usuario más amigable y evitando que las excepciones no controladas afecten la estabilidad del sitio web.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registra los filtros globales para la aplicación. En este método se agrega el filtro personalizado GlobalExceptionFilter a la colección de filtros, lo que garantiza que cualquier excepción no manejada en las acciones del controlador sea capturada y procesada por este filtro, permitiendo una gestión centralizada de errores en toda la aplicación.
        /// </summary>
        /// <param name="filters">Filtro global para manejo de excepciones</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Reemplazar HandleErrorAttribute por defecto con nuestro filtro personalizado
            // filters.Add(new HandleErrorAttribute());
            
            // Filtro global para manejo consistente de errores
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
