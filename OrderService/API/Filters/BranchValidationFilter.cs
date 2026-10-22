namespace OrderService.API.Filters;

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
                return Results.BadRequest(new
                {
                    Error = "Parámetro inválido",
                    Message = $"Las sucursales '{string.Join(", ", invalidBranches)}' no son válidas. " +
                              "Las permitidas son: Norte, Este, Oeste."
                });
            }
        }

        return await next(context);
    }
}
