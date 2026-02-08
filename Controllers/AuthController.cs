using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using NeorisFrontend.Models;
using NeorisFrontend.Services.Interfaces;

namespace NeorisFrontend.Controllers
{
    /// <summary>
    /// Controlador de autenticación que maneja el proceso de login y logout de los usuarios. Utiliza inyección de dependencias para obtener una instancia del servicio de autenticación (IAuthService), lo que permite una separación clara entre la lógica de presentación y la lógica de negocio. El controlador proporciona acciones para mostrar el formulario de login, procesar las credenciales ingresadas por el usuario y cerrar la sesión, gestionando adecuadamente los tokens de autenticación en la sesión del usuario para mantener su estado de autenticación a lo largo de su interacción con la aplicación.
    /// </summary>
    public class AuthController : Controller
    {
        /// <summary>
        /// Referencia al servicio de autenticación, inyectada a través del constructor. Este servicio es responsable de manejar la lógica de autenticación, como enviar las credenciales al backend y recibir el token de acceso. Al utilizar una interfaz (IAuthService), se facilita la prueba unitaria del controlador y se promueve una arquitectura más modular y mantenible.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor del controlador de autenticación, que recibe una instancia de IAuthService a través de inyección de dependencias. Si el servicio de autenticación es nulo, se lanza una excepción ArgumentNullException para garantizar que el controlador siempre tenga una instancia válida del servicio, lo que es esencial para manejar correctamente el proceso de login y logout en la aplicación.
        /// </summary>
        /// <param name="authService">Servicio de autenticación encargado de procesar las solicitudes de login y gestionar los tokens de acceso.</param>
        /// <exception cref="ArgumentNullException">Genera excepción si el servicio de autenticación es nulo al inicializar el controlador.</exception>
        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Permite mostrar el formulario de login al usuario. Si el usuario ya está autenticado (es decir, si hay un token de acceso almacenado en la sesión), se redirige automáticamente al home para evitar que el usuario tenga que iniciar sesión nuevamente. Si no hay un token en la sesión, se muestra la vista de login para que el usuario pueda ingresar sus credenciales. Esta acción es accesible a todos los usuarios, independientemente de su estado de autenticación, ya que es el punto de entrada para iniciar sesión en la aplicación.
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            // Si ya está autenticado, redirigir al home
            if (Session["Token"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Permite procesar las credenciales ingresadas por el usuario en el formulario de login. Primero, se valida que el modelo recibido sea válido; si no lo es, se muestra un mensaje de error indicando que se deben completar todos los campos requeridos. Luego, se intenta realizar el proceso de login utilizando el servicio de autenticación. Si el login es exitoso y se recibe un token de acceso, se almacena el token y el nombre de usuario en la sesión, se muestra un mensaje de bienvenida y se redirige al home. Si las credenciales son incorrectas, se muestra un mensaje de error indicando que el usuario o la contraseña son incorrectos. Además, se manejan excepciones específicas para problemas de conexión con el backend y errores inesperados, mostrando mensajes de error adecuados para cada caso y registrando detalles del error para facilitar la depuración.
        /// </summary>
        /// <param name="model">Modelo que contiene las credenciales de inicio de sesión (nombre de usuario y contraseña).</param>
        /// <returns>Formulario de login con mensajes de error en caso de fallo o redirección al home en caso de éxito.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor complete todos los campos requeridos";
                return View(model);
            }

            try
            {
                var loginResponse = await _authService.LoginAsync(model);
                    
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken))
                {
                    // Guardar token en sesión
                    Session["Token"] = loginResponse.AccessToken;
                    Session["Username"] = model.Username;
                    
                    TempData["Success"] = $"¡Bienvenido {model.Username}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos. Verifica las credenciales.";
                    return View(model);
                }
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                ViewBag.Error = $"No se pudo conectar con el backend. Verifica que esté corriendo en puerto 5000. Error: {httpEx.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inesperado: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error completo en Login: {ex}");
                return View(model);
            }
        }

        /// <summary>
        /// Permite cerrar la sesión del usuario. Al acceder a esta acción, se limpia toda la información almacenada en la sesión, incluyendo el token de acceso y el nombre de usuario, lo que efectivamente cierra la sesión del usuario. Luego, se muestra un mensaje de éxito indicando que la sesión se ha cerrado correctamente y se redirige al formulario de login para que el usuario pueda iniciar sesión nuevamente si lo desea. Esta acción es accesible solo para usuarios autenticados, ya que no tendría sentido permitir el acceso a esta funcionalidad a usuarios que no han iniciado sesión.
        /// </summary>
        /// <returns>Redirección al formulario de login con mensaje de éxito.</returns>
        public ActionResult Logout()
        {
            Session.Clear();
            TempData["Success"] = "Sesión cerrada exitosamente";
            return RedirectToAction("Login");
        }
    }
}
