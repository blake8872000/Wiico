using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.ViewModel.CourseManage;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 課綱管理
    /// </summary>
    [EnableCors("*","*","*")]
    public class SyllabusManageController : ApiController
    {    /// <summary>
         /// 取得課綱資訊
         /// </summary>
         /// <param name="circleKey"></param>
         /// <param name="strAccess"></param>
         /// <returns></returns>
        public IHttpActionResult Get(string strAccess)
        {
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Base.BackendBaseRequest>();
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);
            var checkColumnKey = new string[2] { "token", "circlekey" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<Infrastructure.ViewModel.CourseManage.GetCourseSyllabusResponse>>();
            response.Success = false;
            response.Data = new List<GetCourseSyllabusResponse>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.Token).Result;
            if (checkToken == null)
            {
                response.Message = "已登出";
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(response);
            }
            var syllabusService = new SyllabusService();
            var data = syllabusService.APPGetCourSyllabus(requestData.Token, requestData.CircleKey.ToLower());
            if (data == null && data.Count() < 0)
            {
                response.Success = false;
                response.Message = "查詢失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            response.Success = true;
            response.Message = "查詢成功";
            response.Data = data.ToList();
            return Ok(response);
        }
        /// <summary>
        /// 新增課綱資訊
        /// </summary>
        /// <returns></returns>
        [TokenValidation, HttpPost]
        public IHttpActionResult Post(SyllabusManagePostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<SyllabusManagePostRequest>();
            var checkColumnKeys = new string[3] { "token", "circleKey", "syllabuses" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            var result = new Infrastructure.ViewModel.Base.BaseResponse<List<Infrastructure.Entity.Syllabus>>();
            //取得資料
            if (checkEmpty == false)
            {
                result.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                result.Success = false;
                result.Message = "確認是否遺漏資訊";
                return Content(HttpStatusCode.BadRequest, result);
            }

            var syllabusService = new SyllabusService();
            var data = syllabusService.SyllabusesDataProxy(requestData);


            if (data == null)
            {
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                result.Success = false;
                result.Message = "新增失敗";
                return Ok(result);
            }
            result.Success = true;
            result.Message = "新增成功";
            result.Data = data;
            return Ok(result);
        }

        /// <summary>
        /// 編輯課綱資訊
        /// </summary>
        /// <returns></returns>
        [TokenValidation, HttpPut]
        public IHttpActionResult Put(SyllabusManagePostRequest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<SyllabusManagePostRequest>();
            var checkColumnKeys = new string[5] { "id", "name", "note", "date", "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            var result = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.Entity.Syllabus>();
            if (checkEmpty == false)
            {
                result.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                result.Success = false;
                result.Message = "確認是否遺漏資訊";
                return Content(HttpStatusCode.BadRequest, result);
            }

            var syllabusService = new SyllabusService();
            var data = syllabusService.UpdateSyllabusByToken(requestData.Id.Value,
                                                                                             requestData.Name,
                                                                                             requestData.Note,
                                                                                             requestData.Date,
                                                                                             requestData.Token);
            if (data == null)
            {
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                result.Success = false;
                result.Message = "編輯失敗";
                return Ok(result);
            }
            result.Success = true;
            result.Message = "編輯成功";
            result.Data = data;
            return Ok(result);
        }

        /// <summary>
        /// 刪除學習圈資訊
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [TokenValidation, HttpDelete]
        public IHttpActionResult Delete(int id, string token)
        {
            var requestService = new Service.Utility.RequestDataHelper<SyllabusManagePostRequest>();
            var result = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            if (id <= 0 || token == null || token == string.Empty)
            {
                result.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                result.Success = false;
                result.Message = "確認是否遺漏資訊";
                return Content(HttpStatusCode.BadRequest, result);
            }


            var syllabusService = new SyllabusService();
            var data = syllabusService.DeleteSyllabusByToken(id, token);
            if (data == null)
            {
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                result.Success = false;
                result.Message = "刪除失敗";
                return Ok(result);
            }
            result.Success = true;
            result.Message = "刪除成功";
            return Ok(result);
        }
    }
}
