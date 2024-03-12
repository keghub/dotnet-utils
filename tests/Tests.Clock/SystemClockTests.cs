﻿using System;
using AutoFixture.NUnit3;
using EMG.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class SystemClockTests
    {
        [Test, AutoData]
        public void UtcNow_returns_current_time()
        {
            Assert.That(SystemClock.Instance.UtcNow, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromTicks(2000)));
        }
    }
}