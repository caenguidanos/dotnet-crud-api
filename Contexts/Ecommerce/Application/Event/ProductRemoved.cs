namespace Ecommerce.Application.Event;

using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository;
using Ecommerce.Domain.ValueObject;

public class ProductRemovedEvent : INotification
{
    public Guid Product { get; set; }
}

public class ProductRemovedHandler : INotificationHandler<ProductRemovedEvent>
{
    private readonly IProductRepository _productRepository;

    public ProductRemovedHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(ProductRemovedEvent notification, CancellationToken cancellationToken)
    {
        var newProductEventId = Common.Domain.Schema.NewID();

        var newProductEvent = new ProductEvent(
            new ProductEventId(newProductEventId),
            new ProductId(notification.Product),
            new ProductEventName("ecommerce_product_removed"));

        await _productRepository.SaveEvent(newProductEvent, cancellationToken);
    }
}