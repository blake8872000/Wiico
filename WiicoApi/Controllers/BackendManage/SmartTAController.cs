using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using WiicoApi.Controllers.Common;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Service.Backend;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 提供Smart的API
    /// </summary>
    [EnableCors("*","*","*")]
    public class SmartTAController : BaseController<SmartTAGetResponse, SmartTAGetRequest,SmartTAGetResponse, SmartTAPostRequest, SmartTAGetResponse, string,string,string>
    {
        /// <summary>
        /// 取得smartTA的所有課程時程
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string strAccess) {
            getRequest =JsonConvert.DeserializeObject<SmartTAGetRequest>(strAccess);
            var checkEmptyColumnKeys = new string[1] { "account" };
            var checkEmpty = CheckEmpty(getRequest, checkEmptyColumnKeys) as NegotiatedContentResult<BaseResponse<string>>; ;
            if (checkEmpty.Content.Success == false)
                return checkEmpty;
            var service = new SmartTAService();
            var responseData = service.GetData(getRequest.Account);
            getResponse = new SmartTAGetResponse();
            getResponse = responseData!=null ? responseData : getResponse;

            return Ok(getResponse);

        }

        /// <summary>
        /// 取得某課程的上課地點
        /// </summary>
        /// <param name="classRoomId"></param>
        /// <returns></returns>
        [HttpGet, Route("rest/course/{classRoomId}"),TokenValidation]
        public IHttpActionResult GetCourse(string classRoomId) {

            getRequest = new SmartTAGetRequest() { ClassRoomId=classRoomId};
            var checkEmptyColumnKeys = new string[1] { "classroomid" };
            var checkEmpty = CheckEmpty(getRequest, checkEmptyColumnKeys) as NegotiatedContentResult<BaseResponse<string>>; ;
            if (checkEmpty.Content.Success == false)
                return checkEmpty;
            //驗證token
            var authToken = AuthToken(null, Request) as NegotiatedContentResult<BaseResponse<string>>;
            if (authToken.Content.Success == false)
                return authToken;

            var service = new SmartTAService();
            var responseData = service.GetData(getRequest.ClassRoomId);
            getResponse = new SmartTAGetResponse();
            getResponse = responseData != null ? responseData : getResponse;

            return Ok(getResponse);
        }

        /// <summary>
        /// 建立多筆設備與學習圈的關聯
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public IHttpActionResult Post(SmartTAPostRequest requestData) {
            postResponse = new SmartTAGetResponse();
            var service = new SmartTAService();
            var responseData = service.InsertRelation(requestData);
            postResponse = responseData;
            if (responseData == null)
                return Content(HttpStatusCode.InternalServerError,"新增失敗");
            return Ok(postResponse) ;
        }
    }
}
