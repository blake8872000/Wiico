using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiicoApi.Controllers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Tests.Controllers;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Infrastructure.ViewModel;
using System.Web.Http.Results;

namespace WiicoApi.Controllers.api.Tests
{
    [TestClass()]
    public class CheckUpdateControllerTests : TestBase<CheckUpdateController, string, ResultBaseModel<AppVersionViewModel>, string, string>
    {
        [TestMethod()]
        public void GetTest()
        {
            getTestValue = "IOS";
            target = new CheckUpdateController();
            var targetResponse = target.Get(getTestValue);

            Assert.IsInstanceOfType(targetResponse, getResponse);
            var targetResData = targetResponse as OkNegotiatedContentResult<ResultBaseModel<AppVersionViewModel>>;
            Assert.IsNotNull(targetResData.Content);
            Assert.IsTrue(targetResData.Content.Success);
        }
    }
}