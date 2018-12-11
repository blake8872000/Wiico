using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SchoolApi
{
    public class SignInSyncService/*appiCanAPI*/
    {
        private readonly string apiHost = ConfigurationManager.AppSettings["appiCanAPI"].ToString();
        private readonly GenericUnitOfWork _uow;
        public SignInSyncService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 同步點名資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public LogState SyncToiCan(Infrastructure.ViewModel.School.SignInSynchronize.SignSyncClientRequest requestData)
        {

            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(requestData.Token).Result;
            if (memberInfo == null)
                return LogState.Logout;

            var db = _uow.DbContext;
            var isTeacher = memberInfo.OrganizationRoleId.HasValue ? db.OrganizationRole.FirstOrDefault(t => t.Id == memberInfo.OrganizationRoleId).IsAdmin : true;
            if (isTeacher == false)
                return LogState.Error;

            var iCanRequestData = new Infrastructure.ViewModel.School.SignInSynchronize.SignSynciCanRequest
            {
                Course_no = requestData.ClassID.ToUpper(),
                Syll_id = requestData.syll_id,
                Times = requestData.Times.ToString(),
                Token = requestData.Token.ToUpper()
            };

            var iCanMemberRequestDatas = from rmd in requestData.MemberList
                                         select new Infrastructure.ViewModel.School.SignInSynchronize.SignSynciCanMemberListRequest
                                         {
                                             Manno = rmd.StudID,
                                             Status = Convert.ToInt32(string.Format("{0}0", (int)rmd.Status))
                                         };
            iCanRequestData.ClassmatesStatus = JsonConvert.SerializeObject(iCanMemberRequestDatas);
            var iCanRequestJson = JsonConvert.SerializeObject(iCanRequestData);
            using (var client = new HttpClient())
            {
                var requestUri = string.Format("{0}{1}", apiHost, "UploadAllClassmatesBySyllId");
                var httpContent = new StringContent(iCanRequestJson, Encoding.UTF8, "application/json");
                var response = client.PostAsync(requestUri, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonConvert.DeserializeObject<Infrastructure.ViewModel.Base.BackendBaseRequest>(response.Content.ReadAsStringAsync().Result);
                    if (responseData.Success)
                        return LogState.Suscess;
                    else
                        return LogState.Error;
                }
                else
                    return LogState.NoAccount;
            }
        }
    }
}
