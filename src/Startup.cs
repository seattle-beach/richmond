using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft​.Extensions​.DependencyInjection;
using Richmond.BusClient;
using System.IO;

namespace Richmond
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IFoodTruckRepository, FoodTruckRepository>();
            services.AddSingleton<IDateProvider, DateProvider>();
            services.AddSingleton<IBusScheduleClient, BusScheduleClient>();
            services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IBusFactory, BusFactory>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
