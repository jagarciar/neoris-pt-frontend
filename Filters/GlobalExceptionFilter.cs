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

        private void HandleGenericException(ExceptionContext filterContext, Exception exception)
        {
            ShowErrorView(filterContext, "Ha ocurrido un error inesperado. Por favor, intenta nuevamente.", 500);
        }

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
