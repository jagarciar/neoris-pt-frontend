using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NeorisFrontend.Filters;
using NeorisFrontend.Models;
using NeorisFrontend.Services.Interfaces;

namespace NeorisFrontend.Controllers
{
    /// <summary>
    /// Provee un controlador para gestionar las operaciones CRUD (Crear, Leer, Actualizar, Eliminar) relacionadas con los autores. Este controlador utiliza inyección de dependencias para obtener una instancia del servicio de autores (IAutorService), lo que permite una separación clara entre la lógica de presentación y la lógica de negocio. Además, el controlador está decorado con un filtro de autorización JWT (JwtAuthorizationFilter) para asegurar que solo los usuarios autenticados puedan acceder a sus acciones, garantizando así la seguridad de las operaciones relacionadas con los autores en la aplicación.
    /// </summary>
    [JwtAuthorizationFilter]
    public class AutoresController : Controller
    {
        /// <summary>
        /// Referencia al servicio de autores, inyectada a través del constructor. Este servicio es responsable de manejar la lógica de negocio relacionada con los autores, como obtener la lista de autores, crear un nuevo autor, actualizar un autor existente o eliminar un autor. Al utilizar una interfaz (IAutorService), se facilita la prueba unitaria del controlador y se promueve una arquitectura más modular y mantenible.
        /// </summary>
        private readonly IAutorService _autorService;

        /// <summary>
        /// Constructor del controlador de autores, que recibe una instancia de IAutorService a través de inyección de dependencias. Si el servicio de autores es nulo, se lanza una excepción ArgumentNullException para garantizar que el controlador siempre tenga una instancia válida del servicio, lo que es esencial para manejar correctamente las operaciones relacionadas con los autores en la aplicación.
        /// </summary>
        /// <param name="autorService">Servicio de autores proporcionado por inyección de dependencias.</param>
        /// <exception cref="ArgumentNullException">Genera excepción si el servicio de autores es nulo.</exception>
        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService ?? throw new ArgumentNullException(nameof(autorService));
        }

        /// <summary>
        /// Permite mostrar el formulario para crear un nuevo autor. Esta acción simplemente devuelve la vista correspondiente al formulario de creación de autor, sin realizar ninguna lógica adicional. El formulario permitirá al usuario ingresar los datos necesarios para crear un nuevo autor en el sistema. Dado que esta acción no realiza ninguna operación que pueda generar una excepción, no se incluye un bloque try-catch para manejar errores, ya que la vista se mostrará sin problemas incluso si no hay datos para mostrar en este punto.
        /// </summary>
        /// <returns>Formulario de creación de autor o redirección al índice en caso de error.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Permite procesar el formulario de creación de un nuevo autor. El método es asíncrono y recibe un objeto Autor con los datos ingresados por el usuario en el formulario. Primero, se verifica si el modelo es válido utilizando ModelState.IsValid; si no lo es, se devuelve la vista con el modelo para mostrar los errores de validación al usuario. Si el modelo es válido, se intenta crear el nuevo autor utilizando el servicio de autores, pasando el token de autenticación obtenido de la sesión. Si la creación del autor es exitosa, se almacena un mensaje de éxito en TempData y se redirige al índice de autores. En caso de que la creación del autor falle o ocurra una excepción durante el proceso, se captura el error, se almacena un mensaje de error en TempData y se devuelve la vista con el modelo para que el usuario pueda corregir cualquier problema y volver a intentar la creación del autor.
        /// </summary>
        /// <param name="autor">Autor con los datos ingresados por el usuario en el formulario de creación.</param>
        /// <returns>Formulario de creación de autor o redirección al índice en caso de error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            try
            {
                var success = await _autorService.CreateAsync(autor, GetToken());

                if (success)
                {
                    TempData["Success"] = "Autor creado exitosamente";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "No se pudo crear el autor";
                return View(autor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(autor);
            }
        }

        /// <summary>
        /// Permite mostrar el formulario de confirmación para eliminar un autor específico utilizando su ID. El método es asíncrono y utiliza el servicio de autores para obtener los detalles del autor desde el backend, pasando el token de autenticación obtenido de la sesión. Si el autor con el ID especificado no se encuentra, se almacena un mensaje de error en TempData y se redirige al índice de autores. En caso de que ocurra una excepción durante la obtención de los detalles del autor, se captura el error, se almacena un mensaje de error en TempData y se redirige al índice de autores para evitar que la aplicación se rompa debido a la excepción no manejada.
        /// </summary>
        /// <param name="id">Identificador único del autor a eliminar.</param>
        /// <returns>Formulario de confirmación de eliminación de autor o redirección al índice en caso de error.</returns>
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var autor = await _autorService.GetByIdAsync(id, GetToken());

                if (autor == null)
                {
                    TempData["Error"] = "Autor no encontrado";
                    return RedirectToAction("Index");
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Permite procesar la confirmación de eliminación de un autor específico utilizando su ID. El método es asíncrono y recibe el ID del autor a eliminar. Se intenta eliminar el autor utilizando el servicio de autores, pasando el ID del autor a eliminar y el token de autenticación obtenido de la sesión. Si la eliminación del autor es exitosa, se almacena un mensaje de éxito en TempData y se redirige al índice de autores. En caso de que la eliminación del autor falle o ocurra una excepción durante el proceso, se captura el error, se almacena un mensaje de error en TempData y se redirige al índice de autores para que el usuario pueda intentar eliminar el autor nuevamente o tomar otras acciones según sea necesario.
        /// </summary>
        /// <param name="id">Identificador único del autor a eliminar.</param>
        /// <returns>Formulario de índice de autores o redirección al índice en caso de error.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _autorService.DeleteAsync(id, GetToken());

                if (success)
                {
                    TempData["Success"] = "Autor eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el autor";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Permite obtener los detalles de un autor específico utilizando su ID. El método es asíncrono y utiliza el servicio de autores para realizar la solicitud al backend, pasando el token de autenticación obtenido de la sesión. Si el autor con el ID especificado no se encuentra, se almacena un mensaje de error en TempData y se redirige al índice de autores. En caso de que ocurra una excepción durante la obtención de los detalles del autor, se captura el error, se almacena un mensaje de error en TempData y se redirige al índice de autores para evitar que la aplicación se rompa debido a la excepción no manejada.
        /// </summary>
        /// <param name="id">Identificador único del autor a obtener.</param>
        /// <returns>Formulario de detalles del autor o redirección al índice en caso de error.</returns>
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var autor = await _autorService.GetByIdAsync(id, GetToken());

                if (autor == null)
                {
                    TempData["Error"] = "Autor no encontrado";
                    return RedirectToAction("Index");
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Permite mostrar el formulario para editar un autor específico utilizando su ID. El método es asíncrono y utiliza el servicio de autores para obtener los detalles del autor desde el backend, pasando el token de autenticación obtenido de la sesión. Si el autor con el ID especificado no se encuentra, se almacena un mensaje de error en TempData y se redirige al índice de autores. En caso de que ocurra una excepción durante la obtención de los detalles del autor, se captura el error, se almacena un mensaje de error en TempData y se redirige al índice de autores para evitar que la aplicación se rompa debido a la excepción no manejada.
        /// </summary>
        /// <param name="id">Identificador único del autor a editar.</param>
        /// <returns>Formulario de edición de autor o redirección al índice en caso de error.</returns>
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var autor = await _autorService.GetByIdAsync(id, GetToken());

                if (autor == null)
                {
                    TempData["Error"] = "Autor no encontrado";
                    return RedirectToAction("Index");
                }

                return View(autor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Permite procesar el formulario de edición de un autor específico utilizando su ID. El método es asíncrono y recibe un objeto Autor con los datos actualizados por el usuario en el formulario de edición. Primero, se verifica si el modelo es válido utilizando ModelState.IsValid; si no lo es, se devuelve la vista con el modelo para mostrar los errores de validación al usuario. Si el modelo es válido, se intenta actualizar el autor utilizando el servicio de autores, pasando el ID del autor a editar, el objeto Autor con los datos actualizados y el token de autenticación obtenido de la sesión. Si la actualización del autor es exitosa, se almacena un mensaje de éxito en TempData y se redirige al índice de autores. En caso de que la actualización del autor falle o ocurra una excepción durante el proceso, se captura el error, se almacena un mensaje de error en TempData y se devuelve la vista con el modelo para que el usuario pueda corregir cualquier problema y volver a intentar la edición del autor.
        /// </summary>
        /// <param name="id">Identificador único del autor a editar.</param>
        /// <param name="autor">Autor con los datos actualizados ingresados por el usuario en el formulario de edición.</param>
        /// <returns>Formulario de edición de autor o redirección al índice en caso de error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            try
            {
                var success = await _autorService.UpdateAsync(id, autor, GetToken());

                if (success)
                {
                    TempData["Success"] = "Autor actualizado exitosamente";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "No se pudo actualizar el autor";
                return View(autor);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(autor);
            }
        }


        /// <summary>
        /// Permite obtener el token de autenticación almacenado en la sesión del usuario. Este token es necesario para realizar las solicitudes al backend que requieren autenticación, como las operaciones relacionadas con los autores. El método intenta recuperar el token de la sesión utilizando la clave "Token" y lo devuelve como una cadena. Si no se encuentra un token en la sesión, se devuelve null, lo que indica que el usuario no está autenticado o que el token ha expirado. Este método es utilizado internamente por las acciones del controlador para asegurarse de que las solicitudes al backend incluyan el token de autenticación necesario para acceder a los recursos protegidos.
        /// </summary>
        /// <returns>Token de autenticación del usuario en la sesión o null si no existe.</returns>
        private string GetToken()
        {
            return Session["Token"] as string;
        }

        /// <summary>
        /// Permite obtener la lista de todos los autores desde el backend y mostrarla en la vista. El método es asíncrono y utiliza el servicio de autores para realizar la solicitud al backend, pasando el token de autenticación obtenido de la sesión. Si la solicitud es exitosa, se devuelve la vista con la lista de autores. En caso de que ocurra una excepción durante la carga de los autores, se captura el error, se almacena un mensaje de error en TempData para mostrarlo al usuario y se devuelve una vista vacía (con una lista vacía de autores) para evitar que la aplicación se rompa debido a la excepción no manejada.
        /// </summary>
        /// <returns>Formulario de índice de autores o vista vacía en caso de error.</returns>
        public async Task<ActionResult> Index()
        {
            try
            {
                var autores = await _autorService.GetAllAsync(GetToken());
                return View(autores);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar autores: {ex.Message}";
                return View(Enumerable.Empty<Autor>());
            }
        }


        
    }
}
