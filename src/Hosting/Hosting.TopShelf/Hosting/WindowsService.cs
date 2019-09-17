using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Topshelf;

namespace EMG.Utilities.Hosting
{
    public class WindowsService : IService
    {
        public IConfiguration Configuration { get; }

        public IServiceProvider Services { get; }

        public WindowsService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private IEnumerable<IHostedService> _hostedServices;

        public bool OnStart(HostControl control)
        {
            var hostedServices = Services.GetServices<IHostedService>();

            var startedServices = new List<IHostedService>();

            foreach (var service in hostedServices)
            {
                service.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
                startedServices.Add(service);
            }

            _hostedServices = startedServices;

            return true;
        }

        public bool OnStop(HostControl control)
        {
            foreach (var service in _hostedServices ?? new IHostedService[0])
            {
                service.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
            }

            return true;
        }
    }
}