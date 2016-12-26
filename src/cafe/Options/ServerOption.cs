using cafe.CommandLine;
using System.IO;
using cafe.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace cafe.Options
{
    public class ServerOption : Option
    {
        public ServerOption() : base(new OptionSpecification("server"), "Starts cafe in server mode")
        {

        }

        protected override void RunCore(string[] args)
        {
            var config = new ConfigurationBuilder()
                // .AddCommandLine(args)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            var host = new WebHostBuilder()
                .UseUrls("http://*:59320/")
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}