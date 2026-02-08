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
    /// Implementación del servicio de Libros
    /// </summary>
    public class LibroService : ILibroService
    {
        private readonly ApiClientService _apiClient;
        private const string ApiEndpoint = "/v1/libros";

        public LibroService()
        {
            _apiClient = ApiClientService.Instance;
        }

        public async Task<IEnumerable<Libro>> GetAllAsync(string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var response = await httpClient.GetAsync(ApiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Libro>>(json) ?? new List<Libro>();
                }

                return new List<Libro>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener libros: {ex.Message}", ex);
            }
        }

        public async Task<Libro> GetByIdAsync(int id, string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var response = await httpClient.GetAsync($"{ApiEndpoint}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Libro>(json);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener libro {id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> CreateAsync(Libro libro, string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var json = JsonConvert.SerializeObject(libro);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(ApiEndpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al crear libro: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(int id, Libro libro, string token)
        {
            try
            {
                var httpClient = _apiClient.GetClient(token);
                var json = JsonConvert.SerializeObject(libro);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"{ApiEndpoint}/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al actualizar libro {id}: {ex.Message}", ex);
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
                throw new ApplicationException($"Error al eliminar libro {id}: {ex.Message}", ex);
            }
        }
    }
}
