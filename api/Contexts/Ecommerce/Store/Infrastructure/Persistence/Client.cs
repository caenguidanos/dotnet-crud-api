using FaunaDB.Client;
using api.Contexts.Ecommerce.Store.Infrastructure.Environment;

namespace api.Contexts.Ecommerce.Store.Infrastructure.Persistence
{
    public class DatabaseClient
    {
        public readonly FaunaClient client;

        public DatabaseClient(ConfigurationSettings configuration)
        {
            var http = new HttpClient();

            http.Timeout = TimeSpan.FromSeconds(20);

            Console.WriteLine(configuration.FaunadbEcommerceStoreSecret);

            client = new FaunaClient(
                secret: configuration.FaunadbEcommerceStoreSecret,
                httpClient: http
            );
        }
    }
}