namespace ClientServerLoginPoC.Models
{
    // Este usa como el body de un POST (JSON), Controllers/AuthController
    public record LoginRequest(string Email, string Password);

}

