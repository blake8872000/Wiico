using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Tests.Controllers;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Infrastructure.ViewModel.Base;
using Newtonsoft.Json;
using System.Web.Http.Results;
using System.IO;
using WiicoApi.Service.Utility;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using WiicoApi.Controllers.APPSupport;
using System.Web.Http.Controllers;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class AddDiscussionMessageControllerTests : TestBase<AddDiscussionMessageController,string,string, DiscussionSendMsgRequestModel, ResultBaseModel<DiscussionSendMsg>>
    {
        [TestMethod()]
        public void PostTest()
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
                    {"activityOuterKey", "A6D3B148-5CF7-427B-99DE-4674048EB0E9"},
                    {"msg", string.Format( "[{0}] 單元測試留言",DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"))}
            };

            foreach (string key in formData)
            {
                multiPartContent.Add(new StringContent(formData[key]), key);
            }
            target = new AddDiscussionMessageController()
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
            var targetResponse = target.Post().Result;

            Assert.IsInstanceOfType(targetResponse, postResponse);
            var targetResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<DiscussionSendMsg>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }
    }
}