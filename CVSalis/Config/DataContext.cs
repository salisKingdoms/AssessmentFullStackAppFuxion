using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CVSalis.Config
{
    public class DataContext
    {
        private DBConfig _dbConfig;

        public DataContext(IOptions<DBConfig> dbconfig)
        {
            _dbConfig = dbconfig.Value;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = $"Host={_dbConfig.Server}; Database={_dbConfig.Database}; Username={_dbConfig.UserId}; Password={_dbConfig.Password};";
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
            var connectionString = $"Host={_dbConfig.Server}; Database=postgres; Username={_dbConfig.UserId}; Password={_dbConfig.Password};";
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
