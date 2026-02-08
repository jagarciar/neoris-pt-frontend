using System.Collections.Generic;
using System.Threading.Tasks;
using NeorisFrontend.Models;

namespace NeorisFrontend.Services.Interfaces
{
    /// <summary>
    /// Servicio para operaciones CRUD de Libros
    /// </summary>
    public interface ILibroService
    {
        /// <summary>
        /// Obtiene todos los libros
        /// </summary>
        Task<IEnumerable<Libro>> GetAllAsync(string token);

        /// <summary>
        /// Obtiene un libro por su ID
        /// </summary>
        Task<Libro> GetByIdAsync(int id, string token);

        /// <summary>
        /// Crea un nuevo libro
        /// </summary>
        Task<bool> CreateAsync(Libro libro, string token);

        /// <summary>
        /// Actualiza un libro existente
        /// </summary>
        Task<bool> UpdateAsync(int id, Libro libro, string token);

        /// <summary>
        /// Elimina un libro
        /// </summary>
        Task<bool> DeleteAsync(int id, string token);
    }
}
