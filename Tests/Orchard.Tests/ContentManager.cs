using NUnit.Framework;
using Orchard.Content2.Models2;
using Orchard.ContentManagement;
using Orchard.Environment.ShellBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Tests
{
    [TestFixture]
    public class ContentManager
    {
        [Test]
        public void IsRecord()
        {
            var isRecord = CompositionStrategy.IsRecord(typeof(C2Part));

            Assert.AreEqual(false, isRecord);
        }
    }
}
