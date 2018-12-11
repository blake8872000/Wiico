using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WiicoApi.Infrastructure.ViewModel.Base;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.CourseManage;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class GetAllMyCourseControllerTests
    {
        private readonly GetAllMyCourseRequest testValue = new GetAllMyCourseRequest() {
            Account = "yushuchen",
            ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81"
        };

        [TestMethod()]
        public void GetTest()
        {
            var target = new GetAllMyCourseController();
            var requestJson = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(requestJson);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<GetAllMyCourseResponse>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<GetAllMyCourseResponse>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);
        }

        [TestMethod()]
        public void PostTest()
        {
            var target = new GetAllMyCourseController();
            var targetResponse = target.Post(testValue);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<GetAllMyCourseResponse>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<GetAllMyCourseResponse>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);
        }
    }
}