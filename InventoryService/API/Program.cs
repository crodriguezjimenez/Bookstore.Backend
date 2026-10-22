using FluentValidation;
using InventoryService.API.Endpoints;
using InventoryService.API.Middlewares;
using InventoryService.Endpoints;
using InventoryService.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructureServices();
// MediatR 
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InventoryService.Application.AssemblyReference).Assembly));

//FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(InventoryService.Application.AssemblyReference).Assembly);

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

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Bookstore Inventory API v1");

        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationTokenEndpoint();
app.MapBookEndpoints();

app.Run();

