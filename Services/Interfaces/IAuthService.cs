using System.Threading.Tasks;
using NeorisFrontend.Models;

namespace NeorisFrontend.Services.Interfaces
{
    /// <summary>
    /// Servicio para operaciones de autenticación
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Valida si un token JWT es válido y no ha expirado, lo que permite controlar el acceso a las acciones protegidas en la aplicación y garantizar que solo los usuarios autenticados puedan acceder a ciertos recursos, además de facilitar la gestión de la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        /// <param name="token">Token JWT a validar</param>
        /// <returns>Verdadero si el token es válido, falso en caso contrario</returns>
        bool IsTokenValid(string token);
        /// <summary>
        /// Autentica a un usuario con sus credenciales y devuelve un token JWT si la autenticación es exitosa
        /// </summary>
        /// <param name="loginRequest">Datos de inicio de sesión del usuario</param>
        /// <returns>Resultado de inicio de sesión que incluye el token JWT</returns>
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);

        
    }
}
