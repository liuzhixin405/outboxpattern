using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace TradingAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<OutBoxMessageInterceptor>();
            builder.Services.AddDbContext<TradingDbContext>((sp,ops) =>
            {
                ops.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Traing;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
                var interceptor = sp.GetService<OutBoxMessageInterceptor>();
                ops.AddInterceptors(interceptor);
            }, ServiceLifetime.Scoped);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            using var context = builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetService<TradingDbContext>();
            {
                if (!(context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
            }
            builder.Services.AddRabbitMQEventBus("192.168.237.240", 5672, "guest", "guest", "", eventBusOptionAction: eventBusOption =>
            {
                string assemblyName = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
                eventBusOption.ClientProvidedAssembly(assemblyName);
                eventBusOption.EnableRetryOnFailure(true, 5000, TimeSpan.FromSeconds(30));
                eventBusOption.RetryOnFailure(TimeSpan.FromSeconds(1));
                eventBusOption.MessageTTL(2000);
                eventBusOption.SetBasicQos(10);
                eventBusOption.DeadLetterExchangeConfig(config =>
                {
                    config.Enabled = false;
                    config.ExchangeNameSuffix = "-test";
                });
            });

            builder.Services.AddHostedService<OutBoxMessageBackgroundService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRabbitmqEventBus();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}