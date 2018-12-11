using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Tests.Controllers;
using WiicoApi.Infrastructure.ViewModel.Login;
using WiicoApi.Infrastructure.ViewModel.Base;
using Newtonsoft.Json;
using System.Web.Http.Results;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class DevicePushTokenControllerTests : TestBase<DevicePushTokenController, string,string, LoginRequest, BaseResponse< string>>
    {
        [TestMethod()]
        public void PutTest()
        {
            postTestValue = new LoginRequest();
            postTestValue.Account = "yuschang";
            postTestValue.PushToken = string.Format("UnitTest [{0}]",DateTime.Now);
            
            target = new DevicePushTokenController();
            var targetResponse = target.Put(postTestValue);

            Assert.IsInstanceOfType(targetResponse, postResponse);
            var targetResData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }
    }
}