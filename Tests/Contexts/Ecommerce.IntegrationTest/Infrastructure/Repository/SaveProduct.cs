namespace Ecommerce.IntegrationTest.Infrastructure.Repository;
public sealed class SaveProductIntegrationTest
{
    // private readonly PostgresDatabaseFactory _postgres = new(template: "ecommerce");

    // private readonly IDbContext _dbContext = Mock.Of<IDbContext>();

    // [OneTimeSetUp]
    // public async Task OneTimeSetUp()
    // {
    //     string connectionString = await _postgres.StartAsync();

    //     Mock
    //         .Get(_dbContext)
    //         .Setup(dbContext => dbContext.GetConnectionString())
    //         .Returns(connectionString);
    // }

    // [Test]
    // public async Task GivenProduct_WhenSave_ThenPass()
    // {
    //     var product = new Product
    //     {
    //         Id = new ProductId(Common.Domain.Schema.NewID()),
    //         Title = new ProductTitle("Super title 1"),
    //         Description = new ProductDescription("Super description 1"),
    //         Status = new ProductStatus(ProductStatusValue.Published),
    //         Price = new ProductPrice(200)
    //     };

    //     await new ProductRepository(_dbContext).Save(product, CancellationToken.None);
    // }
}
