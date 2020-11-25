using Npgsql;

namespace Dessert.Infrastructure.Persistence
{
    public class DatabaseSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Host { get; set; }
        public ushort Port { get; set; }

        public string GetConnectionString()
        {
            return new NpgsqlConnectionStringBuilder()
                {
                    Username = User,
                    Password = Password,
                    Database = Database,
                    Host = Host,
                    Port = Port,
                    Pooling = true,
                    MinPoolSize = 1,
                    MaxPoolSize = 100,
                }
                .ToString();
        }
    }
}