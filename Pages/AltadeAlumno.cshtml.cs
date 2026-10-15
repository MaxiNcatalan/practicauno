using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using practicauno.Models;

namespace practicauno.Pages
{
    public class AltadeAlumnoModel : PageModel
    {
        [BindProperty]
        public Alumno Alumno { get; set; } = new Alumno();

        public string SuccessMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Persistencia real aquí (BD, servicio, etc.)
            SuccessMessage = "Alumno guardado correctamente.";

            // Limpiar formulario
            Alumno = new Alumno();
            ModelState.Clear();
            return Page();
        }
    }
}
