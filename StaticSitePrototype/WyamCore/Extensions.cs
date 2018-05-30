using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace StaticSitePrototype.WyamCore
{
    public static class Extensions
    {
        public static IWebHostBuilder UseWyam(this IWebHostBuilder hostBuilder, WyamOptions options = default)
        {
            if (options == default)
            {
                options = new WyamOptions
                {
                    Recipe = new BlogRecipe()
                };
            }

            Task.Run(() => options.Recipe.InvokeAsync()).Wait();

            return hostBuilder;
        }

        //public static WyamOptions UseBlogRecipe(this WyamOptions options)
        //{
        //    services
        //}

        //public static IWebHostBuilder UseWyam(this IWebHostBuilder hostBuilder)
        //{
        //    return hostBuilder;
        //}

        public static IWebHostBuilder UseWyam(this IWebHostBuilder hostBuilder, Action<WyamOptions> options)
        {
            return hostBuilder.UseWyam().ConfigureServices(services =>
            {
                services.Configure(options);
            });
        }
    }

}
