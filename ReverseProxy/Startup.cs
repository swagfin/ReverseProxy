using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReverseProxy.ExtendedPolicies;
using Yarp.ReverseProxy.LoadBalancing;

namespace ReverseProxy
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add partitioning load balacing policy:
            services.AddSingleton<ILoadBalancingPolicy, PartitioningByIPAddressPolicy>();

            services
                .AddReverseProxy()
                .LoadFromConfig(_configuration.GetSection("ReverseProxy"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/", async (context) => {

                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Service Is Available");

                });
                endpoints.MapReverseProxy();
            });
        }
    }
}
