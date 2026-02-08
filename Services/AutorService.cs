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
        private readonly ApiClientService _apiClient;
        private const string ApiEndpoint = "/v1/autores";

        public AutorService()
        {
            _apiClient = ApiClientService.Instance;
        }

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
    }
}
