namespace ClientServerLoginPoC.Models
{
    // se usa típicamente como respuesta (response body) en JSON
    // para indicar "quién soy" según el token enviado.
    // Genera automáticamente una propiedad pública init-only (inmutable tras crearse)
    public record MeResponse(string Email);

}

