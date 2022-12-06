namespace Ecommerce.Store.Test.Infrastructure.Controller.Product;

using Ecommerce.Store.Application.Command;
using Ecommerce.Store.Domain.Entity;
using Ecommerce.Store.Domain.Exceptions;
using Ecommerce.Store.Domain.Notification;
using Ecommerce.Store.Infrastructure.Controller;
using Ecommerce.Store.Infrastructure.DTO;

public class Create
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
    public async Task GivenRequestCommand_WhenReturnsProductIdFromMediator_ThenReplyOK()
    {
        var newProduct = Mock.Of<NewProduct>();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<CreateProductCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(Product.NewID());

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.Create(newProduct, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

        var actionResultObject = (OkObjectResult)actionResult;
        Assert.That(actionResultObject.Value, Is.TypeOf<Guid>());
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsProductIdInvalidExceptionFromMediator_ThenReplyBadRequest()
    {
        var newProduct = Mock.Of<NewProduct>();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<CreateProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<ProductIdInvalidException>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.Create(newProduct, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsProductTitleInvalidExceptionFromMediator_ThenReplyBadRequest()
    {
        var newProduct = Mock.Of<NewProduct>();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<CreateProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<ProductTitleInvalidException>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.Create(newProduct, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<BadRequestResult>());

        Mock
            .Get(_publisher)
            .Verify(publisher => publisher
                .Publish(
                    It.Is<ProductLogNotification>(
                        notification => notification.Event == ProductLog.ControllerCreateBadRequest),
                    It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsProductDescriptionInvalidExceptionFromMediator_ThenReplyBadRequest()
    {
        var newProduct = Mock.Of<NewProduct>();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<CreateProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<ProductDescriptionInvalidException>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.Create(newProduct, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<BadRequestResult>());

        Mock
            .Get(_publisher)
            .Verify(publisher => publisher
                .Publish(
                    It.Is<ProductLogNotification>(
                        notification => notification.Event == ProductLog.ControllerCreateBadRequest),
                    It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsProductPriceInvalidExceptionFromMediator_ThenReplyBadRequest()
    {
        var newProduct = Mock.Of<NewProduct>();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<CreateProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<ProductPriceInvalidException>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.Create(newProduct, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<BadRequestResult>());

        Mock
            .Get(_publisher)
            .Verify(publisher => publisher
                .Publish(
                    It.Is<ProductLogNotification>(
                        notification => notification.Event == ProductLog.ControllerCreateBadRequest),
                    It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsProductStatusInvalidExceptionFromMediator_ThenReplyBadRequest()
    {
        var newProduct = Mock.Of<NewProduct>();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<CreateProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<ProductStatusInvalidException>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.Create(newProduct, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<BadRequestResult>());

        Mock
            .Get(_publisher)
            .Verify(publisher => publisher
                .Publish(
                    It.Is<ProductLogNotification>(
                        notification => notification.Event == ProductLog.ControllerCreateBadRequest),
                    It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task GivenRequestCommand_WhenThrowsAnyExceptionFromMediator_ThenReplyNotImplemented()
    {
        var newProduct = Mock.Of<NewProduct>();

        Mock
            .Get(_sender)
            .Setup(sender => sender
                .Send(
                    It.IsAny<CreateProductCommand>(),
                    It.IsAny<CancellationToken>())).Throws<Exception>();

        var controller = new ProductController(_sender, _publisher);

        var actionResult = await controller.Create(newProduct, CancellationToken.None);
        Assert.That(actionResult, Is.TypeOf<StatusCodeResult>());

        var actionResultObject = (StatusCodeResult)actionResult;
        Assert.That(actionResultObject.StatusCode, Is.EqualTo(StatusCodes.Status501NotImplemented));

        Mock
            .Get(_publisher)
            .Verify(publisher => publisher
                .Publish(
                    It.Is<ProductLogNotification>(
                        notification => notification.Event == ProductLog.ControllerCreateNotImplemented),
                    It.IsAny<CancellationToken>()));
    }
}
