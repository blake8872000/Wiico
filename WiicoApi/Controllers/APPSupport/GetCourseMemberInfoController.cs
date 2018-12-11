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
    /// 取得使用者在課程內的角色資訊
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class GetCourseMemberInfoController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess) {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.ViewModel.MemberManage.GetCourseMemberInfoResponse>();
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.MemberManage.GetCourseMemberInfoRequest>(strAccess);
            if (requestData.Account == null ||
                requestData.CircleKey == null ||
                requestData.ICanToken == null ||
                requestData.QueryAccount == null)
            {
                response.Data = new Infrastructure.ViewModel.MemberManage.GetCourseMemberInfoResponse();
                response.Success = false;
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                response.Success = false;
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }
            var service =new MemberService();
            var data = service.APPGetCourseMemberInfo(requestData);
            if (data == null) {
                response.Data = new Infrastructure.ViewModel.MemberManage.GetCourseMemberInfoResponse();
                response.Success = false;
                response.Message = "查無資料";
                return Ok(response);
            }
            response.Data = data;
            response.Success = true;
            response.Message = "查詢成功";
            response.State = Infrastructure.ViewModel.Base.LogState.Suscess;

            return Ok(response);
        }
    }
}
