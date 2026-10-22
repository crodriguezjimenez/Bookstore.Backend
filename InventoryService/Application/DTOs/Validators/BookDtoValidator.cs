using FluentValidation;

namespace InventoryService.Application.DTOs.Books.Validators;

public class BookDtoValidator : AbstractValidator<BookDto>
{
    public BookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título del libro no puede estar vacío.")
            .MaximumLength(200).WithMessage("El título no puede superar los 200 caracteres.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("El nombre del autor es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del autor no puede superar los 100 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio del libro debe ser mayor a $0.00.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El inventario (stock) no puede ser un número negativo.");
    }
}