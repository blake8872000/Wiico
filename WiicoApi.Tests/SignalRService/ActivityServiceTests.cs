using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Service.SignalRService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WiicoApi.Service.SignalRService.Tests
{
    [TestClass()]
    public class ActivityServiceTests
    {
        [TestMethod()]
        public void SetModuleAuthsTest()
        {
            var service = new ActivityService("AzureUnitTestDB");
            var testResponse = service.SetModuleAuths(true);
            Assert.IsNotNull(testResponse);
            Assert.IsInstanceOfType(testResponse, typeof(JObject));
        }
    }
}