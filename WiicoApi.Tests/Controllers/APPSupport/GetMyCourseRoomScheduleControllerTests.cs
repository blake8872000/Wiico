using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using Newtonsoft.Json;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.Base;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class GetMyCourseRoomScheduleControllerTests
    {
        private readonly GetAllMyCourseRequest testValue = new GetAllMyCourseRequest(){
            Account = "yushuchen",
            ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81"
        };
        [TestMethod()]
        public void GetTest()
        {
            var target = new GetMyCourseRoomScheduleController();
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);

            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<GetMyCourseScheduleResponse>>));
            var targetResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<GetMyCourseScheduleResponse>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }
    }
}