using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Nowin;
using System;

namespace Tenduke.Client.AspNetSampleOwin
{
    /// <summary>
    /// Extensions for building Nowin WebHost.
    /// </summary>
    /// <see cref="https://github.com/aspnet/Docs/blob/master/aspnetcore/fundamentals/owin/sample/src/NowinSample/NowinWebHostBuilderExtensions.cs"/>
    public static class NowinWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseNowin(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IServer, NowinServer>();
            });
        }

        public static IWebHostBuilder UseNowin(this IWebHostBuilder builder, Action<ServerBuilder> configure)
        {
            builder.ConfigureServices(services =>
            {
                services.Configure(configure);
            });
            return builder.UseNowin();
        }
    }
}
