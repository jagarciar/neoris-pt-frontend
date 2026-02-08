using System;
using System.ComponentModel.DataAnnotations;

namespace NeorisFrontend.Models
{
    /// <summary>
    /// Representa un autor con sus propiedades básicas para ser utilizado en vistas y controladores
    /// </summary>
    public class Autor
    {
        /// <summary>
        /// Ciudad de procedencia del autor, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Ciudad de Procedencia")]
        public string CiudadProcedencia { get; set; }

        /// <summary>
        /// Correo electrónico del autor, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de garantizar que el formato del email sea correcto
        /// </summary>

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Fecha de nacimiento del autor, con formato específico para facilitar la entrada y visualización en las vistas, además de permitir valores nulos para casos donde no se conozca esta información
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaNacimiento { get; set; }

        /// <summary>
        /// Identificador único del autor, utilizado para operaciones de CRUD y referencia en la base de datos
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del autor, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación
        /// </summary>
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }



        
    }
}
