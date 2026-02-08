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
        /// <summary>
        /// Endpoint base de la API, configurable desde Web.config para flexibilidad entre entornos
        /// </summary>
        private readonly string _apiBaseUrl;
        /// <summary>
        /// Client HTTP reutilizable para realizar solicitudes a la API
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Provee una instancia única de ApiClientService utilizando Lazy<T> para inicialización perezosa y thread-safe
        /// </summary>
        private static readonly Lazy<ApiClientService> _instance = 
            new Lazy<ApiClientService>(() => new ApiClientService());

        /// <summary>
        /// Constructor privado para evitar instanciación externa y garantizar el uso del singleton
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">Genera excepción si ApiBaseUrl no está configurada</exception>
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

        /// <summary>
        /// Obtiene la instancia singleton de ApiClientService para su uso en toda la aplicación
        /// </summary>
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
