using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.Utility;

namespace WiicoApi.Service.Backend
{
    public class MemberInviteService
    {
        private readonly GenericUnitOfWork _uow = null;

        public MemberInviteService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得課程的邀請狀態
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="inviteType">查詢邀請對象 0: 成員 | 1 : 管理者</param>
        /// <returns></returns>
        public bool GetLearningInviteStatus(string token, string circleKey, int inviteType)
        {
            var db = _uow.DbContext;
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(token).Result;
            if (tokenInfo == null)
                return false;
            var authService = new AuthService();
            if (authService.CheckCourseTeacher(tokenInfo.MemberId, circleKey.ToLower()) || authService.CheckCourseAdmin(tokenInfo.MemberId, circleKey.ToLower()))
            {
                var learningCircleService = new LearningCircleService();
                var learningCircleinfo = learningCircleService.GetDetailByOuterKey(circleKey.ToLower());
                if (learningCircleinfo == null)
                    return false;
                return inviteType == 0 ? learningCircleinfo.InviteEnable : learningCircleinfo.AdminInviteEnable;
            }
            return false;
        }

        /// <summary>
        /// 修改課程的邀請狀態
        /// </summary>
        /// <param name="token"></param>
        /// <param name="circleKey"></param>
        /// <param name="inviteType">修改邀請對象 0: 成員 | 1 : 管理者</param>
        /// <returns></returns>
        public bool UpdateLearningInviteStatus(string token, string circleKey, int inviteType)
        {
            var oldStatus = GetLearningInviteStatus(token, circleKey.ToLower(), inviteType);
            var newStatus = !oldStatus;
            var updateData = _uow.DbContext.LearningCircle.FirstOrDefault(t => t.LearningOuterKey.ToLower() == circleKey.ToLower());
            if (updateData == null)
                return false;

            switch (inviteType)
            {
                case 0:
                    updateData.InviteEnable = newStatus;
                    break;
                case 1:
                    updateData.AdminInviteEnable = newStatus;
                    break;
                default:
                    break;
            }
            _uow.SaveChanges();
            return newStatus;

        }

        public MemberInvite GetDetail(string code, string email = null, string circleKey = null, int? resType = null)
        {
            //是否有email與circleKey條件
            var responseData = (email != null && circleKey != null && resType.HasValue) ?
                //如果有 
                _uow.MemberInviteRepo.GetFirst(t => t.InviteEmail.ToLower() == email && t.CircleKey.ToLower() == circleKey && t.Type == resType.Value) :
                //沒有
                _uow.MemberInviteRepo.GetFirst(t => t.Code.ToLower() == code.ToLower());
            return responseData;
        }
        /// <summary>
        /// 取得課程永久邀請碼
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public MemberInvite GetDetailByCircleKey(string circleKey)
        {
            var responseData = _uow.MemberInviteRepo.GetFirst(t => t.CircleKey == circleKey && t.IsCourseCode == true && t.Enable == true);
            return responseData;
        }

        public List<InviteRecordGetResponse> GetInviteAllList(MemberInvitePostRequest requestData)
        {
            var db = _uow.DbContext;
            var responseData = new List<InviteRecordGetResponse>();
            var joinData = (from m in db.Members
                            join cmr in db.CircleMemberRoleplay on m.Id equals cmr.MemberId
                            join lr in db.LearningCircle on cmr.CircleId equals lr.Id
                            where lr.LearningOuterKey == requestData.CircleKey
                            select new { m, cmr }).ToList();
            for (var resType = 1; resType <= 3; resType++)
            {
                var data = new InviteRecordGetResponse() { ResType = resType };

                switch (resType)
                {
                    case 1: //邀請碼加入 ,
                        var inviteCodeJoin = joinData.Where(t => t.cmr.ResType == resType);
                        if (inviteCodeJoin.FirstOrDefault() == null)
                            data.UseAccounts = new List<string>();
                        else
                            data.UseAccounts = inviteCodeJoin.Select(t => t.m.Account).ToList();
                        data.UnJoinAccounts = new List<string>();
                        break;
                    case 2: //邀請連結加入
                        var urlJoin = joinData.Where(t => t.cmr.ResType == resType);
                        if (urlJoin.FirstOrDefault() == null)
                            data.UseAccounts = new List<string>();
                        else
                            data.UseAccounts = urlJoin.Select(t => t.m.Account).ToList();
                        data.UnJoinAccounts = new List<string>();
                        break;
                    case 3: // email加入
                        var emailJoin = joinData.Where(t => t.cmr.ResType == resType);
                        if (joinData.FirstOrDefault() == null)
                            data.UseAccounts = new List<string>();
                        else
                            data.UseAccounts = emailJoin.Select(t => t.m.Account).ToList();
                        data.UnJoinAccounts = db.MemberInvite.Where
                                                                    (t => t.CircleKey.ToLower() == requestData.CircleKey.ToLower() &&
                                                                    t.Enable == true &&
                                                                    t.IsCourseCode == false &&
                                                                    t.Type == requestData.InviteType).
                                                                    Select(t => t.InviteEmail).ToList();
                        break;
                    default:
                        break;
                }
                responseData.Add(data);
            }
            return responseData;
        }

        /// <summary>
        ///  取得列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="inviteType">0:成員 | 1:管理者</param>
        /// <param name="isJoin">null代表全部查詢</param>
        /// <returns></returns>
        public IEnumerable<MemberInvite> GetList(string circleKey, int inviteType, bool? isJoin)
        {
            var responseData = _uow.MemberInviteRepo.GetList(circleKey, isJoin, inviteType);
            return responseData;
        }
        /// <summary>
        /// 根據新增學習圈建立驗證碼
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public bool CreateFromCreateLearningCircle(string circleKey)
        {
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circleKey.ToLower());
            if (learningCircleInfo == null)
                return false;
            var codeService = new CaptchaHelper();
            var code = codeService.GenerateRandomText(10);
            var checkHadCode = _uow.MemberInviteRepo.GetFirst(t => t.CircleKey.ToLower() == circleKey.ToLower() && t.Enable == true && t.IsCourseCode == true);
            if (checkHadCode == null)
            {
                var entity = new MemberInvite()
                {
                    CircleKey = circleKey.ToLower(),
                    Code = code.ToLower(),
                    CreateDate = DateTime.UtcNow,
                    Enable = true,
                    InviteEmail = null,
                    InviteUrl = null,
                    IsCourseCode = true,
                    Type = 0
                };
                _uow.DbContext.MemberInvite.Add(entity);
                _uow.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// 建立邀請碼資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public bool Create(MemberInvitePostRequest requestData)
        {
            var inviteEmails = requestData.InviteEmail.ToList();
            if (inviteEmails.FirstOrDefault() == null)
                return false;

            var db = _uow.DbContext;
            var captchaHelp = new Utility.CaptchaHelper();
            var mails = new List<MemberInvite>();
            var learningCircleService = new LearningCircleService();
            var learningCircleInfo = learningCircleService.GetDetailByOuterKey(requestData.CircleKey);
            if (learningCircleInfo == null)
                return false;

            var organizationInfo = db.Organizations.FirstOrDefault(t => t.Id == learningCircleInfo.OrgId);
            var flipusOrgName = ConfigurationManager.AppSettings["FlipusOrgName"].ToString();
            var inviteDomain = ConfigurationManager.AppSettings["loginServer"].ToString();

            foreach (var inviteEmail in inviteEmails)
            {
                #region 將舊的code 設定失效
                var setOldInviteCode = db.MemberInvite.Where(t => t.InviteEmail == inviteEmail && t.CircleKey == requestData.CircleKey && t.Enable == true);
                if (setOldInviteCode != null && setOldInviteCode.Count() > 0)
                {
                    foreach (var oldInvite in setOldInviteCode)
                    {
                        oldInvite.Enable = false;
                    }
                }
                #endregion

                var inviteCode = captchaHelp.GenerateRandomText(10).ToLower();
                var inviteUrl = HttpUtility.UrlEncode(string.Format("{3}/FlipusWeb/invite?r=3&o={0}&c={1}&t={2}&on={3}", organizationInfo.OrgCode, inviteCode, requestData.InviteType, inviteDomain, HttpUtility.UrlEncode(organizationInfo.Name)));

                var entity = new MemberInvite()
                {
                    CircleKey = requestData.CircleKey.ToLower(),
                    Code = inviteCode,
                    CreateDate = DateTime.UtcNow,
                    Enable = true,
                    InviteEmail = inviteEmail,
                    IsCourseCode = false,
                    Type = requestData.InviteType,
                    InviteUrl = inviteUrl
                };
                db.MemberInvite.Add(entity);
                mails.Add(entity);
            } //結束新增
            try
            {
                //開始跑db
                db.SaveChanges();

                var emailDomain = ConfigurationManager.AppSettings["MailDomain"].ToString();
                var emailSMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailSMTPPort"].ToString());
                var emailAdminAddress = ConfigurationManager.AppSettings["MailAdminAddress"].ToString();
                var mailService = new MailService();

                var tokenService = new TokenService();
                var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
                var teacherInfo = db.Members.Find(tokenInfo.MemberId);
                var memberService = new MemberService();
                var teachers = memberService.GetTeacherList(learningCircleInfo.LearningOuterKey);
                var teachersName = string.Empty;
                foreach (var teacher in teachers)
                {
                    teachersName = string.Format("{0}、{1}", teachersName, teacher.Name);
                }
                teachersName = teachersName.Substring(1, teachersName.Length - 1);
                var weekTableService = new WeekTableService();
                var weekTableDatas = weekTableService.GetByCirclekey(learningCircleInfo.LearningOuterKey);
                var weekTableTimes = string.Empty;
                foreach (var weekTableData in weekTableDatas.WeekTable)
                {
                    weekTableTimes = string.Format("{0}、({1}){2:HH:mm} ~ {3:HH:mm}", weekTableTimes, weekTableData.Week, weekTableData.StartTime, weekTableData.EndTime);
                }
                weekTableTimes = weekTableTimes.Substring(1, weekTableTimes.Length - 1);

                //送邀請函信
                foreach (var code in mails)
                {
                    var mailContent = flipusOrgName == organizationInfo.OrgCode.ToString()
                                                                                ?
                                                                                //跑Flipus 非組織信字串
                                                                                string.Format("<tbody><tr><td align = \"left\" style = \"padding-top: 15px;\"></td></tr>" +
                                                                                 "<img src=\"http://scedev.eastus.cloudapp.azure.com/iThink/images/inviteimg/logo.png\"  alt = \"logo\" width = \"108\" height = \"48\"  />" +
                                                                                 "<tr><td align=\"left\" style=\"padding - top: 15px; \"><h2>您好!</h2>" +
                                                                                 "{0} 老師 邀請您一起加入，Flipus 翻轉校園學習圈的「{1}」，現在就與我們一同體驗最有溫度的學習互動！<br><br><br>" +
                                                                                 "請點擊以下連結，來完成加入課程的動作：<br>" +
                                                                                 "<a href=\"#\" style=\"color: #5bc289;\">{2}</a><br><br><br>" +
                                                                                 "課程名稱：{1}<br>" +
                                                                                 "適用對象：一般學員<br>" +
                                                                                 "授課老師：{7}<br>" +
                                                                                 "開課日期：{3:yyyy/MM/dd}~{4:yyyy/MM/dd}<br>" +
                                                                                 "上課時段：{5}<br>" +
                                                                                 "課程簡介：{6}" +
                                                                                 "</td></tr>" +
                                                                                 "<tr><td align = \"left\" style = \"padding-top: 30px; font-size: 13px; color: #868b8f;\" >" +
                                                                                 "<hr style = \"margin-bottom: 25px; border: 0; background-color: #ccc; height: 1px;\">" +
                                                                                 "※ 課程日期或上課時段等相關內容若有異動，請以實際課程資訊為主。<br>" +
                                                                                 "※ 此信件是由 Flipus 系統自動產生與寄出，請勿直接回覆。<br>" +
                                                                                 "若需要我們協助的問題，請透過此 <a href = \"mailto:app@g.sce.pccu.edu.tw\" style = \"color: #5bc289;\" > app@g.sce.pccu.edu.tw</a> 與我們聯繫，謝謝您！<br>" +
                                                                                 "<a href = \"http://scedev.eastus.cloudapp.azure.com/iThink/images/inviteimg/footer_banner.png\" title = \"flipus翻轉校園學習圈\" ><img src = \"http://scedev.eastus.cloudapp.azure.com/iThink/images/inviteimg/footer_banner.png\" alt = \"flipus翻轉校園學習圈\" width = \"600\" height = \"100\" style = \"margin-top: 15px;\"></a>" +
                                                                                 "<hr style = \"margin-top: 25px; border: 0; background-color: #ccc; height: 1px;\" ></td></tr><tr> " +
                                                                                 "<td align = \"left\" style = \"padding-top: 15px; font-size: 13px;\" >© 中國文化大學推廣教育部 All Rights Reserved.</td>" +
                                                                                 "</tr></tbody>",
                                                                                 teacherInfo.Name,
                                                                                 learningCircleInfo.Name,
                                                                                 code.InviteUrl,
                                                                                 learningCircleInfo.StartDate.Value.ToLocalTime(),
                                                                                 learningCircleInfo.EndDate.Value.ToLocalTime(),
                                                                                 weekTableTimes,
                                                                                 learningCircleInfo.Description,
                                                                                 teachersName)
                                                                                 :
                                                                                 //跑組織信字串
                                                                                 string.Format("<tbody><tr><td align = \"left\" style = \"padding-top: 15px;\"></td></tr>" +
                                                                                 "<img src=\"http://scedev.eastus.cloudapp.azure.com/iThink/images/inviteimg/logo.png\"  alt = \"logo\" width = \"108\" height = \"48\"  />" +
                                                                                 "<tr><td align=\"left\" style=\"padding - top: 15px; \"><h2>您好!</h2>" +
                                                                                 "{0} 老師 邀請您一起加入，{8}組織專屬課程 -「{1}」，現在就與我們一同體驗最有溫度的學習互動！<br><br><br>" +
                                                                                 "請點擊以下連結，來完成加入課程的動作：<br>" +
                                                                                 "<a href=\"#\" style=\"color: #5bc289;\">{2}</a><br><br><br>" +
                                                                                 "課程名稱：{1}<br>" +
                                                                                 "適用對象：{8}組織學員<br>" +
                                                                                 "授課老師：{7}<br>" +
                                                                                 "開課日期：{3:yyyy/MM/dd}~{4:yyyy/MM/dd}<br>" +
                                                                                 "上課時段：{5}<br>" +
                                                                                 "課程簡介：{6}" +
                                                                                 "</td></tr>" +
                                                                                 "<tr><td align = \"left\" style = \"padding-top: 30px; font-size: 13px; color: #868b8f;\" >" +
                                                                                 "<hr style = \"margin-bottom: 25px; border: 0; background-color: #ccc; height: 1px;\">" +
                                                                                 "※ 課程日期或上課時段等相關內容若有異動，請以實際課程資訊為主。<br>" +
                                                                                 "※ 此信件是由 Flipus 系統自動產生與寄出，請勿直接回覆。<br>" +
                                                                                 "若需要我們協助的問題，請透過此 <a href = \"mailto:app@g.sce.pccu.edu.tw\" style = \"color: #5bc289;\" > app@g.sce.pccu.edu.tw</a> 與我們聯繫，謝謝您！<br>" +
                                                                                 "<a href = \"http://scedev.eastus.cloudapp.azure.com/iThink/images/inviteimg/footer_banner.png\" title = \"flipus翻轉校園學習圈\" ><img src = \"http://scedev.eastus.cloudapp.azure.com/iThink/images/inviteimg/footer_banner.png\" alt = \"flipus翻轉校園學習圈\" width = \"600\" height = \"100\" style = \"margin-top: 15px;\"></a>" +
                                                                                 "<hr style = \"margin-top: 25px; border: 0; background-color: #ccc; height: 1px;\" ></td></tr><tr> " +
                                                                                 "<td align = \"left\" style = \"padding-top: 15px; font-size: 13px;\" >© 中國文化大學推廣教育部 All Rights Reserved.</td>" +
                                                                                 "</tr></tbody>",
                                                                                 teacherInfo.Name,
                                                                                 learningCircleInfo.Name,
                                                                                 code.InviteUrl,
                                                                                 learningCircleInfo.StartDate.Value.ToLocalTime(),
                                                                                 learningCircleInfo.EndDate.Value.ToLocalTime(),
                                                                                 weekTableTimes,
                                                                                 learningCircleInfo.Description,
                                                                                 teachersName,
                                                                                 organizationInfo.Name);

                    mailService.SendMail(emailDomain, emailSMTPPort, emailAdminAddress, new List<string>() { code.InviteEmail }, mailContent, string.Format("Flipus 邀請您加入「{0}」", learningCircleInfo.Name), null);
                }
                if (requestData.CC)
                    mailService.SendMail(emailDomain, emailSMTPPort, emailAdminAddress, new List<string>() { teacherInfo.Email }, "已發送邀請碼完成", string.Format("[{0}] 課程邀請碼", learningCircleInfo.Name), null);

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public IEnumerable<MemberInvite> Update(MemberInvitePutRequest requestData)
        {
            var circleKey = requestData.CircleKey;
            var isUpdateCourseCode = requestData.CircleKey != null;
            //更新課程邀請碼
            if (isUpdateCourseCode)
            {
                var courseCode = GetDetailByCircleKey(requestData.CircleKey);
                if (courseCode != null)
                    courseCode.Enable = false;

                var captchaHelp = new Utility.CaptchaHelper();
                var inviteCode = captchaHelp.GenerateRandomText(10).ToLower();
                var entity = new MemberInvite()
                {
                    CircleKey = requestData.CircleKey.ToLower(),
                    Code = inviteCode,
                    CreateDate = DateTime.UtcNow,
                    Enable = true,
                    IsCourseCode = true,
                    Type = 0
                };
                _uow.DbContext.MemberInvite.Add(entity);

            }
            else
            {//更新特定邀請碼
                var inviteInfo = GetDetail(requestData.InviteCode.ToLower());
                if (inviteInfo == null)
                    return null;
                circleKey = inviteInfo.CircleKey;
                inviteInfo.Enable = requestData.Enable;
            }
            _uow.SaveChanges();
            var responseData = GetList(circleKey, 0, null);
            return responseData;
        }
    }
}
