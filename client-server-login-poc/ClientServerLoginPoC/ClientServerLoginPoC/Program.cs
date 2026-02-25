using ClientServerLoginPoC.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TU REGISTRO VA AQUÍ (antes de Build)
builder.Services.AddSingleton<UserStore>();
builder.Services.AddSingleton<SessionStore>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Si NO estás usando [Authorize], esto no aporta (puedes quitarlo)
app.UseAuthorization();

app.MapControllers();

app.Run();
