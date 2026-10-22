using FluentValidation;

namespace OrderService.Application.DTOs.Validators
{
    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator()
        {
            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("El email no puede estar vacío.")
                .MaximumLength(100).WithMessage("El email no puede superar los 100 caracteres.");
        }
    }
}
