using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.BackendManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Filter;
using Newtonsoft.Json;
using WiicoApi.Infrastructure.ViewModel.Base;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.School;
using WiicoApi.Infrastructure.ViewModel.School.FeedBack;
using WiicoApi.Infrastructure.Entity;

namespace WiicoApi.Controllers.BackendManage.Tests
{
    [TestClass()]
    public class FeedBackControllerTests
    {

        private readonly PagesRows getTestValue = new PagesRows()
        {
            Pages = 1,
            Rows = 20,
            Token = "d0e7674b-3484-4b64-a09d-1a3affb44e70"
        };
        private readonly FeedBackPostRequest postTestValue = new FeedBackPostRequest() {
            FeedBackType = "問題回饋",
            Description = "登入失敗",
            System = "Desktop_Device:unknown_OSVersion:windows-10_Browser:chrome_BrowserVersion:68.0.3440.106",
            Email = "blake8872000@gmail.com",
            Token = "d0e7674b-3484-4b64-a09d-1a3affb44e70"
        };
        private readonly FeedBackPutRequest putTestValue = new FeedBackPutRequest() {
            Id=1,
            Note= "已處理",
            ReContent= "已處理",
            Status = 2,
            Token = "d0e7674b-3484-4b64-a09d-1a3affb44e70"
        };
        [TestMethod(), TokenValidation]
        public void GetTest()
        {
            var target = new FeedBackController();
            var requestJson = JsonConvert.SerializeObject(getTestValue);
            var targetResponse = target.Get(requestJson);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<FeedBackGetResponse>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<FeedBackGetResponse>>;

            //var actual = new FeedBackController();
            //var actualResponse = target.Get(requestJson);
            //Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<FeedBackGetResponse>>));
            //var actualResponseData = actualResponse as OkNegotiatedContentResult<BaseResponse<FeedBackGetResponse>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);
            //Assert.Inconclusive("驗證正確性!!!");
        }

        [TestMethod(), TokenValidation]
        public void PostTest()
        {
            var target = new FeedBackController();

            var targetResponse = target.Post(postTestValue);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<FeedBack>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<FeedBack>>;
            Assert.IsNotNull(targetResponseData);
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);

        }

        [TestMethod(), TokenValidation]
        public void PutTest()
        {
            var target = new FeedBackController();
      
            var targetResponse = target.Put(putTestValue);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<FeedBack>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<FeedBack>>;

            //var actual = new FeedBackController();
            //var actualResponse = target.Put(putTestValue);
            //Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<FeedBack>>));
            //var actualResponseData = actualResponse as OkNegotiatedContentResult<BaseResponse<FeedBack>>;
            Assert.IsNotNull(targetResponseData);
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);

            //Assert.Inconclusive("驗證正確性!!!");

        }
    }
}