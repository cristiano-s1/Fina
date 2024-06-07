using Fina.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string connectionString = "Server=PC-HOME\\SQLSERVER;Database=Fina;User ID=sa;Password=3271;TrustServerCertificate=true";

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
