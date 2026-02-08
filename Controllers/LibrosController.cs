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
    /// Controlador para gestionar las operaciones CRUD de libros.
    /// </summary>
    [JwtAuthorizationFilter]
    public class LibrosController : Controller
    {
        /// <summary>
        /// Referencia al servicio de autores para cargar la lista de autores en los formularios de creación y edición de libros.
        /// </summary>
        private readonly IAutorService _autorService;
        /// <summary>
        /// Referencia al servicio de libros para realizar las operaciones CRUD a través de la API REST.
        /// </summary>
        private readonly ILibroService _libroService;


        /// <summary>
        /// Constructor del controlador que recibe las dependencias de los servicios de libros y autores a través de inyección de dependencias.
        /// </summary>
        /// <param name="libroService">Servicio de libros</param>
        /// <param name="autorService">Servicio de autores</param>
        /// <exception cref="ArgumentNullException">Genera excepción si alguno de los parámetros es nulo.</exception>
        public LibrosController(ILibroService libroService, IAutorService autorService)
        {
            _libroService = libroService ?? throw new ArgumentNullException(nameof(libroService));
            _autorService = autorService ?? throw new ArgumentNullException(nameof(autorService));
        }

        /// <summary>
        /// Permite cargar la lista de autores desde la API REST y asignarla a ViewBag para ser utilizada en los formularios de creación y edición de libros. En caso de error, se asigna una lista vacía para evitar fallos en la vista.
        /// </summary>
        /// <returns>Formulario de creación o edición de libros con la lista de autores cargada en ViewBag.</returns>
        private async Task CargarAutores()
        {
            try
            {
                var autores = await _autorService.GetAllAsync(GetToken());
                ViewBag.Autores = new SelectList(autores, "Id", "Nombre");
            }
            catch
            {
                ViewBag.Autores = new SelectList(Enumerable.Empty<Autor>(), "Id", "Nombre");
            }
        }

        /// <summary>
        /// Permite mostrar el formulario de creación de un nuevo libro, cargando previamente la lista de autores para que el usuario pueda seleccionar el autor del libro. En caso de error al cargar los autores, se muestra un mensaje de error y se asigna una lista vacía para evitar fallos en la vista.
        /// </summary>
        /// <returns>Formulario de creación de libro con la lista de autores cargada en ViewBag.</returns>
        public async Task<ActionResult> Create()
        {
            await CargarAutores();
            return View();
        }

        /// <summary>
        /// Permite crear un nuevo libro enviando los datos del formulario a la API REST. Si el modelo no es válido, se recarga la lista de autores y se muestra el formulario nuevamente con los errores de validación. Si la creación es exitosa, se muestra un mensaje de éxito y se redirige a la vista principal. En caso de error, se muestra un mensaje de error, se recarga la lista de autores y se muestra el formulario nuevamente.
        /// </summary>
        /// <param name="libro">Libro a crear</param>
        /// <returns>Formulario con los datos del nuevo libro creado o formulario con errores de validación.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                await CargarAutores();
                return View(libro);
            }

            try
            {
                var success = await _libroService.CreateAsync(libro, GetToken());

                if (success)
                {
                    TempData["Success"] = "Libro creado exitosamente";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "No se pudo crear el libro";
                await CargarAutores();
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                await CargarAutores();
                return View(libro);
            }
        }

        /// <summary>
        /// Permite mostrar el formulario de confirmación para eliminar un libro específico obteniendo sus datos desde la API REST utilizando su ID. Si el libro no se encuentra, se muestra un mensaje de error y se redirige a la vista principal. En caso de error al obtener los datos del libro, se muestra un mensaje de error y se redirige a la vista principal.
        /// </summary>
        /// <param name="id">Identificador del libro a eliminar</param>
        /// <returns>Formulario de confirmación de eliminación del libro solicitado.</returns>
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var libro = await _libroService.GetByIdAsync(id, GetToken());

                if (libro == null)
                {
                    TempData["Error"] = "Libro no encontrado";
                    return RedirectToAction("Index");
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Permite eliminar un libro específico enviando su ID a la API REST. Si la eliminación es exitosa, se muestra un mensaje de éxito y se redirige a la vista principal. En caso de error, se muestra un mensaje de error y se redirige a la vista principal.
        /// </summary>
        /// <param name="id">Identificador del libro a eliminar</param>
        /// <returns>Formulario  con los datos del libro eliminado o formulario con errores de validación.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var success = await _libroService.DeleteAsync(id, GetToken());

                if (success)
                {
                    TempData["Success"] = "Libro eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el libro";
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
        /// Permite obtener los detalles de un libro específico desde la API REST utilizando su ID y mostrarlo en la vista de detalles. En caso de error, se muestra un mensaje de error y se redirige a la vista principal.
        /// </summary>
        /// <param name="id">Identificador del libro a consultar</param>
        /// <returns>Formulario con los detalles del libro solicitado.</returns>
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var libro = await _libroService.GetByIdAsync(id, GetToken());

                if (libro == null)
                {
                    TempData["Error"] = "Libro no encontrado";
                    return RedirectToAction("Index");
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Permite obtener los datos de un libro específico desde la API REST utilizando su ID para mostrarlo en el formulario de edición. Si el libro no se encuentra, se muestra un mensaje de error y se redirige a la vista principal. En caso de error al obtener los datos del libro, se muestra un mensaje de error y se redirige a la vista principal.
        /// </summary>
        /// <param name="id">Identificador del libro a editar</param>
        /// <returns>Formulario de edición del libro solicitado.</returns>
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var libro = await _libroService.GetByIdAsync(id, GetToken());

                if (libro == null)
                {
                    TempData["Error"] = "Libro no encontrado";
                    return RedirectToAction("Index");
                }

                await CargarAutores();
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Permite actualizar un libro existente enviando los datos del formulario a la API REST. Si el modelo no es válido, se recarga la lista de autores y se muestra el formulario nuevamente con los errores de validación. Si la actualización es exitosa, se muestra un mensaje de éxito y se redirige a la vista principal. En caso de error, se muestra un mensaje de error, se recarga la lista de autores y se muestra el formulario nuevamente.
        /// </summary>
        /// <param name="id">Identificador del libro a actualizar</param>
        /// <param name="libro">Libro con los nuevos datos actualizados</param>
        /// <returns>Formulario con los datos del libro actualizado o formulario con errores de validación.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Libro libro)
        {
            if (!ModelState.IsValid)
            {
                await CargarAutores();
                return View(libro);
            }

            try
            {
                var success = await _libroService.UpdateAsync(id, libro, GetToken());

                if (success)
                {
                    TempData["Success"] = "Libro actualizado exitosamente";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "No se pudo actualizar el libro";
                await CargarAutores();
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                await CargarAutores();
                return View(libro);
            }
        }

        /// <summary>
        /// Permite obtener el token de autenticación almacenado en la sesión del usuario para incluirlo en las solicitudes a la API REST.
        /// </summary>
        /// <returns>Token de autenticación de la sesión del usuario.</returns>
        private string GetToken()
        {
            return Session["Token"] as string;
        }

        /// <summary>
        /// Permite obtener la lista de libros desde la API REST y mostrarla en la vista principal. En caso de error, se muestra un mensaje de error y se asigna una lista vacía para evitar fallos en la vista.
        /// </summary>
        /// <returns>Formulario con la lista de libros cargada en la vista.</returns>
        public async Task<ActionResult> Index()
        {
            try
            {
                var libros = await _libroService.GetAllAsync(GetToken());
                return View(libros);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar libros: {ex.Message}";
                return View(Enumerable.Empty<Libro>());
            }
        }









        

       
    }
}
