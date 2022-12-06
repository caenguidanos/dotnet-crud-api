namespace Ecommerce.Store.Test.Infrastructure.Controller.Product;

using Ecommerce.Store.Application.Command;
using Ecommerce.Store.Domain.Entity;
using Ecommerce.Store.Domain.Exceptions;
using Ecommerce.Store.Domain.Notification;
using Ecommerce.Store.Infrastructure.Controller;

public class DeleteById
{
    private readonly ISender _sender = Mock.Of<ISender>();
    private readonly IPublisher _publisher = Mock.Of<IPublisher>();

    [SetUp]
    public void BeforeEach()
    {
        Mock.Get(_sender).Reset();
        Mock.Get(_publisher).Reset();
    }

    [Test]
    public async Task GivenRequestCommand_WhenReturnsIdFromMediator_ThenReplyWithOK()
    {
        var id = Product.NewID();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<DeleteProductCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(id);

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.DeleteById(id, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

        var actionResultObject = (OkObjectResult)actionResult;
        Assert.That(actionResultObject.Value, Is.TypeOf<Guid>());

        var actionResultObjectPayload = (Guid)actionResultObject.Value;
        Assert.That(actionResultObjectPayload, Is.EqualTo(id));
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsProductNotFoundExceptionFromMediator_ThenReplyWithNotFound()
    {
        var id = Product.NewID();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<DeleteProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<ProductNotFoundException>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.DeleteById(id, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<NotFoundResult>());

        Mock
            .Get(_publisher)
            .Verify(publisher => publisher
                .Publish(
                    It.Is<ProductLogNotification>(
                        notification => notification.Event == ProductLog.ControllerDeleteByIdNotFound),
                    It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsAnyExceptionFromMediator_ThenReplyWithNotImplemented()
    {
        var id = Product.NewID();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<DeleteProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<Exception>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.DeleteById(id, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<StatusCodeResult>());

        var actionResultObject = (StatusCodeResult)actionResult;
        Assert.That(actionResultObject.StatusCode, Is.EqualTo(StatusCodes.Status501NotImplemented));

        Mock
            .Get(_publisher)
            .Verify(publisher => publisher
                .Publish(
                    It.Is<ProductLogNotification>(
                        notification => notification.Event == ProductLog.ControllerDeleteByIdNotImplemented),
                    It.IsAny<CancellationToken>()));
    }
}

