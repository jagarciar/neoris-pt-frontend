using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using NeorisFrontend.Filters;
using NeorisFrontend.Models;
using NeorisFrontend.Services.Interfaces;

namespace NeorisFrontend.Controllers
{
    [JwtAuthorizationFilter]
    public class LibrosController : Controller
    {
        private readonly ILibroService _libroService;
        private readonly IAutorService _autorService;

        /// <summary>
        /// Constructor con inyección de dependencias
        /// Unity resolverá automáticamente ILibroService y IAutorService
        /// </summary>
        public LibrosController(ILibroService libroService, IAutorService autorService)
        {
            _libroService = libroService ?? throw new ArgumentNullException(nameof(libroService));
            _autorService = autorService ?? throw new ArgumentNullException(nameof(autorService));
        }

        private string GetToken()
        {
            return Session["Token"] as string;
        }

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

        // GET: Libros
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

        // GET: Libros/Details/5
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

        // GET: Libros/Create
        public async Task<ActionResult> Create()
        {
            await CargarAutores();
            return View();
        }

        // POST: Libros/Create
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

        // GET: Libros/Edit/5
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

        // POST: Libros/Edit/5
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

        // GET: Libros/Delete/5
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

        // POST: Libros/Delete/5
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
    }
}
