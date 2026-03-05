// Importa ConcurrentDictionary, una estructura de datos segura para múltiples hilos (thread-safe).
// En una API, varias solicitudes pueden ejecutarse al mismo tiempo, y esta colección ayuda a evitar
// errores por acceso concurrente sin tener que usar locks manualmente en la mayoría de casos.
using System.Collections.Concurrent;

// Define el namespace donde se agrupan los servicios del proyecto.
// Normalmente aquí van clases que encapsulan lógica de negocio o almacenamiento (aunque sea en memoria).
namespace ClientServerLoginPoC.Services
{
    // UserStore es un "almacén" simple de usuarios en memoria.
    // En este PoC, guarda pares email -> password para validar credenciales en el login.
    public class UserStore
    {
        // Diccionario concurrente que guarda los usuarios.
        // Clave: email (string)
        // Valor: contraseña (string)
        //
        // "= new();" inicializa el diccionario vacío usando el constructor por defecto.
        // Al ser ConcurrentDictionary, es seguro para accesos simultáneos desde distintos hilos.
        private readonly ConcurrentDictionary<string, string> _users = new();

        // Constructor de la clase.
        // Se ejecuta cuando el contenedor de dependencias (DI) crea una instancia de UserStore.
        public UserStore()
        {
            // Crea un usuario de prueba en memoria.
            // Esto permite que el endpoint /auth/login tenga al menos una cuenta válida.
            //
            // Asigna directamente una entrada al diccionario:
            // - Email: "demo@demo.com"
            // - Password: "Demo123!"
            //
            // Si la clave ya existiera, esta asignación sobrescribe el valor.
            _users["demo@demo.com"] = "Demo123!";
        }

        // Método que valida credenciales.
        // Devuelve true si:
        // 1) Existe un usuario con ese email en el diccionario, y
        // 2) La contraseña guardada coincide exactamente con la contraseña proporcionada.
        public bool Validate(string email, string password)
            // Sintaxis de expresión (=>): el método retorna el resultado de esta expresión booleana.
            //
            // _users.TryGetValue(email, out var saved):
            // - Busca el email en el diccionario.
            // - Si existe, guarda la contraseña almacenada en la variable local "saved" y retorna true.
            // - Si no existe, retorna false.
            //
            // "&& saved == password":
            // - Solo se evalúa si TryGetValue devolvió true (cortocircuito).
            // - Compara la contraseña guardada con la recibida, con igualdad exacta (case-sensitive).
            => _users.TryGetValue(email, out var saved) && saved == password;
    }
}
