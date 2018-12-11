using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave;
using Newtonsoft.Json;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.Base;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class GetAbSenceFormDetailControllerTests
    {
        public readonly GetAbSenceFormDetailRequest testValue = new GetAbSenceFormDetailRequest() {
            Account = "yushuchen",
            ClassID = "9999testcourse",
            ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
            OuterKey = "4E9388EF-FB19-4CC1-B3E1-C8A4D0DAA305"
        };
        [TestMethod()]
        public void GetTest()
        {
            var target = new GetAbSenceFormDetailController();
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<LeaveData>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<LeaveData>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);
        }
    }
}