using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeorisFrontend.Models
{
    /// <summary>
    /// Representa la respuesta recibida después de realizar una solicitud de inicio de sesión, incluyendo el token de acceso, la fecha de expiración y el tipo de token, lo que facilita la gestión de la autenticación y autorización en la aplicación, además de permitir un manejo adecuado de la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token de acceso JWT recibido después de un inicio de sesión exitoso, utilizado para autenticar solicitudes posteriores a la API y mantener la sesión del usuario, con el fin de facilitar la gestión de la autenticación y autorización en la aplicación, además de permitir un manejo adecuado de la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
        /// <summary>
        /// Fecha y hora de expiración del token de acceso en formato UTC, utilizada para determinar cuándo el token ya no es válido y se requiere un nuevo inicio de sesión, lo que facilita la gestión de la autenticación y autorización en la aplicación, además de permitir un manejo adecuado de la sesión del usuario en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>

        [JsonProperty("expiresAtUtc")]
        public string ExpiresAtUtc { get; set; }
        /// <summary>
        /// Tipo de token recibido, generalmente "Bearer
        /// </summary>

        [JsonProperty("tokenType")]
        public string TokenType { get; set; }
    }
}