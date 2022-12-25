namespace Ecommerce.Infrastructure;

public sealed class CreateProductEndpoint : ICreateProductEndpoint
{
    private ISender _sender { get; }

    public CreateProductEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public async Task<IResult> HandleAsync(HttpContext context, [FromBody] CreateProductHttpRequestBody body, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand
        {
            Title = body.Title,
            Description = body.Description,
            Price = body.Price,
            Status = body.Status
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Results.Accepted(),
            exception => Results.Problem(exception.GetProblemDetails(instance: context.Request.Path))
        );
    }
}