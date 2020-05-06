using System;
using System.Collections.Generic;
using System.Linq;
using EMG.Extensions.Configuration.Model;

namespace EMG.Utilities
{
    public class ECSContainerMetadataOptions
    {
        public Func<IReadOnlyList<PortMapping>, PortMapping> PortMappingSelector { get; set; } = list => list.FirstOrDefault();
    }
}
