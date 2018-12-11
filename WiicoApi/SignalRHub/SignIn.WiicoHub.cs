using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.SignalR.SignIn;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.SignIn;
using WiicoApi.SignalR;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.SignalRHub
{
    /// <summary>
    /// SignalRHub
    /// </summary>
    public partial class WiicoHub : WiicoHubBase
    {
        private SignInService signInService = new SignInService();
        private SignInLogService signInLogService = new SignInLogService();
        private static object _signInLock = new object();
        private bool AuthCheck(Guid token, ref ServerCheckItem item)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token.ToString()).Result;
            if (memberInfo == null)
                return false;
            // 是否為合法使用者
            item.MemberId = memberInfo.Id;
            if (item.MemberId > 0)
            {
                // 取得活動Guid
                if (!string.IsNullOrEmpty(item.OuterKey))
                {
                    var eventId = Service.Utility.OuterKeyHelper.CheckOuterKey(item.OuterKey);
                    if (eventId.HasValue)
                        item.EventId = eventId.Value;
                    else
                        return false;
                }
                // 取得學習圈 id
                if (!string.IsNullOrEmpty(item.CircleKey))
                {
                    var learningcircleService = new LearningCircleService();
                    var learningcircleInfo = learningcircleService.GetDetailByOuterKey(item.CircleKey);
                    if (learningcircleInfo != null)
                        item.CircleId = learningcircleInfo.Id;
                    else
                        return false;
                }
                //如果是smartTA的權限，直接true
                if (memberInfo.RoleName == "3") {
                    item.ModuleAuth = true;
                    return true;
                }
     
                // 判斷特定權限
                if (!string.IsNullOrEmpty(item.ModuleFun))
                {
                    var authService = new AuthService();
                    item.ModuleAuth = authService.CheckFunctionAuth(item.CircleId, item.ModuleFun, item.MemberId);
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        public void SignIn_GetAuth(Guid token, string circleKey)
        {
            try
            {
                // 權限檢查
                var auth = new ServerCheckItem() { CircleKey = circleKey };
                bool check = AuthCheck(token, ref auth);
                if (check)
                {
                    var authService = new AuthService();
                    #region // 取得個人模組權限
                    // 是否需要被點名
                    var isMember = authService.CheckFunctionAuth(auth.CircleId, SignInFunction.Member, auth.MemberId);
                    // 是否可管理點名(發起、編輯、開始/停止、更新驗證碼)
                    var isSignInAdmin = authService.CheckFunctionAuth(auth.CircleId, SignInFunction.Admin, auth.MemberId);

                    var data = new Dictionary<string, bool>();
                    data.Add("isMember", isMember);
                    data.Add("isAdmin", isSignInAdmin);
                    #endregion

                    Clients.Caller.signIn_authList(data);
                }
                else
                    Clients.Caller.onError("SignIn_GetAuth", "身分驗證失敗，請重新登入!token:[" + token + "]");
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                Clients.Caller.onError("SignIn_GetAuth", "取得點名權限發生意外: " + msg);
            }
        }

        /// <summary>
        /// 取得點名活動被點名成員
        /// </summary>
        /// <param name="outerKey"></param>
        public BaseResponse<SignInEvent> SignIn_LoadMembers(string outerKey)
        {
            var responseCommonData = new BaseResponse<SignInEvent>();
            responseCommonData.Success = false;
            responseCommonData.Data = new SignInEvent();
            try
            {
                var eventId =Service.Utility.OuterKeyHelper.CheckOuterKey(outerKey);
                //取出所有成員的點名紀錄                
                var obj = signInService.GetSignInEvent(eventId.Value);
                responseCommonData.Success = true;
                responseCommonData.Data = obj;
                Clients.Caller.signIn_RenderPopup(obj);
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                responseCommonData.Message = "SignIn_LoadMembers 載入點名清單發生意外" + msg;
                Clients.Caller.onError("SignIn_LoadMembers", "載入點名清單發生意外: " + msg);
            }
           // var response = new System.Web.Http.Results.OkNegotiatedContentResult<BaseResponse<SignInEvent>>(responseCommonData, new Controllers.APPSupport.EmptyController());
            return responseCommonData;
        }

        /// <summary>
        /// 載入點名活動
        /// </summary>
        /// <param name="token"></param>
        /// <param name="outerKey"></param>
        public void SignIn_LoadDetails(Guid token, string outerKey)
        {
            try
            {
                // 是否為合法使用者
                var auth = new ServerCheckItem() { OuterKey = outerKey };
                bool check = AuthCheck(token, ref auth);
                if (check)
                {
                    // 取出點名活動、簽到記錄
                    var obj = signInService.GetSignInEvent(auth.EventId, auth.MemberId);

                    Clients.Caller.renderDetails(ModuleType.SignIn, obj);
                }
                else
                    Clients.Caller.onError("SignIn_LoadDetails", "身分驗證失敗，請重新登入!token:[" + token + "]");
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                Clients.Caller.onError("SignIn_LoadDetails", "載入點名細節發生意外: " + msg);
            }
        }

        /// <summary>
        /// 載入點名活動
        /// </summary>
        /// <param name="token"></param>
        /// <param name="outerKey"></param>
        public void SignIn_LoadMultipleDetails(Guid token, string outerKey)
        {
            var tokenInfo = CheckToken(token.ToString());
            if (tokenInfo == null)
                return;
            try
            {
                // 是否為合法使用者
                var auth = new ServerCheckItem();
                var check = AuthCheck(token, ref auth);
                if (check)
                {
                    var authService = new AuthService();
                    // 將outerKey轉回event guid
                    var tokenArray = new List<Guid>();
                    outerKey.Split(',').ToList().ForEach(item => tokenArray.Add(Service.Utility.OuterKeyHelper.PageTokenToGuid(item)));
                    var activityInfo = signInService.GetSignInInfomation(tokenArray.FirstOrDefault());
                    var checkisAdminRole = authService.CheckFunctionAuth(activityInfo.LearningId, SignInFunction.Admin, tokenInfo.MemberId);
                    // 取出點名活動、簽到記錄
                    var param = new SignInEventParam() { EventIds = tokenArray, MemberId = auth.MemberId };
                    var obj = signInService.GetMutipEventData(param, checkisAdminRole);

                    Clients.Caller.renderMultipleDetails(obj);
                }
                else
                    Clients.Caller.onError("SignIn_LoadMultipleDetails", "身分驗證失敗，請重新登入!token:[" + token + "]");
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                Clients.Caller.onError("SignIn_LoadMultipleDetails", "載入點名細節發生意外: " + msg);
            }
        }

        /// <summary>
        /// 建立點名活動
        /// </summary>
        /// <param name="learningId">課id</param>
        /// <param name="token">操作者</param>
        /// <param name="key">點名密碼</param>
        /// <param name="duration">活動持續時間(秒)</param>
        /// <returns></returns>
        public async Task<BaseResponse<ActivitysViewModel>> SignIn_CreateActivity(Guid token, string circleKey, string beaconKey, int duration)
        {
            var response = new BaseResponse<ActivitysViewModel>();

            try
            {
                var auth = new ServerCheckItem() { CircleKey = circleKey, ModuleFun = SignInFunction.Admin };
                bool check = AuthCheck(token, ref auth);
                if (check)
                {
                    if (auth.ModuleAuth)
                    {
                        var lock_circleKey = string.Empty;
                        var lock_OuterKey = Guid.Empty;
                        var lock_MemberId = 0;
                        var isActive = hasActiveEvent(circleKey);

                        if (isActive)
                        {
                            response.Success = false;
                            response.Message = "目前仍有點名活動正在進行中，結束後才能再新增點名。";
                            response.State = Infrastructure.ViewModel.Base.LogState.Error;
                            // activity.hub.js中的onError會以下面參數判斷是否要跳提示視窗，若有修改請一併確認
                            Clients.Caller.onError("SignIn_CreateActivity", "目前仍有點名活動正在進行中，結束後才能再新增點名。");

                        }
                        // 2016-12-28 add by sophiee 一次只能有一個點名活動正在進行中
                        else
                        {
                            lock (_signInLock)
                            {
                                string eventName = DateTime.Now.ToString("MM/dd") + "點名活動";
                                var rtn = signInService.InsertSignInData(circleKey, auth.CircleId, auth.MemberId, beaconKey, duration, eventName);
                                response.Success = true;
                                response.Message = "建立成功";
                                response.Data =  rtn ;
                                // 將訊息發送給群組
                                Clients.Group(circleKey.ToLower()).appendActivity(rtn, "");
                                //活動開始
                                // _eventService.StartEvent(auth.CircleId, rtn.OuterKey, auth.MemberId);
                                //點名人員清單可能會有異動，故重新取得點名物件
                                //  var startRtn = _eventService.GetEventData(rtn.OuterKey, auth.MemberId);
                                var startRtn = signInService.GetSignInEvent(rtn.OuterKey, auth.MemberId);
                                rtn.sOuterKey = Service.Utility.OuterKeyHelper.GuidToPageToken(rtn.OuterKey);
                                //告訴同班的有活動開始
                                Clients.Group(circleKey.ToLower()).signIn_eventStart(rtn.sOuterKey, startRtn);

                                //單元測試直接回傳結果
                                if (beaconKey == "unitTest")
                                    return response;

                                var signalrService = new SignalrService();
                                //發通知
                                var connectIdAndData = signalrService.GetConnectIdAndData(circleKey.ToLower(), auth.MemberId, SignalrConnectIdType.All, SignalrDataType.Activity);
                                SignalrClientHelper.ShowRecordListById(connectIdAndData);

                                // 更新活動紀錄
                                //     ShowRecordList(circleKey.ToLower());

                                lock_circleKey = circleKey;
                                lock_OuterKey = rtn.OuterKey;
                                lock_MemberId = auth.MemberId;
                            }
                            // 發送推播
                            await PushOnCreatedSignIn(lock_circleKey.ToLower(), lock_OuterKey, lock_MemberId);
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "沒有權限";
                        response.State =LogState.Error;
                        Clients.Caller.onError("SignIn_CreateActivity", "您沒有權限!");
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "沒有權限";
                    response.State = LogState.NoAccount;
                    Clients.Caller.onError("SignIn_CreateActivity", "身分驗證失敗，請重新登入!token:[" + token + "]");
                }
            }
            catch (Exception ex)
            {

                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);
                response.Success = false;
                response.Message = msg;
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                Clients.Caller.onError("SignIn_CreateActivity", "建立點名活動發生意外: " + msg);
            }
            return response;
        }

        /// <summary>
        /// 是否有一個點名活動正在進行
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        public void SignIn_GetIsActive(Guid token, string circleKey)
        {
            try
            {
                var auth = new ServerCheckItem() { CircleKey = circleKey };
                bool check = AuthCheck(token, ref auth);
                if (check)
                {
                    var isActive = hasActiveEvent(circleKey);
                    Clients.Caller.signIn_isActive(isActive);
                }
                else
                    Clients.Caller.onError("SignIn_CreateActivity", "身分驗證失敗，請重新登入!token:[" + token + "]");
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace);

                Clients.Caller.onError("SignIn_GetIsActive", "取得是否有進行中的點名活動發生意外: " + msg);
            }
        }
        /// <summary>
        /// 是否為最新的點名活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        private bool IsNewestSignEvent(string circleKey, string outerKey)
        {
            bool isActive = false;
            var eventId =Service.Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            var activityData = eventId.HasValue? activityService.GetByEventId(eventId.Value):null;
            if (activityData == null)
                return isActive;
            var theNewest =activityService.GetNewestByModuleType(ModuleType.SignIn,circleKey.ToLower());
                if (theNewest.Id == activityData.Id)
                    isActive = true;
            return isActive;
        }
        private bool hasActiveEvent(string circleKey)
        {
            return activityService.CheckActivityStarting(circleKey.ToLower(), ModuleType.SignIn);
        }

        /// <summary>
        /// 更新活動持續時間
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="outerKey"></param>
        /// <param name="duration"></param>
        public void SignIn_UpdateDuration(Guid token, string circleKey, string outerKey, int duration)
        {
            try
            {
                var auth = new ServerCheckItem() { CircleKey = circleKey, OuterKey = outerKey, ModuleFun = SignInFunction.Admin };
                bool check = AuthCheck(token, ref auth);
                if (check)
                {
                    if (auth.ModuleAuth)
                    {
                        signInService.UpdateDuration(auth.EventId, auth.MemberId, duration);
                        //2017-01-09 暫時先給APP整個點名物件(未來調整為最省資源)
                        var rtn = signInService.GetSignInEvent(auth.EventId, auth.MemberId);
                        Clients.Group(circleKey.ToLower()).signIn_DurationChanged(outerKey, rtn);
                    }
                }
                else
                {
                    Clients.Caller.onError("SignIn_UpdateDuration", "您沒有變更活動時間的權限");
                }
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("SignIn_UpdateDuration", "變更點名活動時間發生意外: " + ex.Message);
            }
        }

        /// <summary>
        /// 開始活動
        /// </summary>
        /// <param name="circleKey">課id</param>
        /// <param name="token">操作者</param>
        /// <param name="outerKey">點名活動Guid</param>
        public BaseResponse<string> SignIn_StartEvent(string circleKey, Guid token, string outerKey)
        {
            var result = new BaseResponse<string>();
            try
            {
                var auth = new ServerCheckItem() { OuterKey = outerKey, CircleKey = circleKey, ModuleFun = SignInFunction.Admin };
                //查看是否有正在進行中的點名活動
                var isActive = hasActiveEvent(circleKey);

                if (!isActive && IsNewestSignEvent(circleKey, outerKey))
                {
                    bool check = AuthCheck(token, ref auth);
                    if (check)
                    {
                        if (auth.ModuleAuth)
                        {
                            //活動開始
                            signInService.StartEvent(auth.CircleId, auth.EventId, auth.MemberId);

                            //點名人員清單可能會有異動，故重新取得點名物件
                            // var rtn = _eventService.GetEventData(auth.EventId, auth.MemberId);
                            var rtn = signInService.GetSignInEvent(auth.EventId, auth.MemberId);
                            //告訴同班的有活動開始
                            Clients.Group(circleKey.ToLower()).signIn_eventStart(outerKey, rtn);
                            result.Success = true;
                            result.Message = "成功";
                        }
                        else
                        {
                            Clients.Caller.onError("SignIn_StartEvent", "您沒有權限!");
                            result.Success = false;
                            result.Message = "SignIn_StartEvent 您沒有權限!";
                        }

                    }
                    else
                    {
                        Clients.Caller.onError("SignIn_StartEvent", "身分驗證失敗，請重新登入!token:[" + token + "]");
                        result.Success = false;
                        result.Message = "SignIn_StartEvent身分驗證失敗，請重新登入!token:[" + token + "]";
                    }

                }
                else
                {
                    // activity.hub.js中的onError會以下面參數判斷是否要跳提示視窗，若有修改請一併確認
                    Clients.Caller.onError("SignIn_StartEvent", "目前仍有點名活動正在進行中，結束後才能再新增點名。");
                    result.Success = false;
                    result.Message = "SignIn_StartEvent  目前仍有點名活動正在進行中，結束後才能再新增點名。";
                }



            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "SignIn_StartEvent  開始活動發生意外: " + ex.Message;
                Clients.Caller.onError("SignIn_StartEvent", "開始活動發生意外: " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 停止活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <param name="outerKey"></param>
		public BaseResponse<string> SignIn_StopEvent(string circleKey, Guid token, string outerKey)
        {
            var result = new BaseResponse<string>();
            try
            {
                var auth = new ServerCheckItem() { OuterKey = outerKey, CircleKey = circleKey, ModuleFun = SignInFunction.Admin };
                bool chekc = AuthCheck(token, ref auth);
                if (chekc)
                {
                    if (auth.ModuleAuth)
                    {
                        //把活動開始時間改成現在
                        var duration = signInService.UpdateStartDate(auth.EventId, auth.MemberId, false);

                        //告訴同班的有活動結束
                        Clients.Group(circleKey.ToLower()).signIn_eventStop(outerKey, duration);
                        result.Success = true;
                        result.Message = "成功";
                    }
                    else
                    {
                        Clients.Caller.onError("SignIn_StopEvent", "您沒有權限!");
                        result.Success = false;
                        result.Message = "SignIn_StartEvent 您沒有權限!";
                    }
                }
                else
                {
                    Clients.Caller.onError("SignIn_StopEvent", "身分驗證失敗，請重新登入!token:[" + token + "]");
                    result.Success = false;
                    result.Message = "SignIn_StartEvent身分驗證失敗，請重新登入!token:[" + token + "]";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "SignIn_StopEvent  停止活動發生意外: " + ex.Message;
                Clients.Caller.onError("SignIn_StopEvent", "停止活動發生意外: " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 更新手動點名驗證碼
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <param name="outerKey"></param>
        public BaseResponse<string> SignIn_PasswordRefresh(string circleKey, Guid token, string outerKey)
        {
            var response = new BaseResponse<string>();
            try
            {
                var auth = new ServerCheckItem() { CircleKey = circleKey, OuterKey = outerKey, ModuleFun = SignInFunction.Admin };
                bool check = AuthCheck(token, ref auth);
                if (check)
                {
                    if (auth.ModuleAuth)
                    {
                        string newPwd = signInService.UpdateSignInPwd(auth.EventId, auth.MemberId);
                        Clients.Group(circleKey.ToLower()).signIn_PasswordChanged(circleKey, "", outerKey, newPwd);
                        response.Success = true;
                        response.Data = newPwd;
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "您沒有變更密碼的權限";
                    Clients.Caller.onError("SignIn_PasswordRefresh", "您沒有變更密碼的權限");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "變更點名驗證碼發生意外";
                Clients.Caller.onError("SignIn_PasswordRefresh", "變更點名驗證碼發生意外: " + ex.Message);
            }
            return response;
        }

        /// <summary>
        /// 老師更新點名簽到記錄
        /// </summary>
        /// <param name="learningId">學習圈Id</param>
        /// <param name="token">老師token</param>
        /// <param name="eventId">點名活動Guid</param>
        /// <param name="studId">被編輯點名狀態的學生MemberId</param>
        /// <param name="status">點名狀態 1出席,2缺席,3遲到,4早退,5請假</param>
        /// <returns></returns>
        public async Task SignIn_Modify(string circleKey, Guid token, string outerKey, int stuId, int status)
        {
            try
            {
                var auth = new ServerCheckItem() { OuterKey = outerKey, CircleKey = circleKey, ModuleFun = SignInFunction.Admin };
                bool chekc = AuthCheck(token, ref auth);
                if (chekc)
                {
                    if (auth.ModuleAuth)
                    {
                        if (AttendanceState.Status.ContainsKey(status.ToString()))
                        {
                            // 更新點名紀錄
                            var log = signInLogService.UpdateLog(auth.MemberId, auth.EventId, stuId, status.ToString());

                            // 新增一筆通知
                            string stateName = AttendanceState.GetStateName(status.ToString());
                            string text = "您的狀態已更新：" + stateName;
                            var noticeService = new NoticeService();
                            noticeService.AddNoticeSaveChange(circleKey, stuId, auth.EventId, text);
                            var signalrService = new SignalrService();
                            //發通知給學生
                            var connectIdAndNoticeData = signalrService.GetConnectIdAndData(circleKey.ToLower(), log.StuId, SignalrConnectIdType.One,SignalrDataType.Notice);

                            SignalrClientHelper.SendNotice(connectIdAndNoticeData);
                            //告訴同學點名狀態已改變
                            Clients.Group(circleKey.ToLower()).signIn_StatusChanged(outerKey, log);

                            // 發送推播
                            await PushOnChangedSignIn(circleKey.ToLower(), auth.EventId, log.StudId, stateName);
                        }
                        else
                            Clients.Caller.onError("SignIn_Modify", "狀態錯誤!");
                    }
                    else
                        Clients.Caller.onError("SignIn_Modify", "您沒有權限!");
                }
                else
                    Clients.Caller.onError("SignIn_Modify", "身分驗證失敗，請重新登入!token:[" + token + "]");
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("SignIn_Modify", "老師點名發生意外: " + ex.Message);
            }
        }

        /// <summary>
        /// beacon簽到
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <param name="outerKey"></param>
        /// <param name="keys"></param>
        public void SignIn_BeaconSignIn(string circleKey, Guid token, string outerKey, string[] keys)
        {
            UpdateSignIn(circleKey, token, outerKey, "beacon", keys);
        }

        /// <summary>
        /// 學生簽到
        /// </summary>
        /// <param name="learningId">課id</param>
        /// <param name="token">操作者</param>
        /// <param name="eventId">活動id</param>
        public BaseResponse<string> SignIn_PasswordSignIn(string circleKey, Guid token, string outerKey, string inputPassword)
        {
            return UpdateSignIn(circleKey, token, outerKey, "pwd", new string[] { inputPassword });
        }

        private BaseResponse<string> UpdateSignIn(string circleKey, Guid token, string outerKey, string checkType, string[] keys)
        {
            var response = new BaseResponse<string>();
            try
            {
                var auth = new ServerCheckItem() { OuterKey = outerKey, CircleKey = circleKey, ModuleFun = SignInFunction.Member };
                bool check = AuthCheck(token, ref auth);
                if (check)
                {
                    // 取出目前的點名活動
                    var eve = signInService.GetSignInInfomation(auth.EventId);

                    #region //確認活動正在進行

                    var act = activityService.GetByEventId(auth.EventId);

                    var endTime = act.StartDate.Value.Add(new TimeSpan(0, 0, act.Duration.Value));
                    if (act.StartDate != null && (DateTime.UtcNow <= act.StartDate || DateTime.UtcNow >= endTime))
                    {
                        response.Success = false;
                        response.Message = "現在活動不是進行中";
                        Clients.Caller.signIn_Failed("UpdateSignIn", outerKey, "現在活動不是進行中");
                        return response;
                    }
                    #endregion

                    #region //確認學生可以被點名

                    if (auth.ModuleAuth == false)
                    {
                        response.Success = false;
                        response.Message = "你沒有修這堂課";
                        Clients.Caller.signIn_Failed("UpdateSignIn", outerKey, "你沒有修這堂課");
                        return response;
                    }

                    #endregion

                    #region //缺席才可簽到

                    var log = signInLogService.GetLogData(eve.Id ,auth.MemberId ,AttendanceState.Absence);
                    if (log == null)
                    {
                        Clients.Caller.signIn_Failed("UpdateSignIn", outerKey, "只有缺席的學生可以簽到");
                        response.Success = false;
                        response.Message = "只有缺席的學生可以簽到";
                        return response;
                    }
                    #endregion

                    #region // 簽到碼檢查

                    switch (checkType)
                    {
                        // 若學生不在教室，回傳錯誤訊息
                        case "beacon":
                            if (!keys.Contains(eve.SignInKey))
                            {
                                var msg = string.Format("你不在教室!\n教室beacons:\n{0}\n\n偵測到:\n{1}", eve.SignInKey, string.Join("\n", keys));
                                Clients.Caller.signIn_Failed("SignIn_BeaconSignIn", outerKey, msg);
                                response.Success = false;
                                response.Message = msg;
                                return response;
                            }
                            break;

                        // 簽到密碼錯誤，回傳錯誤訊息
                        case "pwd":
                            if (eve.SignInPwd != keys[0])
                            {
                                response.Success = false;
                                response.Message = "驗證碼錯誤";
                                Clients.Caller.signIn_Failed("SignIn_PasswordSignIn", outerKey, "驗證碼錯誤！");
                                return response;
                            }
                            break;
                    }
                    #endregion

                    string stateCode = AttendanceState.Attend;

                    // 檢查通過，save data
                    var mylog = signInLogService.UpdateLog(auth.MemberId, auth.EventId, auth.MemberId, stateCode);

                    // 新增一筆通知
                    string stateName = AttendanceState.GetStateName(stateCode);
                    string text = "您的狀態已更新：" + stateName;
                    AddNotice(circleKey, auth.EventId, auth.MemberId, text);
                    //告訴同學點名狀態已改變
                    Clients.Group(circleKey.ToLower()).signIn_StatusChanged(outerKey, mylog);


                    // 2016-09-20 mark by sophiee:APP team告知，如果是學生自己簽到就不發送推播
                    // await PushOnChangedSignIn(circleKey, auth.EventId, mylog.StudId, stateName);
                    response.Success = true;
                }
                else
                {
                    response.Message = "身分驗證失敗，請重新登入";
                    Clients.Caller.onError("UpdateSignIn", "身分驗證失敗，請重新登入!token:[" + token + "]");
                    response.Success = false;
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "學生簽到發生意外";
                Clients.Caller.onError("UpdateSignIn", "學生簽到發生意外: " + string.Format("{0}\n\n{1}", ex.Message, ex.StackTrace));
            }
            return response;
        }

        #region // 推播
        public async Task PushOnCreatedSignIn(string circleKey, Guid eventId, int? myId)
        {
            var creatorName = string.Empty;
            var memberService = new MemberService();
            if (myId != null)
            {
                creatorName = memberService.UserIdToAccount(myId.Value).Name;
            }
            var learningcircleService = new LearningCircleService();
            var members = learningcircleService.GetCircleMemberList(circleKey, myId);
            var eventMessage = string.Format("{0}新增了點名活動", creatorName);
            var pushService = new PushService();
            // 推播文字:點名活動即將開始({日期} {時間})
            // var message = string.Format("{0}(建立時間:{1:yyyy/M/d HH:mm})", eventMessage, DateTime.Now);
            var message = string.Format("{0}", eventMessage);
            if (members.Count > 0)
                await pushService.PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看點名-開始", members.ToArray(), message);
        }

        public async Task PushOnChangedSignIn(string circleKey, Guid eventId, string studentId, string updateStatus)
        {
            var students = new List<string>() { studentId };
            var eventMessage = string.Format("您的狀態已更新：{0}", updateStatus);

            // 推播文字→ [點名活動({建立時間:yyyy/M/d hh:mm})]您的狀態已更新：{ 出席狀態}
            //  var message = string.Format("[點名活動(建立時間:{1:yyyy/M/d HH:mm})]{0}", eventMessage, DateTime.Now);
            var message = string.Format("點名活動({1:yyyy/M/d HH:mm}){0}", eventMessage, DateTime.Now);
            var pushService = new PushService();
            await pushService.PushMsgAsync("ToSignInActivity", circleKey, eventId, "推播_查看點名-更新", students.ToArray(), message);
        }
        #endregion
    }
}