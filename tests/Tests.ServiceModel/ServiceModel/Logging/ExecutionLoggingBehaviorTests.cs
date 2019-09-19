using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel.Logging;
using NUnit.Framework;

namespace Tests.ServiceModel.Logging
{
    [TestFixture]
    public class ExecutionLoggingBehaviorTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ExecutionLoggingBehavior).GetConstructors());
        }

    }
}