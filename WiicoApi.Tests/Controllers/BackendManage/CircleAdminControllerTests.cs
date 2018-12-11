using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.BackendManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Tests.Controllers;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using Newtonsoft.Json;
using System.Web.Http.Results;
using WiicoApi.Repository;
using Moq;
using WiicoApi.Service.Backend;

namespace WiicoApi.Controllers.BackendManage.Tests
{
    [TestClass()]
    public class CircleAdminControllerTests : TestBase<CircleAdminController, BackendBaseRequest, BaseResponse<List<CourseManagerGetResponse>>, CourseManagerPostRequest, InviteResponseData>
    {
        [TestMethod()]
        public void GetTest()
        {
            getTestValue = new BackendBaseRequest() {
            //    Account = "yushuchen",
                CircleKey = "9999testcourse",
                Token = "a24b8db6-1883-4c31-9c0d-23b2b1250bdb"
                //   ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                //Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81"
            };

            var mock = new Mock<GenericUnitOfWork>();

            target = new CircleAdminController(mock.Object);
            var jsonString = JsonConvert.SerializeObject(getTestValue);
            var targetResponse = target.Get(jsonString);

            Assert.IsInstanceOfType(targetResponse, getResponse);
            var targetResData = targetResponse as OkNegotiatedContentResult<BaseResponse<List<CourseManagerGetResponse>>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }

        [TestMethod()]
        public void PostTest()
        {
            postTestValue = new CourseManagerPostRequest()
            {
                Account = "yushuchen",
                ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                Accounts = new string[1] { "yushuchen" },
                InviteCode = "9875641258",
                ResType = 1,
                OrgCode = "Sce"
            };


             // target = new CircleAdminController(mock.Object);
             var courseManagerService = new CourseManagerService("FlipusDB");
            //    var targetResponse = target.Post(postTestValue);
            var targetResponse = courseManagerService.CreateMutiple(postTestValue);
            //Assert.IsInstanceOfType(targetResponse, getResponse);
            var targetResData = targetResponse as InviteResponseData;
            Assert.IsNotNull(targetResData);
          //  Assert.IsTrue(targetResData.Content.Success);
            //     throw new NotImplementedException();
        }

        [TestMethod()]
        public void PutTest()
        {
       //     throw new NotImplementedException();
        }

        [TestMethod()]
        public void DeleteTest()
        {
        //    throw new NotImplementedException();
        }
    }
}