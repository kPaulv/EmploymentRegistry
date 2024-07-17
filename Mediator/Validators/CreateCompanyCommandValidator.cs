using FluentValidation;
using FluentValidation.Results;
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

        // custom validation to replace validation inside controller actions
        public override ValidationResult Validate(ValidationContext<CreateCompanyCommand> context)
        {
            if (context.InstanceToValidate.CompanyCreateDto is null)
                return new ValidationResult(new[]
                {
                    new ValidationFailure("CompanyCreateDto", "CompanyCreateDto is null")                
                });

            return base.Validate(context);
        }
    }
}
