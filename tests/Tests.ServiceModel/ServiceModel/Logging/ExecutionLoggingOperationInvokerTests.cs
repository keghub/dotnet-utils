using System.ServiceModel.Dispatcher;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel.Logging;
using NUnit.Framework;

namespace Tests.ServiceModel.Logging
{
    [TestFixture, Ignore("Hard to setup for tests")]
    public class ExecutionLoggingOperationInvokerTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ExecutionLoggingOperationInvoker).GetConstructors());
        }
    }
}