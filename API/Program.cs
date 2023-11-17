using Microsoft.EntityFrameworkCore;
using SolveChess.DAL;
using SolveChess.DAL.Model;
using AuthenticationService = SolveChess.Logic.Service.AuthenticationService;
using IAuthenticationService = SolveChess.Logic.ServiceInterfaces.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySQL(Environment.GetEnvironmentVariable("SolveChess_MySQLConnectionString") ?? throw new Exception("No connection string found in .env variables!"));
});

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(provider =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    var jwtSecret = Environment.GetEnvironmentVariable("SolveChess_JwtSecret") ?? throw new Exception("No jwt secret string found in .env variables!");

    return new AuthenticationService(new AuthenticationDAL(dbContextOptions), jwtSecret);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder => builder.WithOrigins("https://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowOrigin");

app.MapControllers();

app.Run();
