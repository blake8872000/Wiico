using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
using Newtonsoft.Json;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.School;
using WiicoApi.Infrastructure.ViewModel.CourseManage;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class GetWeekBySemeControllerTests
    {
        private readonly BackendBaseRequest testValue = new BackendBaseRequest() {
            Account = "yushuchen",
            ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81"
        };
        [TestMethod()]
        public void GetTest()
        {
            var target = new GetWeekBySemeController();
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);

            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<GetWeekBySemeResponse>>));
            var targetResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<GetWeekBySemeResponse>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }
    }
}