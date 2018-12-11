using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using WiicoApi.Service.Utility;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using WiicoApi.Tests.Controllers;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace WiicoApi.Controllers.api.APPSupport.Tests
{
    [TestClass()]
    public class SetNewAbsenceFormControllerTests : TestBase<SetNewAbsenceFormController, string, BaseResponse<string>, string, BaseResponse<string>>
    {
        [TestMethod()]
        public void GetTest()
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
                    {"icantoken", "a0c4399e-c724-42d8-9d05-a0b2b0548783"},
                    {"classid", "9999testcourse"},
                    {"title", string.Format("[單元測試]-{0}",DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"))},
                    {"content", string.Format("[單元測試假單內容]-{0}",DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"))},
                    {"leavedate", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss")},
                    {"leavecategoryid","1"}
            };

            foreach (string key in formData)
            {
                multiPartContent.Add(new StringContent(formData[key]), key);
            }
            target = new SetNewAbsenceFormController()
            {
                Request = new HttpRequestMessage()
                {
                    Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                    Method = HttpMethod.Get
                },
                ControllerContext = new HttpControllerContext()
                {
                    Request = new HttpRequestMessage()
                    {
                        Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                        Method = HttpMethod.Get
                    }
                }
            };
            var targetResponse = target.Get().Result;
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
        }

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
                    {"icantoken", "a0c4399e-c724-42d8-9d05-a0b2b0548783"},
                    {"classid", "9999testcourse"},
                    {"title", string.Format("[單元測試]-{0}",DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"))},
                    {"content", string.Format("[單元測試假單內容]-{0}",DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"))},
                    {"leavedate", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss")},
                    {"leavecategoryid","1"}
            };

            foreach (string key in formData)
            {
                multiPartContent.Add(new StringContent(formData[key]), key);
            }
            target = new SetNewAbsenceFormController()
            {
                Request = new HttpRequestMessage()
                {
                    Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                    Method = HttpMethod.Get
                },
                ControllerContext = new HttpControllerContext()
                {
                    Request = new HttpRequestMessage()
                    {
                        Content = new MultipartFormDataContent("boundary=---011000010111000001101001") { multiPartContent },
                        Method = HttpMethod.Get
                    }
                }
            };
            var targetResponse = target.Post().Result;
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
        }
    }
}