using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
using Newtonsoft.Json;
using WiicoApi.Infrastructure.ViewModel.Weather;
using System.Web.Http.Results;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class GetCurrentWeatherControllerTests
    {
        private readonly BackendBaseRequest testValue = new BackendBaseRequest() {
            Account = "yushuchen",
            ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81"
        };
        [TestMethod()]
        public void GetTest()
        {
            var target = new GetCurrentWeatherController();
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);

            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<WeatherDataModel>>));
            var targetResData = targetResponse as OkNegotiatedContentResult<BaseResponse<WeatherDataModel>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }
    }
}