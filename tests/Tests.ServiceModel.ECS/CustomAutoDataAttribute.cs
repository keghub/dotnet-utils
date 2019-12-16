using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using EMG.Extensions.Configuration.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(FixtureHelpers.CreateFixture) { }

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CustomInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public CustomInlineAutoDataAttribute(params object[] arguments) : base(FixtureHelpers.CreateFixture, arguments) { }
    }

    public static class FixtureHelpers
    {
        public static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Customize<ECSContainerMetadata>(o => o
                                                        .With(p => p.PortMappings, (Generator<PortMapping> g) => g.Take(1).ToArray())
            );

            fixture.Customize<IConfiguration>(o => o.FromFactory((ConfigurationBuilder builder, ECSContainerMetadata metadata) =>
            {
                builder.AddObject(metadata);

                return builder.Build();
            }));

            fixture.Customize<ServiceCollection>(o => o.FromFactory(() =>
            {
                var services = new ServiceCollection();

                services.AddOptions();

                return services;
            }));

            fixture.Customize<ServiceEndpoint>(o => o.Without(p => p.Address));

            return fixture;
        }
    }
}