using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.Endpoints;
using OrderService.API.Middlewares;
using OrderService.Endpoints;
using OrderService.Infrastructure.DependencyInjection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOrderInfrastructureServices();

// MediatR 
builder.Services.AddMediatR(cfg =>cfg.RegisterServicesFromAssembly(typeof(OrderService.Application.AssemblyReference).Assembly));

//FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(OrderService.Application.AssemblyReference).Assembly);

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadInventory", policy => policy.RequireClaim("scope", "inventory:read"));
    options.AddPolicy("CanWriteInventory", policy => policy.RequireClaim("scope", "inventory:write"));
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();


app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Bookstore Order Service API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationTokenEndpoint();
app.MapOrdersEndpoints();

app.Run();


