using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NeorisFrontend.Infrastructure;
using NeorisFrontend.Models;
using NeorisFrontend.Services.Interfaces;

namespace NeorisFrontend.Services
{
    /// <summary>
    /// Implementación del servicio de Autenticación
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApiClientService _apiClient;
        private const string ApiEndpoint = "/v1/auth/login";

        public AuthService()
        {
            _apiClient = ApiClientService.Instance;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var httpClient = _apiClient.GetClient();
                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(ApiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<LoginResponse>(responseJson);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al autenticar: {ex.Message}", ex);
            }
        }

        public bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                
                // Verificar si el token tiene el formato correcto
                if (!handler.CanReadToken(token))
                {
                    return false;
                }

                var jwtToken = handler.ReadJwtToken(token);
                
                // Verificar si el token ha expirado
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
