using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Filter;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Service.Backend;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.BackendManage
{
    /// <summary>
    /// 學習圈管理
    /// </summary>
    [EnableCors("*","*","*")]
    public class LearningCircleManageController : ApiController
    {
        /// <summary>
        /// 取得學習圈列表
        /// </summary>
        /// <param name="strAccess"></param>
        /// <returns></returns>
        public IHttpActionResult Get(string strAccess)
        {
            var requestService = new Service.Utility.RequestDataHelper<LearningCircleGetRequest>();
            var requestData = JsonConvert.DeserializeObject<LearningCircleGetRequest>(strAccess);
            var checkColumnKeys = new string[1] { "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            var backendResponse = new Infrastructure.ViewModel.Base.BaseResponse<List<LearningCircleManageViewModel>>();
            backendResponse.Success = false;
            backendResponse.Data = new List<LearningCircleManageViewModel>();
            if (checkEmpty == false)
            {
                backendResponse.Message = "請確認是否遺漏資訊";
                return Content(HttpStatusCode.BadRequest, backendResponse);
            }
            var learningCircleService = new LearningCircleService();
            //    var data = service.GetLearningCircleListByToken(token, orgId, searchName);
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.Token).Result;
            if (tokenInfo == null)
            {
                backendResponse.Message = "已登出";
                backendResponse.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(backendResponse);
            }
            //查詢列表
            if (requestData.CircleKey == null || requestData.CircleKey == string.Empty)
            {
                var data = learningCircleService.GetLearningCircleManageList(tokenInfo.MemberId, requestData.SearchName, requestData.OrgId);
                if (data == null)
                {
                    backendResponse.Success = false;
                    backendResponse.Message = "查詢失敗";
                    backendResponse.State = Infrastructure.ViewModel.Base.LogState.Error;
                    return Ok(backendResponse);
                }

                backendResponse.Message = "查詢成功";
                backendResponse.Success = true;
                backendResponse.Data = data.ToList();
                return Ok(backendResponse);
            }
            //查詢單個課程
            else
            {
                var detailResponse = new Infrastructure.ViewModel.Base.BaseResponse<LearningCircleGetResponse>();
                var data = learningCircleService.GetLearningCircleInfo(requestData.CircleKey.ToLower());
                if (data == null)
                {
                    detailResponse.Success = false;
                    detailResponse.Message = "查詢失敗";
                    detailResponse.State = Infrastructure.ViewModel.Base.LogState.Error;
                }
                else
                {
                    detailResponse.Success = true;
                    detailResponse.Message = "查詢成功";
                    detailResponse.Data = data;
                }
                return Ok(detailResponse);
            }
        }
        /// <summary>
        /// 新增學習圈資訊
        /// </summary>
        /// <returns></returns>
        [TokenValidation]
        public IHttpActionResult Post(LearningCirclePostRequest requestData)
        {
            var result = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.Entity.LearningCircle>();
            var requestService = new Service.Utility.RequestDataHelper<LearningCirclePostRequest>();
            var checkColumnKeys = new string[7] { "name", "description", "token", "startdate", "enddate", "weeks", "place" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);

            if (checkEmpty == false)
            {
                result.Success = false;
                result.Message = "確認是否遺漏資訊";
                return Content(HttpStatusCode.BadRequest, result);
            }
            var service = new LearningCircleService();
            var data = service.CreateLearningCircle(requestData.Name,
                                                                                        requestData.CircleKey,
                                                                                         requestData.Description,
                                                                                        requestData.Token,
                                                                                        requestData.StartDate.Value,
                                                                                        requestData.EndDate.Value,
                                                                                        requestData.OrgId
                                                                                        );
            if (data == null)
            {
                result.Success = false;
                result.Message = "新增失敗";
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(result);
            }
            result.Success = true;
            var learningRoleService = new LearningRoleService();
            var authService = new AuthService();
            //建立老師角色身分
            var teacherRole = learningRoleService.AddLearningEditRole(data.CreateUser.Value, data.Id, "老師", true, false, 1);
            //建立助教角色身分
            var surpportTeacherRole = learningRoleService.AddLearningEditRole(data.CreateUser.Value, data.Id, "助教", true, false, 2);
            //建立學生角色身分
            var studentRole = learningRoleService.AddLearningEditRole(data.CreateUser.Value, data.Id, "學生", false);
            //建立學習圈內角色權限
            var insertAuthSuccess = authService.InsertLearningCircleAllRoleAuth(data.Id, data.CreateUser.Value);

            var weekService = new WeekTableService();
            var weekDatas = weekService.CreateWeekDatas(requestData.Token, data.LearningOuterKey, 0, requestData.Place, requestData.StartDate.Value, requestData.EndDate.Value, requestData.Weeks);
            var timeTableService = new TimeTableService();

            var timeTableDatas = requestData.ClassWeekType > 0 ? timeTableService.CreateByCircleKey(data.LearningOuterKey, requestData.ClassWeekType) : timeTableService.CreateByCircleKey(data.LearningOuterKey);

            result.Message = "新增成功";
            result.Data = data;
            return Ok(result);
        }

        /// <summary>
        /// 編輯學習圈資訊
        /// </summary>
        /// <returns></returns>
        [TokenValidation]
        public IHttpActionResult Put(LearningCirclePostRequest requestData)
        {
            var result = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.Entity.LearningCircle>();
            var requestService = new Service.Utility.RequestDataHelper<LearningCirclePostRequest>();
            var checkColumnKeys = new string[4] { "name", "circlekey", "description", "token" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            //取得資料
            var requestFormData = HttpContext.Current.Request.Form;

            if (checkEmpty == false)
            {
                result.Success = false;
                result.Message = "確認是否遺漏資訊";
                result.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, result);
            }
            var service = new LearningCircleService();
            var data = service.UpdateLearningCircle(requestData.Name,
                                                                                        requestData.CircleKey,
                                                                                      requestData.Description,
                                                                                        requestData.Token, true, requestData.Objective,
                                                                                        requestData.Remark
                                                                                      );
            if (data == null)
            {
                result.Success = false;
                result.Message = "編輯失敗";
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
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
        /// <returns></returns>

        public IHttpActionResult Delete(string token, string circlekey)
        {
            var result = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.Entity.LearningCircle>();
            if (token == null || circlekey == null)
            {
                result.Success = false;
                result.Message = "確認是否遺漏資訊";
                return Content(HttpStatusCode.BadRequest, result);
            }
            var service = new LearningCircleService();
            var data = service.DeleteLearningCircleByCircleKey(circlekey.ToLower(), token);
            if (data == false)
            {
                result.Success = false;
                result.Message = "刪除失敗";
                result.State = Infrastructure.ViewModel.Base.LogState.Error;
                return Ok(result);
            }
            result.Success = true;
            result.Message = "刪除成功";
            return Ok(result);
        }
    }
}
