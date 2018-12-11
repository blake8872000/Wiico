using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SchoolApi
{
    public class SceLearningMapService
    {
        private readonly GenericUnitOfWork _uow;
        public SceLearningMapService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 計算已經取得的學分數
        /// 00:尚未歸屬
        /// 10:共同必修
        /// 20:專業必修
        /// 30:專業選修
        /// 31:專業選修主
        /// 32:專業選修副
        /// 40:選修
        /// 50:基礎課程
        /// 90:未列入畢業學分
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public decimal GetCorrespondCredit(List<Infrastructure.ViewModel.School.learningMapColls> data)
        {
            decimal notCorrespond = 0; //未對應
            decimal summaryCredit = 0;
            decimal notInGoal = 0; //未列入畢業學分

            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    if (item.ID_CredType == "00")
                    {
                        //未對應加總
                        notCorrespond += item.Stud_SumCredit.Value;
                    }
                    if (item.ID_CredType == "90")
                    {
                        //未對應加總
                        notInGoal += item.Stud_SumCredit.Value;
                    }
                    else if (item.ID_CredType == "50")
                    {
                        //碩專不計算基礎課程學分
                        continue;
                    }

                    summaryCredit += item.Stud_SumCredit.Value;
                }

                return summaryCredit - notCorrespond - notInGoal;
            }
            else
            {
                return 0;
            }
        }


        public Infrastructure.ViewModel.School.GetLearningMapDataResponse GetData(string token)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(token).Result;
            if (memberInfo == null)
                return null;

            var db = _uow.DbContext;
            var isTeacher = memberInfo.OrganizationRoleId.HasValue ? db.OrganizationRole.FirstOrDefault(t => t.Id == memberInfo.OrganizationRoleId).IsAdmin : true;
            if (isTeacher)
                return null;

            using (var client = new HttpClient())
            {
                var requestUri = string.Format("http://140.137.200.178/API/learningmap/{0}", memberInfo.Account.ToLower());
                client.Timeout = TimeSpan.FromSeconds(10);
                try
                {
                    var response = client.GetAsync(requestUri).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        var datas = JsonConvert.DeserializeObject<Infrastructure.ViewModel.School.SceLearningMapResponse>(responseContent);
                        var responseData = new Infrastructure.ViewModel.School.GetLearningMapDataResponse()
                        {
                            BriefName_coll = datas.BriefName_coll,
                            Colls = new List<Infrastructure.ViewModel.School.learningMapColls>(),
                            Coll_SemeGrade = (datas.Coll_SemeGrade != null && datas.Coll_SemeGrade != string.Empty) ?
                                                                Convert.ToInt32(datas.Coll_SemeGrade) :
                                                                0,
                            Curss = new List<Infrastructure.ViewModel.School.LearningMapCurss>(),
                            Domas = new List<Infrastructure.ViewModel.School.LearningMapDomas>(),
                            GradCredit = (datas.GradCredit != null && datas.GradCredit != string.Empty) ?
                                                     Convert.ToDecimal(datas.GradCredit) :
                                                     0,
                            ID_coll = datas.ID_coll,
                            ID_MainDoma = datas.ID_MainDoma,
                            ID_syst = datas.ID_syst /* (datas.ID_syst !=null && datas.ID_syst !=string.Empty) ?
                                           Convert.ToInt32(datas.ID_syst):
                                           0*/,
                            Name_coll = datas.Name_coll,
                            StudStatus = datas.StudStatus,
                            Stud_Grade = (datas.Stud_Grade != null && datas.Stud_Grade != string.Empty) ?
                                                        Convert.ToInt32(datas.Stud_Grade) :
                                                        0,
                            Stud_SchlInYear = (datas.Stud_SchlInYear != null && datas.Stud_SchlInYear != string.Empty) ?
                                                                Convert.ToInt32(datas.Stud_SchlInYear) :
                                                                0
                        };
                        if ((datas.Curss == null && datas.Colls == null && datas.Domas == null) ||
                            (datas.Curss.Count <= 0 && datas.Domas.Count <= 0 && datas.Colls.Count <= 0))
                            return responseData;
                        var collsDatas = (from cs in datas.Colls
                                          select new Infrastructure.ViewModel.School.learningMapColls
                                          {
                                              CombinedDoma = Convert.ToBoolean(cs.CombinedDoma),
                                              Cred_Coll = (cs.Cred_Coll != string.Empty && cs.Cred_Coll != null) ?
                                                                     Convert.ToDecimal(cs.Cred_Coll) :
                                                                     0,
                                              ID_CredType = cs.ID_CredType/*(cs.ID_CredType!=null && cs.ID_CredType!=string.Empty)? 
                                                                     Convert.ToInt32(cs.ID_CredType):
                                                                     0*/,
                                              Memo_Cred = cs.Memo_Cred,
                                              NAME_CredType = cs.NAME_CredType,
                                              Stud_SumCredit = (cs.Stud_SumCredit != null && cs.Stud_SumCredit != string.Empty) ?
                                                                                  Convert.ToDecimal(cs.Stud_SumCredit) :
                                                                                  0
                                          }).ToList();

                        var domaDatas = (from dm in datas.Domas
                                         select new Infrastructure.ViewModel.School.LearningMapDomas
                                         {
                                             Cred_Doma = (dm.Cred_Doma != null && dm.Cred_Doma != string.Empty) ?
                                                                        Convert.ToDecimal(dm.Cred_Doma) :
                                                                        0,
                                             ID_CredType = dm.ID_CredType/*(dm.ID_CredType != null && dm.ID_CredType != string.Empty) ?
                                                                   Convert.ToInt32(dm.ID_CredType) :
                                                                   0*/,
                                             ID_Doma = dm.ID_Doma,
                                             Name_Doma = dm.Name_Doma,
                                             OrderNo_Doma = (dm.OrderNo_Doma != null && dm.OrderNo_Doma != string.Empty) ?
                                                                        Convert.ToInt32(dm.OrderNo_Doma) :
                                                                        0,
                                             Stud_Doma_SumCredit = (dm.Stud_Doma_SumCredit != null && dm.Stud_Doma_SumCredit != string.Empty) ?
                                                                        Convert.ToDecimal(dm.Stud_Doma_SumCredit) :
                                                                        0
                                         }).ToList();

                        var curssDatas = (from cs in datas.Curss
                                          select new Infrastructure.ViewModel.School.LearningMapCurss
                                          {
                                              Credit = (cs.Credit != null && cs.Credit != string.Empty) ?
                                                                        Convert.ToDecimal(cs.Credit) :
                                                                        0,
                                              ID_CredType = cs.ID_CredType/*(cs.ID_CredType != null && cs.ID_CredType != string.Empty) ?
                                                                   Convert.ToInt32(cs.ID_CredType) :
                                                                   0*/,
                                              ID_Curs = cs.ID_Curs,
                                              ID_Doma = cs.ID_Doma,
                                              Memo = cs.Memo,
                                              Name_Clop = cs.Name_Clop,
                                              Name_Curs = cs.Name_Curs,
                                              OrderNo_Curs = (cs.OrderNo_Curs != null && cs.OrderNo_Curs != string.Empty) ?
                                                                        Convert.ToInt32(cs.OrderNo_Curs) :
                                                                        0,
                                              Passed = (cs.Passed != null && cs.Passed != string.Empty) ?
                                                                        Convert.ToBoolean(cs.Passed) :
                                                                        false,
                                              SumCredit = (cs.SumCredit != null && cs.SumCredit != string.Empty) ?
                                                                        Convert.ToDecimal(cs.SumCredit) :
                                                                        0
                                          }).ToList();
                        responseData.Curss.AddRange(curssDatas);
                        responseData.Domas.AddRange(domaDatas);
                        responseData.Colls.AddRange(collsDatas);
                        return responseData;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
        }
    }
}
