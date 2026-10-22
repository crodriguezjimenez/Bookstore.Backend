using FluentValidation;

namespace OrderService.Filters
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var argument = context.Arguments.FirstOrDefault(x => x is T) as T;

            if (argument == null)
            {
                return Results.BadRequest(new { Error = "Error de validación", Message = "El cuerpo de la petición no puede estar vacío." });
            }

            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator != null)
            {
                var validationResult = await validator.ValidateAsync(argument);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Results.BadRequest(new
                    {
                        Status = 400,
                        Title = "Uno o más errores de validación han ocurrido.",
                        Errors = errors
                    });
                }
            }

            return await next(context);
        }
    }
    
}
