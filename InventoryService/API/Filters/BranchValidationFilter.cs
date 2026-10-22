using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace InventoryService.API.Filters;

public class BranchValidationFilter : IEndpointFilter
{
    private static readonly string[] AllowedBranches = { "NORTE", "ESTE", "OESTE" };

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var branchesParam = context.GetArgument<string?>(1);

        if (!string.IsNullOrWhiteSpace(branchesParam))
        {
            var branchList = branchesParam.Split(',').Select(b => b.Trim().ToUpper()).ToList();

            
            var invalidBranches = branchList.Where(b => !AllowedBranches.Contains(b)).ToList();

            if (invalidBranches.Any())
            {
                // Si hay inválidos, cortamos el pipeline y devolvemos un 400 Bad Request estructurado
                return Results.BadRequest(new
                {
                    Error = "Sucursal no válida",
                    Message = $"Las siguientes sucursales no existen: '{string.Join(", ", invalidBranches)}'. " +
                              "Las sucursales permitidas son: Norte, Este, Oeste."
                });
            }
        }

        return await next(context);
    }
}
