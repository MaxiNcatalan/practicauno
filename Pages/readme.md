# Bitácora de uso de IA: TP Obligatorio N° 1

**Materia:** Programación Web II (Docente: Ing. Gutiérrez Andrés)
**Trabajo:** Inscripción a Talleres Extracurriculares (Razor Pages, proyecto `practicauno`)
**Herramienta:** GitHub Copilot Chat (en Visual Studio)
**Alumno:** Catalan Maximiliano




---


## Prompt 1: Esqueleto de la Razor Page

**Prompt (tal cual):**

```
Estoy en un proyecto ASP.NET Core con Razor Pages. Creá una página nueva llamada
InscripcionTaller (InscripcionTaller.cshtml y InscripcionTaller.cshtml.cs) para
un formulario de inscripción a talleres extracurriculares. Por ahora solo quiero
el esqueleto: la directiva @page, @model, la clase InscripcionTallerModel que
herede de PageModel, un método OnGet vacío y un handler OnPostInscribir vacío.
No agregues propiedades ni lógica todavía. Explicame brevemente qué hace cada
parte.
```

**Qué generó Copilot:** el código de ambos archivos, pero solo lo mostró en el chat; no creó los archivos.

**Qué acepté:** el contenido de los dos archivos.

**Qué modifiqué:** creé los archivos a mano en `Pages` y pegué el código. Ajusté el `@model` al namespace del proyecto (`practicauno.Pages.InscripcionTallerModel`).

**Qué entendí:** `@page` convierte el archivo en una Razor Page con URL propia; `@model` la vincula con su clase; `OnGet` atiende el GET y `OnPostInscribir` atiende el POST del formulario enviado con `asp-page-handler="Inscribir"`.

---

## Prompt 2: Propiedades y tipos de datos

**Prompt (tal cual):**

```
En InscripcionTallerModel agregá las propiedades con [BindProperty] para estos
campos, usando el tipo de dato correcto para cada uno (no todo string):
nombre completo (string), DNI (int), email (string), confirmar email (string),
fecha de nacimiento (DateTime), taller elegido (string), turno (string),
acepta términos (bool), comentarios (string). Todavía NO agregues Data
Annotations. Explicame por qué elegiste cada tipo.
```

**Qué generó Copilot:** las nueve propiedades con `[BindProperty]`, con `DNI` como `int` y `FechaNacimiento` como `DateTime` (no anulables), y una explicación de cada tipo.

**Qué acepté:** la estructura y los tipos de texto (`string`), `bool` para el checkbox.

**Qué modifiqué después:** `int` y `DateTime` no anulables tienen un problema: si el campo llega vacío, el binding les asigna `0` o `01/01/0001`, y `[Required]` no lo detecta. Los cambié a `int?` y `DateTime?` 

---

## Prompt 3: Data Annotations

**Prompt (tal cual):**

```
Agregá Data Annotations a las propiedades de InscripcionTallerModel:
- Nombre: [Required] y [StringLength] mínimo 3, máximo 60
- DNI: [Required] y [Range] con valores razonables para un DNI argentino real
- Email: [Required] y [EmailAddress]
- Confirmar email: [Compare("Email")]
- Fecha de nacimiento: [Required] y [Range(typeof(DateTime), ...)] con un rango
  razonable
- Taller y Turno: [Required]
- Comentarios: opcional, [StringLength(300)]
Todos con mensajes de error en español, claros para el usuario. Explicame
cualquier limitación que tenga alguno de estos atributos.
```

**Qué generó Copilot:** todas las anotaciones con mensajes en español, ya con `int?` y `DateTime?`, y una lista de limitaciones:
- `[Required]` no detecta vacío en tipos de valor no anulables.
- `[Range(typeof(DateTime), ...)]` exige literales estáticos (usó `"2026-10-07"`, la fecha de hoy, como tope).
- `[EmailAddress]` valida solo el formato.
- `[Compare]` no obliga a que el campo de confirmación esté presente.
- Un `bool` no anulable siempre tiene valor, así que `[Required]` no obliga a marcar el checkbox.

**Qué acepté:** las anotaciones, los tipos anulables y todas las limitaciones, que verifiqué como correctas.

**Qué detecté y modifiqué:**
- **Rango del DNI:** `[Range(1, 99999999)]` deja pasar un DNI de `5`. Un DNI real tiene 7 u 8 dígitos, por lo que el mínimo correcto es `1000000`.
- **Tope fijo de la fecha:** `2026-10-07` quedaría desactualizado, y ninguna fecha fija puede expresar "no mayor que hoy". Lo cambié a `2026-12-31` y resolví "no futura" en el handler (Prompt 12).
- **`ConfirmarEmail` sin `[Required]`:** si queda vacío, el mensaje de `[Compare]` sería confuso. Agregué `[Required]`.
- **`AceptaTerminos`:** no lleva atributo; se valida a mano en el handler. Descarté la sugerencia de Copilot de usar `IValidatableObject` o un atributo personalizado por ser más complejo de lo que el TP requiere y porque no podría defenderlo línea por línea.

---

## Prompt 4: Ajustes derivados de las limitaciones

**Prompt (tal cual):**

```
Aplicá estos ajustes a InscripcionTallerModel:
1. Cambiá DNI a int? y FechaNacimiento a DateTime? para que [Required]
   detecte el campo vacío.
2. Agregá [Required] a ConfirmarEmail, con mensaje en español.
3. En el [Range] de FechaNacimiento usá un rango fijo en formato "yyyy-MM-dd"
   desde "1900-01-01" hasta "2026-12-31" (sin depender de la cultura), y
   avisame que la validación de "no fecha futura" la voy a hacer en el handler.
4. Dejá AceptaTerminos como bool sin atributos de validación, porque lo voy a
   validar manualmente en el handler con AddModelError.
No cambié nada más.
```



**Qué generó Copilot:** aplicó los cuatro cambios, y me recordó que la validación de fecha futura iría en el handler.

**Qué acepté:** todo.

**Qué quedó pendiente:** este prompt no incluía el rango del DNI, por lo que el `[Range(1, 99999999)]` siguió igual. Lo corregí aparte a `[Range(1000000, 99999999)]`

---

## Prompt 5: Constantes y `CalcularEdad`

**Prompt (tal cual):**

```
En InscripcionTallerModel agregá:
1. Constantes (const) para los valores fijos del negocio: las edades mínimas de
   cada taller (Programación 14, Diseño Gráfico 16, Oficios 18).
2. Un método privado CalcularEdad(DateTime fechaNacimiento) que devuelva un int
   con la edad exacta en años a la fecha de hoy, contemplando si ya cumplió años
   este año o no.
Explicame la lógica del cálculo línea por línea.
```

**Qué generó Copilot:** tres constantes `MIN_EDAD_*` y el método:

```csharp
var hoy = DateTime.Today;
int edad = hoy.Year - fechaNacimiento.Year;
if (fechaNacimiento > hoy.AddYears(-edad)) { edad--; }
return edad;
```

**Qué acepté:** todo, sin cambios. Entiendo la lógica: resta los años en bruto y, si el cumpleaños de este año todavía no ocurrió, resta uno.

**Qué verifiqué:** pregunté qué hace `hoy.AddYears(-edad)` (Copilot respondió que devuelve la fecha de hoy trasladada `edad` años atrás, o sea el último cumpleaños). Noté que Copilot había afirmado antes que el método "maneja correctamente años bisiestos", y en la respuesta siguiente matizó que para nacidos el 29-feb `AddYears` ajusta a 28-feb. Es una afirmación que primero fue demasiado confiada. Para este TP no es relevante, pero lo registro.

---

## Prompt 6: `ObtenerEdadMinima` con switch (Unidad IV)

**Prompt (tal cual):**

```
Creá un método privado ObtenerEdadMinima(string taller) que devuelva un int
usando un switch para devolver la edad mínima según el taller elegido
(usando las constantes que definimos). Para un taller desconocido, que devuelva
0 o maneje el caso en el default de forma segura. Explicame por qué elegiste
switch en lugar de if/else if.
```

**Qué generó Copilot:**

```csharp
switch (taller.Trim())
{
    case "Programación":
    case "Programacion":
        return MIN_EDAD_PROGRAMACION;
    case "Diseño Gráfico":
    case "Diseno Grafico":
    case "DisenoGráfico":
    case "DisenoGrafico":
        return MIN_EDAD_DISENO_GRAFICO;
    case "Oficios":
        return MIN_EDAD_OFICIOS;
    default:
        return 0;
}
```

**Qué rechacé y por qué:** las variantes sin tilde y el `Trim()`. El taller se elige desde un `<select>` con opciones fijas, así que el valor que llega es exactamente el texto de la opción. Eran casos imposibles: código defensivo innecesario. Además, los nombres de los talleres estaban escritos a mano en el `switch` y se repetirían en la vista, con riesgo de desincronizarse.

**Qué hice:** lo corregí con el Prompt 8.

---

## Prompt 7: Handler `OnPostInscribir`

**Prompt (tal cual):**

```
Implementá el handler OnPostInscribir con esta lógica, en este orden:
1. Si el checkbox de términos no está marcado, agregar un error al ModelState
   con AddModelError.
2. Calcular la edad con CalcularEdad y compararla con ObtenerEdadMinima del
   taller elegido; si es menor, agregar un error al ModelState con un mensaje
   claro (que mencione el taller y la edad mínima).
3. Si ModelState no es válido, usar un foreach que recorra ModelState.Values y
   sus Errors para armar una List<string> de errores, guardarla en una propiedad
   para mostrarla en la vista, y devolver Page().
4. Si todo es válido, guardar un mensaje de éxito y devolver Page().
Explicame cómo funciona el postback en este flujo.
```

**Qué generó Copilot:** el handler completo (`IActionResult OnPostInscribir`), las propiedades `Errores` y `MensajeExito`, el `AddModelError` para el checkbox, la regla de edad con `edadMinima > 0 && edad < edadMinima`, y los dos `foreach` anidados.

**Qué acepté:** toda la estructura, que cumple la consigna: handler con nombre significativo, `AddModelError` para el checkbox, regla de edad en código propio y `foreach` sobre `ModelState`.




---

## Prompt 8: Corrección del switch (sugerencia innecesaria de la IA)

**Prompt (tal cual):**

```
Corregí ObtenerEdadMinima en InscripcionTallerModel:
1. Dejá un solo case por taller, usando las constantes TALLER_PROGRAMACION,
   TALLER_DISENO_GRAFICO y TALLER_OFICIOS. Sacá el Trim() y todas las variantes
   sin tilde, porque el valor viene de un select con opciones fijas.
2. Definí esas tres constantes con los textos exactos "Programación",
   "Diseño Gráfico" y "Oficios", y usá esas mismas constantes para llenar una
   propiedad List<string> Talleres, así el texto no se escribe en dos lugares.
3. Mantené el default devolviendo 0.
4. En el handler OnPostInscribir, agregá una validación: si Taller no está en
   la lista Talleres, agregá un error al ModelState con AddModelError
   (nameof(Taller)) y el mensaje "El taller seleccionado no es válido."
No cambies nada más.
```

**Qué generó Copilot:** un solo `case` por taller con las constantes, `List<string> Talleres` inicializada con esas mismas constantes, y la validación de taller en el handler.

**Qué acepté:** todo. Ahora el texto de cada taller está escrito en un solo lugar y el select (`Model.Talleres`) y el switch no se pueden desincronizar.

**Qué modifiqué:** en mi prompt dije `nameof(Taller)`, pero mi propiedad se llama `TallerElegido`. Copilot lo resolvió correctamente usando `nameof(TallerElegido)`. Dejó además el chequeo `string.IsNullOrWhiteSpace(taller)` al inicio de `ObtenerEdadMinima`, que no pedí quitar; es redundante pero inofensivo.

**Por qué importa la validación de taller:** aunque el usuario solo pueda elegir del desplegable, alguien podría manipular la petición y mandar un taller falso, que con `default: return 0` pasaría la regla de edad.

---

## Prompt 9: Vista `.cshtml`

**Prompt (tal cual):**

```
Completá InscripcionTaller.cshtml con el formulario (method="post",
asp-page-handler="Inscribir"). Usá: input de texto para nombre, input numérico
para DNI, inputs para email y confirmar email, <input type="date"> para la
fecha de nacimiento, <select> con los 3 talleres, botones de radio para el
turno (Mañana / Tarde), checkbox para términos, <textarea> para comentarios.
Cada campo con su label y su span asp-validation-for. Arriba del formulario,
mostrá la lista de errores recopilada con el foreach y el mensaje de éxito si
corresponde. Incluí los scripts de validación del lado del cliente
(_ValidationScriptsPartial). Usá Bootstrap básico para que se vea prolijo.
```

**Qué generó Copilot:** el formulario completo con todos los controles pedidos, la lista de errores con `foreach`, el mensaje de éxito y la inclusión de `_ValidationScriptsPartial`.

**Qué acepté:** todo, y luego lo audité (Prompt 10). Revisé los `id`/`for` de los radios y del checkbox, y que cada campo tuviera su `asp-validation-for`.


---

## Prompt 12: Validación de fecha futura

**Prompt (tal cual):**

```
En el handler OnPostInscribir, antes de calcular la edad, agregá una validación:
si FechaNacimiento tiene valor y es posterior a la fecha de hoy, agregá un
error al ModelState con AddModelError(nameof(FechaNacimiento)) y el mensaje
"La fecha de nacimiento no puede ser futura."
Restricciones:
- No uses IValidatableObject ni atributos personalizados.
- No modifiques el [Range] de FechaNacimiento.
- No agregues validación del lado del cliente ni scripts de jQuery: la
  validación del servidor es suficiente.
- No cambies nada más.
```

**Qué generó Copilot:** agregó el bloque en el handler, ubicado antes del cálculo de edad:

```csharp
if (FechaNacimiento.HasValue && FechaNacimiento.Value > DateTime.Today)
{
    ModelState.AddModelError(nameof(FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
}
```

**Qué acepté:** el bloque tal cual. Cumplió las restricciones: no tocó el `[Range]`, no agregó JavaScript ni `IValidatableObject`.

**Qué detecté después (problema real de este código):**

1. **Se corrompieron los acentos en todo el archivo.** En la respuesta de este prompt, Copilot devolvió el `.cs` con `�` en lugar de las vocales acentuadas y de la ñ: `"Programaci�n"`, `"Dise�o Gr�fico"`, `"n�mero"`, `"electr�nico"`, `"v�lido"`. Es consecuencia del problema de codificación del punto 1 de la auditoría (ver caso de error 1). Pedí un cambio de tres líneas y la IA reescribió el archivo entero y dañó el texto. Si hubiera pegado el resultado sin revisarlo, el select y los mensajes mostrarían caracteres rotos.


---

## Prompt 13: Ajustes de la vista

**Prompt (tal cual):**

```
Revisá InscripcionTaller.cshtml y hacé solo estos ajustes:
1. En el <form>, sacá la clase "needs-validation" porque no tiene efecto (no hay
   JavaScript propio que la use). Mantené el atributo novalidate.
2. En el input del DNI, cambiá min="1" por min="1000000" y agregá
   max="99999999" para que coincida con el [Range] del modelo. Mantené
   type="number" y step="1".
3. No cambies los labels, el select, los radios, el checkbox, la lista de
   Errores ni la inclusión de _ValidationScriptsPartial.
4. No agregues JavaScript, estilos nuevos ni validación del lado del cliente.
No cambies nada más.
```

**Qué generó Copilot:** el `.cshtml` sin la clase `needs-validation`, con `<form method="post" asp-page-handler="Inscribir" novalidate>` y el DNI con `min="1000000" max="99999999" step="1"`.

**Qué acepté:** todo, y verifiqué que no tocó nada más.

**Qué dejé a propósito y por qué:**
- `novalidate`: la validación queda en el servidor, y así se ve funcionar el `foreach` de `Errores`, que pide la consigna.
- Los errores aparecen dos veces (lista superior y debajo de cada campo): es esperable, la consigna pide ambos mecanismos.

---

## Casos concretos de error, alucinación o sugerencia incorrecta de la IA

### Caso 1 (principal): codificación de caracteres

- **Qué pasó:** en la auditoría, Copilot señaló como "crítico" que las constantes tenían caracteres mal codificados (`Programaci�n`). Al principio lo tomé como un falso positivo, porque en Visual Studio mis acentos se veían bien. Después abrí el archivo y la barra de estado mostraba **Windows 1252**, no UTF-8. Es decir: el archivo estaba guardado en una codificación que otras herramientas (Copilot, GitHub, el equipo del docente) pueden leer mal. Cuando Copilot reescribió el archivo en el Prompt 12, escribió los `�` en el código real, confirmando el problema.
- **Por qué importa:** el `select` y el `switch` usan las mismas constantes, así que no habría desajuste entre ambos, pero el texto visible para el usuario y los mensajes de error saldrían con caracteres rotos.
- **Qué hice:** guardé `InscripcionTaller.cshtml.cs` y `InscripcionTaller.cshtml` con codificación **UTF-8 con firma** (Archivo → Guardar como → Guardar con codificación) y verifiqué que los acentos y la ñ se vieran bien.
- **Valoración:** el hallazgo era parcialmente correcto. El código no estaba roto, pero la codificación sí era un problema real; Copilot lo exageró al llamarlo "crítico" y su propia corrección terminó dañando el archivo.
- **Lección:** una IA que reescribe un archivo completo para hacer un cambio pequeño puede introducir errores nuevos. Hay que revisar siempre el resultado, incluso en partes que no pedí tocar.

### Caso 2: rango del DNI demasiado permisivo (regla de negocio mal interpretada)

- **Qué generó Copilot:** `[Range(1, 99999999)]`.
- **Por qué era un error:** la consigna pide un rango "acorde a un DNI real", y con mínimo `1` un DNI de `5` pasa la validación. Un DNI argentino tiene 7 u 8 dígitos.
- **Corrección:** `[Range(1000000, 99999999, ErrorMessage = "El DNI debe ser un número entre {1} y {2}.")]`, y en la vista `min="1000000" max="99999999"` para mantener coherencia.
- **Prompt usado:**

```
En InscripcionTallerModel, cambiá el [Range] de la propiedad DNI de
(1, 99999999) a (1000000, 99999999), manteniendo el mismo mensaje de error
con {1} y {2}. No cambies nada más.
```



### Caso 3: `[Required]` con `bool` y tipos no anulables

- **Qué pasó:** Copilot generó `int` y `DateTime` no anulables, que no permiten que `[Required]` detecte un campo vacío. Su propia explicación de la Fase 3 reconoció la limitación.
- **Corrección:** `int?` y `DateTime?`. Para `AceptaTerminos`, `[Required]` no sirve porque `false` es un valor válido; lo resolví con `AddModelError` en el handler.

### Caso 4: sugerencia innecesaria (variantes sin tilde)

- **Qué pasó:** Copilot agregó casos como `"Programacion"`, `"Diseno Grafico"` y `Trim()` en el `switch`, lógica para entradas que no pueden ocurrir con un select de opciones fijas.
- **Corrección:** un solo `case` por taller con constantes compartidas (Prompt 8).

---

## Conclusión final

- Copilot acertó en lo estructural (handler, `ModelState`, `foreach`, tipos anulables), pero cometió errores de negocio (rango del DNI), agregó código innecesario (variantes del switch), marcó como problemas cosas que no lo eran (`[Display]`, `kvp.Key`) y dañó la codificación del archivo al reescribirlo.
- Verifiqué cada sugerencia contra mi código antes de aplicarla y acoté los prompts de corrección con restricciones ("No cambies nada más", "No uses IValidatableObject") para evitar cambios no pedidos.
- Todo el código aceptado puedo explicarlo línea por línea: el cálculo de edad con `AddYears`, la condición `edadMinima > 0 && edad < edadMinima`, los dos `foreach` anidados sobre `ModelState.Values` y el flujo de postback (binding, validación automática, validaciones propias con `AddModelError`, `IsValid` y `return Page()`).