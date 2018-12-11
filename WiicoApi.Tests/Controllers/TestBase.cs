using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;

namespace WiicoApi.Tests.Controllers
{
    /// <summary>
    /// 測試Base
    /// </summary>
    /// <typeparam name="ControllerT">要測試的controllers</typeparam>
    /// <typeparam name="GetRequestT">測試Get的參數</typeparam>
    /// <typeparam name="GetResponseT">測試Get的回傳值</typeparam>
    /// <typeparam name="PostRequestT">測試Post的參數</typeparam>
    /// <typeparam name="PostResponseT">測試Post的回傳值</typeparam>
    [TestClass]
    public class TestBase<ControllerT,GetRequestT,GetResponseT,PostRequestT,PostResponseT>
    {
        /// <summary>
        /// Get測試參數
        /// </summary>
        public GetRequestT getTestValue { get ; set; }
        /// <summary>
        /// Get測試回傳值
        /// </summary>
        public Type getResponse { get { return typeof(OkNegotiatedContentResult<GetResponseT>); } }
        /// <summary>
        /// Post測試參數
        /// </summary>
        public PostRequestT postTestValue { get; set; }
        /// <summary>
        /// Post測試回傳值
        /// </summary>
        public Type postResponse { get { return typeof(OkNegotiatedContentResult<PostResponseT>);} }
        /// <summary>
        /// 測試Controller
        /// </summary>
        public ControllerT target{ get; set; }
        public TestBase()
        {
        }       
    }
}
