using System.Net.Http.Json;

const string baseUrl = "https://localhost:7215";

var handler = new HttpClientHandler
{
    // SOLO para desarrollo local (certificado dev)
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

using var http = new HttpClient(handler)
{
    BaseAddress = new Uri(baseUrl)
};

Console.WriteLine("1) Health check...");
var health = await http.GetAsync("/health");
Console.WriteLine($"Health: {(int)health.StatusCode} {health.StatusCode}");
Console.WriteLine(await health.Content.ReadAsStringAsync());
Console.WriteLine();

Console.WriteLine("2) Login...");
var loginRequest = new LoginRequest("demo@demo.com", "Demo123!");
var loginResponse = await http.PostAsJsonAsync("/auth/login", loginRequest);

Console.WriteLine($"Login: {(int)loginResponse.StatusCode} {loginResponse.StatusCode}");
var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

if (!loginResponse.IsSuccessStatusCode || loginBody is null || string.IsNullOrWhiteSpace(loginBody.Token))
{
    Console.WriteLine("Login failed. Body:");
    Console.WriteLine(await loginResponse.Content.ReadAsStringAsync());
    return;
}

Console.WriteLine($"Token: {loginBody.Token}");
Console.WriteLine();

Console.WriteLine("3) Call /auth/me with X-Session-Token...");
http.DefaultRequestHeaders.Remove("X-Session-Token");
http.DefaultRequestHeaders.Add("X-Session-Token", loginBody.Token);

var meResponse = await http.GetAsync("/auth/me");
Console.WriteLine($"Me: {(int)meResponse.StatusCode} {meResponse.StatusCode}");
Console.WriteLine(await meResponse.Content.ReadAsStringAsync());

record LoginRequest(string Email, string Password);
record LoginResponse(string Token);
