namespace Entities.Exceptions.Validation
{
    public class FluentValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; set; }

        public FluentValidationException(IReadOnlyDictionary<string, string[]> errors) : 
            base("One or more errors occured during Validation.") => Errors = errors;
    }
}
