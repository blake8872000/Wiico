using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.Service.SignalRService.Discussion;
using WiicoApi.Service.SignalRService.SignIn;

namespace WiicoApi.Controllers.APPSupport
{
    /// <summary>
    /// 活動列表控制器
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class ActivityListController : ApiController
    {
        private TokenService tokenService;
        private DiscussionService discussionService;
        private SignInService signInService;
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="token">查詢者代碼</param>
        /// <param name="id">活動代號</param>
        /// <param name="strAccess">APPJsonStr</param>
        /// <param name="circleKey">學習圈代碼</param>
        /// <param name="rows">查詢數量</param>
        /// <param name="pages">查詢索引</param>
        /// <returns></returns>

        public IHttpActionResult Get(string strAccess = "", Guid? token = null, int? id = 3, string circleKey = "", int? rows = null, int? pages = 1)
        {
            //APP 呼叫
            if (strAccess != "")
            {
                var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.ActivityFunction.ActivityListRequest>(strAccess);
                if (requestData.Account == null ||
                    requestData.CircleKey == null ||
                    requestData.ICanToken == null ||
                    (!requestData.Id.HasValue))
                {
                    var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
                    response.Success = false;
                    response.Message = "遺漏參數";
                    response.Data = null;
                    response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                    return Content(HttpStatusCode.BadRequest, response);
                }

                id = (int)requestData.Id.Value;
                token = Guid.Parse(requestData.ICanToken);
                circleKey = requestData.CircleKey;
                pages = requestData.Pages.HasValue ? requestData.Pages : 1;
                rows = requestData.Rows.HasValue ? requestData.Rows : 20;
            }
            tokenService = new TokenService();
            switch (id.Value)
            {
                //查詢主題討論的列表
                case (int)activityEnum.Discussion:
                    discussionService = new DiscussionService();

                    var discussionResponse = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionModuleList>();
                    var discussionData = discussionService.GetDiscussionList(circleKey, pages, rows);

                    if (discussionData != null && discussionData.FirstOrDefault() != null)
                    {
                        discussionResponse.Data = discussionData.ToArray();
                        discussionResponse.Success = true;
                        return Ok(discussionResponse);
                    }
                    else
                    {
                        discussionResponse.Data = new Infrastructure.ViewModel.ActivityFunction.Discussion.DiscussionModuleList[0];
                        discussionResponse.Success = true;
                        discussionResponse.Message = "查無資料";
                        return Ok(discussionResponse);
                    }

                case (int)activityEnum.SignIn:

                    try
                    {
                        var checkMember = tokenService.GetTokenInfo(token.Value.ToString()).Result;
                        var signInResponse = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.ActivityFunction.SignIn.SignInModuleList>();
                        signInService = new SignInService();

                        var signIndata = signInService.GetSignInList(checkMember.MemberId, circleKey, pages, rows);
                        if (signIndata.FirstOrDefault() != null)
                        {
                            signInResponse.Data = signIndata.ToArray();
                            signInResponse.Success = true;
                            signInResponse.Message = "查詢成功";
                            return Ok(signInResponse);
                        }
                        else
                        {
                            signInResponse.Success = true;
                            signInResponse.Message = "查無資料";
                            signInResponse.Data = new Infrastructure.ViewModel.ActivityFunction.SignIn.SignInModuleList[0];
                            return Ok(signInResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Ok(ex.Message);
                    }

                case (int)activityEnum.Vote:
                    var voteService = new VoteService();
                    var voteList = voteService.GetList(circleKey);
                    var voteResponse = new Infrastructure.ViewModel.Base.ResultBaseModel<Infrastructure.ViewModel.ActivityFunction.Vote.VoteListResponse>();
                    if (voteList == null)
                        voteResponse.Message = "無資料";
                    else
                    {
                        voteResponse.Message = "查詢成功";
                        voteResponse.Data = voteList.ToArray();
                    }
                    voteResponse.Success = true;
                    return Ok(voteResponse);
                case (int)activityEnum.Material:
                    var materialService = new MaterialService();
                    try
                    {
                        var materialList = new List<MaterialViewModel>();
                        if (pages.HasValue && rows.HasValue)
                            materialList = materialService.GetFiles(circleKey, pages.Value, rows.Value);
                        else
                            materialList = materialService.GetFiles(circleKey);
                        var materialResponse = new Infrastructure.ViewModel.Base.ResultBaseModel<MaterialViewModel>();
                        if (materialList == null)
                            materialResponse.Message = "無資料";
                        else
                        {
                            materialResponse.Message = "查詢成功";
                            materialResponse.Data = materialList.ToArray();
                        }
                        materialResponse.Success = true;
                        return Ok(materialResponse);
                    }
                    catch (Exception ex)
                    {
                        return Ok(ex.Message);
                    }

                default:
                    return Content(HttpStatusCode.InternalServerError, "錯誤的查詢");
            }

        }
    }
}
