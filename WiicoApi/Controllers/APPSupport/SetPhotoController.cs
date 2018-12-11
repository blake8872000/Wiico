using WiicoApi.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Service.CommenService;
using WiicoApi.Infrastructure.ViewModel.Base;

namespace WiicoApi.Controllers.api.APPSupport
{
    /// <summary>
    /// 修改成員照片
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SetPhotoController : ApiController
    {
        /// <summary>
        /// 要求更新使用者照片
        /// </summary>
        /// <param name="strAccess">json資料,登入基本驗證資訊</param>
        /// <returns>使用者上傳照片結果</returns>

        public IHttpActionResult Get([FromUri]string strAccess)    
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.MemberManage.MemberPhotoRequest>(strAccess);

            if (requestData.Photo == null || requestData.ICanToken == null || requestData.Account == null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數資訊");

            var result = new Infrastructure.ViewModel.Base.BaseResponse<Infrastructure.ViewModel.MemberManage.MemberPhotoResponse>();
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (checkToken == null)
            {
                result.Success = false;
                result.Message = "已登出";
                result.State = Infrastructure.ViewModel.Base.LogState.Logout;
                return Ok(result);
            }

            var service = new FileService();
      
            var data = service.SaveMemberPhotoFile(requestData.ICanToken, requestData.Account, requestData.Photo);
            if (data == null)
            {
                result.Success = false;
                result.Message = "修改失敗";
                return Ok( result);
            }
            result.Success = true;
            result.Message = "修改成功";
            result.Data = data;
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost,Route("rest/setMyPhoto")]
        public IHttpActionResult Post() {
            var result = new ResultBaseModel<Infrastructure.ViewModel.MemberManage.MemberPhotoResponse>();


            var requestFormData = HttpContext.Current.Request.Form;
            var memberPhoto = HttpContext.Current.Request.Files;
            var token = (requestFormData["icanToken"] != null) ? requestFormData["icanToken"].ToString() : null;

            if (token == null ||  memberPhoto.Count<=0)
                return Content(HttpStatusCode.BadRequest, "遺漏參數資訊");
            var service = new FileService();
            var data =service.SaveMemberPhotoFile(token, null, new Infrastructure.ViewModel.FileViewModel() {
                ContentType = memberPhoto[0].ContentType,
                FileName = memberPhoto[0].FileName,
                InputStream = memberPhoto[0].InputStream,
                ContentLength = memberPhoto[0].ContentLength}
            );
            if (data == null)
            {
                result.Success = false;
                result.Message = "修改失敗";
                result.State = LogState.Error;
                return Ok(result);
            }
            result.Success = true;
            result.Message = "修改成功";
            result.Data = new Infrastructure.ViewModel.MemberManage.MemberPhotoResponse[1] { data };
            return Ok(result);
        }

        /// <summary>
        /// 變更大頭照
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post(Infrastructure.ViewModel.MemberManage.MemberPhotoRequest value) {
            var result = new ResultBaseModel<Infrastructure.ViewModel.MemberManage.MemberPhotoResponse>();
         
            var service = new FileService();

            var token = (value !=null &&value.ICanToken != null && value.ICanToken != string.Empty) ? value.ICanToken : null;
            var account = (value !=null && value.Account != null && value.Account != string.Empty) ? value.Account :null;

            if (token == null || account==null)
                return Content(HttpStatusCode.BadRequest, "遺漏參數資訊");

            var data = (value.Photo!=null && value.Photo!=string.Empty) ? service.SaveMemberPhotoFile(token,account,value.Photo) :  null;
            if (data == null)
            {
                result.Success = false;
                result.Message = "修改失敗";
                result.State = LogState.Error;
                return Ok(result);
            }
            result.Success = true;
            result.Message = "修改成功";
            result.Data =new Infrastructure.ViewModel.MemberManage.MemberPhotoResponse[1] { data };
            return Ok(result);
        }
    }
}
