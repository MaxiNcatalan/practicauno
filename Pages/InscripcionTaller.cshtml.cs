using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace practicauno.Pages
{
    public class InscripcionTallerModel : PageModel
    {
        private const int MIN_EDAD_PROGRAMACION = 14;
        private const int MIN_EDAD_DISENO_GRAFICO = 16;
        private const int MIN_EDAD_OFICIOS = 18;

        private const string TALLER_PROGRAMACION = "Programación";
        private const string TALLER_DISENO_GRAFICO = "Diseño Gráfico";
        private const string TALLER_OFICIOS = "Oficios";

        public List<string> Talleres { get; } = new List<string>
        {
            TALLER_PROGRAMACION,
            TALLER_DISENO_GRAFICO,
            TALLER_OFICIOS
        };

        [BindProperty]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre {2} y {1} caracteres.")]
        public string NombreCompleto { get; set; }

        // Se usa int? para que [Required] detecte ausencia de valor en el binding.
        [BindProperty]
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [Range(1000000, 99999999, ErrorMessage = "El DNI debe ser un número entre {1} y {2}.")]
        public int? DNI { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Debe confirmar el correo electrónico.")]
        [Compare("Email", ErrorMessage = "Los correos electrónicos no coinciden.")]
        public string ConfirmarEmail { get; set; }

        // Se usa DateTime? para que [Required] funcione correctamente con el binding.
        [BindProperty]
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [Range(typeof(DateTime), "1900-01-01", "2026-12-31", ErrorMessage = "La fecha de nacimiento debe estar entre {1} y {2}.")]
        public DateTime? FechaNacimiento { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Debe elegir un taller.")]
        public string TallerElegido { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Debe elegir un turno.")]
        public string Turno { get; set; }

        [BindProperty]
        public bool AceptaTerminos { get; set; }

        [BindProperty]
        [StringLength(300, ErrorMessage = "Los comentarios no pueden superar {1} caracteres.")]
        public string Comentarios { get; set; }

        // Propiedades para la vista
        public List<string> Errores { get; set; }
        public string MensajeExito { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostInscribir()
        {
            // 1) Validar checkbox de términos
            if (!AceptaTerminos)
            {
                ModelState.AddModelError(nameof(AceptaTerminos), "Debe aceptar los términos y condiciones.");
            }

            // Validar que el taller seleccionado esté en la lista de talleres válidos
            if (string.IsNullOrWhiteSpace(TallerElegido) || !Talleres.Contains(TallerElegido))
            {
                ModelState.AddModelError(nameof(TallerElegido), "El taller seleccionado no es válido.");
            }

            // Nueva validación: fecha de nacimiento no puede ser futura
            if (FechaNacimiento.HasValue && FechaNacimiento.Value > DateTime.Today)
            {
                ModelState.AddModelError(nameof(FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
            }

            // 2) Calcular edad y comparar con la edad mínima del taller
            if (FechaNacimiento.HasValue)
            {
                int edad = CalcularEdad(FechaNacimiento.Value);
                int edadMinima = ObtenerEdadMinima(TallerElegido);
                if (edadMinima > 0 && edad < edadMinima)
                {
                    ModelState.AddModelError(nameof(FechaNacimiento),
                        $"Para inscribirse en el taller \"{TallerElegido}\" se requiere tener al menos {edadMinima} años (usted tiene {edad}).");
                }
            }

            // 3) Si hay errores en ModelState, recopilar mensajes y devolver Page()
            if (!ModelState.IsValid)
            {
                Errores = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            Errores.Add(error.ErrorMessage);
                        }
                        else if (error.Exception != null)
                        {
                            Errores.Add(error.Exception.Message);
                        }
                    }
                }
                return Page();
            }

            // 4) Todo válido: guardar mensaje de éxito y devolver Page()
            MensajeExito = "Inscripción realizada con éxito.";
            return Page();
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            int edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento > hoy.AddYears(-edad))
            {
                edad--;
            }
            return edad;
        }

        private int ObtenerEdadMinima(string taller)
        {
            if (string.IsNullOrWhiteSpace(taller))
            {
                return 0;
            }

            switch (taller)
            {
                case TALLER_PROGRAMACION:
                    return MIN_EDAD_PROGRAMACION;
                case TALLER_DISENO_GRAFICO:
                    return MIN_EDAD_DISENO_GRAFICO;
                case TALLER_OFICIOS:
                    return MIN_EDAD_OFICIOS;
                default:
                    return 0;
            }
        }
    }
}
