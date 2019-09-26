# EMG.Utilities.ServiceModel

This library contains a set of utilities for quickly creating a WCF service in an application created using latest .NET Standard extensions libraries.

This is all you need to start a WCF service that exposes an endpoint via named pipes.

```csharp
static async Task Main(string[] args)
{
    var services = new ServiceCollection();

    services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Trace));

    services.AddWcfService<TestService>(service =>
    {
        service.AddNamedPipeEndpoint(typeof(ITestService), new Uri("net.pipe://localhost/test"));

        service.EnableDefaultMetadata();
    });

    var serviceProvider = services.BuildServiceProvider();

    var hostedService = serviceProvider.GetService<IHostedService>();

    await hostedService.StartAsync(default);
    
    Console.WriteLine("Press ENTER to exit...");
    Console.ReadLine();

    await hostedService.StopAsync(default);
}
```

Your service can expose more than one endpoint and your application can host as many services as you want.

You can find more examples in the [samples](../../../samples/ServiceModel/) folder.

Since WCF is only available for .NET Framework, this library is targeting only .NET Framework 4.6.1 but we're looking at the [CoreWCF](https://github.com/corewcf/corewcf/) project.

## Features

### Dependency Injection
Each service will be instantiated using the `Microsoft.Extensions.DependencyInjection` framework. You don't need to do anything but register your dependencies with the familiar `services.Add...` syntax.

This allows you to provide your WCF service with dependencies while keeping your service easy to test.

```csharp
public class TestService : ITestService
{
    private readonly ILogger<TestService> _logger;

    public TestService(ILogger<TestService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DoSomethingAsync()
    {
        _logger.LogInformation("We're going to do something amazing");

        await Task.Delay(TimeSpan.FromSeconds(1));
    }
}
```

### Execution logging
If you want, you can make sure that every invocation of your service is properly logged.

```csharp
services.AddWcfService<TestService>(service =>
{
    service.AddNamedPipeEndpoint(typeof(ITestService), new Uri("net.pipe://localhost/test"));

    service.AddExecutionLogging();
});
```

Since we're using the `Microsoft.Extensions.Logging` framework, you can start using directly your favourite provider.

### Publishing metadata
Especially during development, you want your service to expose its metadata. This makes it possible for other tool to generate the clients to interact with your service.

```csharp
services.AddWcfService<TestService>(service =>
{
    service.AddNamedPipeEndpoint(typeof(ITestService), new Uri("net.pipe://localhost/test"));

    service.AddMetadataEndpoints();
});
```

`AddMetadataEndpoints` will go through your registered endpoints and create a metadata endpoint for each supported binding with the address based on the one you set up. By default, `mex` will be appended but you can customize it as you prefer.

In this example, the metadata endpoint will be reachable via named pipe at the address `net.pipe://localhost/test/mex`.

Finally, you can customize the `ServiceMetadataBehavior` by providing a configuring action.

```csharp
services.AddWcfService<TestService>(service =>
{
    service.AddNamedPipeEndpoint(typeof(ITestService), new Uri("net.pipe://localhost/test"));

    service.AddMetadataEndpoints(behavior =>
    {
        // do something with the behavior
    });
});
```

### Service discovery
Whenever possible, you should avoid targeting your services directly. 

Instead, you can use WCF service discovery (you will need a service registry where your service can announce itself).

You can enable this behavior on each endpoint you want to be announced.

Additionally, you'll have to register the `AnnouncementService` that will take care of periodically announce your endpoint to your service registry.

```csharp
services.AddWcfService<TestService>(service =>
{
    service.AddNetTcpEndpoint(typeof(ITestService), new Uri("net.tcp://localhost:10000/test")).Discoverable();
});

services.AddDiscovery<NetTcpBinding>(new Uri("net.tcp://your-service-registry:12345"), TimeSpan.FromSeconds(10));
```

### Binding configuration
You can customize your bindings by providing a configuration delegate when adding your endpoint.

```csharp
services.AddWcfService<TestService>(service =>
{
    service.AddNamedPipeEndpoint(typeof(ITestService), new Uri("net.pipe://localhost/test"), binding =>
    {
        // configure the binding
        binding.Security.Mode = NetNamedPipeSecurityMode.None;
    });
});
```

### Your own customizations
The beauty of WCF lies on the high degree of customization. You can still customize your service by attaching service and endpoint behaviors programmatically.

```csharp
services.AddWcfService<TestService>(service =>
{
    service.AddNamedPipeEndpoint(typeof(ITestService), new Uri("net.pipe://localhost/test")).AddBehavior(new AnnounceableBehavior());

    service.AddServiceBehavior(new ServiceBehaviorAttribute
    {
        IncludeExceptionDetailInFaults = true
    });
});
```