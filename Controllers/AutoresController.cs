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
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        /// <summary>
        /// Constructor con inyección de dependencias
        /// Unity resolverá automáticamente IAutorService
        /// </summary>
        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService ?? throw new ArgumentNullException(nameof(autorService));
        }

        private string GetToken()
        {
            return Session["Token"] as string;
        }

        // GET: Autores
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

        // GET: Autores/Details/5
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

        // GET: Autores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Autores/Create
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

        // GET: Autores/Edit/5
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

        // POST: Autores/Edit/5
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

        // GET: Autores/Delete/5
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

        // POST: Autores/Delete/5
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
    }
}
