using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.IO;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.Base;
using System.Web;
using Moq;
using System.Net.Http.Headers;
using WiicoApi.Service.Utility;

namespace WiicoApi.Controllers.Common.Tests
{
    [TestClass()]
    public class FileControllerTests
    {
        private readonly Guid token = Guid.Parse("d0e7674b-3484-4b64-a09d-1a3affb44e70");
        private readonly string fileGuid = "0A8F463A-D1C0-4663-A666-2D9C0BC97524";
        private readonly int img_w = 640;
        private readonly int img_h = 480;
        //[TestMethod()]
        //public void UploadTest()
        //{
        //    var filePath = "F:\\1.txt";
        //    var stream = File.Create(filePath);
        //    var content = new StreamContent(stream /* stream in the file */);
        //    content.Headers.Add("Content-Disposition", "form-data");
        //    var controllerContext = new HttpControllerContext()
        //    {
        //        Request = new HttpRequestMessage
        //        {
        //            Content = new MultipartContent { content },
        //            Method = HttpMethod.Post
        //        }
        //    };

        //    //   controllerContext.Request.Headers.Add("Accept", "application/xml");
        //    //  controllerContext.Request.Headers.Add("Content-Disposition", "form-data");
        //    var controller = new FileController();
        //    controller.ControllerContext = controllerContext;
        //    controller.ActionContext = new HttpActionContext()
        //    {
        //        ControllerContext = new HttpControllerContext()
        //        {
        //            Request = new HttpRequestMessage
        //            {
        //                Content = new MultipartContent { content },
        //                Method = HttpMethod.Post
        //            }
        //        }
        //    };
        //    var targetResponse = controller.Upload(token).Result; //List<Infrastructure.Entity.FileStorage>
        //    Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<List<Infrastructure.Entity.FileStorage>>));
        //    var targetResponseData = targetResponse as OkNegotiatedContentResult<List<Infrastructure.Entity.FileStorage>>;
        //    Assert.IsNotNull(targetResponseData.Content);
        //}


        [TestMethod()]
        public void GetTest()
        {
            var controller = new FileController();
            var targetResponse = controller.Get(fileGuid); //List<Infrastructure.Entity.FileStorage>
            Assert.IsInstanceOfType(targetResponse, typeof(HttpResponseMessage));
            var targetResponseData = targetResponse as HttpResponseMessage;
            Assert.IsNotNull(targetResponseData.Content);
        }

        [TestMethod()]
        public void GetImageTest()
        {
            var controller = new FileController();
            var targetResponse = controller.GetImage(fileGuid, img_w, img_h).Result; //List<Infrastructure.Entity.FileStorage>
            Assert.IsInstanceOfType(targetResponse, typeof(HttpResponseMessage));
            var targetResponseData = targetResponse as HttpResponseMessage;
            Assert.IsNotNull(targetResponseData.Content);
        }

        [TestMethod()]
        public void PostTest()
        {
            var testFile = @"D:\iCanFileData\upload\1.txt";
            var fileStream = new FileStream(testFile, FileMode.Open, FileAccess.Read);
            var streamContent = new StreamContent(fileStream);
            var fileTools = new FileTools();
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileTools.ConvertContentType(".txt"));
            var multiPartContent = new MultipartFormDataContent("boundary=---011000010111000001101001");
            multiPartContent.Add(streamContent, "TheFormDataKeyForYourFile", testFile);
            var controller = new FileController()
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
            var targetResponse = controller.Post(token).Result; //List<Infrastructure.Entity.FileStorage>
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<bool>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<bool>;
            Assert.IsNotNull(targetResponseData.Content);
        }


    }
}