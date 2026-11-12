
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using EmployeeManagement.Api.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

// Register API services.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure database persistence.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(
    provider => provider.GetRequiredService<ApplicationDbContext>());

// Register application services.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Replace the existing AddControllers registration.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();
app.UseExceptionHandler();
// Configure development services.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await DatabaseSeeder.SeedAsync(context);

    app.MapOpenApi();

    // Serve Swagger UI using the built-in OpenAPI document.
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Employee Management API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
