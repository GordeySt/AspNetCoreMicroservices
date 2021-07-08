using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryAvailability = retry.Value;

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();

            try
            {
                logger.LogInformation("Migration postgresql database.");

                using var connection = new NpgsqlConnection
                    (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                connection.Open();

                using var command = new NpgsqlCommand
                {
                    Connection = connection
                };

                command.CommandText = "drop table if exists coupon";
                command.ExecuteNonQuery();

                command.CommandText = @"create table Coupon(Id serial primary key,
                                                                ProductName varchar(24) not null,
                                                                description text,
                                                                amount int)";
                command.ExecuteNonQuery();

                command.CommandText = "insert into Coupon(ProductName, Description, Amount) " +
                    "values('IPhone X', 'IPhone Discount', 100)";
                command.ExecuteNonQuery();

                command.CommandText = "insert into Coupon(ProductName, Description, Amount) " +
                    "values('Samsung 10', 'Samsung Discount', 150)";
                command.ExecuteNonQuery();

                logger.LogInformation("Migrated postgresql database.");
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "An error occured while migrating the postgresql database.");

                if (retryAvailability < 50)
                {
                    retryAvailability++;
                    Thread.Sleep(2000);
                    MigrateDatabase<TContext>(host, retryAvailability);
                }

                throw;
            }

            return host;
        }
    }
}
