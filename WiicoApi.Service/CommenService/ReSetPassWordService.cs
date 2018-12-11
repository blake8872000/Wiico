using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Repository;
using WiicoApi.Service.Utility;

namespace WiicoApi.Service.CommenService
{
    public class ReSetPassWordService
    {

        private readonly GenericUnitOfWork _uow;
        public ReSetPassWordService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 重設密碼 - 根據email
        /// </summary>
        /// <param name="code">client所輸入的驗證碼</param>
        /// <param name="checkCode">系統所產生的驗證碼</param>
        /// <param name="email">client所輸入的驗證信箱</param>
        /// <returns></returns>
        public bool Do(string code, string checkCode, string email)
        {
            var memberService = new MemberService();
            var checkeMail = _uow.MembersRepo.GetFirst(t => t.Email == email);

            //驗證該信箱是否註冊過
            if (checkeMail == null)
                return false;
            //驗證碼是否輸入正確
            if (code.ToLower() != checkCode.ToLower())
                return false;

            var captchaHelper = new Utility.CaptchaHelper();
            var randomPwd = captchaHelper.GenerateRandomText(10).ToLower();
            var encryptionService = new Utility.Encryption();
            //設定新密碼
            var newPassWord = encryptionService.StringToSHA256(string.Format("{0}{1}", randomPwd, checkeMail.Account));

            checkeMail.PassWord = newPassWord;
            _uow.SaveChanges();
            var mailService = new MailService();
            var emailDomain = ConfigurationManager.AppSettings["MailDomain"].ToString();
            var emailAdminAddress = ConfigurationManager.AppSettings["MailAdminAddress"].ToString();
            var emailSMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailSMTPPort"].ToString());
            //收信者
            var recipient = new List<string>() { email };
            var msg = string.Format("{0}您好!!您的密碼是{1}", checkeMail.Name, randomPwd);
            var sendMail = mailService.SendMail(emailDomain, emailSMTPPort, emailAdminAddress, recipient, msg, "Locus重設密碼", null).Result;
            return sendMail;
        }

        /// <summary>
        /// 重設密碼 - 根據使用者token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="code"></param>
        /// <param name="checkCode"></param>
        /// <param name="reSetPassWord"></param>
        /// <returns></returns>
        public bool DoByToken(string token, string oldPassWord, string reSetPassWord)
        {
            var appKey= ConfigurationManager.AppSettings["AppLoginKey"].ToString();
            var db = _uow.DbContext;
            var memberInfo = (from m in db.Members
                              join us in db.UserToken on m.Id equals us.MemberId
                              where us.Token.ToLower() == token.ToLower()
                              select m).FirstOrDefault();
            if (memberInfo == null)
                return false;

            var encryptionService = new Utility.Encryption();
            oldPassWord = encryptionService.DecryptString(oldPassWord, appKey).ToLower();
            reSetPassWord = encryptionService.DecryptString(reSetPassWord, appKey).ToLower();
            var oldPassWordEncode = encryptionService.StringToSHA256(string.Format("{0}{1}", oldPassWord, memberInfo.Account.ToLower()));
            if (memberInfo.PassWord.ToString() != oldPassWordEncode.ToString())
                return false;
            //設定新密碼
            var newPassWord = encryptionService.StringToSHA256(string.Format("{0}{1}", reSetPassWord, memberInfo.Account.ToLower()));
            memberInfo.PassWord = newPassWord;
            db.SaveChanges();

            var mailService = new MailService();
            var emailDomain = ConfigurationManager.AppSettings["MailDomain"].ToString();
            var emailAdminAddress = ConfigurationManager.AppSettings["MailAdminAddress"].ToString();
            var emailSMTPPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailSMTPPort"].ToString());
            //收信者
            var recipient = new List<string>() { memberInfo.Email };
            var msg = string.Format("{0}您好!!您修改後的密碼是{1}", memberInfo.Name, reSetPassWord);
            var sendMail = mailService.SendMail(emailDomain, emailSMTPPort, emailAdminAddress, recipient, msg, "Flipus重設密碼", null).Result;
            return true;
        }
    }
}
