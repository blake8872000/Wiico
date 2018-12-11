using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WiicoApi.Infrastructure.ViewModel.uDollar;
using WiicoApi.Service.CommenService;
using WiicoApi.Service.SchoolApi;

namespace WiicoApi.Controllers.api.APPSupport
{
    [EnableCors("*", "*", "*")]
    /// <summary>
    /// 取得指定帳號目前的線上uDollar餘額(已經扣除預購金額)
    /// </summary>
    public class GetuDollarOnlineBalanceController : ApiController
    {
        private readonly UDollarService uDollarService = new UDollarService();
        /// <summary>
        /// 取得指定帳號目前的線上uDollar餘額(已經扣除預購金額)
        /// </summary>
        /// <param name="strAccess">json資料,登入基本驗證資訊</param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]string strAccess)
        {
            var requestData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(strAccess);
            return MakeApiCall(requestData);
        }

        /// <summary>
        /// 取得指定帳號目前的線上uDollar餘額(已經扣除預購金額)
        /// </summary>
        /// <param name="requestData">json資料,登入基本驗證資訊</param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]Infrastructure.ViewModel.Base.BackendBaseRequest requestData)
        {
            return MakeApiCall(requestData);
        }

        private IHttpActionResult MakeApiCall(Infrastructure.ViewModel.Base.BackendBaseRequest requestData)
        {
            var response = new Infrastructure.ViewModel.Base.BaseResponse<List<OnlineBalanceResult>>();
            response.Data = new List<OnlineBalanceResult>();
            var requestService = new Service.Utility.RequestDataHelper<Infrastructure.ViewModel.Base.BackendBaseRequest>();
            var checkColumnKeys = new string[2] { "account","icantoken" };
            var checkEmpty = requestService.CheckColumnEmpty(requestData, checkColumnKeys);
            if (checkEmpty == false)
            {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.RequestDataError;
                response.Message = "參數傳遞錯誤(strAccess)";
            }
            var tokenService = new TokenService();
            var tokenInfo = tokenService.GetTokenInfo(requestData.ICanToken).Result;
            if (tokenInfo == null) {
                response.Success = false;
                response.State = Infrastructure.ViewModel.Base.LogState.Logout;
                response.Message = "已登出";
            }
            var getOnlineBalance =Convert.ToDecimal( uDollarService.GetuDollarData("GetOnlineBalance",requestData.Account));
            var getOrderAmount = uDollarService.GetuDollarData("GetOrderAmountResult", requestData.Account,false);
            var errorMsg =JsonConvert.DeserializeObject<List<uDollarErrorMsg>>(uDollarService.GetuDollarData("ParaDefintion", requestData.Account, false));
            var responseData = new OnlineBalanceResult()
            {
                account = requestData.Account,
                orderAmount = getOrderAmount != "0" ? Convert.ToInt32(getOrderAmount) : 0,
                uDollarAmount = getOnlineBalance >=0 ? getOnlineBalance : 0,
                uPursePerhapsBlance = 0
            };
            response.Message = getOnlineBalance < 0 ? errorMsg.FirstOrDefault(t => t.Value == getOnlineBalance).Description : "查詢成功";
            response.Data.Add(responseData);
            response.Success = getOnlineBalance >= 0 ? true : false;
            return Ok(response);
        }

        /// <summary>
        /// 帳號目前uDollar金額相關狀態
        /// </summary>
        public class OnlineBalanceResult
        {
            /// <summary>
            /// uDollar剩餘金額(判斷餘額是否足夠時請以此金額進行判斷)
            /// </summary>
            public decimal uDollarAmount { get; set; }
            /// <summary>
            /// 已經預購的商品金額總數
            /// </summary>
            public int orderAmount { get; set; }
            /// <summary>
            /// 查詢的帳號
            /// </summary>
            public string account { get; set; }
            /// <summary>
            /// 系統估算uPurse剩餘金額
            /// </summary>
            public decimal uPursePerhapsBlance { get; set; }
        }
    }
}

