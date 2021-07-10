using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Contracts.Persistence;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;


namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<OrderContext>(options =>
            {
                options
                   .UseSqlServer(configuration.GetConnectionString("OrderingConnectionString"),
                   b => b.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
            });

            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
