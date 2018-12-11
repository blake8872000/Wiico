using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.BackendManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Service.CommenService.Fakes;
using WiicoApi.Infrastructure.Entity;
using Microsoft.QualityTools.Testing.Fakes;
using WiicoApi.Tests.Controllers;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.Login;
using Newtonsoft.Json;
using System.Web.Http.Results;

namespace WiicoApi.Controllers.BackendManage.Tests
{
    [TestClass()]
    public class UserControllerTests :TestBase<UserController, MemberManageGetRequest, ResultBaseModel<Member>, RegisterRequest, BaseResponse<string>>
    {
        [TestMethod()]
        public void GetTest()
        {
            getTestValue = new MemberManageGetRequest() {
                Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                OrgId = 2,
            };
            target = new UserController();
            var targetRequest = JsonConvert.SerializeObject(getTestValue);
            var targetResponse = target.Get(targetRequest);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<IEnumerable<UserGetResponse>>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<IEnumerable<UserGetResponse>>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);

            getTestValue.OrgId = 4;
            targetRequest = JsonConvert.SerializeObject(getTestValue);
            targetResponse = target.Get(targetRequest);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<IEnumerable<UserGetResponse>>>));
            targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<IEnumerable<UserGetResponse>>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Message == "沒有資料");
            Assert.IsTrue(targetResponseData.Content.Success);
        }
        [TestMethod()]
        public void GetErrorTest()
        {
            getTestValue = new MemberManageGetRequest()
            {
                OrgId = 2,
            };
            target = new UserController();
            var targetRequest = JsonConvert.SerializeObject(getTestValue);
            var targetResponse = target.Get(targetRequest);
            Assert.IsInstanceOfType(targetResponse, typeof(NegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as NegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsFalse(targetResponseData.Content.Success);
        }

        [TestMethod()]
        public void PostTest()
        {
            postTestValue = new RegisterRequest()
            {
                Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                Account = "unittestuser",
                Email = "unittestuser@gmail.com",
                Pwd= "1jV5hDxywOQ=", //9999
                Name = "單元測試帳號",
                OrgCode= "Sce"
            };
            target = new UserController();

            var targetResponse = target.Post(postTestValue);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<UserPostResponse>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<UserPostResponse>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsNotNull(targetResponseData.Content.Data);
            // Assert.IsTrue(targetResponseData.Content.Success);
        }
        [TestMethod()]
        public void PostErrorTest()
        {
            postTestValue = new RegisterRequest()
            {
                Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                //Account = "unittestuser",
             //   Email = "unittestuser@gmail.com",
                Pwd = "1jV5hDxywOQ=", //9999
                Name = "單元測試帳號",
                OrgCode = "Sce"
            };
            target = new UserController();

            var targetResponse = target.Post(postTestValue);
            Assert.IsInstanceOfType(targetResponse, typeof(NegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as NegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
             Assert.IsFalse(targetResponseData.Content.Success);
        }

        [TestMethod()]
        public void PutTest()
        {
            var putTestValue = new MemberManagePutRequest() {
                Account = "unittestuser",
                OrgId=2,
                Email="unittestuser@gmail.com",
                Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                Name="單元測試修改帳號"
            };
            target = new UserController();

            var targetResponse = target.Put(putTestValue);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<Member>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<Member>>;
            Assert.IsNotNull(targetResponseData.Content);
        }

        [TestMethod()]
        public void PutErrorTest()
        {
            var putTestValue = new MemberManagePutRequest()
            {
              //  Account = "unittestuser",
                OrgId = 2,
                Email = "unittestuser@gmail.com",
                Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                Name = "單元測試修改帳號"
            };
            target = new UserController();

            var targetResponse = target.Put(putTestValue);
            Assert.IsInstanceOfType(targetResponse, typeof(NegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as NegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsFalse(targetResponseData.Content.Success);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            target = new UserController();
            var deleteRequestData = new MemberManageDeleteRequest()
            {
                Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
                Members = new List<int>()
            };
            deleteRequestData.Members.Add(1757);
            var jsonRequestData = JsonConvert.SerializeObject(deleteRequestData);
            var targetResponse = target.Delete(jsonRequestData);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as OkNegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsTrue(targetResponseData.Content.Success);
        }
        [TestMethod()]
        public void DeleteErrorTest()
        {
            target = new UserController();
            var deleteRequestData = new MemberManageDeleteRequest()
            {
            //    Token = "4a01b7ed-27d7-4312-9a63-71c53a70dc81",
               // Members = new List<int>()
            };
         //   deleteRequestData.Members.Add(1757);
            var jsonRequestData = JsonConvert.SerializeObject(deleteRequestData);
            var targetResponse = target.Delete(jsonRequestData);
            Assert.IsInstanceOfType(targetResponse, typeof(NegotiatedContentResult<BaseResponse<string>>));
            var targetResponseData = targetResponse as NegotiatedContentResult<BaseResponse<string>>;
            Assert.IsNotNull(targetResponseData.Content);
            Assert.IsFalse(targetResponseData.Content.Success);
        }
        //[TestMethod()]
        //public void UserForgetPassWordTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod()]
        //public void PutTest1()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod()]
        //public void ReGetAccountInfoTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod()]
        //public void LoginTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[TestMethod()]
        //public void SignUpTest()
        //{
        //    throw new NotImplementedException();
        //}
    }
}