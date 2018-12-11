using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.Common
{
    /// <summary>
    /// 提供驗證token與取得version的API
    /// </summary>
    /// <typeparam name="GetResponseT">Get回傳model</typeparam>
    /// <typeparam name="GetRequestT">Get參數model</typeparam>
    /// <typeparam name="PostResponseT">Post回傳model</typeparam>
    /// <typeparam name="PostRequestT">Post參數model</typeparam>
    /// <typeparam name="PutResponseT">Put回傳model</typeparam>
    /// <typeparam name="PutRequestT">Put參數model</typeparam>
    /// <typeparam name="DeleteResponseT">Delete回傳model</typeparam>
    /// <typeparam name="DeleteRequestT">Delete參數model</typeparam>
    [EnableCors("*", "*", "*")]
    public class BaseController<GetResponseT, GetRequestT, PostResponseT, PostRequestT, PutResponseT, PutRequestT, DeleteResponseT, DeleteRequestT> : ApiController
    {
        /// <summary>
        /// 回傳Get API的model values
        /// </summary>
        public GetResponseT getResponse { get; set; }

        /// <summary>
        /// Get API的request
        /// </summary>
        public GetRequestT getRequest { get; set; }
        /// <summary>
        /// 回傳Post API的model values
        /// </summary>
        public PostResponseT postResponse { get; set; }
        /// <summary>
        /// 回傳Put API的model values
        /// </summary>
        public PutResponseT putResponse{ get; set; }
        /// <summary>
        /// 回傳Delete API的model values
        /// </summary>
        public DeleteResponseT deleteResponse { get; set; }
        /// <summary>
        /// DeleteAPI的request
        /// </summary>
        public DeleteRequestT deleteRequest { get; set; }
        /// <summary>
        /// 顯示結果版本號
        /// </summary>
        public static int _version;
        /// <summary>
        /// 驗證碼
        /// </summary>
        public static string _token;

        /// <summary>
        /// 初始化
        /// </summary>
        public BaseController() {
            GetVersion();
        }

        /// <summary>
        /// 回傳errorResponse
        /// </summary>
        /// <param name="status">錯誤的status</param>
        /// <param name="methods">Get | Post | Put | Delete</param>
        /// <returns></returns>
        [HttpHead]
        public IHttpActionResult ErrorResponse(HttpStatusCode status, string methods)
        {
            switch (methods.ToLower())
            {
                case "get":
                    return Content(status, getResponse);
                case "post":
                    return Content(status, postResponse);
                case "put":
                    return Content(status, putResponse);
                case "delete":
                    return Content(status, deleteResponse);
                default:
                    return Content(status, "發生錯誤");
            }
        }

        /// <summary>
        /// 取得version
        /// </summary>
        /// <returns></returns>
        [HttpHead]
        public int GetVersion()
        {
            _version = 1;
            if (Request == null)
                return _version;
            var headers = Request.Headers;
            if (headers.Contains("X-Version"))
                _version = Convert.ToInt32(headers.GetValues("X-Version").FirstOrDefault());
            return _version;
        }

        /// <summary>
        /// 驗證需求欄位是否為空
        /// </summary>
        /// <param name="checkRequest"></param>
        /// <param name="checkColumnKeys"></param>
        /// <returns></returns>
        [HttpHead]
        public IHttpActionResult CheckEmpty(object checkRequest,object[] checkColumnKeys) {
            var requestService = new Service.Utility.RequestDataHelper<object>();
            var checkEmpty = requestService.CheckColumnEmpty(checkRequest, checkColumnKeys);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>()
            {
                Success = false,
                Data = string.Empty,
                Message = "遺漏參數",
                State= Infrastructure.ViewModel.Base.LogState.RequestDataError
            };
            if (checkEmpty == false)
                return Content(HttpStatusCode.BadRequest, response);
            response.Success = true;
            return Content(HttpStatusCode.OK,response);
        }

        /// <summary>
        /// 驗證token - 根據header或參數
        /// </summary>
        /// <returns></returns>
        [HttpHead, TokenValidation]
        public IHttpActionResult AuthToken(string token = null,HttpRequestMessage request=null)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>()
            {
                Success = false,
                Data = string.Empty,
                Message = "驗證失敗",
                State= Infrastructure.ViewModel.Base.LogState.Logout
            };

            if (token == null || token == string.Empty)
            {
                if (request == null)
                    return Content(HttpStatusCode.BadRequest, response);
                var headers = request.Headers;
                if (headers.Contains("X-Token"))
                    token = headers.GetValues("X-Token").FirstOrDefault();
                //沒有任何token參數
                if (token == string.Empty)
                {
                    response.Message = "請輸入token資料";
                    return Content(HttpStatusCode.Unauthorized, response);
                }
            }

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            //驗證失敗
            if (checkToken == null)
                return Content(HttpStatusCode.Forbidden, response);
            response.Success = true;
            response.Data = checkToken.Token;
            response.State = Infrastructure.ViewModel.Base.LogState.Suscess;
            _token = checkToken.Token;
            return Content(HttpStatusCode.OK, response);
        }
    }
}
