using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CVSalis.Config
{
    public class DataContext
    {
        private readonly IConfiguration _configuration;
        private DBConfig _dbConfig;
        string connectionId = "DefaultConnection";
        public DataContext(IOptions<DBConfig> dbconfig, IConfiguration configuration)
        {
            _dbConfig = dbconfig.Value;
            _configuration = configuration;
           
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString(connectionId); 
            return new NpgsqlConnection(connectionString);
        }

        public async Task Init()
        {
            await _initDatabase();
            await _initTables();
        }

        private async Task _initDatabase()
        {
            // create database if it doesn't exist
            var connectionString = _configuration.GetConnectionString(connectionId);
            using var connection = new NpgsqlConnection(connectionString);
            var sqlDbCount = $"SELECT COUNT(*) FROM pg_database WHERE datname = '{_dbConfig.Database}';";
            var dbCount = await connection.ExecuteScalarAsync<int>(sqlDbCount);
            if (dbCount == 0)
            {
                var sql = $"CREATE DATABASE \"{_dbConfig.Database}\"";
                await connection.ExecuteAsync(sql);
            }
        }

        private async Task _initTables()
        {
            // create tables if they don't exist
            using var connection = CreateConnection();

        }
    }
}
