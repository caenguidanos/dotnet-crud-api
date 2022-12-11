namespace Ecommerce.Infrastructure.Repository;

using Dapper;
using Npgsql;

using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Exceptions;
using Ecommerce.Domain.Repository;
using Ecommerce.Domain.ValueObject;
using Ecommerce.Infrastructure.DataTransfer;
using Ecommerce.Infrastructure.Persistence;

public sealed class ProductRepository : IProductRepository
{
    private IDbContext _dbContext { get; init; }

    public ProductRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Product>> Get(CancellationToken cancellationToken)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_dbContext.GetConnectionString());
            await conn.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT * FROM public.product
            ";

            var command = new CommandDefinition(sql, cancellationToken: cancellationToken);

            var result = await conn.QueryAsync<ProductPrimitives>(command).ConfigureAwait(false);
            if (result is null)
            {
                return Enumerable.Empty<Product>();
            }

            Func<ProductPrimitives, Product> productsSelector = p =>
            {
                var product = new Product
                {
                    Id = new ProductId(p.Id),
                    Title = new ProductTitle(p.Title),
                    Description = new ProductDescription(p.Description),
                    Status = new ProductStatus(p.Status),
                    Price = new ProductPrice(p.Price)
                };

                product.AddTimeStamp(p.updated_at, p.created_at);

                return product;
            };

            return result.Select(productsSelector).AsList();
        }
        catch (Exception ex)
        {
            if (ex is PostgresException)
            {
                throw new ProductPersistenceException(ex.Message);
            }

            throw;
        }
    }

    public async Task<Product> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_dbContext.GetConnectionString());
            await conn.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT * FROM public.product WHERE id = @Id
            ";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            var result = await conn.QueryFirstOrDefaultAsync<ProductPrimitives>(command).ConfigureAwait(false);
            if (result is null)
            {
                throw new ProductNotFoundException();
            }

            var product = new Product
            {
                Id = new ProductId(result.Id),
                Title = new ProductTitle(result.Title),
                Description = new ProductDescription(result.Description),
                Status = new ProductStatus(result.Status),
                Price = new ProductPrice(result.Price)
            };

            product.AddTimeStamp(result.updated_at, result.created_at);

            return product;
        }
        catch (Exception ex)
        {
            if (ex is PostgresException)
            {
                throw new ProductPersistenceException(ex.Message);
            }

            throw;
        }
    }

    public async Task Save(Product product, CancellationToken cancellationToken)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_dbContext.GetConnectionString());
            await conn.OpenAsync(cancellationToken);

            const string sql = @"
                INSERT INTO public.product (id, title, description, price, status)
                VALUES (@Id, @Title, @Description, @Price, @Status)
            ";

            var primitives = product.ToPrimitives();

            var parameters = new DynamicParameters();
            parameters.Add("Id", primitives.Id);
            parameters.Add("Title", primitives.Title);
            parameters.Add("Description", primitives.Description);
            parameters.Add("Price", primitives.Price);
            parameters.Add("Status", primitives.Status);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            await conn.ExecuteAsync(command).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex is not PostgresException postgresException)
            {
                throw;
            }

            switch (postgresException.SqlState)
            {
                case PostgresErrorCodes.UniqueViolation:
                    {
                        if (postgresException.ConstraintName == ProductConstraints.UniqueTitle)
                        {
                            throw new ProductTitleUniqueException();
                        }

                        break;
                    }
                case PostgresErrorCodes.CheckViolation:
                    {
                        switch (postgresException.ConstraintName)
                        {
                            case ProductConstraints.CheckTitle:
                                throw new ProductTitleInvalidException();

                            case ProductConstraints.CheckDescription:
                                throw new ProductDescriptionInvalidException();

                            case ProductConstraints.CheckStatus:
                                throw new ProductStatusInvalidException();

                            case ProductConstraints.CheckPrice:
                                throw new ProductPriceInvalidException();

                            default:
                                break;
                        }

                        break;
                    }

                default:
                    break;
            }

            throw new ProductPersistenceException(ex.Message);

        }
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_dbContext.GetConnectionString());
            await conn.OpenAsync(cancellationToken);

            const string sql = @"
                DELETE FROM public.product WHERE id = @Id
            ";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            int result = await conn.ExecuteAsync(command).ConfigureAwait(false);
            if (result is 0)
            {
                throw new ProductNotFoundException();
            }
        }
        catch (Exception ex)
        {
            if (ex is PostgresException)
            {
                throw new ProductPersistenceException(ex.Message);
            }

            throw;
        }
    }

    public async Task Update(Product product, CancellationToken cancellationToken)
    {
        try
        {
            await using var conn = new NpgsqlConnection(_dbContext.GetConnectionString());
            await conn.OpenAsync(cancellationToken);

            const string sql = @"
                UPDATE public.product
                SET title = @Title, description = @Description, price = @Price, status = @Status
                WHERE id = @Id
            ";

            var primitives = product.ToPrimitives();

            var parameters = new DynamicParameters();
            parameters.Add("Id", primitives.Id);
            parameters.Add("Title", primitives.Title);
            parameters.Add("Description", primitives.Description);
            parameters.Add("Price", primitives.Price);
            parameters.Add("Status", primitives.Status);

            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

            await conn.ExecuteAsync(command).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex is not PostgresException postgresException)
            {
                throw;
            }

            switch (postgresException.SqlState)
            {
                case PostgresErrorCodes.UniqueViolation:
                    {
                        if (postgresException.ConstraintName == ProductConstraints.UniqueTitle)
                        {
                            throw new ProductTitleUniqueException();
                        }

                        break;
                    }
                case PostgresErrorCodes.CheckViolation:
                    {
                        switch (postgresException.ConstraintName)
                        {
                            case ProductConstraints.CheckTitle:
                                throw new ProductTitleInvalidException();

                            case ProductConstraints.CheckDescription:
                                throw new ProductDescriptionInvalidException();

                            case ProductConstraints.CheckStatus:
                                throw new ProductStatusInvalidException();

                            case ProductConstraints.CheckPrice:
                                throw new ProductPriceInvalidException();

                            default:
                                break;
                        }

                        break;
                    }

                default:
                    break;
            }

            throw new ProductPersistenceException(ex.Message);
        }
    }
}
