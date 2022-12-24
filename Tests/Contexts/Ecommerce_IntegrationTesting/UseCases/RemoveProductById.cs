namespace Ecommerce_IntegrationTesting;

public sealed class RemoveProductByIdIntegrationTest
{
    private EcommerceWebApplicationFactory _server = Mock.Of<EcommerceWebApplicationFactory>();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _server = new EcommerceWebApplicationFactory();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _server.DropDatabaseAsync();
    }

    [Test]
    public async Task GivenNoProductsOnDatabase_WhenRequestById_ThenReturnProblemDetails()
    {
        using var httpClient = _server.CreateClient();

        await _server.ExecuteSqlAsync("""
            TRUNCATE product;
        """);

        var response = await httpClient.DeleteAsync("/product/092cc0ea-a54f-48a3-87ed-0e7f43c023f1");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content.Headers.ContentType, Is.EqualTo(MediaTypeHeaderValue.Parse("application/problem+json")));
        });

        var responseBody = await response.Content.ReadAsStringAsync();

        const string responseBodySnapshot = """
            {
                "title": "NotFound",
                "status": 404,
                "detail": "Product not found with criteria",
                "instance": "/product/092cc0ea-a54f-48a3-87ed-0e7f43c023f1"
            }
        """;

        Assert.That(responseBody, Is.EqualTo(Json.MinifyString(responseBodySnapshot)));
    }

    [Test]
    public async Task GivenProductsOnDatabase_WhenRequestById_ThenReturnAccepted()
    {
        using var httpClient = _server.CreateClient();

        await _server.ExecuteSqlAsync("""
            TRUNCATE product;

            INSERT INTO product (id, title, description, price, status)
            VALUES ('092cc0ea-a54f-48a3-87ed-0e7f43c023f1', 'American Professional II Stratocaster', 'Great guitar', 219900, 1);

            INSERT INTO product (id, title, description, price, status)
            VALUES ('8a5b3e4a-3e08-492c-869e-317a4d04616a', 'Mustang Shelby GT500', 'Great car', 7900000, 1);
        """);

        var response = await httpClient.DeleteAsync("/product/8a5b3e4a-3e08-492c-869e-317a4d04616a");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Accepted));
            Assert.That(response.Content.Headers.ContentType, Is.EqualTo(MediaTypeHeaderValue.Parse("text/plain")));
        });

        var responseBody = await response.Content.ReadAsStringAsync();
        const string responseBodySnapshot = """
            Accepted
        """;

        Assert.That(responseBody, Is.EqualTo(Json.MinifyString(responseBodySnapshot)));

        var responsePostRemove = await httpClient.GetAsync("/product/8a5b3e4a-3e08-492c-869e-317a4d04616a");

        Assert.That(responsePostRemove.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}