using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Service.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Backend;

namespace WiicoApi.Service.Backend.Tests
{
    [TestClass()]
    public class SmartTAServiceTests
    {
        [TestMethod()]
        public void GetTest()
        {
            var service = new SmartTAService("AzureUnitTestDB");
            var datas = service.GetData("9704F0BF-9CFA-4266-B601-C55D31B937E1");
            Assert.IsInstanceOfType(datas, typeof(SmartTAGetResponse));
            datas = service.GetData("123");
            Assert.IsNull(datas);
        }
        [TestMethod()]
        public void InsertRelationTest()
        {
            var testValue = new SmartTAPostRequest()
            {
                CircleKeys = new List<string>(),
                ClassRoomId= "smartTAClassRoom1"
            };
            testValue.CircleKeys.Add("9999testcourse");
            var service = new SmartTAService("AzureUnitTestDB");
            var testResponse = service.InsertRelation(testValue);
           // Assert.IsNotNull(testResponse);
            Assert.IsInstanceOfType(testResponse, typeof(SmartTAGetResponse));

        }
    }
}