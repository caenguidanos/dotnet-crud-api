namespace Ecommerce.Infrastructure.Controller;

using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;

using Common.Application;

using Ecommerce.Application.Command;
using Ecommerce.Application.Query;
using Ecommerce.Infrastructure.DataTransfer;

[ApiController]
[Route("[controller]")]
public sealed class ProductController : ControllerBase
{
    private ISender _sender { get; init; }

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery();

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            body => new HttpResultResponse
            {
                Body = body,
                ContentType = MediaTypeNames.Application.Json
            },
            exception => new HttpResultResponse
            {
                Body = exception.GetProblemDetails(instance: Request.Path)
            }
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetProductById(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductQuery { Id = id };

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            body => new HttpResultResponse
            {
                Body = body,
                ContentType = MediaTypeNames.Application.Json
            },
            exception => new HttpResultResponse
            {
                Body = exception.GetProblemDetails(instance: Request.Path)
            }
        );
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductHttpRequestBody body,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand
        {
            Title = body.Title,
            Description = body.Description,
            Price = body.Price,
            Status = body.Status,
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            body => new HttpResultResponse
            {
                Body = body,
                ContentType = MediaTypeNames.Application.Json
            },
            exception => new HttpResultResponse
            {
                Body = exception.GetProblemDetails(instance: Request.Path)
            }
        );
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> RemoveProduct(
        [FromRoute(Name = "id")] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new RemoveProductCommand { Id = id };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            body => new HttpResultResponse
            {
                Body = body,
                StatusCode = HttpStatusCode.Accepted,
                ContentType = MediaTypeNames.Application.Json
            },
            exception => new HttpResultResponse
            {
                Body = exception.GetProblemDetails(instance: Request.Path)
            }
        );
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute(Name = "id")] Guid id,
        [FromBody] UpdateProductHttpRequestBody body,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Title = body.Title,
            Description = body.Description,
            Price = body.Price,
            Status = body.Status
        };

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            body => new HttpResultResponse
            {
                Body = body,
                StatusCode = HttpStatusCode.Accepted,
                ContentType = MediaTypeNames.Application.Json
            },
            exception => new HttpResultResponse
            {
                Body = exception.GetProblemDetails(instance: Request.Path)
            }
        );
    }
}
