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
        /// Crea un nuevo autor
        /// </summary>
        /// <param name="autor">Autor a crear</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Verdadero si la creación fue exitosa, falso en caso contrario</returns>
        Task<bool> CreateAsync(Autor autor, string token);

        /// <summary>
        /// Permite eliminar un autor por su identificador, lo que facilita la gestión de datos en la aplicación y garantiza que solo los usuarios autenticados puedan realizar esta acción, además de proporcionar una forma clara de manejar la eliminación de recursos en las vistas y controladores relacionados con los autores
        /// </summary>
        /// <param name="id">Identificador del autor a eliminar</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Verdadero si la eliminación fue exitosa, falso en caso contrario</returns>
        Task<bool> DeleteAsync(int id, string token);

        /// <summary>
        /// Obtiene todos los autores
        /// </summary>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Lista de autores</returns>
        Task<IEnumerable<Autor>> GetAllAsync(string token);

        /// <summary>
        /// Obtiene un autor por su identificador
        /// </summary>
        /// <param name="id">Identificador del autor</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Autor encontrado</returns>
        Task<Autor> GetByIdAsync(int id, string token);



        /// <summary>
        /// Actualiza un autor existente
        /// </summary>
        /// <param name="id">Identificador del autor</param>
        /// <param name="autor">Datos actualizados del autor</param>
        /// <param name="token">Token de actualización</param>
        /// <returns>Verdadero si la actualización fue exitosa, falso en caso contrario</returns>
        Task<bool> UpdateAsync(int id, Autor autor, string token);

        
    }
}
