// Importa los tipos necesarios de ASP.NET Core MVC,
// incluyendo ControllerBase, IActionResult y atributos como ApiController/Route/HttpGet.
using Microsoft.AspNetCore.Mvc;

// Define el namespace del controlador para mantener organizada la estructura del proyecto.
namespace ClientServerLoginPoC.Controllers
{
    // Indica que esta clase es un controlador de API.
    // Esto activa convenciones de API (por ejemplo, ayuda con el model binding y metadatos).
    [ApiController]

    // Define la ruta base para este controlador.
    // Con esto, los endpoints de esta clase quedan bajo: /health
    [Route("health")]
    public class HealthController : ControllerBase
    {
        // Indica que este método responde a una solicitud HTTP GET.
        // Como no especifica ruta adicional, responde exactamente a:
        // GET /health
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "OK" });
        // Método Get:
        // - Devuelve un IActionResult, que representa una respuesta HTTP.
        // - Usa Ok(...) para responder con HTTP 200.
        // - El cuerpo de la respuesta es un objeto anónimo con una propiedad "status" = "OK".
        //   ASP.NET Core lo serializa automáticamente a JSON, típicamente:
        //   { "status": "OK" }
        // Este endpoint se usa como "health check" para verificar que la API está viva y respondiendo.
    }
}
