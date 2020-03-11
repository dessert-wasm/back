using System;
using Npgsql;

namespace Dessert.Persistance
{
    public class PgDatabaseSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Host { get; set; }
        public ushort Port { get; set; }
    }

    public class SqlLiteDatabaseSettings
    {
        public string Source { get; set; }
    }

    public enum DatabaseTypeEnum
    {
        Postgres,
        SQLite,
    }

    public class DatabaseSettings
    {
        public string Type { get; set; }
        public PgDatabaseSettings Postgres { get; set; }
        public SqlLiteDatabaseSettings SQLite { get; set; }

        public DatabaseTypeEnum DatabaseTypeEnum 
            => Enum.Parse<DatabaseTypeEnum>(Type);

        public string GetConnectionString()
        {
            if (DatabaseTypeEnum == DatabaseTypeEnum.Postgres)
            {
                return new NpgsqlConnectionStringBuilder()
                    {
                        Username = Postgres.User,
                        Password = Postgres.Password,
                        Database = Postgres.Database,
                        Host = Postgres.Host,
                        Port = Postgres.Port,
                        Pooling = true,
                        MinPoolSize = 1,
                        MaxPoolSize = 100,
                    }
                    .ToString();
            }

            if (DatabaseTypeEnum == DatabaseTypeEnum.SQLite)
            {
                return $"Data Source={SQLite.Source}";
            }

            throw new Exception($"Invalid database type: {Type}");
        }
    }
}