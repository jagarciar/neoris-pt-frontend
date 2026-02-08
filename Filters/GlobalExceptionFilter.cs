using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NeorisFrontend.Filters
{
    /// <summary>
    /// Filtro global para manejo de excepciones no controladas
    /// Proporciona respuestas consistentes y logging centralizado
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Captura excepciones de tipo ApplicationException para mostrar mensajes amigables al usuario
        /// </summary>
        /// <param name="filterContext">Contexto de la excepción</param>
        /// <param name="exception">Excepción de aplicación</param>
        private void HandleApplicationException(ExceptionContext filterContext, Exception exception)
        {
            // ApplicationException contiene errores de la capa de servicios/API
            var errorMessage = exception.Message.Contains("Error al")
                ? exception.Message
                : "Ha ocurrido un error al procesar tu solicitud";

            filterContext.Controller.TempData["Error"] = errorMessage;

            // Redirigir a la acción anterior o al home
            var routeData = filterContext.RouteData;
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", routeData.Values["controller"] ?? "Home" },
                    { "action", "Index" }
                });
        }

        /// <summary>
        /// Captura cualquier excepción no controlada y muestra una vista de error genérica
        /// </summary>
        /// <param name="filterContext">Contexto de la excepción</param>
        /// <param name="exception">Excepción generica</param>
        private void HandleGenericException(ExceptionContext filterContext, Exception exception)
        {
            ShowErrorView(filterContext, "Ha ocurrido un error inesperado. Por favor, intenta nuevamente.", 500);
        }


        /// <summary>
        /// Captura excepciones HTTP específicas (404, 500) y muestra vistas personalizadas
        /// </summary>
        /// <param name="filterContext">Contexto de la excepción</param>
        /// <param name="httpException">Excepción</param>
        private void HandleHttpException(ExceptionContext filterContext, HttpException httpException)
        {
            var statusCode = httpException.GetHttpCode();

            if (statusCode == (int)HttpStatusCode.NotFound)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/NotFound.cshtml"
                };
            }
            else
            {
                ShowErrorView(filterContext, "Ha ocurrido un error en la aplicación", statusCode);
            }
        }

        /// <summary>
        /// Captura excepciones de acceso no autorizado y redirige al login
        /// </summary>
        /// <param name="filterContext">Contexto de excepción</param>
        private void HandleUnauthorizedException(ExceptionContext filterContext)
        {
            filterContext.HttpContext.Session.Clear();
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Auth" },
                    { "action", "Login" }
                });
        }

       

        /// <summary>
        /// Permite manejar excepciones no controladas en toda la aplicación
        /// </summary>
        /// <param name="filterContext">Contexto de la excepción</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            var exception = filterContext.Exception;

            // TODO: Implementar logging aquí
            // Logger.Error($"Error no controlado: {exception.Message}", exception);

            // Log en trace de ASP.NET por ahora
            System.Diagnostics.Trace.TraceError($"Error: {exception.Message}\nStackTrace: {exception.StackTrace}");

            filterContext.ExceptionHandled = true;

            // Manejar diferentes tipos de excepciones
            if (exception is UnauthorizedAccessException)
            {
                HandleUnauthorizedException(filterContext);
            }
            else if (exception is HttpException httpException)
            {
                HandleHttpException(filterContext, httpException);
            }
            else if (exception is ApplicationException)
            {
                HandleApplicationException(filterContext, exception);
            }
            else
            {
                HandleGenericException(filterContext, exception);
            }
        }

        /// <summary>
        /// Permite mostrar una vista de error personalizada con un mensaje específico y código de estado HTTP
        /// </summary>
        /// <param name="filterContext">Contexto de excepción</param>
        /// <param name="message">Mensaje de error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        private void ShowErrorView(ExceptionContext filterContext, string message, int statusCode)
        {
            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary
                {
                    Model = new
                    {
                        Message = message,
                        StatusCode = statusCode
                    }
                }
            };
        }
    }
}
