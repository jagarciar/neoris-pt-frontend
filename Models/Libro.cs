using System.ComponentModel.DataAnnotations;

namespace NeorisFrontend.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El título debe tener entre 1 y 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El año es requerido")]
        [Range(1000, 9999, ErrorMessage = "El año debe estar entre 1000 y 9999")]
        [Display(Name = "Año")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El autor es requerido")]
        [Display(Name = "Autor")]
        public int AutorId { get; set; }

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        // Propiedad de navegación
        public Autor Autor { get; set; }
    }
}
