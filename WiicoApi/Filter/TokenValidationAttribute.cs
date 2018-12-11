using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Filter
{
    /// <summary>
    /// Token驗證
    /// </summary>
    public class TokenValidationAttribute : ActionFilterAttribute
    {
        private readonly TokenService tokenService = new TokenService();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string apiToken = RequestHelper.GetValueByKey(actionContext, "token");
            //先從header取得
            if (apiToken == null || apiToken == string.Empty)
            {
                var headers = actionContext.Request.Headers;
                if (headers.Contains("X-Token"))
                    apiToken = headers.GetValues("X-Token").FirstOrDefault();
            }
            //可能有舊的API接口為ICanToken驗證欄位
            if (apiToken == null || apiToken == string.Empty)
                apiToken = RequestHelper.GetValueByKey(actionContext, "ICanToken");
            //icantoken
            if (apiToken == null || apiToken == string.Empty)
                apiToken = RequestHelper.GetValueByKey(actionContext, "icantoken");
            //icantoken
            if (apiToken == null || apiToken == string.Empty)
                apiToken = RequestHelper.GetValueByKey(actionContext, "iCanToken");



            var jsonResponse = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            if (apiToken != null)
            {
                Guid token = new Guid();
                if (!Guid.TryParse(apiToken, out token))
                {
                    jsonResponse.Success = false;
                    jsonResponse.Message = "token不合法，請重新登入 token:[" + apiToken + "]";
                    jsonResponse.State = Infrastructure.ViewModel.Base.LogState.Logout;
                    actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Forbidden,jsonResponse);
                }
                else
                {
                    var userToken = tokenService.GetTokenInfo(token.ToString()).Result;

                    if (userToken==null)
                    {
                        jsonResponse.Success = false;
                        jsonResponse.Message = "身分驗證失敗，請重新登入 token:[" + apiToken + "]";
                        jsonResponse.State = Infrastructure.ViewModel.Base.LogState.Logout;
                        actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Forbidden, jsonResponse);
                    }
                    else
                    {
                        actionContext.Request.Properties.Add("UserToken", userToken);
                    }
                }
            }
            else
            {
                jsonResponse.Success = false;
                jsonResponse.Message = "請輸入 Token";
                jsonResponse.State = Infrastructure.ViewModel.Base.LogState.Logout;
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized, jsonResponse);
            }
        }
    }
}