// Importa los modelos (DTOs) que este controlador usa como entrada/salida,
// por ejemplo: LoginRequest, LoginResponse, MeResponse.
using ClientServerLoginPoC.Models;

// Importa servicios/almacenamientos internos donde se validan usuarios y se manejan sesiones,
// por ejemplo: UserStore y SessionStore.
using ClientServerLoginPoC.Services;

// Importa tipos base de ASP.NET Core MVC, como ControllerBase, ActionResult, atributos, etc.
using Microsoft.AspNetCore.Mvc;

// Define el namespace del controlador para organizar el código dentro del proyecto.
namespace ClientServerLoginPoC.Controllers
{
    // Marca esta clase como un controlador de API.
    // Habilita comportamientos útiles como:
    // - Enlace automático de parámetros (model binding)
    // - Respuestas 400 automáticas en algunos casos (dependiendo configuración)
    // - Convenciones de API (metadatos para Swagger, etc.)
    [ApiController]

    // Define la ruta base del controlador.
    // Con esto, todos los endpoints aquí dentro quedarán bajo:
    // /auth/...
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        // Dependencia para manejo/validación de usuarios (por ejemplo credenciales).
        // "readonly" asegura que solo se asigna en el constructor.
        private readonly UserStore _users;

        // Dependencia para crear y validar sesiones (tokens) de autenticación.
        private readonly SessionStore _sessions;

        // Constructor del controlador.
        // ASP.NET Core inyecta automáticamente (DI) instancias de UserStore y SessionStore
        // si están registrados en el contenedor de dependencias.
        public AuthController(UserStore users, SessionStore sessions)
        {
            // Guarda la referencia al servicio de usuarios para usarlo en los endpoints.
            _users = users;

            // Guarda la referencia al servicio de sesiones para crear/validar tokens.
            _sessions = sessions;
        }

        // Indica que este método responde a HTTP POST en la ruta:
        // POST /auth/login
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            // Valida que Email y Password vengan con contenido (no nulos, no vacíos, no solo espacios).
            // Si falta alguno, responde 400 Bad Request con un mensaje explicativo.
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and Password are required.");

            // Verifica las credenciales usando el UserStore.
            // Validate(...) debería devolver true si el correo/clave coinciden con un usuario válido.
            var valid = _users.Validate(request.Email, request.Password);

            // Si las credenciales no son válidas, devuelve 401 Unauthorized (sin cuerpo en este caso).
            if (!valid)
                return Unauthorized();

            // Si el login es válido, crea una sesión asociada al email.
            // CreateSession(...) típicamente genera un token único y lo almacena en SessionStore.
            var token = _sessions.CreateSession(request.Email);

            // Devuelve 200 OK con un LoginResponse que contiene el token.
            // Ese token luego lo usará el cliente en el header X-Session-Token.
            return Ok(new LoginResponse(token));
        }

        // Indica que este método responde a HTTP GET en la ruta:
        // GET /auth/me
        [HttpGet("me")]
        public ActionResult<MeResponse> Me()
        {
            // Lee el header "X-Session-Token" del request actual.
            // Request.Headers[...] devuelve StringValues; ToString() lo convierte a string.
            var token = Request.Headers["X-Session-Token"].ToString();

            // Si el cliente no mandó el token (o viene vacío), responde 401 Unauthorized
            // con un mensaje indicando qué header falta.
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized("Missing X-Session-Token.");

            // Intenta encontrar el email asociado a ese token en el SessionStore.
            // Si el token existe y es válido, devuelve true y coloca el email en la variable out.
            // Si no existe/no es válido, devuelve false.
            if (!_sessions.TryGetEmail(token, out var email))
                return Unauthorized("Invalid token.");

            // Si el token es válido, responde 200 OK con un MeResponse que contiene el email.
            // Esto representa "quién soy" (el usuario autenticado actual).
            return Ok(new MeResponse(email));
        }
    }
}


