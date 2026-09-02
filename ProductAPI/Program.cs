


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductApi.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// ===============================
// ADD CONTROLLERS
// ===============================

builder.Services.AddControllers();


// ===============================
// DATABASE
// ===============================

builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration
            .GetConnectionString(
                "DefaultConnection"));
    });


// ===============================
// JWT AUTHENTICATION
// ===============================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("=================================");
                Console.WriteLine("JWT AUTHENTICATION ERROR:");
                Console.WriteLine(context.Exception.Message);
                Console.WriteLine("=================================");

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = false,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };
    });

// ===============================
// SWAGGER
// ===============================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
                "Enter JWT token"
        });


    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});


var app = builder.Build();


// ===============================
// SWAGGER
// ===============================

app.UseSwagger();

app.UseSwaggerUI();


// ===============================
// HTTPS
// ===============================

app.UseHttpsRedirection();


// ===============================
// AUTHENTICATION
// ===============================

app.UseAuthentication();


// ===============================
// AUTHORIZATION
// ===============================

app.UseAuthorization();


// ===============================
// CONTROLLERS
// ===============================

app.MapControllers();


// ===============================
// RUN
// ===============================

app.Run();

