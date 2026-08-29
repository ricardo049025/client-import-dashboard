using Domain.Entities.Contexts;
using Domain.Entities.Seeders;
using Infraestructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Services.Main;
using webApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddConfiguredCors(builder.Configuration);
builder.Services.AddDbContext<ApiDbContext>(options => options.UseInMemoryDatabase("ClientImportDb"));
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
await AppDbSeeder.SeedAsync(context);

app.UseHttpsRedirection(); 
app.UseCors(CorsConfigurationExtension.FrontendPolicyName);
app.UseAuthorization();
app.ConfigureApiEndpoints();
app.MapControllers();
app.Run();
