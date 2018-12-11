using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.APPSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction;
using Newtonsoft.Json;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.SignIn;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Vote;

namespace WiicoApi.Controllers.APPSupport.Tests
{
    [TestClass()]
    public class ActivityListControllerTests
    {
        private readonly ActivityListRequest testValue = new ActivityListRequest() {
            Account = "yushuchen",
            CircleKey = "9999testcourse",
            ICanToken = "4a01b7ed-27d7-4312-9a63-71c53a70dc81"
        };
        /// <summary>
        /// 主題討論列表測試
        /// </summary>
        [TestMethod()]
        public void GetDiscussionTest()
        {
            var target = new ActivityListController();
            testValue.Id = activityEnum.Discussion;
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);
            Assert.IsInstanceOfType(targetResponse,typeof(OkNegotiatedContentResult<ResultBaseModel<DiscussionModuleList>>));
            var responseResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<DiscussionModuleList>>;
            Assert.IsNotNull(responseResData.Content);
            Assert.IsTrue(responseResData.Content.Success);
        }
        /// <summary>
        /// 點名列表測試
        /// </summary>
        [TestMethod()]
        public void GetSignInTest()
        {
            var target = new ActivityListController();
            testValue.Id = activityEnum.SignIn;
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<SignInModuleList>>));
            var responseResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<SignInModuleList>>;
            Assert.IsNotNull(responseResData.Content);
            Assert.IsTrue(responseResData.Content.Success);
        }

        /// <summary>
        /// 上傳檔案列表測試
        /// </summary>
        [TestMethod()]
        public void GetMaterialTest()
        {
            var target = new ActivityListController();
            testValue.Id = activityEnum.Material;
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<MaterialViewModel>>));
            var responseResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<MaterialViewModel>>;
            Assert.IsNotNull(responseResData.Content);
            Assert.IsTrue(responseResData.Content.Success);
        }
        /// <summary>
        /// 投票活動列表測試
        /// </summary>
        [TestMethod()]
        public void GetVoteTest()
        {
            var target = new ActivityListController();
            testValue.Id = activityEnum.Vote;
            var jsonString = JsonConvert.SerializeObject(testValue);
            var targetResponse = target.Get(jsonString);
            Assert.IsInstanceOfType(targetResponse, typeof(OkNegotiatedContentResult<ResultBaseModel<VoteListResponse>>));
            var responseResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<VoteListResponse>>;
            Assert.IsNotNull(responseResData.Content);
            Assert.IsTrue(responseResData.Content.Success);
        }
    }
}