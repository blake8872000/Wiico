using WiicoApi.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 重新設定信箱與是否公開
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SetPersonalMailController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess)
        {
            var requestData = new Infrastructure.ViewModel.MemberManage.MailSettingViewModel();
            try
            {
                requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.MemberManage.MailSettingViewModel>(strAccess);
                if (requestData.Account == null ||
                    requestData.ICanToken == null ||
                    requestData.EmailAddress == null
                    )
                    return Content(HttpStatusCode.BadRequest, "遺漏參數");

                var memberService = new MemberService();
                var result = new Infrastructure.ViewModel.Base.BaseResponse<string>();
                var tokenService = new TokenService();
                var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
                if (checkToken == null)
                {
                    result.Success = false;
                    result.Message = "已登出";
                    result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                    return Content(HttpStatusCode.BadRequest, result);
                }
                var response = memberService.UpdateMemberEmailInfo(requestData.ICanToken, requestData.Account, requestData.EmailAddress, requestData.ShowMail);


      
                if (!response)
                {
                    result.Success = false;
                    result.Message = "修改失敗";
                    result.State = Infrastructure.ViewModel.Base.LogState.Error;
                    return Ok(result);
                }
                result.Success = true;
                result.Message = "修改成功";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "錯誤的參數");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TokenValidation]
        public IHttpActionResult Post([FromBody]Infrastructure.ViewModel.MemberManage.MailSettingViewModel value)
        {
            try
            {
                if (value.Account == null ||
                       value.ICanToken == null ||
                       value.EmailAddress == null
                       )
                    return Content(HttpStatusCode.BadRequest, "遺漏參數");

                var memberService = new MemberService();
                var result = new Infrastructure.ViewModel.Base.BaseResponse<string>();
                var tokenService = new TokenService();
                var checkToken = tokenService.GetTokenInfo(value.ICanToken).Result;
                if (checkToken == null)
                {
                    result.Success = false;
                    result.Message = "已登出";
                    result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                    return Ok(result);
                }
                var response = memberService.UpdateMemberEmailInfo(value.ICanToken, value.Account, value.EmailAddress, value.ShowMail);

                if (!response)
                {
                    result.Success = false;
                    result.Message = "修改失敗";
                    return Ok(result);
                }
                result.Success = true;
                result.Message = "修改成功";
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "錯誤的參數");
            }
        }
    }
}
