using Microsoft.EntityFrameworkCore;
using SolveChess.DAL;
using SolveChess.DAL.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationService = SolveChess.Logic.Service.AuthenticationService;
using IAuthenticationService = SolveChess.Logic.ServiceInterfaces.IAuthenticationService;
using SolveChess.API.Exceptions;
using SolveChess.Logic.DAL;
using SolveChess.Logic.Interfaces;
using SolveChess.Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var mysqlConnectionString = Environment.GetEnvironmentVariable("SolveChess_MySQLConnectionString") ?? throw new MissingEnvVariableException("No connection string found in .env variables!");
    options.UseMySQL(mysqlConnectionString);
});

builder.Services.AddScoped<IAuthenticationDal, AuthenticationDal>(provider =>
{
    var dbContextOptions = provider.GetRequiredService<DbContextOptions<AppDbContext>>();

    return new AuthenticationDal(dbContextOptions);
});

builder.Services.AddScoped<IJwtProvider, JwtProvider>(options =>
{
    var jwtSecret = Environment.GetEnvironmentVariable("SolveChess_JwtSecret") ?? throw new MissingEnvVariableException("No jwt secret string found in .env variables!");

    return new JwtProvider(jwtSecret);
});

builder.Services.AddScoped<HttpClient>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

string origins = Environment.GetEnvironmentVariable("SolveChess_CorsUrls") ?? throw new MissingEnvVariableException("No cors urls found in .env variables!");
string[] originArray = origins.Split(';');

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder => builder.WithOrigins(originArray)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecret = Environment.GetEnvironmentVariable("SolveChess_JwtSecret") ?? throw new MissingEnvVariableException("No jwt secret string found in .env variables!");

    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidIssuer = "SolveChess Authenticator",
        ValidAudience = "SolveChess API",
    };

    options.TokenValidationParameters = tokenValidationParameters;
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AccessToken"];
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("AllowOrigin");

app.MapControllers();

app.Run();
