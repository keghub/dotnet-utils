using System;
using System.Collections.Generic;
using System.Text;
using Nybus.Bridge;

namespace Tests
{
    public class TestCommand : ICommand
    {
        public string Value { get; set; }
    }

    public class TestEvent : IEvent
    {
        public string Value { get; set; }
    }
}
