using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddSingleton<ILoadBalancingPolicy, IPAddressPolicy>();
            services.AddSingleton<ILoadBalancingPolicy, PartitionKeyRouteValuePolicy>();
            services.AddSingleton<ILoadBalancingPolicy, PartitionKeyQueryValuePolicy>();
            services.AddReverseProxy()
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
                endpoints.MapReverseProxy();
            });
        }
    }
}
