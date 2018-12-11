using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class TokenService
    {
        private readonly GenericUnitOfWork _uow;

        public TokenService()
        {
            _uow = new GenericUnitOfWork();
        }
        public TokenService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }
        public TokenService(string connecitonName)
        {
            _uow = new GenericUnitOfWork(connecitonName);
        }
        public UserToken GetTokenByMemberId(int memberId)
        {
            var db = _uow.DbContext;
            var data = (from ut in db.UserToken
                        join m in db.Members on ut.MemberId equals m.Id
                        where ut.MemberId == memberId && ut.Enable == true
                        select ut).OrderByDescending(t => t.Created.Utc).FirstOrDefault();

            return data;
        }
        /// <summary>
        /// 根據來源取得token資訊
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<UserToken> GetTokenInfo(string token)
        {
            var guidToken = Guid.NewGuid();
            var tryGuidFormat = Guid.TryParse(token, out guidToken);
            if (tryGuidFormat)
            {
                //先取得iCan6Token
                var checkToken = GetiCan6Token(token);
                if (checkToken == null)
                {
                    /*var ican5Token = await GetiCan5TokenByAPI(token);
                     if (ican5Token == null)
                            ican5Token = _uow.UserTokensRepo.GetiCan5Token(token);
                        
                    checkToken = ican5Token != null ? ican5Token : null;*/
                    return null;
                }
                return checkToken;
            }
            return null;
        }

        public UserToken GetiCan6TokenByAccount(string account)
        {
            var db = _uow.DbContext;
            var data = (from ut in db.UserToken
                        join m in db.Members on ut.MemberId equals m.Id
                        where m.Account == account && ut.Enable == true
                        select ut).OrderByDescending(t => t.Created.Utc).FirstOrDefault();

            return data;
        }

        /// <summary>
        /// 透過API驗證iCan5Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<UserToken> GetiCan5TokenByAPI(string token)
        {
            var checkTokenUrl = ConfigurationManager.AppSettings["ican-checktoken-url"].ToString();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-Token", token);
                var response = httpClient.GetAsync(checkTokenUrl);
                var account = await response.Result.Content.ReadAsStringAsync();
                if (account == "NULL" || account == null || account == string.Empty || account == "")
                    return null;

                var db = _uow.DbContext;
                var memberInfo = db.Members.FirstOrDefault(t => t.Account == account);
                if (memberInfo == null)
                    return null;

                var result = new Infrastructure.Entity.UserToken()
                {
                    Enable = true,
                    MemberId = memberInfo.Id,
                    OrgId = memberInfo.OrgId,
                    Token = token,
                };
                return result;
            }
        }
   
        /// <summary>
        /// 取得iCan6的token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserToken GetiCan6Token(string token)
        {
            var db = _uow.DbContext;
            var result = db.UserToken.FirstOrDefault(t => t.Token.ToLower() == token.ToLower() && t.Enable == true);
            return result != null ? result : null;
        }

        /// <summary>
        /// 新增登入紀錄
        /// </summary>
        /// <param name="requestSystem">裝置型號</param>
        /// <param name="phoneId">裝置代碼</param>
        /// <param name="loginMemberId">登入者編號</param>
        /// <param name="simulatorId">模擬登入者編號</param>
        /// <returns></returns>
        public UserToken InsertUserTokenByOrganization(string requestSystem, string phoneId, Member loginMemberInfo, string pushToken, string orgToken, int? simulatorId = null)
        {
            return _uow.UserTokensRepo.InsertUserTokenByOrganization(requestSystem, phoneId, loginMemberInfo, pushToken, orgToken, simulatorId);
        }

        /// <summary>
        /// 新增登入紀錄
        /// </summary>
        /// <param name="requestSystem">裝置型號</param>
        /// <param name="phoneId">裝置代碼</param>
        /// <param name="loginMemberId">登入者編號</param>
        /// <param name="simulatorId">模擬登入者編號</param>
        /// <returns></returns>
        public UserToken InsertUserToken(string requestSystem, string phoneId, Member loginMemberInfo, string pushToken, int? simulatorId = null)
        {
            return _uow.UserTokensRepo.InsertUserToken(requestSystem, phoneId, loginMemberInfo, pushToken, simulatorId);
        }

        /// <summary>
        /// 刪除token紀錄
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public bool DeleteUserToken(int memberId, string requestSystem, bool? isSimulator = false)
        {
            try
            {
                var checkDevice = requestSystem.Split(';');

                var db = _uow.DbContext;
                //查詢過去登入紀錄
                if (isSimulator == false)
                {
                    var requestStartText = checkDevice[0].ToLower();
                    if (checkDevice.Count() > 1 && !requestStartText.ToLower().StartsWith("desktop"))
                    {
                        var loginLog = db.UserToken.Where
                                                    (t => t.MemberId == memberId &&
                                                    (t.RequestSystem.ToLower().StartsWith("ios") ||
                                                        t.RequestSystem.ToLower().StartsWith("android"))
                                                        );

                        if (loginLog.FirstOrDefault() != null)
                        {
                            db.UserToken.RemoveRange(loginLog);
                            db.SaveChanges();
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetPushCountClear(string token)
        {

            var tokenInfo = GetTokenInfo(token).Result;
            if (tokenInfo == null)
                return false;
            var tokenList = _uow.DbContext.UserToken.Where(t => t.MemberId == tokenInfo.MemberId);
            var pushLogs = (from pl in _uow.DbContext.PushLog
                            join ut in tokenList on pl.DeviceId equals ut.PushToken
                            where pl.Enable == false
                            select pl).ToList();
            foreach (var pushlog in pushLogs)
            {
                pushlog.Enable = true;
            }
            try
            {
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// 更新pushToken
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pushToken"></param>
        /// <returns></returns>
        public bool UpdateMemberPushToken(string account, string pushToken)
        {
            var db = _uow.DbContext;
            var getToken = (from ut in db.UserToken
                            join m in db.Members on ut.MemberId equals m.Id
                            where m.Account == account
                            orderby ut.Id descending
                            select ut).FirstOrDefault();
            if (getToken == null)
                return false;
            try
            {
                getToken.PushToken = pushToken;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
    }
}
