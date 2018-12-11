using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SchoolApi
{
    public class PDSService
    {
        public Infrastructure.ViewModel.School.PdsFlowModel GetData(int semesterYear, string collCode, string token)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            if (memberInfo == null)
                return null;

            using (var client = new HttpClient())
            {
                var requestUri = string.Format("http://140.137.200.178/API/PDS_Flow/{0}/{1}/{2}", semesterYear, collCode, memberInfo.Account.ToLower());
                var response = client.GetAsync(requestUri).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var datas = JsonConvert.DeserializeObject<Infrastructure.ViewModel.School.PdsFlowModel>(responseContent);
                    if (datas.PDS_Flows == null)
                        return datas;
                    foreach (var data in datas.PDS_Flows)
                    {
                        data.Flow_status = data.Flow_status == null ? Infrastructure.ViewModel.School.PdsEnums.UnComplete : data.Flow_status.Value;
                    }
                    return datas;
                }
                else
                    return null;
            }

        }
    }
}
