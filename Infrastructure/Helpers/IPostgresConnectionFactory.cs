using Npgsql;

namespace Infrastructure.Helpers
{
    public interface IPostgresConnectionFactory
    {
        NpgsqlConnection GetConnection();
    }
}
