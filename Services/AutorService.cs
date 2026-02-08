using System;
using System.Collections.Generic;
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
    /// Implementación del servicio de Autores
    /// </summary>
    public class AutorService : IAutorService
    {
        /// <summary>
        /// API Endpoint para las operaciones de autores, utilizado para realizar las solicitudes HTTP a la API RESTful y gestionar los datos de los autores en la aplicación, además de proporcionar una forma clara de manejar las operaciones CRUD en las vistas y controladores relacionados con los autores
        /// </summary>
        private const string ApiEndpoint = "v1/autores";
        /// <summary>
        /// Referencia al servicio de cliente HTTP para realizar solicitudes a la API, lo que permite enviar las solicitudes HTTP necesarias para gestionar los datos de los autores en la aplicación, además de proporcionar una forma clara de manejar las operaciones CRUD en las vistas y controladores relacionados con los autores
        /// </summary>
        private readonly ApiClientService _apiClient;

        /// <summary>
        /// Constructor del servicio de autores, que recibe una instancia del servicio de cliente HTTP para realizar las solicitudes a la API, lo que permite enviar las solicitudes HTTP necesarias para gestionar los datos de los autores en la aplicación, además de proporcionar una forma clara de manejar las operaciones CRUD en las vistas y controladores relacionados con los autores
        /// </summary>
        /// <param name="apiClient">Cliente HTTP para acceder a la API</param>
        /// <exception cref="ArgumentNullException">Genera una excepción cuando el cliente HTTP es nulo</exception>
        public AutorService(ApiClientService apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Crea un nuevo autor
        /// </summary>
        /// <param name="autor">Autor a crear</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Verdadero si la creación fue exitosa, falso en caso contrario</returns>
        /// <exception cref="ApplicationException">Genera una excepción cuando ocurre un error al crear el autor</exception>
        public async Task<bool> CreateAsync(Autor autor, string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var json = JsonConvert.SerializeObject(autor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(ApiEndpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al crear autor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Permite eliminar un autor por su identificador, lo que facilita la gestión de datos en la aplicación y garantiza que solo los usuarios autenticados puedan realizar esta acción, además de proporcionar una forma clara de manejar la eliminación de recursos en las vistas y controladores relacionados con los autores
        /// </summary>
        /// <param name="id">Identificador del autor a eliminar</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Verdadero si la eliminación fue exitosa, falso en caso contrario</returns>
        /// <exception cref="ApplicationException">Genera excepción si ocurre un error al eliminar el autor</exception>
        public async Task<bool> DeleteAsync(int id, string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var response = await httpClient.DeleteAsync($"{ApiEndpoint}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al eliminar autor {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los autores
        /// </summary>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Lista de autores</returns>
        /// <exception cref="ApplicationException">Genera excepción si ocurre un error al obtener los autores</exception>
        public async Task<IEnumerable<Autor>> GetAllAsync(string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var response = await httpClient.GetAsync(ApiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Autor>>(json) ?? new List<Autor>();
                }

                return new List<Autor>();
            }
            catch (Exception ex)
            {
                // Log error aquí cuando se implemente logging
                throw new ApplicationException($"Error al obtener autores: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un autor por su identificador
        /// </summary>
        /// <param name="id">Identificador del autor</param>
        /// <param name="token">Token de autenticación</param>
        /// <returns>Autor encontrado</returns>
        /// <exception cref="ApplicationException">Genera excepción si ocurre un error al obtener el autor</exception>
        public async Task<Autor> GetByIdAsync(int id, string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var response = await httpClient.GetAsync($"{ApiEndpoint}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Autor>(json);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener autor {id}: {ex.Message}", ex);
            }
        }

        

        public async Task<bool> UpdateAsync(int id, Autor autor, string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var json = JsonConvert.SerializeObject(autor);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{ApiEndpoint}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al actualizar autor {id}: {ex.Message}", ex);
            }
        }

        
    }
}
