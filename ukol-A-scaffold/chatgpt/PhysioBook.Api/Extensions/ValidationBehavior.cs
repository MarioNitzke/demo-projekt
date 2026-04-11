namespace PhysioBook.Api.Extensions;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IQueryPipelineBehavior<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error is not null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
