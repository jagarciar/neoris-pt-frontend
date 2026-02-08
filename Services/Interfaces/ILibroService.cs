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
        /// Crea un nuevo libro
        /// </summary>
        /// <param name="libro">Libro a crear</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Verdadero si la creación fue exitosa, falso de lo contrario</returns>
        Task<bool> CreateAsync(Libro libro, string token);

        /// <summary>
        /// Eliminación de un libro por su ID, lo que permite gestionar los datos de manera eficiente y garantiza que solo los usuarios autenticados puedan realizar esta acción, además de proporcionar una forma clara de manejar la eliminación de recursos en las vistas y controladores relacionados con los libros
        /// </summary>
        /// <param name="id">Identificador del libro a eliminar</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Verdadero si la eliminación fue exitosa, falso de lo contrario</returns>
        Task<bool> DeleteAsync(int id, string token);

        /// <summary>
        /// Obtiene todos los libros
        /// </summary>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Todos los libros disponibles</returns>
        Task<IEnumerable<Libro>> GetAllAsync(string token);

        /// <summary>
        /// Permite obtener un libro por su ID
        /// </summary>
        /// <param name="id">Identificador del libro</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Libro que coincide con el ID</returns>
        Task<Libro> GetByIdAsync(int id, string token);

        /// <summary>
        /// Actualiza un libro existente
        /// </summary>
        /// <param name="id">Identificador del libro a actualizar</param>
        /// <param name="libro">Libro a actualizar</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Verdadero si la actualización fue exitosa, falso de lo contrario</returns>
        Task<bool> UpdateAsync(int id, Libro libro, string token);

        
    }
}
