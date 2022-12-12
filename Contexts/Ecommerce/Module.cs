namespace Ecommerce;

using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Ecommerce.Application.Service;
using Ecommerce.Domain.Repository;
using Ecommerce.Domain.Service;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Repository;

public static class EcommerceModule
{
    public static IServiceCollection AddEcommerceModule(this IServiceCollection services)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddMediator();

        services.AddSingleton<IDbContext, DbContext>();
        services.AddSingleton<IDbSeed, DbSeed>();
        services.AddSingleton<IProductCreatorService, ProductCreatorService>();
        services.AddSingleton<IProductUpdaterService, ProductUpdaterService>();
        services.AddSingleton<IProductRemoverService, ProductRemoverService>();
        services.AddSingleton<IProductRepository, ProductRepository>();

        return services;
    }

    public static IHost UseEcommerceDataSeed(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        scope.ServiceProvider.GetRequiredService<IDbSeed>().RunAsync().Wait();

        return host;
    }
}
