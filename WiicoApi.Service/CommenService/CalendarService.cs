using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class CalendarService
    {
        private readonly GenericUnitOfWork _uow;
        public CalendarService()
        {
            _uow = new GenericUnitOfWork();
        }
        public string GetPeriodWeek(DateTime targetStartDate, DateTime targetEndDate, int orgId)
        {
            var db = _uow.DbContext;
            var sectionData = db.Sections.FirstOrDefault(t => t.IsNowSeme == true && t.OrgId == orgId);
            if (sectionData == null)
                return null;
            var dateTimeTools = new Utility.DateTimeTools();
            var weekDatas = dateTimeTools.GetIntervalDateList(sectionData.StartDate.ToLocalTime(), sectionData.EndDate.ToLocalTime(), 7);
            var responseData = string.Empty;
            var index = 1;
            foreach (var weekData in weekDatas)
            {
                if (
                        (weekData.Start_date.Date <= targetStartDate.Date &&
                            weekData.End_date.Date >= targetStartDate.Date) ||
                        (weekData.Start_date.Date <= targetEndDate.Date &&
                            weekData.End_date >= targetEndDate.Date) ||
                        (targetStartDate.Date <= weekData.Start_date.Date &&
                                targetEndDate >= weekData.End_date.Date)
                    )
                    responseData = string.Format("{0},{1}", responseData, index.ToString());

                index++;
            }
            responseData = responseData != string.Empty && responseData != null ? responseData.Substring(1, responseData.Length - 1) : responseData;

            return responseData;
        }


        public List<Infrastructure.ViewModel.School.GetCampusEventGetResponse> GetList
            (Infrastructure.ViewModel.Base.BackendBaseRequest requestData)
        {
            var db = _uow.DbContext;
            var responseData = new List<Infrastructure.ViewModel.School.GetCampusEventGetResponse>();
            var memberService = new MemberService();
            var memberInfo = memberService.TokenToMember(requestData.Token).Result;
            if (memberInfo == null)
                return null;

            var sectionService = new SectionService();
            var sectionData = sectionService.GetOrgNowSeme(memberInfo.OrgId);

            var dbDatas = (from c in db.Calendar
                           join m in db.Members on c.Creator equals m.Id
                           join f in db.FileStorage on c.FileId equals f.Id into ps
                           from o in ps.DefaultIfEmpty()
                           join csg in db.CalendarSemester on c.Id equals csg.CalendarId into csgg
                           from csggt in csgg.DefaultIfEmpty()
                           join cor in db.CalendarOrganizationRole on c.Id equals cor.CalendarId into corg
                           from corgt in corg.DefaultIfEmpty()
                           join cd in db.CalendarDept on c.Id equals cd.CalendarId into cdg
                           from cdgt in cdg.DefaultIfEmpty()
                           where c.OrgId == memberInfo.OrgId && (c.StartDate >= sectionData.StartDate && c.StartDate <= sectionData.EndDate)
                           select new Infrastructure.ViewModel.School.GetCampusEventGetResponse
                           {
                               Id = c.Id,
                               BoarderID = c.Code,
                               CreateDate = c.CreateDate.Value,
                               CreateMan = m.Account,
                               EndDate = c.EndDate,
                               IsBigEvent = c.IsBigEvent,
                               StartDate = c.StartDate,
                               TitleC = c.Title,
                               UpdateDate = c.UpdateDate,
                               Updater = c.Updater,
                               Url = o.FileUrl,
                               DeptId = cdgt.DeptId,
                               OrganizationRoleId = corgt.OrganizationRoleId,
                               SemesterGradeId = csggt.SemesterId
                           }).ToList();

            if (dbDatas.FirstOrDefault() == null)
                return null;

            var memberDept = memberInfo.DeptId.HasValue ? db.Depts.FirstOrDefault(t => t.Id == memberInfo.DeptId.Value) : new Infrastructure.Entity.Dept();
            var memberSemesterGradeInfo = memberInfo.SemesterGradeId.HasValue ? db.SemesterGrade.FirstOrDefault(t => t.Id == memberInfo.SemesterGradeId.Value) : new Infrastructure.Entity.SemesterGrade();
            var memberOrgRoleInfo = memberInfo.OrganizationRoleId.HasValue ? db.OrganizationRole.FirstOrDefault(t => t.Id == memberInfo.OrganizationRoleId) : new Infrastructure.Entity.OrganizationRole();
            responseData.AddRange(dbDatas);
            //整理資料
            foreach (var dbData in dbDatas)
            {
                if (dbData.Updater.HasValue)
                {
                    dbData.UpdateDate = dbData.UpdateDate.HasValue ? dbData.UpdateDate.Value.ToLocalTime() : dbData.UpdateDate;
                    dbData.UpDateMan = (dbData.UpDateMan != null && dbData.UpDateMan != string.Empty) ? dbData.UpDateMan : null;
                }
                dbData.CreateDate = dbData.CreateDate.Value.ToLocalTime();
                dbData.StartDate = dbData.StartDate.ToLocalTime();
                dbData.EndDate = dbData.EndDate.ToLocalTime();
                dbData.PeriodWeek = GetPeriodWeek(dbData.StartDate, dbData.EndDate, memberInfo.OrgId);
                //假設事件有限制學制顯示
                if (dbData.SemesterGradeId.HasValue)
                    //不是該學制的看不到
                    if (memberSemesterGradeInfo.Id <= 0 || memberSemesterGradeInfo.Id != dbData.SemesterGradeId.Value)
                        responseData.Remove(dbData);
                //事件有限制角色顯示
                if (dbData.OrganizationRoleId.HasValue)
                    //不是該角色看不到
                    if (memberOrgRoleInfo.Id <= 0 || memberOrgRoleInfo.Id != dbData.OrganizationRoleId.Value)
                        responseData.Remove(dbData);
                //事件有限制學院分類顯示
                if (dbData.DeptId.HasValue)
                    //不屬於該學院分類看不到
                    if (memberDept.Id <= 0 || memberDept.Id != dbData.OrganizationRoleId.Value)
                        responseData.Remove(dbData);
            }

            return responseData;
        }
    }
}
