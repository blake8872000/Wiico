using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.School;
using WiicoApi.Infrastructure.ViewModel.School.FeedBack;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{
    public class FeedBackService
    {
        private readonly GenericUnitOfWork _uow;

        public FeedBackService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public FeedBackGetResponse GetList(int orgId, int? pages = 1, int? rows = 20)
        {
            var responseDataCount = _uow.DbContext.FeedBack.Where(t => t.OrgId == orgId && t.Enable == true).Count();
            var responseData = _uow.DbContext.FeedBack.Where(t => t.OrgId == orgId).OrderBy(t => t.Id).Skip((pages.Value - 1) * rows.Value).Take(rows.Value).ToList();
            var response = new FeedBackGetResponse();
            if (responseData != null && responseDataCount > 0)
            {
                foreach (var data in responseData)
                {
                    //設定建立者帳號
                    data.Account = data.Creator > 0 ?
                        (_uow.DbContext.Members.Find(data.Creator) != null ?
                                    _uow.DbContext.Members.Find(data.Creator).Account :
                                    null) :
                        null;
                    //設定當地時間
                    data.CreateTime = data.CreateTime != null ? data.CreateTime.ToLocalTime() : data.CreateTime;
                    //設定修改人帳號
                    data.UpdateAccount = data.Updater.HasValue ?
                        (_uow.DbContext.Members.Find(data.Updater) != null ?
                                    _uow.DbContext.Members.Find(data.Updater).Account :
                                    null) :
                         null;
                    //設定修改時間為當地時間
                    data.UpdateTime = data.UpdateTime.HasValue ? data.UpdateTime.Value.ToLocalTime() : data.UpdateTime;
                }
                response.FeedBacks = responseData;
                var pagesRowsTools = new Service.Utility.PagesRowsTools();
                response.NextPage = pagesRowsTools.NextPage(pages.Value, responseDataCount, rows.Value);
                response.LastPage = pages.Value - 1;
                response.SumPages = pagesRowsTools.CountPage(responseDataCount, rows.Value);
                return response;
            }
            else
                return null;

        }

        /// <summary>
        /// 建立一筆問題回報
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public FeedBack Insert(FeedBackPostRequest requestData)
        {
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(requestData.Token).Result;
            if (memberInfo == null)
                return null;

            var entity = new FeedBack()
            {
                Account = memberInfo.Account,
                CreateTime = DateTime.UtcNow,
                Creator = memberInfo.Id,
                Description = requestData.Description,
                Email = requestData.Email,
                Enable = true,
                FeedBackType = requestData.FeedBackType,
                OrgId = memberInfo.OrgId,
                Status = 0,
                System = requestData.System
            };
            try
            {
                _uow.FeedBackRepo.Insert(entity);
                _uow.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                var errorService = new ErrorService();
                errorService.InsertError(2, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 更新資料
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>

        public FeedBack Update(FeedBackPutRequest requestData)
        {

            var checkData = _uow.FeedBackRepo.Get(t => t.Enable == true && t.Id == requestData.Id).FirstOrDefault();
            if (checkData == null)
                return null;
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(requestData.Token).Result;
            if (memberInfo == null)
                return null;

            checkData.Note = (requestData.Note != null && requestData.Note != string.Empty) ?
                                                requestData.Note :
                                                checkData.Note;
            checkData.ReContent = (requestData.ReContent != null && requestData.ReContent != string.Empty) ?
                                                requestData.ReContent :
                                                checkData.ReContent;
            checkData.Status = requestData.Status;
            checkData.Updater = memberInfo.Id;
            checkData.UpdateTime = DateTime.UtcNow;

            _uow.SaveChanges();
            return checkData;
        }
    }
}
