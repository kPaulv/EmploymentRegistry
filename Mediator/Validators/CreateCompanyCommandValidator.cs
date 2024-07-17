using FluentValidation;
using MediatorService.Commands;

namespace MediatorService.Validators
{
    public sealed class CreateCompanyCommandValidator : 
        AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator() {
            RuleFor(c => c.CompanyCreateDto.Name).NotEmpty().MaximumLength(60);
            RuleFor(c => c.CompanyCreateDto.Address).NotEmpty().MaximumLength(60);
        }
    }
}
