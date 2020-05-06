using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(CreateFixture) { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            return fixture;
        }
    }
}