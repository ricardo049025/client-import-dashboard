using Domain.Domain.Interfaces.Services;
using Domain.Entities.Contexts;
using Domain.Entities.Seeders;
using Infraestructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using webApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApiDbContext>(options => options.UseInMemoryDatabase("ClientImportDb"));
builder.Services.RegisterRepositories();
builder.Services.AddScoped<IGenresService, Services.Main.GenresService>();
builder.Services.AddScoped<IAlbumsService, Services.Main.AlbumsService>();
builder.Services.AddScoped<ITracksService, Services.Main.TracksService>();
builder.Services.AddScoped<IDashboardService, Services.Main.DashboardService>();

var app = builder.Build();
//if (app.Environment.IsDevelopment()) app.MapOpenApi();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
await AppDbSeeder.SeedAsync(context);

app.UseHttpsRedirection(); 
app.UseAuthorization();
app.ConfigureApiEndpoints();
app.MapControllers();
app.Run();
