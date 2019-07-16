using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureB2CWithGroups
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //Console.WriteLine($@"Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
            //if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(GOOGLE_APPLICATION_CREDENTIALS)))
            //Environment.SetEnvironmentVariable(GOOGLE_APPLICATION_CREDENTIALS,AppDomain.CurrentDomain.BaseDirectory);

            //WebHost.CreateDefaultBuilder(args)
            //    .UseStartup<Startup>();
            var aspEnv = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return new WebHostBuilder()
                .UseKestrel()
                // when we run on surface pro we need to include this to 'override' kestrel project config, because it
                // doesn't run, for some reason
                .ConfigureKestrel((context, options) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        options.Listen(IPAddress.Loopback, 9000,
                            opts => opts.UseHttps("cert.pfx", "password"));
                    }
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                            optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                    }
                    else
                    {
                        //logging.AddConsole();
                        //logging.AddDebug();
                        //logging.AddEventSourceLogger();
                    }
                })
                .UseStartup<Startup>();
        }
    }
}
