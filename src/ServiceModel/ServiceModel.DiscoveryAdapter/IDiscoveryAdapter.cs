using System;
using System.ServiceModel;
using System.Xml;

namespace EMG.Utilities
{
    [ServiceContract]
    public interface IDiscoveryAdapter
    {
        [OperationContract]
        Uri Discover(XmlQualifiedName type);
    }
}
