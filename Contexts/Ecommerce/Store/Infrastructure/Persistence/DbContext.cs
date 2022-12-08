namespace Ecommerce.Store.Infrastructure.Persistence;

public class DbContext
{
    private readonly string _connectionString;

    public DbContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Ecommerce.Store") ?? throw new InvalidDataException("Trying to get [ConnectionStrings]");
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }

    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
