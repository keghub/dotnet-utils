using System;
using System.ServiceModel;
using System.Threading.Tasks;
using EMG.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetNamedPipeService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Trace));

            services.AddWcfService<TestService>(service =>
            {
                service.AddEndpoint<NetNamedPipeBinding>(typeof(ITestService), new Uri("net.pipe://localhost/test"));

                service.EnableDefaultMetadata();
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
