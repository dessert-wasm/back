using System.IO;
using YamlDotNet.Serialization;

namespace Dessert
{
    public class Configuration
    {
        public DatabaseSettings Database { get; set; }

        public static Configuration LoadSettings(string fileName)
        {
            var content = File.ReadAllText(fileName);
            var deSerializer = new Deserializer();
            return deSerializer.Deserialize<Configuration>(content);
        }
    }

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

    public class DatabaseSettings
    {
        public string Type { get; set; }
        public PgDatabaseSettings Postgres { get; set; }
        public SqlLiteDatabaseSettings SQLite { get; set; }

        public const string PostgresName = "Postgres";
        public const string SQLiteName = "SQLite";

        public string PgConnectionString
            => $"User ID={Postgres.User};" +
               $"Password={Postgres.Password};" +
               $"Host={Postgres.Host};" +
               $"Port={Postgres.Port};" +
               $"Database={Postgres.Database};";

        public string SQLiteConnectionString
            => $"Data Source={SQLite.Source}";
    }
}