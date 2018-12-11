using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.Login;
using WiicoApi.Repository;
using WiicoApi.Service.Backend;
using WiicoApi.Service.SchoolApi;
using WiicoApi.Service.Utility;

namespace WiicoApi.Service.CommenService
{
    public class LoginService
    {
        private readonly GenericUnitOfWork _uow;
        private Encryption encryptionService = null;
        private readonly string appKey = ConfigurationManager.AppSettings["AppLoginKey"].ToString();
        public LoginService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 重新取得登入資訊
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public LoginResponse ReGetAccountInfo(BackendBaseRequest data)
        {
            var db = _uow.DbContext;
            var loginMember = db.Members.FirstOrDefault(t => t.Account == data.Account);
            var result = SetLoginResponse(null, loginMember, data.Code.ToLower());
            return result;
        }

        /// <summary>
        /// 登入資料處理 - 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public LoginResponse LoginProxy(LoginRequest data)
        {
            //設定回傳資訊
            var result = new LoginResponse();

            //判斷是否有遺漏資訊
            if (!PassLoginDataCheck(data))
                return null;

            var checkAccountPwd = AccountCheck(data);
            //驗證帳號密碼
            if (!checkAccountPwd)
                return null;
            else
            {
                //查出是否有模擬登入帳號
                var accounts = GetSimulatorAccount(data.Account, data.OrgId);
                //登入者帳號資訊
                var loginMemberInfo = accounts.Count() > 1 ? accounts.ToArray()[1] : accounts.FirstOrDefault();

                //新增登入紀錄
                if (InsertLoginLog(data, loginMemberInfo, accounts) == null)
                    return null;
                else
                    result = SetLoginResponse(data.RequestSystem, loginMemberInfo);
            }
            //設定登入的cookie
            //FormsAuthentication.SetAuthCookie(result.ICanToken.Replace("-", ""), false);
            return result;
        }

        /// <summary>
        /// 登出資料處理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool LogoutProxy(UserTokenRequest data)
        {
            var tokenService = new TokenService();
            //判斷是否有遺漏資訊
            if (!PassLogoutDataCheck(data))
                return false;

            var accounts = GetSimulatorAccount(data.Account);
            //取得登入者資訊
            var memberInfo = accounts.Count() > 1 ? accounts.ToArray()[1] : accounts.FirstOrDefault();
            //是否本來為有效的登入
            var checkSuccess = CheckLoginSuccess(memberInfo.Id, null, data.Code.ToLower());
            if (checkSuccess == null)
                return false;
            //將過去的token設為失效
            tokenService.DeleteUserToken(memberInfo.Id, data.Code);
            return true;
        }

        /// <summary>
        /// 新增登入紀錄
        /// </summary>
        /// <param name="data"></param>
        /// <param name="loginMemberInfo"></param>
        /// <param name="simulators"></param>
        /// <returns></returns>
        public Infrastructure.Entity.UserToken InsertLoginLog(LoginRequest data, Member loginMemberInfo, IEnumerable<Member> simulators)
        {
            var tokenService = new TokenService();
            var db = _uow.DbContext;
            if (loginMemberInfo == null)
                return null;

            //var checkOrgLogin = (from ut in db.UserToken
            //                     join o in db.Organizations on ut.OrgId equals o.Id
            //                     orderby ut.Id descending
            //                     where ut.MemberId == loginMemberInfo.Id && 
            //                     o.OrgCode != "amateur" && 
            //                     ut.DeviceKey.ToLower()==data.RequestSystem.ToLower()
            //                     select ut).FirstOrDefault();
            //if (checkOrgLogin != null)
            //    return checkOrgLogin;
            //判斷是否為組織登入過
            var checkOrgLogin = db.UserToken.FirstOrDefault(t => t.MemberId == loginMemberInfo.Id && t.OrgId == loginMemberInfo.OrgId && t.RequestSystem.ToLower() == data.RequestSystem.ToLower());
            if (checkOrgLogin != null)
                return checkOrgLogin;
            try
            {
                if (simulators.Count() > 1)
                {
                    //tokenService.DeleteUserToken(loginMemberInfo.Id, data.RequestSystem, true);
                    var response = tokenService.InsertUserToken(data.RequestSystem, data.PhoneID, loginMemberInfo, data.PushToken, simulators.FirstOrDefault().Id);
                    return response;
                }
                else
                {
                    tokenService.DeleteUserToken(loginMemberInfo.Id, data.RequestSystem);
                    var response = tokenService.InsertUserToken(data.RequestSystem, data.PhoneID, loginMemberInfo, data.PushToken);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 虛擬登入
        /// </summary>
        /// <returns></returns>
        public async Task<Guid> SimulationLogin(Guid token, string loginAccount, string sessionId)
        {
            var memberService = new MemberService();
            var simulatorMember = await memberService.TokenToMember(token);

            if (simulatorMember != null)
            {
                var db = _uow.DbContext;

                var userTokenService = new TokenService();
                //判斷是否有資格模擬登入
                if (simulatorMember.IsOrgAdmin)
                {
                    var loginMember = db.Members.FirstOrDefault(t => t.Account == loginAccount);
                    var checkedToken = userTokenService.InsertUserToken("iCAN API Viewer", sessionId, loginMember, null, simulatorMember.Id);

                    return Guid.Parse(checkedToken.Token);
                }
                else
                    return Guid.Empty;
            }
            else
                return Guid.Empty;
        }


        /// <summary>
        /// 驗證是否為有效的登入
        /// </summary>
        /// <param name="deviceKey">登入裝置</param>
        /// <param name="loginMemberInfo">登入者資訊</param>
        /// <returns></returns>
        private LoginResponse SetLoginResponse(string requestSystem, Member loginMemberInfo, string code = null)
        {
            var db = _uow.DbContext;
            //驗證是否為有效的登入
            var checkSuccess = (requestSystem != null && requestSystem != string.Empty) ?
                CheckLoginSuccess(loginMemberInfo.Id, requestSystem.ToLower()) :
             CheckLoginSuccess(loginMemberInfo.Id, null, code.ToLower())
                ;
            if (checkSuccess == null)
                return null;
            else
            {
                int? nil = null;
                var result = new LoginResponse()
                {
                    AcpdId = loginMemberInfo.Account,
                    AcpdName = loginMemberInfo.Name,
                    Code = checkSuccess.DeviceKey,
                    Email = loginMemberInfo.Email,
                    ICanToken = checkSuccess.Token,
                    ShowMail = loginMemberInfo.IsShowEmail,
                    Photo = loginMemberInfo.Photo,
                    IsOrgAdmin = loginMemberInfo.IsOrgAdmin,
                    OrgId = loginMemberInfo.OrgId
                };
                var extraInfo = new LoginLearningMapBasic();

                //塞院系名稱
                if (loginMemberInfo.DeptId.HasValue && loginMemberInfo.DeptId >= 0)
                {
                    var deptInfo = db.Depts.Find(loginMemberInfo.DeptId.Value);
                    if (deptInfo != null)
                    {
                        extraInfo.Name_coll = deptInfo.Name;
                        result.ManColl = deptInfo.DeptCode;
                        result.CollName = deptInfo.Name;

                        if (loginMemberInfo.OrganizationRoleId.HasValue)
                        {
                            var organizationRoleInfo = (from or in db.OrganizationRole
                                                        join m in db.Members on or.Id equals m.OrganizationRoleId
                                                        where m.Id == loginMemberInfo.Id
                                                        select or).FirstOrDefault();
                            if (organizationRoleInfo != null && organizationRoleInfo.Level < 3)
                                result.CollBrief = organizationRoleInfo.Name;
                            else
                            //塞該學生的目前年級
                            if (loginMemberInfo.Grade.HasValue)
                            {
                                result.CollBrief = string.Format("{0}{1}年級", deptInfo.ShortName, loginMemberInfo.Grade.Value);
                                extraInfo.Stud_Grade = loginMemberInfo.Grade.Value;
                            }
                        }
                        else
                        {
                            result.CollBrief = "系統管理員";
                        }


                    }
                }
                extraInfo.ID_MainDoma = loginMemberInfo.IDMainDoma;
                extraInfo.Name_MainDoma = loginMemberInfo.NameMainDoma;
                if (loginMemberInfo.Grade.HasValue)
                    extraInfo.Stud_Grade = loginMemberInfo.Grade;
                //塞該學生的在學狀態
                if (loginMemberInfo.GraduationStatus.HasValue)
                {
                    switch (loginMemberInfo.GraduationStatus.Value)
                    {
                        case 10:
                            extraInfo.StudStatus = "在學";
                            break;
                        case 20:
                            extraInfo.StudStatus = "畢業";
                            break;
                        case 30:
                            extraInfo.StudStatus = "退學";
                            break;
                        case 40:
                            extraInfo.StudStatus = "休學";
                            break;
                        default:

                            break;
                    }
                }

                //塞該學生的入學年
                if (loginMemberInfo.SchoolRoll != null && loginMemberInfo.SchoolRoll != string.Empty)
                    extraInfo.Stud_SchlInYear = Convert.ToInt32(loginMemberInfo.SchoolRoll);
                //塞所屬班級的起始年
                if (loginMemberInfo.ClassGrade.HasValue)
                    extraInfo.Coll_SemeGrade = loginMemberInfo.ClassGrade.Value;

                //塞學制資料
                if (loginMemberInfo.SemesterGradeId.HasValue)
                {
                    var semesterGradeInfo = db.SemesterGrade.Find(loginMemberInfo.SemesterGradeId.Value);
                    if (semesterGradeInfo != null)
                    {
                        extraInfo.ID_syst = semesterGradeInfo.Code;
                        extraInfo.GradeYears = semesterGradeInfo.GradeYears;
                    }
                }

                //塞目前總共幾學期
                if (loginMemberInfo.OrgId > 0)
                {
                    var orgInfo = db.Organizations.Find(loginMemberInfo.OrgId);
                    if (orgInfo != null && orgInfo.SemesterLength.HasValue)
                        extraInfo.SemesterLength = orgInfo.SemesterLength.Value;
                }

                var sectionInfo = db.Sections.FirstOrDefault(t => t.IsNowSeme == true && t.OrgId == loginMemberInfo.OrgId);
                if (sectionInfo != null)
                {
                    extraInfo.cour_year = Convert.ToInt32(sectionInfo.FullName);
                    extraInfo.cour_seme = Convert.ToInt32(sectionInfo.Serial);
                }

                var checkIsTeacher = db.OrganizationRole.FirstOrDefault(t => t.Id == loginMemberInfo.OrganizationRoleId.Value);
                if (checkIsTeacher != null)
                    result.IsTeacher = checkIsTeacher.IsAdmin;
                else
                {

                    if (extraInfo.Stud_SchlInYear.HasValue &&
                        extraInfo.Stud_Grade.HasValue &&
                        (extraInfo.StudStatus != null || extraInfo.StudStatus != string.Empty) &&
                        (extraInfo.ID_syst != null && extraInfo.ID_syst != string.Empty) &&
                        (extraInfo.Coll_SemeGrade.HasValue))
                        result.IsTeacher = false;
                    else
                        result.IsTeacher = true;
                }
                var isSce = Convert.ToBoolean(ConfigurationManager.AppSettings["isSce"].ToString());

                //假設是文大
                if (isSce)
                {
                    var learningMapService = new SceLearningMapService();
                    var learningMapResponseData = learningMapService.GetData(checkSuccess.Token);
                    if (learningMapResponseData != null)
                    {
                        extraInfo.Coll_SemeGrade = learningMapResponseData.Coll_SemeGrade;
                        extraInfo.goalCredit = (learningMapResponseData.GradCredit.HasValue) ?
                            learningMapResponseData.GradCredit.Value :
                            0;
                        extraInfo.correspondCredit = (learningMapResponseData.Colls != null &&
                                                                                           learningMapResponseData.Colls.FirstOrDefault() != null) ?
                                                                                           learningMapService.GetCorrespondCredit(learningMapResponseData.Colls) :
                                                                                           0;
                    }

                }
                result.ManType = (loginMemberInfo.RoleName != null && loginMemberInfo.RoleName != string.Empty) ? Convert.ToInt32(loginMemberInfo.RoleName) : nil;
                result.ExtraInfo = extraInfo;

                return result;
            }
        }

        /// <summary>
        /// 確認目前登入者是否有效
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="phoneId"></param>
        /// <returns></returns>
        public Infrastructure.Entity.UserToken CheckLoginSuccess(int memberId, string requestSystem, string code = null)
        {
            var db = _uow.DbContext;
            if (requestSystem == null && code == null)
            {
                return null;
            }
            //驗證是否為有效的登入
            var checkSuccess = (requestSystem != null && requestSystem != string.Empty) ?
                db.UserToken.FirstOrDefault(t => t.MemberId == memberId && t.RequestSystem.ToLower() == requestSystem.ToLower()) :
                db.UserToken.FirstOrDefault(t => t.MemberId == memberId && t.DeviceKey.ToLower() == code.ToLower());
            return checkSuccess != null ? checkSuccess : null;
        }

        /// <summary>
        /// 取得登入 / 模擬帳號 [0] : 登入帳號 | [1] : 模擬帳號
        /// </summary>
        /// <param name="loginAccount"></param>
        /// <returns></returns>
        private IEnumerable<Infrastructure.Entity.Member> GetSimulatorAccount(string loginAccount, int? orgId = null)
        {
            var db = _uow.DbContext;
            var accounts = loginAccount.Split('@');
            var result = new List<Infrastructure.Entity.Member>();
            foreach (var account in accounts)
            {
                var member = orgId.HasValue ? db.Members.FirstOrDefault(t => t.Account.ToLower() == account.ToLower() && t.OrgId == orgId.Value) : db.Members.FirstOrDefault(t => t.Account.ToLower() == account.ToLower());
                result.Add(member);
            }
            return result;
        }
        /// <summary>
        /// 驗證密碼 -APP驗證
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private bool CheckAccountPwd(string account, string pwd)
        {
            var db = _uow.DbContext;
            encryptionService = new Encryption();
            var deCode3DES = encryptionService.DecryptString(pwd, appKey);
            var authPwd = encryptionService.StringToSHA256(string.Format("{0}{1}", deCode3DES, account));
            var result = db.Members.FirstOrDefault(t => t.Account.ToLower() == account && t.PassWord == authPwd);
            return result != null ? true : false;
        }

        /// <summary>
        /// 驗證密碼 -APP驗證
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private bool CheckAccountPwdForWeb(string account, string pwd)
        {
            var db = _uow.DbContext;
            encryptionService = new Encryption();
            var authPwd = encryptionService.StringToSHA256(string.Format("{0}{1}", pwd, account));
            var result = db.Members.FirstOrDefault(t => t.Account == account && t.PassWord == authPwd);
            return result != null ? true : false;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string[] AccountAuthCheck(string account, string password)
        {
            var db = _uow.DbContext;
            encryptionService = new Encryption();
            var enCode3Des = encryptionService.EncryptString(password, appKey);
            var deCode3DES = encryptionService.DecryptString(enCode3Des, appKey);
            var authPwd = encryptionService.StringToSHA256(string.Format("{0}{1}", password, account));
            var result = db.Members.FirstOrDefault(t => t.Account == account && t.PassWord == authPwd);

            return new string[3] { account, enCode3Des, result.Photo };
        }

        /// <summary>
        /// 組織登入API
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <param name="deviceId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<bool> OrganizationLoginCheck(string loginUrl, LoginRequest data, int orgId)
        {
            var db = _uow.DbContext;
            var response = false;
            var loginColumnKeys = db.OrganizationLoginColumn.Where(t => t.OrgId == orgId);
            var checkColumn = loginColumnKeys.FirstOrDefault();
            if (checkColumn == null)
                return response;

            var contentJson = string.Empty;
            var tokenInfo = new Infrastructure.ViewModel.Base.BackendBaseRequest();
            var schoolLoginKey = ConfigurationManager.AppSettings["iCanLoginKey"].ToString();
            var appLoginKey = ConfigurationManager.AppSettings["AppLoginKey"].ToString();
            var encryptionService = new Encryption();
            var accounts = data.Account.Split('@');
            var account = accounts[0];
            var simulateAccount = accounts.Count() > 1 ? accounts[1] : account;
            //有模擬登入者，需要驗證模擬登入者是否為系統管理者
            if (accounts.Count() > 1)
            {
                var checkSimulatorAuth = db.Members.FirstOrDefault(t => t.Account == account).RoleName == "1";
                if (checkSimulatorAuth == false)
                    return false;
            }


            using (var httpClient = new HttpClient())
            {

                var password = encryptionService.EncryptString(encryptionService.DecryptString(data.Password, appLoginKey), schoolLoginKey);
                if (checkColumn.Method.ToLower() == "post")
                {
                    var jsonData = "{";
                    foreach (var column in loginColumnKeys)
                    {
                        switch (column.Type.ToLower())
                        {
                            case "account":
                                jsonData = string.Format("{0}\"{1}\":\"{2}\",", jsonData, column.ColumnKey, account);
                                break;
                            case "pwd":
                                jsonData = string.Format("{0}\"{1}\":\"{2}\",", jsonData, column.ColumnKey, password);
                                break;
                            case "devicekey":
                                jsonData = string.Format("{0}\"{1}\":\"{2}\",", jsonData, column.ColumnKey, data.PhoneID);
                                break;
                            case "simulate":
                                jsonData = string.Format("{0}\"{1}\":\"{2}\",", jsonData, column.ColumnKey, simulateAccount);
                                break;
                            default:
                                break;
                        }
                    }
                    jsonData = jsonData.Substring(0, jsonData.Length - 1);
                    httpClient.BaseAddress = new Uri(loginUrl);
                    jsonData += "}";
                    var request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
                    request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var responseContent = httpClient.SendAsync(request);
                    if (responseContent.Result.IsSuccessStatusCode)
                    {
                        var responseString = responseContent.Result.Content.ReadAsStringAsync().Result;
                        tokenInfo = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(responseString);
                    }
                }
                else if (checkColumn.Method.ToLower() == "get")
                {
                    loginUrl += "?";
                    foreach (var column in loginColumnKeys)
                    {
                        switch (column.Type.ToLower())
                        {
                            case "account":
                                loginUrl = string.Format("{0}{1}={2}&", loginUrl, column.ColumnKey, account);
                                break;
                            case "pwd":
                                loginUrl = string.Format("{0}{1}={2}&", loginUrl, column.ColumnKey, password);
                                break;
                            case "devicekey":
                                loginUrl = string.Format("{0}{1}={2}&", loginUrl, column.ColumnKey, data.PhoneID);
                                break;
                            case "simulate":
                                loginUrl = string.Format("{0}{1}={2}&", loginUrl, column.ColumnKey, simulateAccount);
                                break;
                            default:
                                break;
                        }
                    }
                    loginUrl = loginUrl.Substring(0, loginUrl.Length - 1);
                    var responseContent = await httpClient.GetAsync(loginUrl);
                    var responseString = responseContent.Content.ReadAsStringAsync().Result;
                    tokenInfo = JsonConvert.DeserializeObject<BackendBaseRequest>(responseString);
                }

                response = tokenInfo.Success;
                if (tokenInfo.Success)
                {
                    var tokenService = new TokenService();
                    var memberService = new MemberService();
                    var orgToken = tokenInfo.Token != null && tokenInfo.Token != string.Empty ? tokenInfo.Token : Guid.NewGuid().ToString().ToLower();
                    var loginMemberInfo = data.OrgId.HasValue ?
                                                                 db.Members.FirstOrDefault(t => t.OrgId == data.OrgId.Value && t.Account == simulateAccount) :
                                                                 db.Members.FirstOrDefault(t => t.Account == simulateAccount);
                    var loginLogResponse = tokenService.InsertUserTokenByOrganization(data.RequestSystem, data.PhoneID, loginMemberInfo, data.PushToken, orgToken, loginMemberInfo.Id);
                }
            }
            return response;
        }

        /// <summary>
        /// 檢查帳號是否合法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool AccountCheck(LoginRequest data)
        {
            var db = _uow.DbContext;
            var isOrgLogin = false;
            var accounts = data.Account.Split('@');
            var account = accounts[0];
            var isSimulateLogin = accounts.Count() > 1;
            var simulateAccount = isSimulateLogin ? accounts[1] : account;
            var members = data.OrgId.HasValue ? db.Members.Where(t => t.Account == simulateAccount && t.OrgId == data.OrgId.Value) : db.Members.Where(t => t.Account == simulateAccount);

            var memberInfo = data.OrgId.HasValue ? members.FirstOrDefault(t => t.OrgId == data.OrgId.Value) : (from m in db.Members join org in db.Organizations on m.OrgId equals org.Id where org.Id == members.FirstOrDefault().OrgId && m.Id == members.FirstOrDefault().Id select m).FirstOrDefault();
            if (memberInfo == null)
                return false;
            //模擬登入
            if (isSimulateLogin)
            {
                var checkSimulatorAuth = db.Members.FirstOrDefault(t => t.Account == account).RoleName == "1";
                if (checkSimulatorAuth == false)
                    return false;
            }
            else
            {
                //判斷該組織是否有登入API
                var checkOrgHasLoginAPI = (from exr in db.ExtResources
                                           join ext in db.ExtResTypes on exr.ExternalResTypeId equals ext.Id
                                           where ext.AsyncTypeCode == "loginAuth" &&
                                                        exr.OrgId == memberInfo.OrgId
                                           select exr).FirstOrDefault();
                isOrgLogin = (checkOrgHasLoginAPI != null && (memberInfo.ExternalRid != null));

                //使用對方組織的登入機制
                if (isOrgLogin)
                {
                    var orgLoginResponse = OrganizationLoginCheck(checkOrgHasLoginAPI.Uri, data, memberInfo.OrgId);
                    return orgLoginResponse.Result;
                }
            }
            //驗證密碼
            return CheckAccountPwd(accounts[0], data.Password);
        }

        /// <summary>
        /// 設定登入資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="simulateAccount"></param>
        public SessionData SetMemberSession(Guid token, string simulateAccount = null)
        {
            var db = _uow.DbContext;
            var memberService = new MemberService();
            var loginMember = memberService.TokenToMember(token);
            var courseService = new CourseService();
            var departmentAdminId = new List<Dept>();
            if (loginMember != null)
            {
                departmentAdminId = courseService.DeptAdminList(loginMember.Result.Id);
                // entity framework
                var dbInfo = db.Members.Find(loginMember.Result.Id);
                var sessionData = new SessionData
                {
                    LoginAccount = dbInfo.Account,
                    LoginName = dbInfo.Name,
                    LoginMemberId = dbInfo.Id,
                    SimulateAccount = simulateAccount,
                    OrgId = dbInfo.OrgId,
                    Token = token,
                    IsOrgAdmin = dbInfo.IsOrgAdmin,
                    DeptAdminList = departmentAdminId
                };
                return sessionData;
            }
            return null;
        }
        /// <summary>
        /// 檢查登入資料中是不是有空值存在
        /// </summary>
        /// <param name="data">登入資訊</param>
        /// <returns>true:資料驗證通過</returns>
        public bool PassLoginDataCheck(LoginRequest data)
        {
            if (string.IsNullOrEmpty(data.Account) ||
                string.IsNullOrEmpty(data.RequestSystem) ||
                string.IsNullOrEmpty(data.Password) ||
                string.IsNullOrEmpty(data.PhoneID))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 檢查登出資料中是不是有空值存在
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool PassLogoutDataCheck(UserTokenRequest data)
        {
            if (string.IsNullOrEmpty(data.Account) ||
                string.IsNullOrEmpty(data.Code) ||
                string.IsNullOrEmpty(data.ICanToken))
                return false;
            return true;
        }
        public bool LogOut(string token)
        {
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            var deleteToken = tokenService.DeleteUserToken(tokenInfo.MemberId, tokenInfo.RequestSystem);
            return deleteToken;
        }
    }
}
