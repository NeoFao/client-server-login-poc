// Importa extensiones que facilitan trabajar con JSON en HttpClient.
// En particular, habilita métodos como PostAsJsonAsync(...) y ReadFromJsonAsync<T>(...),
// que serializan/deserializan automáticamente usando System.Text.Json.
using System.Net.Http.Json;

// URL base del API al que este cliente va a llamar.
// En este caso apunta a tu backend local (https) corriendo en el puerto 7215.
const string baseUrl = "https://localhost:7215";

// Crea un HttpClientHandler, que es el "motor" de bajo nivel que maneja
// cómo HttpClient realiza las conexiones HTTP/HTTPS (certificados, proxies, etc.).
var handler = new HttpClientHandler
{
    // SOLO para desarrollo local (certificado dev)
    // Esta línea configura una validación de certificado personalizada para HTTPS.
    // Normalmente, un cliente HTTPS valida el certificado del servidor para evitar ataques MITM.
    // En entornos locales, el certificado de desarrollo puede no ser de confianza y fallar.
    //
    // DangerousAcceptAnyServerCertificateValidator: indica que se aceptará CUALQUIER certificado.
    // Eso evita errores de SSL/TLS en localhost, pero es inseguro y NO debe usarse en producción.
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

// Crea un HttpClient usando el handler anterior.
// "using var" asegura que HttpClient se libere (Dispose) automáticamente al terminar el scope,
// cerrando conexiones y liberando recursos asociados.
using var http = new HttpClient(handler)
{
    // BaseAddress define la URL base para solicitudes relativas.
    // Así, cuando uses "/health" realmente se enviará a "https://localhost:7215/health".
    BaseAddress = new Uri(baseUrl)
};

// Imprime en consola el paso que se va a ejecutar para que el flujo sea claro al correr el programa.
Console.WriteLine("1) Health check...");

// Hace una solicitud GET al endpoint /health.
// await: espera de forma asíncrona sin bloquear el hilo mientras llega la respuesta.
var health = await http.GetAsync("/health");

// Muestra el código HTTP de respuesta de /health en formato numérico y de enum.
// Ej: "200 OK" o "503 ServiceUnavailable".
Console.WriteLine($"Health: {(int)health.StatusCode} {health.StatusCode}");

// Lee el contenido (body) de la respuesta como texto plano (string) y lo imprime.
// Esto sirve para ver el detalle que el endpoint devuelve (por ejemplo "Healthy").
Console.WriteLine(await health.Content.ReadAsStringAsync());

// Escribe una línea en blanco para separar visualmente secciones en la salida de la consola.
Console.WriteLine();

// Indica el siguiente paso: autenticación (login).
Console.WriteLine("2) Login...");

// Construye el objeto que se enviará al endpoint de login.
// LoginRequest es un "record" (tipo inmutable) definido al final del archivo.
// Aquí se pasan credenciales de prueba.
var loginRequest = new LoginRequest("demo@demo.com", "Demo123!");

// Envía un POST a /auth/login con el objeto loginRequest serializado como JSON.
// PostAsJsonAsync:
// - Serializa loginRequest a JSON
// - Lo pone como cuerpo del request (Content-Type: application/json)
// - Envía la solicitud al servidor
// - Devuelve HttpResponseMessage con status, headers y body.
var loginResponse = await http.PostAsJsonAsync("/auth/login", loginRequest);

// Imprime el status code devuelto por el endpoint de login.
Console.WriteLine($"Login: {(int)loginResponse.StatusCode} {loginResponse.StatusCode}");

// Intenta leer/deserializar el body JSON de la respuesta a un LoginResponse.
// LoginResponse es un record que tiene una propiedad Token.
// Si el JSON no coincide, está vacío, o no se puede parsear, esto puede devolver null.
var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

// Valida que el login haya sido realmente exitoso:
// - IsSuccessStatusCode: true si el status está en rango 200-299
// - loginBody is null: por si no se pudo deserializar el JSON o no venía body
// - Token vacío o espacios: por si la API respondió pero no incluyó token válido
if (!loginResponse.IsSuccessStatusCode || loginBody is null || string.IsNullOrWhiteSpace(loginBody.Token))
{
    // Si algo falló, se informa por consola y se imprime el body crudo
    // para poder diagnosticar el error (mensaje, detalle, etc.).
    Console.WriteLine("Login failed. Body:");
    Console.WriteLine(await loginResponse.Content.ReadAsStringAsync());

    // Termina el programa inmediatamente para no continuar con llamadas autenticadas sin token.
    return;
}

// Si el login fue exitoso, imprime el token obtenido.
// Usualmente este token se usa para autorizar solicitudes subsecuentes.
Console.WriteLine($"Token: {loginBody.Token}");
Console.WriteLine();

// Indica el siguiente paso: llamar a /auth/me enviando un header con el token de sesión.
Console.WriteLine("3) Call /auth/me with X-Session-Token...");

// Asegura que no exista un header previo con el mismo nombre.
// Esto evita duplicar el header o mantener un valor viejo si el programa se ejecuta varias veces.
http.DefaultRequestHeaders.Remove("X-Session-Token");

// Agrega el header "X-Session-Token" con el token que devolvió el login.
// DefaultRequestHeaders aplica a todas las solicitudes que se hagan con este HttpClient después de esto.
http.DefaultRequestHeaders.Add("X-Session-Token", loginBody.Token);

// Hace un GET a /auth/me.
// Este endpoint típicamente devuelve la información del usuario autenticado,
// usando el token del header para identificar la sesión/usuario.
var meResponse = await http.GetAsync("/auth/me");

// Imprime el status code de la respuesta del endpoint /auth/me.
Console.WriteLine($"Me: {(int)meResponse.StatusCode} {meResponse.StatusCode}");

// Imprime el body completo como texto.
// Dependiendo del backend, podría ser un JSON con datos del usuario.
Console.WriteLine(await meResponse.Content.ReadAsStringAsync());

// Define el tipo de datos que se enviará en el login.
// record: crea un tipo inmutable con propiedades (Email, Password) y value-based equality.
// Este record se serializa automáticamente a JSON por PostAsJsonAsync.
record LoginRequest(string Email, string Password);

// Define el tipo de datos esperado en la respuesta del login.
// Se espera que el JSON de respuesta tenga una propiedad "token" (o "Token" según config)
// que se mapeará a esta propiedad Token.
record LoginResponse(string Token);
