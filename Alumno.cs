using practicauno.Models;

namespace practicauno.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Alumno
    {

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La carrera es obligatoria")]
        [StringLength(100, ErrorMessage = "La carrera no puede exceder 100 caracteres")]
        public string Carrera { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(18, 60, ErrorMessage = "La edad debe estar entre 18 y 60 años")]
        public int Edad { get; set; }
    }

}



