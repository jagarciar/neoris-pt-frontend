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
        /// <summary>
        /// API Endpoint para el inicio de sesión, utilizado para enviar las credenciales del usuario y recibir el token JWT en respuesta, lo que facilita la gestión de la autenticación y autorización en la aplicación, además de proporcionar una forma clara de manejar la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        private const string ApiEndpoint = "v1/auth/login";
        /// <summary>
        /// Referencia al servicio de cliente HTTP para realizar solicitudes a la API, lo que permite enviar las credenciales del usuario y recibir el token JWT en respuesta, facilitando así la gestión de la autenticación y autorización en la aplicación, además de proporcionar una forma clara de manejar la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        private readonly ApiClientService _apiClient;

        /// <summary>
        /// Constructor del servicio de autenticación, que recibe una instancia del servicio de cliente HTTP para realizar las solicitudes a la API, lo que permite enviar las credenciales del usuario y recibir el token JWT en respuesta, facilitando así la gestión de la autenticación y autorización en la aplicación, además de proporcionar una forma clara de manejar la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        /// <param name="apiClient">Cliente HTTP para hacer peticiones a la API</param>
        /// <exception cref="ArgumentNullException">Genera excepción si el cliente API es nulo</exception>
        public AuthService(ApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Valida si un token JWT es válido y no ha expirado, lo que permite controlar el acceso a las acciones protegidas en la aplicación y garantizar que solo los usuarios autenticados puedan acceder a ciertos recursos, además de facilitar la gestión de la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        /// <param name="token">Token JWT a validar</param>
        /// <returns>Verdadero si el token es válido, falso en caso contrario</returns>
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

        /// <summary>
        /// Permite iniciar sesión enviando las credenciales del usuario a la API y recibiendo un token JWT en respuesta, lo que facilita la gestión de la autenticación y autorización en la aplicación, además de proporcionar una forma clara de manejar la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        /// <param name="loginRequest">Datos para el inicio de sesión del usuario</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Genera excepción si ocurre un error durante el proceso de inicio de sesión</exception>
        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var httpClient = _apiClient.GetClient();
                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Construir URL completa para logging
                var fullUrl = new Uri(httpClient.BaseAddress, ApiEndpoint).ToString();
                
                System.Diagnostics.Debug.WriteLine($"[AuthService] BaseAddress: {httpClient.BaseAddress}");
                System.Diagnostics.Debug.WriteLine($"[AuthService] ApiEndpoint: {ApiEndpoint}");
                System.Diagnostics.Debug.WriteLine($"[AuthService] URL COMPLETA: {fullUrl}");
                System.Diagnostics.Debug.WriteLine($"[AuthService] Payload: {json}");

                var response = await httpClient.PostAsync(ApiEndpoint, content);
                
                System.Diagnostics.Debug.WriteLine($"[AuthService] Status Code: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[AuthService] Request URI: {response.RequestMessage?.RequestUri}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[AuthService] Response JSON: {responseJson}");
                    
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseJson);
                    
                    if (loginResponse != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AuthService] AccessToken deserializado: {loginResponse.AccessToken?.Substring(0, Math.Min(20, loginResponse.AccessToken?.Length ?? 0))}...");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[AuthService] loginResponse es NULL después de deserializar");
                    }
                    
                    return loginResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[AuthService] Error response: {errorContent}");
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService] EXCEPCIÓN: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AuthService] StackTrace: {ex.StackTrace}");
                throw new ApplicationException($"Error al autenticar: {ex.Message}", ex);
            }
        }

        
    }
}
