using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.BackendManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.MQTT;
using Newtonsoft.Json;
using System.Threading;

namespace WiicoApi.Controllers.BackendManage.Tests
{
    [TestClass()]
    public class IOTControllerTests
    {
        private readonly IOTGetRequest getTestValue = new IOTGetRequest()
        {
            ICanToken = "d0e7674b-3484-4b64-a09d-1a3affb44e70",
            Account = "pako",
            ClassId = "9999testcourse"
        };
        private readonly IOTPostRequest postTestValue = new IOTPostRequest() {
            ICanToken = "d0e7674b-3484-4b64-a09d-1a3affb44e70",
            Account= "pako",
            DeviceId = "5418391279",
            Id= "screen",
            Value="1"
        };

        [TestMethod()]
        public void GetTest()
        {
            var target = new IOTController();
            var requestJson = JsonConvert.SerializeObject(getTestValue);
            var targetResponse = target.Get(requestJson);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<RoomDeviceViewModel>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<RoomDeviceViewModel>>;
            Assert.IsNotNull(targetResponseData);
            Assert.IsNotNull(targetResponseData.Content);
            //Assert.Inconclusive("驗證正確性!!!");

        }

        [TestMethod()]
        public void PostTest()
        {
            var target = new IOTController();
            var targetResponse = target.Post(postTestValue);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData);
            Assert.IsTrue(targetResponseData.Content.Success);
            //Assert.Inconclusive("驗證正確性!!!");
        }
    }
}