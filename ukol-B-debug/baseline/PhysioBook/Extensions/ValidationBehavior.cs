using Cortex.Mediator.Queries;

namespace PhysioBook.Extensions;

public class ValidationBehavior<TQuery, TResult> : IQueryPipelineBehavior<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IEnumerable<IValidator<TQuery>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TQuery>> validators)
    {
        _validators = validators;
    }

    public async Task<TResult> Handle(TQuery query, QueryHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TQuery>(query);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }
        return await next();
    }
}
