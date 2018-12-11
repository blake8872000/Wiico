using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.SignalRHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;

namespace WiicoApi.SignalRHub.Tests
{
    [TestClass()]
    public class WiicoHubTests
    {
        [TestMethod()]
        public void CheckTokenToMemberInfoTest()
        {
            var testValue = Guid.Parse("4a01b7ed-27d7-4312-9a63-71c53a70dc81");
            var target = new WiicoHub();
            var targetResponse = target.CheckTokenToMemberInfo(testValue);
            Assert.IsInstanceOfType(targetResponse, typeof(Member));
            Assert.IsNotNull(targetResponse);

        }
    }
}