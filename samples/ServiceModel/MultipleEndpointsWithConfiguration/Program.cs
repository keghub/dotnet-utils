using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using EMG.Utilities.ServiceModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Samples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new Dictionary<string, string>
            {
                // NamedPipe
                ["Sample:WCF:Endpoints:NamedPipe:Path"] = "test",
                // BasicHttp
                ["Sample:WCF:Endpoints:BasicHttp:Host"] = "localhost",
                ["Sample:WCF:Endpoints:BasicHttp:Port"] = "10001",
                ["Sample:WCF:Endpoints:BasicHttp:Path"] = "test",
                // NetTcp
                ["Sample:WCF:Endpoints:NetTcp:Host"] = "localhost",
                ["Sample:WCF:Endpoints:NetTcp:Port"] = "10000",
                ["Sample:WCF:Endpoints:NetTcp:Path"] = "test"
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(settings);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Trace));

            services.AddWcfService<TestService>(service =>
            {
                service.AddNamedPipeEndpoint(typeof(ITestService), configuration.GetSection("Sample:WCF:Endpoints:NamedPipe").GetNamedPipeEndpointAddress());

                service.AddBasicHttpEndpoint(typeof(ITestService), configuration.GetSection("Sample:WCF:Endpoints:BasicHttp").GetBasicHttpEndpointAddress());

                service.AddNetTcpEndpoint(typeof(ITestService), configuration.GetSection("Sample:WCF:Endpoints:NetTcp").GetNetTcpEndpointAddress());

                service.AddMetadataEndpoints();
            });

            var serviceProvider = services.BuildServiceProvider();

            var hostedService = serviceProvider.GetService<IHostedService>();

            await hostedService.StartAsync(default);

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();

            await hostedService.StopAsync(default);
        }
    }

    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        Task DoSomethingAsync();
    }

    public class TestService : ITestService
    {
        public async Task DoSomethingAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}