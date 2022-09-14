namespace Fanex.NAP.SPU.CTS.CC.JobService
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Sentry;
    using System;

    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                SentrySdk.CaptureMessage(string.Format("{0} has been started at {1}", typeof(Program).FullName, DateTime.Now));

                host.Run();
                
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:5000");

                    webBuilder
                    .UseSentry()
                    .UseStartup<Startup>();
                });
        }
    }
}