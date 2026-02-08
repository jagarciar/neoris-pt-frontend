using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NeorisFrontend.Models
{
    /// <summary>
    /// Representa la información necesaria para realizar una solicitud de inicio de sesión, incluyendo validaciones para asegurar que se ingresen datos adecuados y evitar errores en la capa de presentación, además de facilitar la gestión de datos en las vistas y controladores relacionados con la autenticación de usuarios
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Contraseña del usuario, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de utilizar el tipo de dato Password para mejorar la seguridad en la entrada de datos en las vistas
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        /// <summary>
        /// Usuario para el inicio de sesión, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de establecer una relación clara entre el usuario y su contraseña para facilitar la gestión de datos en las vistas y controladores relacionados con la autenticación de usuarios
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }
    }

    
}
