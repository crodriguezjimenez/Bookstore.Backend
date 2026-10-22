using InventoryService.API.Auth;

namespace InventoryService.Endpoints
{
    public static class AuthenticationTokenEndpoint
    {
        public static IEndpointRouteBuilder MapAuthenticationTokenEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/auth/token", (TokenRequest request, IConfiguration config) =>
            {
                var response = TokenService.GenerateToken(request, config);

                return response is not null
                    ? Results.Ok(response)
                    : Results.Json(new { Error = "invalid_client", Message = "Client ID o Secret incorrecto." }, statusCode: 401);
            })
            .WithName("GetAuthToken")
            .WithTags("Authentication")
            .WithSummary("Genera un token JWT usando Client Credentials para comunicación M2M");

            return app;
        }

    }
}

