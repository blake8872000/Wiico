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
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 學習圈角色
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class LearningRoleController : ApiController
    {
        private readonly LearningRoleService learningRoleService = new LearningRoleService();
        public IHttpActionResult Get(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Base.BackendBaseRequest>();
            var checkColumnKey = new string[2] { "token", "circlekey" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<LearningRoleGetResponse>>();
            response.Success = false;
            response.Data = new List<LearningRoleGetResponse>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var responseData = learningRoleService.GetLearningRolesByCircleKey(requestData.CircleKey.ToLower(), requestData.Token);
            if (responseData != null)
            {
                response.Success = true;
                response.Message = "查詢成功";
                response.Data = responseData.ToList();
            }
            else
            {
                response.Message = "查詢失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
            }
            return Ok(response);
        }

        /// <summary>
        /// 建立角色
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [TokenValidation, HttpPost]
        public IHttpActionResult Post(LearningRolePostResquest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Backend.LearningRolePostResquest>();
            var checkColumnKey = new string[3] { "token", "circlekey", "roles" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<LearningRole>>();
            response.Success = false;
            response.Data = new List<LearningRole>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var authService = new AuthService();
            var checkManageAuth = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
            //除了課程管理者與老師可以修改，其他角色都不得修改
            if (checkManageAuth == null || checkManageAuth.CircleRoleSetting.AddCircleRole == false)
            {
                response.Message = "無權限新增";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            var responseData = learningRoleService.CreateMutiple(requestData);
            if (responseData == null)
            {
                response.Message = "建立失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            response.Success = true;
            response.Message = "建立成功";
            response.Data = responseData.ToList();
            return Ok(response);
        }

        [HttpPut]
        public IHttpActionResult Put(LearningRolePostResquest requestData)
        {
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Backend.LearningRolePostResquest>();
            var checkColumnKey = new string[3] { "token", "circlekey", "roles" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<IEnumerable<LearningRoleGetResponse>>();
            response.Success = false;
            response.Data = new List<LearningRoleGetResponse>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var authService = new AuthService();
            var checkManageAuth = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
            //除了課程管理者與老師可以修改，其他角色都不得修改
            if (checkManageAuth == null || checkManageAuth.CircleRoleSetting.AddCircleRole == false)
            {
                response.Message = "無權限修改";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            var responseData = learningRoleService.UpdateMutiple(requestData);
            if (responseData)
            {
                var datas = learningRoleService.GetLearningRolesByCircleKey(requestData.CircleKey.ToLower(), requestData.Token);
                if (datas.FirstOrDefault() != null)
                    response.Data = datas;
                response.Success = true;
                response.Message = "修改成功";
            }
            else
            {
                response.Message = "修改失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
            }
            return Ok(response);
        }

        [HttpDelete]
        public IHttpActionResult Delete(string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<LearningRoleDeleteResquest>(strAccess);
            var requestService = new Service.Utility.RequestDataHelper<LearningRoleDeleteResquest>();
            var checkColumnKey = new string[3] { "token", "circlekey", "ids" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKey);
            var response = new Infrastructure.ViewModel.Base.BaseResponse<IEnumerable<LearningRoleGetResponse>>();
            response.Success = false;
            response.Data = new List<LearningRoleGetResponse>();
            if (checkEmpty == false)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }
            var authService = new AuthService();
            var checkManageAuth = authService.CheckCourseManageAuth(requestData.Token, requestData.CircleKey.ToLower());
            //除了課程管理者與老師可以修改，其他角色都不得修改
            if (checkManageAuth == null || checkManageAuth.CircleRoleSetting.AddCircleRole == false)
            {
                response.Message = "無權限刪除";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(response);
            }
            var responseData = learningRoleService.DeleteMutiple(requestData);
            if (responseData)
            {
                var datas = learningRoleService.GetLearningRolesByCircleKey(requestData.CircleKey.ToLower(), requestData.Token);
                if (datas.FirstOrDefault() != null)
                    response.Data = datas;
                response.Success = true;
                response.Message = "刪除成功";
            }
            else
            {
                response.Message = "刪除失敗";
                response.State = Infrastructure.ViewModel.Base.LogState.Error;
            }
            return Ok(response);
        }
    }
}
