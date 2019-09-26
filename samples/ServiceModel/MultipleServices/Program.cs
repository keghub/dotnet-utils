using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using EMG.Utilities.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MultipleServices
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Trace));

            services.AddWcfService<TestService>(service =>
            {
                service.AddEndpoint<BasicHttpBinding>(typeof(ITestService), new Uri("http://localhost:10001/test"));

                service.EnableDefaultMetadata();
            });

            services.AddWcfService<AnotherTestService>(service =>
            {
                service.AddEndpoint<BasicHttpBinding>(typeof(ITestService), new Uri("http://localhost:10002/test"));

                service.EnableDefaultMetadata();
            });

            var serviceProvider = services.BuildServiceProvider();

            var hostedServices = serviceProvider.GetServices<IHostedService>().ToArray();

            foreach (var hostedService in hostedServices)
            {
                await hostedService.StartAsync(default);
            }
            
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();

            foreach (var hostedService in hostedServices)
            {
                await hostedService.StopAsync(default);
            }
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

    public class AnotherTestService : ITestService
    {
        public async Task DoSomethingAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
