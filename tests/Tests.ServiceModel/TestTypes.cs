using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Tests
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        Task DoSomethingAsync();
    }

    [ServiceContract]
    public interface IInvalidTestService
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

    public class TestHttpBinding : Binding
    {
        public override BindingElementCollection CreateBindingElements()
        {
            throw new NotImplementedException();
        }

        public override string Scheme { get; } = "http";
    }

    public class TestBinding : Binding
    {
        public override BindingElementCollection CreateBindingElements()
        {
            throw new NotImplementedException();
        }

        public override string Scheme { get; } = "";
    }
}