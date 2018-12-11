using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave;
using Newtonsoft.Json;
using WiicoApi.Infrastructure.ViewModel.Base;
using System.Web.Http.Results;
using WiicoApi.Tests.Controllers;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class GetAbsenceFormListControllerTests : TestBase<GetAbsenceFormListController,GetAbsenceFormListRequest, BaseResponse<GetAbsenceFormListResponse>,string,string>
    {
        /* private readonly GetAbsenceFormListRequest testValue = new GetAbsenceFormListRequest()
         {
             ClassID = "9999testcourse",
             Account = "yushuchen",

             ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81"
         };*/
        [TestMethod()]
        public void GetTest()
        {
            getTestValue = new GetAbsenceFormListRequest();
            getTestValue.Account = "yushuchen";
            getTestValue.ClassID = "9999testcourse";
            getTestValue.ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81";
            target = new GetAbsenceFormListController();
            var jsonString = JsonConvert.SerializeObject(getTestValue);
            var targetResponse = target.Get(jsonString);

            Assert.IsInstanceOfType(targetResponse, getResponse);
            var targetResData = targetResponse as OkNegotiatedContentResult<BaseResponse<GetAbsenceFormListResponse>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }
    }
}