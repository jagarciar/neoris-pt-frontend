using System.Web.Optimization;

namespace NeorisFrontend
{
    /// <summary>
    /// Configuraciión de los estilos y scripts del sitio web
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Permite registrar los bundles de scripts y estilos para optimizar la carga de recursos en el sitio web. Se definen bundles para jQuery, Modernizr, los scripts personalizados del sitio y los estilos CSS. Al registrar estos bundles, se mejora el rendimiento del sitio al reducir el número de solicitudes HTTP necesarias para cargar los recursos, ya que los archivos incluidos en cada bundle se combinan y minifican automáticamente por el framework de optimización de ASP.NET.
        /// </summary>
        /// <param name="bundles">Lista de los bundles existentes en la aplicación</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/site.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Site.css"));
        }
    }
}
