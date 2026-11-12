using FirebirdSql.Data.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

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
