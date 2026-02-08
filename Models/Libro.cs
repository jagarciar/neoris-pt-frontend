using System.ComponentModel.DataAnnotations;

namespace NeorisFrontend.Models
{
    /// <summary>
    /// Representa un libro con sus propiedades básicas para ser utilizado en vistas y controladores, incluyendo validaciones para asegurar que se ingresen datos adecuados y evitar errores en la capa de presentación
    /// </summary>
    public class Libro
    {
        /// <summary>
        /// Año de publicación del libro, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de establecer un rango razonable para el año de publicación
        /// </summary>
        [Required(ErrorMessage = "El año es requerido")]
        [Range(1900, 2026, ErrorMessage = "El año debe estar entre 1900 y 2026")]
        [Display(Name = "Año")]
        public int Anio { get; set; }
        /// <summary>
        /// Autor del libro, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de establecer una relación clara entre el libro y su autor para facilitar la gestión de datos en las vistas y controladores
        /// </summary>
        public Autor Autor { get; set; }
        /// <summary>
        /// Identificador del autor del libro, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de establecer una relación clara entre el libro y su autor para facilitar la gestión de datos en las vistas y controladores
        /// </summary>
        [Required(ErrorMessage = "El autor es requerido")]
        [Display(Name = "Autor")]
        public int AutorId { get; set; }
        /// <summary>
        /// Descripción del libro, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de limitar la longitud para mantener la consistencia en la visualización de las vistas
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        /// <summary>
        /// Genero del libro, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de limitar la longitud para mantener la consistencia en la visualización de las vistas
        /// </summary>
        [Required(ErrorMessage = "El género es requerido")]
        [StringLength(100, ErrorMessage = "El género no puede exceder 100 caracteres")]
        [Display(Name = "Género")]
        public string Genero { get; set; }
        /// <summary>
        /// Identificador único del libro, utilizado para operaciones de CRUD y referencia en la base de datos
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Número de páginas del libro, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de establecer un rango razonable para el número de páginas y garantizar que sea un valor positivo
        /// </summary>
        [Required(ErrorMessage = "El número de páginas es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de páginas debe ser mayor que cero")]
        [Display(Name = "Número de páginas")]
        public int NumeroPaginas { get; set; }
        /// <summary>
        /// Título del libro, con validaciones para asegurar que se ingrese un valor adecuado y evitar errores en la capa de presentación, además de limitar la longitud para mantener la consistencia en la visualización de las vistas
        /// </summary>
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El título debe tener entre 1 y 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }


        

        

        

        
    }
}
