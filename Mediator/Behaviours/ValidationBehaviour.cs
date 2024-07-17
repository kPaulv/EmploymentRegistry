using FluentValidation;
using MediatR;
using Entities.Exceptions.Validation;

namespace MediatorService.Behaviours
{
    public sealed class ValidationBehaviour<TRequest, TResponse> : 
        IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        // validators for CUD commands collection
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) => 
            _validators = validators;

        public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if(!_validators.Any())
                return await next();

            var validationContext = new ValidationContext<TRequest>(request);

            var errorsDict = _validators
                                .Select(x => x.Validate(validationContext))
                                .SelectMany(x => x.Errors)
                                .Where(x => x != null)
                                .GroupBy(x => x.PropertyName
                                                .Substring(x.PropertyName.IndexOf('.') + 1),
                                         x => x.ErrorMessage,
                                         (propertyName, errorMessages) => new
                                         {
                                             Key = propertyName,
                                             Values = errorMessages.Distinct().ToArray()
                                         })
                                .ToDictionary(x => x.Key, x => x.Values);

            if (errorsDict.Any())
                throw new FluentValidationException(errorsDict);

            return await next();
        }
    }
}
