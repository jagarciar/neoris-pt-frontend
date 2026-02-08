using System.Collections.Generic;
using System.Threading.Tasks;
using NeorisFrontend.Models;

namespace NeorisFrontend.Services.Interfaces
{
    /// <summary>
    /// Servicio para operaciones CRUD de Autores
    /// </summary>
    public interface IAutorService
    {
        /// <summary>
        /// Obtiene todos los autores
        /// </summary>
        Task<IEnumerable<Autor>> GetAllAsync(string token);

        /// <summary>
        /// Obtiene un autor por su ID
        /// </summary>
        Task<Autor> GetByIdAsync(int id, string token);

        /// <summary>
        /// Crea un nuevo autor
        /// </summary>
        Task<bool> CreateAsync(Autor autor, string token);

        /// <summary>
        /// Actualiza un autor existente
        /// </summary>
        Task<bool> UpdateAsync(int id, Autor autor, string token);

        /// <summary>
        /// Elimina un autor
        /// </summary>
        Task<bool> DeleteAsync(int id, string token);
    }
}
