// Importa ConcurrentDictionary, una estructura de datos thread-safe (segura para múltiples hilos)
// que permite leer/escribir sin que tengas que manejar locks manualmente en la mayoría de casos.
using System.Collections.Concurrent;

// Define el namespace donde se agrupan los servicios del proyecto.
// "Services" normalmente contiene clases que encapsulan lógica de negocio o infraestructura
// (como almacenamiento en memoria, acceso a datos, etc.).
namespace ClientServerLoginPoC.Services
{
    // SessionStore es una clase que funciona como "almacenamiento de sesiones" en memoria.
    // En este PoC, una sesión asocia un token (string) con un email (string).
    // La idea es: token -> email, para poder identificar al usuario autenticado.
    public class SessionStore
    {
        // Diccionario concurrente que guarda las sesiones.
        // Clave: token de sesión (string)
        // Valor: email asociado a ese token (string)
        //
        // ConcurrentDictionary permite acceso concurrente desde múltiples requests/hilos,
        // lo cual es importante en una API (varias llamadas al mismo tiempo).
        //
        // "= new();" inicializa la colección vacía usando el constructor por defecto.
        private readonly ConcurrentDictionary<string, string> _sessions = new();

        // Crea una nueva sesión para un email dado.
        // Devuelve el token generado para que el cliente lo use en futuras llamadas.
        public string CreateSession(string email)
        {
            // Genera un nuevo GUID (identificador único) y lo convierte a string.
            // "N" significa formato sin guiones (32 caracteres hex), por ejemplo:
            //  d85b140733d94a1bb2f3e7c96c0a4c1f
            // Esto se usa como token de sesión.
            var token = Guid.NewGuid().ToString("N");

            // Guarda o actualiza en el diccionario la asociación token -> email.
            // Si la clave (token) no existe, la agrega.
            // Si existiera (muy improbable porque el GUID es único), sobrescribe el valor.
            _sessions[token] = email;

            // Devuelve el token para que el controlador lo devuelva al cliente (LoginResponse).
            return token;
        }

        // Intenta obtener el email asociado a un token.
        // Devuelve:
        // - true si el token existe en el diccionario
        // - false si no existe (token inválido/no creado)
        //
        // "out string email" permite retornar el email encontrado sin crear otro objeto de retorno.
        public bool TryGetEmail(string token, out string email)
            // Expresión lambda con "=>": cuerpo del método en una sola línea.
            // TryGetValue intenta encontrar la clave "token".
            // Si la encuentra, coloca el valor en "email" y devuelve true.
            // Si no la encuentra, deja "email" con el valor por defecto y devuelve false.
            //
            // El "email!" es el operador null-forgiving:
            // - Le dice al compilador "confía en mí, email no será null aquí"
            // - Se usa para suprimir advertencias de nulabilidad en ciertos contextos
            // - No cambia el comportamiento en tiempo de ejecución (es solo para análisis del compilador)
            => _sessions.TryGetValue(token, out email!);
    }
}
