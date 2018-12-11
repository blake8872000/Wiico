using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Controllers.Common;
using WiicoApi.Infrastructure.DataTransferObject;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SignalRService;
using WiicoApi.SignalR;
using WiicoApi.SignalRHub;

namespace WiicoApi.Controllers.APPSupport
{

    /// <summary>
    /// 活動牆圖片上傳
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class MaterialUploadFIleController : MultipartFormDataFilesController<UploadFileModel>
    {
        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();
        private MaterialService materialService = new MaterialService();
        /// <summary>
        /// 取得檔案詳細資訊
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(Guid eventId)
        {
            var service = new MaterialService();
            var data = service.GetFile(eventId.ToString());
            return Ok(data);
        }

        /// <summary>
        /// 給APP用的新增教材活動
        /// </summary>
        [HttpPost]
        public async Task<IHttpActionResult> POST()
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<string>();
            response.Success = false;
            response.Data = "";
            await SetFileData();
            multipartFormDataModel = new UploadFileModel();
            if (multipartFormDataStreamProvider.FormData == null)
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            //設定參數
            var token = multipartFormDataStreamProvider.FormData.Get("icantoken") != null ? multipartFormDataStreamProvider.FormData.GetValues("icantoken")[0] : null;
            var circleKey = multipartFormDataStreamProvider.FormData.Get("classid") != null ? multipartFormDataStreamProvider.FormData.GetValues("classid")[0] : null;
            var clientkey = multipartFormDataStreamProvider.FormData.Get("clientkey") != null ? multipartFormDataStreamProvider.FormData.GetValues("clientkey")[0] : null;
           
            if (token == null || circleKey == null || clientkey == null )
            {
                response.Message = "遺漏參數";
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                return Content(HttpStatusCode.BadRequest, response);
            }

            multipartFormDataModel.Token = Guid.Parse(token);
            multipartFormDataModel.CircleKey = circleKey.ToLower();
            multipartFormDataModel.ClientKey = clientkey;
            
            //新增上傳檔案活動
            var objHub = GlobalHost.ConnectionManager.GetHubContext<WiicoApi.SignalRHub.WiicoHub>();
            #region 舊的設定參數方法
          /*  var _info = HttpContext.Current.Request.Form;
            var files = HttpContext.Current.Request.Files;


            //文大版 參數
            if (_info["token"] != null)
            {
                uploadInfo.CircleKey = _info["circleKey"].ToString();
                uploadInfo.ClientKey = _info["clientKey"].ToString();
                uploadInfo.Token = Guid.Parse(_info["token"].ToString());
            }
            //雲端版 參數
            if (_info["icantoken"] != null)
            {
                uploadInfo.Token = Guid.Parse(_info["icantoken"].ToString());
                uploadInfo.CircleKey = _info["classid"].ToString();
                uploadInfo.ClientKey = _info["clientkey"].ToString();
            }*/
            //uploadInfo.Title = _info["title"].ToString();
            //uploadInfo.Content = _info["content"].ToString();
            #endregion
            var data = materialService.Insert(multipartFormDataModel.CircleKey, multipartFormDataModel.Token.ToString(), multipartFormDataFiles, fileStreams.ToArray());
            var rtn = materialService.addPanelInfo(data.OuterKey, Convert.ToInt32(data.Creator), multipartFormDataModel.CircleKey);

            objHub.Clients.Group(multipartFormDataModel.CircleKey.ToLower()).appendActivity(rtn, multipartFormDataModel.ClientKey.ToLower());
            response.Success = true;
            response.Message = "上傳成功";
            response.Data = "上傳成功";
            //單元測試結果
            if (HttpContext.Current == null)
                return Ok(response);

            var signalrService = new SignalrService();
            //發通知
            var connectIdAndData = signalrService.GetConnectIdAndData(multipartFormDataModel.CircleKey, Convert.ToInt32(data.Creator), SignalrConnectIdType.All, SignalrDataType.Activity);
            SignalrClientHelper.ShowRecordListById(connectIdAndData);
            //發通知
            // ShowRecordList(uploadInfo.CircleKey.ToLower());
            // 推播
            PushMaterialOnCreatedAsync(multipartFormDataModel.CircleKey.ToLower(), data.OuterKey, multipartFormDataModel.Title, Convert.ToInt32(data.Creator));
            return Ok(response);
        }

        /// <summary>
        /// 老師新增教材
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="myId"></param>
        /// <returns></returns>
        public async Task PushMaterialOnCreatedAsync(string circleKey, Guid eventId, string eventName, int? myId)
        {
            var cacheService = new CacheService();
            var members = cacheService.GetCircleMemberList(circleKey,myId);
            var memberService = new MemberService();
            var memberInfo = memberService.UserIdToAccount(Convert.ToInt32(myId));
            var eventMessage = string.Format("{0}傳送了圖片", memberInfo.Name, eventName);
            var pushService = new PushService();
            await pushService.PushMsgAsync("ToEventCard", circleKey, eventId, "推播_查看新的-互動圖片", members.ToArray(), eventMessage);

        }

        private void ShowRecordList(string circleKey)
        {
            var _actService = new ActivityService();
            var learningCircleService = new LearningCircleService();

            var members = learningCircleService.GetCircleMemberIdList(circleKey);
            foreach (var mem in members)
            {
                var myConn = HttpContext.Current.Cache.Get(mem.ToString()) as List<string>;
                if (myConn != null)
                {
                    var data = _actService.GetLatestList(mem, "");
                    var objHub = GlobalHost.ConnectionManager.GetHubContext<SignalRHub.WiicoHub>();

                    // 所有歸屬在這個id下的connection都會收到
                    foreach (var connId in myConn)
                    {
                        objHub.Clients.Client(connId).showRecordList(data);
                    }
                }
            }
        }


    }
}

