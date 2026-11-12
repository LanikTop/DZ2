using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Helpers
{
    public class PostgresConnectionFactory : IPostgresConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public PostgresConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public NpgsqlConnection GetConnection() => new NpgsqlConnection(_configuration.GetConnectionString("Postgres"))
            ?? throw new ArgumentNullException("Postgres connection string not found");
    }
}
