using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Tests.Controllers;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using System.Web.Http.Results;
using System.IO;
using WiicoApi.Service.Utility;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Collections.Specialized;

namespace WiicoApi.Controllers.APPSupport.Tests
{
    [TestClass()]
    public class DiscussionControllerTests : TestBase<DiscussionController, string, ResultBaseModel<DiscussionDetail>, DiscussionCreateRequestModel, BaseResponse<string>>
    {
        [TestMethod()]
        public void GetTest()
        {
            getTestValue = "A6D3B148-5CF7-427B-99DE-4674048EB0E9";

            target = new DiscussionController();
            var targetResponse = target.Get(getTestValue);

            Assert.IsInstanceOfType(targetResponse, getResponse);
            var targetResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<DiscussionDetail>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }

        [TestMethod()]
        public void POSTTest()
        {
            var testFile = @"D:\iCanFileData\upload\4JMQBV.gif";
            var fileStream = new FileStream(testFile, FileMode.Open, FileAccess.Read);
            var streamContent = new StreamContent(fileStream);
            var fileTools = new FileTools();
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileTools.ConvertContentType(".gif"));

            var multiPartContent = new MultipartFormDataContent("boundary=---011000010111000001101001");

            multiPartContent.Add(streamContent, "TheFormDataKeyForYourFile", testFile);
            //測試資料
            var formData = new NameValueCollection
                {
                    {"ICanToken", "4a01b7ed-27d7-4312-9a63-71c53a70dc81"},
                    {"ClassID", "9999testcourse"},
                    {"Title", string.Format("[單元測試] {0} ",DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"))},
                    {"Content", "單元測試內容"}
            };

            foreach (string key in formData)
            {
                multiPartContent.Add(new StringContent(formData[key]), key);
            }
            target = new DiscussionController()
            {
                Request = new HttpRequestMessage()
                {
                    Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                    Method = HttpMethod.Post
                },
                ControllerContext = new HttpControllerContext()
                {
                    Request = new HttpRequestMessage()
                    {
                        Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                        Method = HttpMethod.Post
                    }
                }
            };

            var targetResponse = target.POST().Result; //List<Infrastructure.Entity.FileStorage>
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
        }

        [TestMethod()]
        public void PutTest()
        {
            var testFile = @"D:\iCanFileData\upload\4JMQBV.gif";
            var fileStream = new FileStream(testFile, FileMode.Open, FileAccess.Read);
            var streamContent = new StreamContent(fileStream);
            var fileTools = new FileTools();
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileTools.ConvertContentType(".gif"));

            var multiPartContent = new MultipartFormDataContent("boundary=---011000010111000001101001");

            multiPartContent.Add(streamContent, "TheFormDataKeyForYourFile", testFile);
            //測試資料
            var formData = new NameValueCollection
                {
                    {"icantoken", "4a01b7ed-27d7-4312-9a63-71c53a70dc81"},
                    {"classid", "9999testcourse"},
                    {"title", string.Format("[修改單元測試] {0} ",DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"))},
                    {"content", "修改單元測試內容"},
                { "activityOuterKey","135EF2B9-7D44-4C43-8EA4-F59C28C74F39"}
            };

            foreach (string key in formData)
            {
                multiPartContent.Add(new StringContent(formData[key]), key);
            }
            var controller = new DiscussionController()
            {
                Request = new HttpRequestMessage()
                {
                    Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                    Method = HttpMethod.Put
                },
                ControllerContext = new HttpControllerContext()
                {
                    Request = new HttpRequestMessage()
                    {
                        Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                        Method = HttpMethod.Put
                    }
                }
            };

            var targetResponse = controller.Put().Result; //List<Infrastructure.Entity.FileStorage>
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
        }
    }
}