using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.Common
{
    /// <summary>
    /// 氣象局API
    /// </summary>
    [EnableCors("*","*","*")]
    public class OpenWeatherController : ApiController
    {
        /// <summary>
        /// 取得資料
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get(string methods = null,string location ="臺北市",string qs=null) {
            var service = new OpenWeatherAPIService();
            methods = methods == null ? "F-C0032-001" : methods;
            var parameters = string.Format("format=json&locationName={0}", location);
            var responseData = qs ==null ? service.GetDatas(methods, parameters) : service.GetDatas(methods,qs);

            return Ok(responseData);
        }
    }

}
