using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NeorisFrontend.Infrastructure
{
    /// <summary>
    /// Servicio singleton para gestionar HttpClient de manera eficiente
    /// Evita socket exhaustion y mejora el rendimiento
    /// </summary>
    public class ApiClientService
    {
        private static readonly Lazy<ApiClientService> _instance = 
            new Lazy<ApiClientService>(() => new ApiClientService());

        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        private ApiClientService()
        {
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            
            if (string.IsNullOrEmpty(_apiBaseUrl))
            {
                throw new ConfigurationErrorsException("ApiBaseUrl no está configurada en Web.config");
            }

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            // Headers por defecto
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static ApiClientService Instance => _instance.Value;

        /// <summary>
        /// Obtiene un HttpClient configurado con el token JWT si se proporciona
        /// </summary>
        /// <param name="token">Token JWT de autenticación</param>
        /// <returns>HttpClient configurado</returns>
        public HttpClient GetClient(string token = null)
        {
            // Limpiar header de autorización anterior
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Agregar token si existe
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return _httpClient;
        }

        /// <summary>
        /// URL base de la API
        /// </summary>
        public string ApiBaseUrl => _apiBaseUrl;
    }
}
