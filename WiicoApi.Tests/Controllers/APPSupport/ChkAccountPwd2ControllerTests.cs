using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WiicoApi.Infrastructure.ViewModel.Login;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.Base;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class ChkAccountPwd2ControllerTests
    {
        private LoginRequest testValue = new LoginRequest() {
            Account = "yushuchen",
            Password = "1jV5hDxywOQ=",
            PhoneID="3565784b-5ebc-495e-9981-7cbf8bd56077",
            PushToken="dt7PKr7cocI:APA91bEOpVLjHYrqKne88apmIWGfHFcGF_KqsPMQH_Z3sdY7joTCMuvDzURtXqwWQMEqTsgPoPATzrWerbPSm__TBNokP_7CPraTJh8AS7TemQPX1p_vSvsPhVqjYe3cnD0TMSnPDs8i",
           RequestSystem="Android;SM-N950FAndroid+SDK+Version:+26;1.20180910.1"
        };
        [TestMethod()]
        public void GetTest()
        {
            var target = new ChkAccountPwd2Controller();
            var requestJson = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(requestJson);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<LoginResponse>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<LoginResponse>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);
        }

        [TestMethod()]
        public void PostTest()
        {
            var target = new ChkAccountPwd2Controller();
            var targetResponse = target.Post(testValue);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<LoginResponse>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<LoginResponse>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);
        }
    }
}