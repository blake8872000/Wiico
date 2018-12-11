using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Moq;
using Microsoft.AspNet.SignalR.Hubs;
using System.Dynamic;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using Newtonsoft.Json.Linq;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.SignalRHub;
using Microsoft.AspNet.SignalR;
using System.Security.Principal;
using Microsoft.Owin;
using WiicoApi.Tests.SignalRHub;
using System.Web.Http.Results;
using WiicoApi.Infrastructure.ViewModel.ActivityFunction.Discussion;

namespace WiicoApi.Tests.SignalRHub
{

    /// <summary>
    /// 註冊signalr client回傳的methods
    /// </summary>
    public interface IClientContract
    {

        #region ConectionTest 登入後連線SignalR的測試

        /// <summary>
        /// 顯示連線version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        string showversion(string version);

        /// <summary>
        ///  conneciton結果
        /// </summary>
        /// <param name="token"></param>
        /// <param name="connectionid"></param>
        /// <returns></returns>
        string onSysConnected(string token, string connectionid);
        /// <summary>
        /// 取得課程列表getGroup 監聽結果
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<LearningCircle> showCircleList(List<LearningCircle> list);
        /// <summary>
        /// 取得活動詳細資料LoadActivityDetail 監聽結果
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <param name="detailData"></param>
        /// <returns></returns>
        object renderDetails(string moduleKey, object detailData);
        /// <summary>
        /// 顯示訊息紀錄列表LoadRecordList 監聽結果
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        object showRecordList(IEnumerable<ActivitysLatest> datas);

        /// <summary>
        /// 顯示即時訊息列表 LoadNoticeList  監聽結果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object showNoticeList(ActivitysNoticeViewModel data);

        #endregion

        #region JoinGroupTest 進入學習圈後會跑的基本功能測試
        /// <summary>
        /// 進入課程Join Group 監聽結果
        /// </summary>
        /// <param name="groupKey"></param>
        /// <param name="token"></param>
        /// <param name="connId"></param>
        /// <param name="authList"></param>
        /// <returns></returns>
        object onConnected(string groupKey, Guid token, string connId, JObject authList);

        /// <summary>
        /// 初始化活動牆 取得活動列表 LoadInitialActivities 監聽結果
        /// </summary>
        /// <param name="activitys"></param>
        /// <returns></returns>
        object showInitActivities(ReadMarkResult<ActivitysViewModel> activitys);

        /// <summary>
        /// 取得較舊的活動列表 LoadOlderActivities監聽結果
        /// </summary>
        /// <param name="activitys"></param>
        /// <returns></returns>
        object showOlderActivities(ReadMarkResult<ActivitysViewModel> activitys);

        /// <summary>
        /// 取得較舊的活動列表  LoadOlderActivities監聽結果
        /// </summary>
        /// <param name="activitys"></param>
        /// <param name="isolder"></param>
        /// <returns></returns>
        object showLastActivities(ReadMarkResult<ActivitysViewModel> activitys,bool isolder);

        /// <summary>
        /// 取得較新的活動列表 LoadNewerActivities監聽結果
        /// </summary>
        /// <param name="activitys"></param>
        /// <returns></returns>
        object showNewerActivities(ReadMarkResult<ActivitysViewModel> activitys);

        /// <summary>
        ///  顯示學習圈成員列表 - 含角色 ShowMemberRoleList監聽結果
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        object showInitMemberRoleList(IEnumerable<Infrastructure.BusinessObject.LearningCircleMemberList> list);

        #endregion

        #region SignInActivityTest 點名活動測試
        /// <summary>
        /// 取得被點名成員列表 SignIn_LoadMembers 的監聽結果
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        object signIn_RenderPopup(SignInEvent members);

        /// <summary>
        /// 點名開始 SignIn_StartEvent監聽結果
        /// </summary>
        /// <param name="outerKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        object signIn_eventStart(string outerKey, SignInEvent data);

        /// <summary>
        /// 點名結束 SignIn_StopEvent監聽結果
        /// </summary>
        /// <param name="outerKey"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        object signIn_eventStop(string outerKey, int duration);

        /// <summary>
        /// 老師修改點名後結果 SignIn_Modify監聽結果
        /// </summary>
        /// <param name="outetKey"></param>
        /// <param name="logData"></param>
        /// <returns></returns>
        object signIn_StatusChanged(string outetKey, SignInLog logData);

        /// <summary>
        /// 老師修改點名密碼 SignIn_PasswordRefresh監聽結果
        /// </summary>
        /// <param name="circlekey"></param>
        /// <param name="empty"></param>
        /// <param name="outerkey"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        object signIn_PasswordChanged(string circlekey, string empty, string outerkey, string pwd);

        /// <summary>
        /// 點名失敗原因 UpdateSignIn 監聽結果
        /// </summary>
        /// <param name="methodsName"></param>
        /// <param name="outerKey"></param>
        /// <param name="failMsg"></param>
        /// <returns></returns>
        object signIn_Failed(string methodsName, string outerKey, string failMsg);

        #endregion

        #region DiscussionActivityTest 主題討論活動測試

        /// <summary>
        /// 取得主題討論留言內頁資料 LoadCommentDetail監聽結果
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        object loadCommentDetail(DiscussionCommentDetail comment);

        /// <summary>
        /// 取得比較新的主題討論留言列表 LoadNewerComments監聽結果
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        object loadNewerComments(DiscussionLoadComment comments);

        /// <summary>
        /// 取得比較舊的主題討論留言列表 LoadOldwerComments監聽結果
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        object loadOlderComments(DiscussionLoadComment comments);

        /// <summary>
        /// 修改按讚狀態 SwitchLike監聽結果
        /// </summary>
        /// <param name="likeInfo"></param>
        /// <returns></returns>
        object updateLikeInfo(DiscussionUpdateLikeInfo likeInfo);
        #endregion

        /// <summary>
        /// 建立一筆新的活動在活動牆上
        /// </summary>
        /// <param name="datas">建立的活動</param>
        /// <param name="outerkey">要塞在哪個活動位置的下面</param>
        /// <returns></returns>
        object appendActivity(ActivitysViewModel datas, string outerkey);

        void onError(string errorFunction, string errorMsg);
    }

    /// <summary>
    /// WiicoHubSignalRTests
    /// </summary>
    public class WiicoHubSignalRTests
    {
        private readonly bool showData = false;
        private const string connectionId = "1234";
        private const string groupName = "9999testcourse";
        private WiicoHub hub;
        private Mock<IClientContract> client;
        private Mock<IGroupManager> group;
        private Mock<IHubCallerConnectionContext<dynamic>> mockClients;
        private Mock<IPrincipal> mockUser;
        private Mock<IRequest> request;



        public WiicoHubSignalRTests() {

            #region 初始化SignalRHub
            client = new Mock<IClientContract>();
            group = new Mock<IGroupManager>();
            hub = new WiicoHub();
            mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
            mockUser = new Mock<IPrincipal>();
            request = new Mock<IRequest>();
            request.Setup(r => r.User).Returns(mockUser.Object);
            var mockHubCallerContext = new Mock<HubCallerContext>(request.Object, connectionId);
            mockHubCallerContext.Setup(t => t.ConnectionId).Returns(connectionId);
            hub.Context = mockHubCallerContext.Object;

            //註冊回傳的對象 Caller All Group
            mockClients.Setup(m => m.Caller).Returns(client.Object);
            mockClients.Setup(m => m.All).Returns(client.Object);
            mockClients.Setup(m => m.Group(groupName)).Returns(client.Object);

            //註冊GroupName
            group.Object.Add(connectionId, groupName);

            hub.Groups = group.Object;

            hub.Clients = mockClients.Object;

            //註冊回傳的對象 Caller All
            mockClients.Setup(m => m.Caller).Returns(client.Object);
            mockClients.Setup(m => m.All).Returns(client.Object);
            #endregion

            #region 註冊模擬client回傳的signalrMethods

            /*ConnectionTest*/
            client.Setup(m => m.showversion(It.IsAny<string>())).Verifiable();
            client.Setup(m => m.onSysConnected(It.IsAny<string>(),It.IsAny<string>())).Verifiable();
            client.Setup(m => m.showCircleList(It.IsAny<List<LearningCircle>>())).Verifiable();
            client.Setup(m => m.renderDetails(It.IsAny<string>(), It.IsAny<object>())).Verifiable();
            client.Setup(m => m.showRecordList(It.IsAny<IEnumerable<ActivitysLatest>>())).Verifiable();
            client.Setup(m => m.showNoticeList(It.IsAny<ActivitysNoticeViewModel>())).Verifiable();

            /*JoinGroupTest*/
            client.Setup(m => m.onConnected(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<JObject>())).Verifiable();
            client.Setup(m => m.showInitActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>())).Verifiable();
            client.Setup(m => m.showOlderActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>())).Verifiable();
            client.Setup(m => m.showLastActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>(),It.IsAny<bool>())).Verifiable();
            client.Setup(m => m.showNewerActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>())).Verifiable();
            client.Setup(m => m.showInitMemberRoleList(It.IsAny<IEnumerable<Infrastructure.BusinessObject.LearningCircleMemberList>>())).Verifiable();

            /*SignInActivityTest*/
            client.Setup(m => m.signIn_RenderPopup(It.IsAny<SignInEvent>())).Verifiable();
            client.Setup(m => m.appendActivity(It.IsAny<ActivitysViewModel>(),It.IsAny<string>())).Verifiable();
            client.Setup(m => m.signIn_eventStart(It.IsAny<string>(), It.IsAny<SignInEvent>())).Verifiable();
            client.Setup(m => m.signIn_eventStop(It.IsAny<string>(), It.IsAny<int>())).Verifiable();
            client.Setup(m => m.signIn_StatusChanged(It.IsAny<string>(), It.IsAny<SignInLog>()));
            client.Setup(m => m.signIn_PasswordChanged(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            client.Setup(m => m.signIn_Failed(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            /*DisucssionActivityTest*/
            client.Setup(m => m.loadCommentDetail(It.IsAny<DiscussionCommentDetail>())).Verifiable();
            client.Setup(m=>m.loadNewerComments(It.IsAny<DiscussionLoadComment>())).Verifiable();
            client.Setup(m=>m.loadOlderComments(It.IsAny<DiscussionLoadComment>())).Verifiable();
            client.Setup(m => m.updateLikeInfo(It.IsAny<DiscussionUpdateLikeInfo>())).Verifiable();

            #endregion
        }
        /// <summary>
        /// 連線測試
        /// </summary>
        [Fact]
        public void ConectionTest()
        {
            var testTokenValue = Guid.Parse("4a01b7ed-27d7-4312-9a63-71c53a70dc81");

            var getConnectionResponseData = hub.connection(testTokenValue.ToString()); //測試連線
            //呼叫測試methods
            var getGroupResponseData = hub.getGroup(testTokenValue); //取得上課列表
      
            var loadRecordListResponseData = hub.LoadRecordList(testTokenValue);//取得訊息紀錄列表
            var loadNoticeListResponseData = hub.LoadNoticeList(testTokenValue);//取得即時訊息列表

            //驗證server showCircleList的caller資料
            client.Verify(m => m.showCircleList(It.IsAny<List<LearningCircle>>()));
            //驗證server showRecordList的caller資料
            client.Verify(m => m.showRecordList(It.IsAny<IEnumerable<ActivitysLatest>>()));
            //驗證server showNoticeList的caller資料
            client.Verify(m => m.showNoticeList(It.IsAny<ActivitysNoticeViewModel>()));
            if (showData)
                ShowData(client.Invocations);
        }

        /// <summary>
        /// 進入課程測試
        /// </summary>
        [Fact]
        public void JoinGroupTest()
        {
            #region 設定測試參數
            var testTokenValue = Guid.Parse("4a01b7ed-27d7-4312-9a63-71c53a70dc81");
            var testCircleKey = "9999testcourse";
            #endregion

            //呼叫測試methods
            var joinGroupResponseData = hub.joinGroup(testCircleKey, testTokenValue);//進入課程
            var loadInitialActivitiesResponseData = hub.LoadInitialActivities(testTokenValue,testCircleKey);//進入課程
           var loadOlderActivitiesResponseData = hub.LoadOlderActivities(testTokenValue, testCircleKey); //查詢較舊的活動列表
            var loadNewerActivitiesResponseData = hub.LoadNewerActivities(testTokenValue, testCircleKey); //查詢較新的活動列表
            var updateNoticeClick = hub.UpdateNoticeClick(testTokenValue, 1);//更新即時訊息已按過狀態
            var updateNoticeClickResponse = updateNoticeClick as BaseResponse<string>;
            var updateRead = hub.UpdateRead(testTokenValue, testCircleKey); //更新活動牆已讀狀態
            var updateReadResponse = updateRead as BaseResponse<IEnumerable<ActivitysLatest>>;

            //驗證server joinGroup的caller資料
            client.Verify(m => m.onConnected(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<JObject>()));
            //驗證server LoadInitialActivities的caller資料
            client.Verify(m => m.showInitActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>()));
              //驗證server showLastActivities的caller資料
            client.Verify(m => m.showLastActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>(), It.IsAny<bool>()));
            //驗證server showOlderActivities的caller資料
           // client.Verify(m => m.showOlderActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>()));
            //驗證server showNewerActivities的caller資料
            client.Verify(m => m.showNewerActivities(It.IsAny<ReadMarkResult<ActivitysViewModel>>()));

            //驗證UpdateNoticeClick 的回傳資料，是否修改成功
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(updateNoticeClick, typeof(BaseResponse<string>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(updateNoticeClickResponse.Success);

            //updateRead 的回傳資料，是否修改成功
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(updateRead, typeof(BaseResponse<IEnumerable<ActivitysLatest>>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(updateReadResponse.Success);

            if (showData)
                //可下中斷點查看資料內容
                ShowData(client.Invocations);
        }

        /// <summary>
        /// 顯示學習圈內資訊
        /// </summary>
        [Fact]
        public void LoadActivityDetailTest()
        {
            #region 設定測試參數
            var testTokenValue = Guid.Parse("4a01b7ed-27d7-4312-9a63-71c53a70dc81");
            var testCircleKey = "9999testcourse";
            var testModuleKey = "discussion";
            var testOuterKey = "A6D3B148-5CF7-427B-99DE-4674048EB0E9";
            #endregion

            //呼叫測試methods
            var clearNoticeUnreadCount = hub.ClearNoticeUnreadCount(testTokenValue); //清除訊息紅點
            var discussionDetailResponseData = hub.LoadActivityDetail(testTokenValue, testModuleKey, testOuterKey);//取得主題討論單一活動詳細資訊
            var showMemberRoleList = hub.ShowMemberRoleList(testTokenValue, testCircleKey);//顯示學習圈角色成員
            var showMemberRoleListResponseData = showMemberRoleList as List<Infrastructure.BusinessObject.LearningCircleMemberList>;

            testModuleKey = "signin";
            testOuterKey = "5A104E55-560B-4BBA-B071-6D8B8D4C58F6";
            var signInDetailResponseData = hub.LoadActivityDetail(testTokenValue, testModuleKey, testOuterKey);//取得點名單一活動詳細資訊
            testModuleKey = "message";
            testOuterKey = "3F1D8434-C53C-4D1A-BC67-3C6C476B2D96";
            var messageDetailResponseData = hub.LoadActivityDetail(testTokenValue, testModuleKey, testOuterKey);//取得活動牆訊息單一活動詳細資訊
            testModuleKey = "material";
            testOuterKey = "662B49FD-8DB4-42B2-A226-F0F5A23AA9C3";
            var materialDetailResponseData = hub.LoadActivityDetail(testTokenValue, testModuleKey, testOuterKey);//取得活動牆圖片單一活動詳細資訊
            testModuleKey = "leave";
            testOuterKey = "B9B3D942-2324-403B-B99C-AF0517A28963";
            var leaveDetailResponseData = hub.LoadActivityDetail(testTokenValue, testModuleKey, testOuterKey);//取得請假單一活動詳細資訊
            testModuleKey = "vote";
            testOuterKey = "798252F4-B9A1-4842-9098-2C3F1C7CD2DF";
            var voteDetailResponseData = hub.LoadActivityDetail(testTokenValue, testModuleKey, testOuterKey);//取得投票單一活動詳細資訊

            //驗證server renderDetails的caller資料
            client.Verify(m => m.renderDetails(It.IsAny<string>(), It.IsAny<object>()));

            //驗證server showInitMemberRoleList的caller資料
            client.Verify(m => m.showInitMemberRoleList( It.IsAny<IEnumerable<Infrastructure.BusinessObject.LearningCircleMemberList>>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(showMemberRoleList, typeof(List<Infrastructure.BusinessObject.LearningCircleMemberList>));

           
            if (showData)
                //可下中斷點查看資料內容
                ShowData(client.Invocations);
        }

        /// <summary>
        /// 點名活動測試
        /// </summary>
        [Fact]
        public void SignInActivityTest()
        {
            #region 設定測試參數
            var testTokenValue = Guid.Parse("4a01b7ed-27d7-4312-9a63-71c53a70dc81");
            var testStudentToken = Guid.Parse("c5f6f98b-2923-4b38-bb8c-b782f40a109b");
            var testCircleKey = "9999testcourse";
            var testOuterKey = string.Empty;
            var testPwd = string.Empty;
            var testBeaconKey = "61eb8d82-c4dc-404b-8598-ee48378764fb_12345_67890";
            var testDuration = 1;
            var testStuId = 5;//學生編號
            var testStatus = 2;
            #endregion

            //呼叫測試methods
            var signIn_CreateActivity = hub.SignIn_CreateActivity(testTokenValue,testCircleKey,testBeaconKey,testDuration).Result; //建立點名活動
            testOuterKey = signIn_CreateActivity.Data.OuterKey.ToString();

           // var signIn_StopEvent = hub.SignIn_StopEvent(testCircleKey, testTokenValue, testOuterKey);//停止點名
            //var signIn_StartEvent = hub.SignIn_StartEvent(testCircleKey, testTokenValue, testOuterKey);//開始點名

            var signIn_PasswordRefresh = hub.SignIn_PasswordRefresh(testCircleKey, testTokenValue, testOuterKey);//修改點名驗證碼
            testPwd = signIn_PasswordRefresh.Data;
            var signIn_PasswordSignIn = hub.SignIn_PasswordSignIn(testCircleKey, testStudentToken, testOuterKey, testPwd);//學生點名
            hub.SignIn_StopEvent(testCircleKey, testTokenValue, testOuterKey);//停止點名

            var signIn_LoadMembers = hub.SignIn_LoadMembers(testOuterKey); //取得點名成員列表
            var signIn_LoadMembersResponseData = signIn_LoadMembers as BaseResponse<SignInEvent>;

            hub.SignIn_Modify(testCircleKey, testTokenValue, testOuterKey, testStuId, testStatus);//老師修改點名狀態


            //驗證server signIn_CreateActivity的caller資料
            client.Verify(m => m.appendActivity(It.IsAny<ActivitysViewModel>(),It.IsAny<string>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(signIn_CreateActivity, typeof(BaseResponse<ActivitysViewModel>));
           // Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(signIn_CreateActivity.Success);

            //驗證server signIn_StartEvent的caller資料
            client.Verify(m => m.signIn_eventStart(It.IsAny<string>(), It.IsAny<SignInEvent>()));

            //驗證server signIn_StopEvent的caller資料
            client.Verify(m => m.signIn_eventStop(It.IsAny<string>(), It.IsAny<int>()));
          //  Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(signIn_StopEvent, typeof(BaseResponse<string>));
          //  Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(signIn_StopEvent.Success);

            //驗證server signIn_PasswordRefresh的caller資料
            client.Verify(m => m.signIn_PasswordChanged(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(signIn_PasswordRefresh, typeof(BaseResponse<string>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(signIn_PasswordRefresh.Success);

            //驗證server signIn_LoadMembers的caller資料
            client.Verify(m => m.signIn_RenderPopup(It.IsAny<SignInEvent>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(signIn_LoadMembers, typeof(BaseResponse<SignInEvent>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(signIn_LoadMembersResponseData.Success);

            //驗證server SignIn_Modify的caller資料
            client.Verify(m => m.signIn_StatusChanged(It.IsAny<string>(), It.IsAny<SignInLog>()));

            //驗證server updateSignIn的caller資料
           // client.Verify(m => m.signIn_Failed(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            if (showData)
                //可下中斷點查看資料內容
                ShowData(client.Invocations);
        }

        /// <summary>
        /// 主題討論活動測試
        /// </summary>
        [Fact]
        public void DiscussionActivityTest()
        {
            #region 設定測試參數
            var testTokenValue = Guid.Parse("4a01b7ed-27d7-4312-9a63-71c53a70dc81");
            var testOlderMsgOuterKey = "8D92446F-B788-440C-A33B-AAAD574F2F43";
            var testNewerMsgOuterKey = "6D037937-F5B7-4AD2-9AB9-2703A837D534";
            var testCircleKey = "9999testcourse";
            var testOuterKey = "A6D3B148-5CF7-427B-99DE-4674048EB0E9";
            var testPwd = string.Empty;

            #endregion

            //呼叫測試methods
            var loadCommentDetail = hub.LoadCommentDetail(testTokenValue, testOuterKey); //進入主題討論留言內頁
            var loadOlderComments = hub.LoadOlderComments(testTokenValue,testCircleKey,testOuterKey,testOlderMsgOuterKey,5);//查詢舊的留言
            var loadNewerComments = hub.LoadNewerComments(testTokenValue, testCircleKey, testOuterKey, testNewerMsgOuterKey, 5);//查詢新的留言
            var switchLile = hub.SwitchLike(testTokenValue, testCircleKey, testOuterKey, testOlderMsgOuterKey).Result;//留言按讚

            //驗證server loadCommentDetail的caller資料
            client.Verify(m => m.loadCommentDetail(It.IsAny<DiscussionCommentDetail>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(loadCommentDetail, typeof(ResultBaseModel<DiscussionCommentDetail>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(loadCommentDetail.Success);

            //驗證server loadNewerComments的caller資料
            client.Verify(m => m.loadNewerComments(It.IsAny<DiscussionLoadComment>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(loadNewerComments, typeof(ResultBaseModel<DiscussionLoadComment>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(loadNewerComments.Success);

            //驗證server loadOlderComments的caller資料
            client.Verify(m => m.loadOlderComments(It.IsAny<DiscussionLoadComment>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(loadOlderComments, typeof(ResultBaseModel<DiscussionLoadComment>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(loadOlderComments.Success);

            //驗證server updateLikeInfo的caller資料
            client.Verify(m => m.updateLikeInfo(It.IsAny<DiscussionUpdateLikeInfo>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(switchLile, typeof(BaseResponse<string>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(switchLile.Success);


            if (showData)
                //可下中斷點查看資料內容
                ShowData(client.Invocations);
        }

        /// <summary>
        /// 活動牆訊息測試
        /// </summary>
        [Fact]
        public void MessageActivityTest() {
            #region 設定測試參數
            var testTokenValue = Guid.Parse("4a01b7ed-27d7-4312-9a63-71c53a70dc81");
            var testText = string.Format("單元測試留言-{0}",DateTime.Now);
            var testCircleKey = "9999testcourse";

            #endregion
            //呼叫測試methods
            var loadCommentDetail = hub.Message_CreateActivity(testTokenValue,testCircleKey,"", testText); //建立活動牆留言
            var loadCommentDetailResponseData = loadCommentDetail as OkNegotiatedContentResult<BaseResponse<ActivitysViewModel>>;
            //驗證server message_CreateActivity的caller資料
            client.Verify(m => m.appendActivity(It.IsAny<ActivitysViewModel>(), It.IsAny<string>()));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(loadCommentDetail, typeof(OkNegotiatedContentResult<BaseResponse<ActivitysViewModel>>));
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(loadCommentDetailResponseData.Content.Success);


        }

        /// <summary>
        /// 顯示查看資料
        /// </summary>
        /// <param name="datas"></param>
        private void ShowData(IInvocationList datas) {
            //可下中斷點查看資料內容
            foreach (var signalrResponseData in datas)
            {
                var showDatas = new object();
                if (signalrResponseData != null && signalrResponseData.Arguments != null)
                    showDatas = signalrResponseData.Arguments;
            }
            //取得signalr回傳的資料
            /* var getGroupResponseDatas = ( client.Invocations!=null && client.Invocations[0]!=null && client.Invocations[0].Arguments!=null && client.Invocations[0].Arguments[0]!=null) ?
             client.Invocations[0].Arguments[0]
             : null;
             //判斷是否取為正確的資料
             Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(getGroupResponseDatas, typeof(List<LearningCircle>));
             //將資料轉換成正確的資料
             var targetResData = getGroupResponseDatas as List<LearningCircle>;
             //判斷是否為null
             Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(targetResData);*/
        }
    }
}
