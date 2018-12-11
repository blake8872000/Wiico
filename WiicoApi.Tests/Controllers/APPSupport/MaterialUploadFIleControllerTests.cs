using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.APPSupport;
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
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.Base;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace WiicoApi.Controllers.APPSupport.Tests
{
    [TestClass()]
    public class MaterialUploadFIleControllerTests : TestBase<MaterialUploadFIleController,Guid, MaterialViewModel,string, BaseResponse<string>>
    {
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
                    {"icantoken", "4a01b7ed-27d7-4312-9a63-71c53a70dc81"},
                    {"classid", "9999testcourse"},
                    {"clientkey", "testclientKey"}
            };

            foreach (string key in formData)
            {
                multiPartContent.Add(new StringContent(formData[key]), key);
            }
            target = new MaterialUploadFIleController()
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
            var targetResponse = target.POST().Result;
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
        }
    }
}