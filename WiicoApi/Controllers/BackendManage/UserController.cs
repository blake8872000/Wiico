using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using WiicoApi.Controllers.Common;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.Login;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{

    /// <summary>
    /// 使用者帳號管理
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class UserController : BaseController
        <BaseResponse<IEnumerable<UserGetResponse>>, MemberManageGetRequest,
        BaseResponse<UserPostResponse>, RegisterRequest,
        BaseResponse<Member>, MemberManagePutRequest,
        BaseResponse<string>, string>
    {

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<MemberManageGetRequest>(strAccess);

            //驗證token
            var authToken = AuthToken(requestData.Token, Request) as NegotiatedContentResult<BaseResponse<string>>;
            if (authToken.Content.Success == false)
                return authToken;

            getResponse = new BaseResponse<IEnumerable<UserGetResponse>>()
            {
                Success = false,
                Data = new List<UserGetResponse>()
            };

            var service = new MemberService();
            var responseData = service.GetBackendMemberListByOrgId(requestData.OrgId, "");
            if (responseData.FirstOrDefault() == null)
            {
                getResponse.State =LogState.Error;
                getResponse.Success = true;
                getResponse.Message = "沒有資料";
                return Ok(getResponse);
            }
            getResponse.Success = true;
            getResponse.Message = "查詢成功";
            getResponse.Data = (requestData.Pages.HasValue && requestData.Rows.HasValue) ? 
                responseData.Skip((requestData.Pages.Value-1)*requestData.Rows.Value).Take(requestData.Rows.Value).ToList():
                responseData.ToList();
            return Ok(getResponse);
        }

        /// <summary>
        /// 註冊帳號
        /// </summary>
        [HttpPost]
        public IHttpActionResult Post(RegisterRequest requestData)
        {
            var checkColumnKeys = new string[4] { "name", "account", "email", "pwd" };
            //判斷是否遺漏參數
            var checkEmpty = CheckEmpty(requestData, checkColumnKeys) as NegotiatedContentResult<BaseResponse<string>>;
            if (checkEmpty.Content.Success == false)
                return checkEmpty;

            //驗證token
            var authToken = AuthToken(requestData.Token, Request) as NegotiatedContentResult<BaseResponse<string>>; ;
            if (authToken.Content.Success == false)
                return authToken;

            requestData.Token = _token;

            postResponse = new BaseResponse<UserPostResponse>() {
                Success=false,
                Data = new UserPostResponse(),
                State= LogState.Error,
                Message = "註冊失敗"
        };
            
            var appKey = ConfigurationManager.AppSettings["AppLoginKey"].ToString();
            var encryptionService = new Service.Utility.Encryption();

            var service = new MemberService();
            var responseData = service.RegisterMember(requestData, null);
            if (responseData!=null)
            {
                postResponse.Success = true;
                postResponse.Message = "註冊成功";
                postResponse.Data = responseData;
                postResponse.State = LogState.Suscess;
                return Ok(postResponse);
            }
            else
                return Ok(postResponse);
        }

        /// <summary>
        /// 編輯成員資訊
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult Put(MemberManagePutRequest requestData)
        {
            putResponse = new BaseResponse<Member>() { Success=false, Data = new Member(),Message="編輯失敗" };
            var checkColumnKeys = new string[3] { "account", "orgid", "email" };
            //判斷是否遺漏參數
            var checkEmpty = CheckEmpty(requestData, checkColumnKeys) as NegotiatedContentResult<BaseResponse<string>>;
            if (checkEmpty.Content.Success == false)
                return checkEmpty;

            //驗證token
            var authToken = AuthToken(requestData.Token, Request) as NegotiatedContentResult<BaseResponse<string>>; 
            if (authToken.Content.Success == false)
                return authToken;
            requestData.Token = _token;
            var service = new MemberService();
            var responseData = service.UpdateMemberInfo(requestData);
            if (responseData!=null)
            {
                putResponse.Success = true;
                putResponse.Message = "編輯成功";
                putResponse.Data = responseData;
            }

            return Ok(putResponse);
        }

        /// <summary>
        /// 刪除成員資訊
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult Delete(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<MemberManageDeleteRequest>(strAccess);
            //驗證token
            var authToken = AuthToken(requestData.Token, Request) as NegotiatedContentResult<BaseResponse<string>>; ;
            if (authToken.Content.Success == false)
                return authToken;
            requestData.Token = _token;
            deleteResponse = new BaseResponse<string>() { Success=false,Message="刪除失敗"};
            var service = new MemberService();
            var responseData = service.DeleteMultipleMember(requestData);
            if (responseData)
            {
                deleteResponse.Success = true;
                deleteResponse.Message = "刪除成功";
            }
            return Ok(deleteResponse);
        }

        //===================以下為相關APIACTION===========================


        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("rest/user/resetPwd")]
        public IHttpActionResult UserForgetPassWord(ReSetPassWordPostRequest requestData)
        {
            deleteResponse = new BaseResponse<string>();
            deleteResponse.Success = false;
            deleteResponse.Message = "驗證碼失敗或查無此信箱，請確認輸入信箱是否有註冊!!!";
            deleteResponse.State = LogState.Error;

            var checkColumnKeys = new string[2] { "code", "email" };
            //確認是否遺漏參數
           var checkEmpty = CheckEmpty(requestData, checkColumnKeys) as NegotiatedContentResult<BaseResponse<string>>;
            if (checkEmpty.Content.Success == false)
                return checkEmpty;

            var captchaCacheKey = "captchaString";
            var getcaptchaKey = HttpContext.Current != null ? HttpContext.Current.Cache.Get(captchaCacheKey).ToString() : null;
            if (getcaptchaKey == null)
            {
                deleteResponse.Success = false;
                deleteResponse.Message = "重新確認驗證碼";
                deleteResponse.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, deleteResponse);
            }
            var reSetPassWordService = new ReSetPassWordService();
            var response = reSetPassWordService.Do(requestData.Code, getcaptchaKey, requestData.Email);

            if (response)
            {
                var captchaImg = ConfigurationManager.AppSettings["DrivePath"].ToString();
                //刪除驗證碼
                System.IO.File.Delete(string.Format("{0}\\captcha\\{1}.gif", captchaImg.ToString(), requestData.Code));
                deleteResponse.Success = true;
                deleteResponse.Message = "修改成功，請收信";
                return Ok(deleteResponse);
            }

            return Content(HttpStatusCode.BadRequest, deleteResponse);
        }
        /// <summary>
        ///  登入者修改密碼
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [TokenValidation, HttpPut, Route("rest/user/resetPwd")]
        public IHttpActionResult Put(ReSetPassWordPutRequest requestData)
        {
            var responseData = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            responseData.Success = false;
            responseData.Message = "修改失敗!!!";
            responseData.State = Infrastructure.ViewModel.Base.LogState.Error;
            var requestService = new Service.Utility.RequestDataHelper<ReSetPassWordPutRequest>();


            var checkColumnKeys = new string[2] { "oldpassword", "newpassword" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkEmpty == false)
            {
                responseData.Success = false;
                responseData.Message = "遺漏參數!!!";
                responseData.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, responseData);
            }
            var reSetPassWordService = new ReSetPassWordService();
            var encryptionService = new Service.Utility.Encryption();

            var response = reSetPassWordService.DoByToken(requestData.Token.ToLower(), requestData.OldPassword, requestData.NewPassword);

            if (response)
            {
                responseData.Success = true;
                responseData.Message = "修改成功!!!";
                responseData.State = Infrastructure.ViewModel.Base.LogState.Suscess;
                return Ok(responseData);
            }

            return Content(HttpStatusCode.NoContent, responseData);
        }
        [HttpGet, Route("rest/user/login")]
        public IHttpActionResult ReGetAccountInfo() { return Ok(true); }
        [HttpPost, Route("rest/user/login")]
        public IHttpActionResult Login() { return Ok(false); }

        [HttpPost, Route("rest/user/signup")]
        public IHttpActionResult SignUp()
        {
            return Ok();
        }
    }
}
