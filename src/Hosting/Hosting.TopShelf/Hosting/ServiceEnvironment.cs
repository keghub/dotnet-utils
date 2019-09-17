using Microsoft.Extensions.Configuration;

namespace EMG.Utilities.Hosting
{
    public class ServiceEnvironmentOptions
    {
        public string Environment { get; set; } = "Development";
    }

    public interface IServiceEnvironment
    {
        IConfiguration Configuration { get; }

        string EnvironmentName { get; }
    }

    public class ServiceEnvironment : IServiceEnvironment
    {
        public string EnvironmentName { get; set; }

        public IConfiguration Configuration { get; set; }
    }
}