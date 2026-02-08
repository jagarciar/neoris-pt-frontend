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
        /// Autentica un usuario y retorna el token JWT
        /// </summary>
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);

        /// <summary>
        /// Valida si un token JWT es válido y no ha expirado
        /// </summary>
        bool IsTokenValid(string token);
    }
}
