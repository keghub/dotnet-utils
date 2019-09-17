using System;
using Microsoft.Extensions.Configuration;

namespace EMG.Utilities.Hosting
{
    public static class ServiceEnvironmentExtensions
    {
        public static bool IsEnvironment(this IServiceEnvironment environment, string environmentName) => string.Equals(environment.EnvironmentName, environmentName, StringComparison.OrdinalIgnoreCase);

        public static bool IsProduction(this IServiceEnvironment environment) => IsEnvironment(environment, "Production");

        public static bool IsDevelopment(this IServiceEnvironment environment) => IsEnvironment(environment, "Development");

        public static bool IsStaging(this IServiceEnvironment environment) => IsEnvironment(environment, "Staging");

        public static IServiceEnvironment GetServiceEnvironment(this IConfiguration configuration)
        {
            var options = new ServiceEnvironmentOptions();
            configuration.Bind(options);

            var serviceEnvironment = new ServiceEnvironment
            {
                EnvironmentName = options.Environment,
                Configuration = configuration
            };

            return serviceEnvironment;
        }
    }
}