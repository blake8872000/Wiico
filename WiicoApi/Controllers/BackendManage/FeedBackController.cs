using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel.School;
using WiicoApi.Infrastructure.ViewModel.School.FeedBack;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 問題回饋控制器
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class FeedBackController : ApiController
    {
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get 
        ///     {
        ///        "pages":1,
        ///        "rows": 20
        ///     }
        ///     
        /// </remarks>
        /// <param name="strAccess">pages int 頁碼 ,rows int 筆數</param>

        /// <returns></returns>
        [HttpGet, TokenValidation]
        public IHttpActionResult Get(string strAccess = null)
        {
            var requestData = new PagesRows();
            if (strAccess != null)
                requestData = JsonConvert.DeserializeObject<PagesRows>(strAccess);
            var token = string.Empty;

            if (Request != null && Request.Headers.Contains("X-Token"))
                token = Request.Headers.GetValues("X-Token").FirstOrDefault();
            else if (requestData.Token != null)
                token = requestData.Token;
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            var response = new BaseResponse<FeedBackGetResponse>();
            response.Success = false;
            response.Data = new FeedBackGetResponse();

            if (memberInfo == null)
            {
                response.State = LogState.Logout;
                response.Message = "已登出";
                return Ok(response);
            }
            var authService = new AuthService();
            var isSystemAccount = authService.CheckSystemAdmin(memberInfo.Id);
            if (isSystemAccount == false)
            {
                response.State = LogState.Error;
                response.Message = "沒有權限";
                return Ok(response);
            }
            var feedBackService = new FeedBackService();
            var responseData = (requestData.Pages != null && requestData.Rows != null) ?
                                                     feedBackService.GetList(memberInfo.OrgId, requestData.Pages, requestData.Rows) :
                                                     feedBackService.GetList(memberInfo.OrgId);
            if (responseData == null)
            {
                response.Message = "查無資料";
                response.State = LogState.Error;
            }
            else
            {
                response.Success = true;
                response.Message = "查詢成功";
                response.Data = responseData;
            }
            return Ok(response);
        }
        /// <summary>
        /// 建立問題回報
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Post /Todo
        ///     {
        ///            "feedBackType":"系統錯誤",
        ///            "description":"就是錯誤無法登入",
        ///            "system":"Desktop_Device:unknown_OSVersion:windows-10_Browser:chrome_BrowserVersion:68.0.3440.106",
        ///            "email":"yushuchen@sce.pccu.edu.tw"
        ///            }
        ///
        /// </remarks>
        /// <param name="requestData"></param>

        /// <returns></returns>
        [TokenValidation, HttpPost]
        public IHttpActionResult Post(FeedBackPostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<FeedBackPostRequest>();
            var checkColumnKey = new string[3] { "feedbacktype", "system", "email" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<FeedBack>();
            response.Success = false;
            response.Data = new FeedBack();

            if (Request != null && Request.Headers.Contains("X-Token"))
                requestData.Token = Request.Headers.GetValues("X-Token").FirstOrDefault();
            else if (requestData.Token != string.Empty && requestData.Token != null)
                requestData.Token = requestData.Token;
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            /*判斷email格式是否有問題*/
            if (requestData.Email.Contains("@") == false)
            {
                response.Message = "email格式錯誤";
                response.State = LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var feedBackService = new FeedBackService();
            var responseData = feedBackService.Insert(requestData);
            if (responseData != null)
            {
                response.Success = true;
                response.Data = responseData;
                response.Message = "新增成功";
            }
            else
            {
                response.Message = "新增失敗";
                response.State = LogState.Error;
            }
            return Ok(response);
        }

        [HttpPut, TokenValidation]
        public IHttpActionResult Put(FeedBackPutRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<FeedBackPutRequest>();
            var checkColumnKey = new string[2] { "id", "status" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<FeedBack>();
            response.Success = false;
            response.Data = new FeedBack();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            if (Request != null && Request.Headers.Contains("X-Token"))
                requestData.Token = Request.Headers.GetValues("X-Token").FirstOrDefault();
            else if (requestData.Token != string.Empty && requestData.Token != null)
                requestData.Token = requestData.Token;

            var feedBackService = new FeedBackService();
            var responseData = feedBackService.Update(requestData);
            if (responseData != null)
            {
                response.Success = true;
                response.Data = responseData;
                response.Message = "更新成功";
            }
            else
            {
                response.Message = "更新失敗";
                response.State = LogState.Error;
            }
            return Ok(response);
        }
    }
}
