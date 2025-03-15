using System; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore; 
using csharpapi.Data; 
using csharpapi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar la conexión a la base de datos desde `appsettings.json`
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb"))
);

// Agrega soporte para controladores MVC
builder.Services.AddControllers();

// Agrega dependencias y servicios
builder.Services.AddSingleton<ControlConexion>();
builder.Services.AddSingleton<TokenService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Habilitar sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Habilitar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api Genérica C#",
        Version = "v1",
        Description = "API de prueba con ASP.NET Core y Swagger",
        Contact = new OpenApiContact
        {
            Name = "Soporte API",
            Email = "soporte@miapi.com",
            Url = new Uri("https://miapi.com/contacto")
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Middleware de Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Genérica C#");
        c.RoutePrefix = "swagger";
    });
}

// Middleware de seguridad y configuración
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseSession();
app.UseAuthorization();

app.MapControllers();
app.Run();
