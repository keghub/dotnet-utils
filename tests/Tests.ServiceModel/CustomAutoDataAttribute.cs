using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base (FixtureHelpers.CreateFixture) {}

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

            fixture.Customize<ServiceHost>(o => o.FromFactory(() => new ServiceHost(typeof(TestService))));

            fixture.Customize<ServiceEndpoint>(o => o.Without(p => p.Address));

            fixture.Customize<IList<Action<ServiceHost>>>(o => o.FromFactory(() => new List<Action<ServiceHost>>()));

            return fixture;

        }
    }
}