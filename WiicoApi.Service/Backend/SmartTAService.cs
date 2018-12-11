using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.Backend
{

    public class SmartTAService
    {
        private readonly GenericUnitOfWork _uow;
        public SmartTAService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }

        public SmartTAService(string connectionString = null)
        {
            _uow = new GenericUnitOfWork(connectionString);
        }

        public SmartTAGetResponse GetDataByClassRoomId(string classRoomId, int orgId)
        {
            var smartTAInfo = _uow.DbContext.Members.FirstOrDefault(t => t.Name.ToLower() == classRoomId.ToLower() && t.OrgId == orgId);
            if (smartTAInfo == null)
                return null;
            return GetData(smartTAInfo.Account.ToLower());
        }
        /// <summary>
        /// 查詢資料
        /// </summary>
        /// <param name="smartAccount"></param>
        /// <returns></returns>
        public SmartTAGetResponse GetData(string smartAccount)
        {
            var SmartTAInfo = _uow.DbContext.Members.FirstOrDefault(t => t.Account.ToLower() == smartAccount.ToLower());
            if (SmartTAInfo == null)
                return null;
            var tokenService = new TokenService(_uow);
            var tokenInfo = tokenService.GetiCan6TokenByAccount(smartAccount);
            //如果smartTA沒登入過，就自動幫他登入
            if (tokenInfo == null)
            {
                var memberService = new MemberService(_uow);
                tokenInfo = tokenService.InsertUserToken("smartTA", "smartTA", SmartTAInfo, null);
            }
            var sqlDatas = (from tt in _uow.DbContext.TimeTable
                            where tt.ClassRoomId.ToLower() == SmartTAInfo.Name.ToLower()
                            select tt).ToList();
            DateTime? _now = DateTime.UtcNow;
            sqlDatas = sqlDatas.Where(t => t.StartDate.Value.Date== _now.Value.Date).ToList();
            if (sqlDatas.FirstOrDefault() == null)
                return null;
            var responseData = new SmartTAGetResponse
            {
                Token = tokenInfo.Token,
                Datas = new List<SmartTAGetResponseData>()
            };
            var datas = from d in sqlDatas
                        select new SmartTAGetResponseData
                        {
                            CircleKey = d.Course_No.ToLower(),
                            StartTime = d.StartDate.Value.ToLocalTime(),
                            EndTime = d.EndDate.Value.ToLocalTime()
                        };
            responseData.Datas = datas.ToList();
            return responseData;
        }

        /// <summary>
        /// 建立SmartTA與學習圈的關聯
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public SmartTAGetResponse InsertRelation(SmartTAPostRequest requestData)
        {
            if (requestData.CircleKeys.FirstOrDefault() == null)
                return null;
            var learningCircleService = new LearningCircleService(_uow);
            int? orgId = 1;
            foreach (var circlekey in requestData.CircleKeys)
            {

                var learningCircleInfo = learningCircleService.GetDetailByOuterKey(circlekey.ToLower());
                if (learningCircleInfo == null)
                    return null;
                orgId = learningCircleInfo.OrgId;
                var smartTAInfo = _uow.DbContext.Members.FirstOrDefault(t=>t.Name.ToLower()==requestData.ClassRoomId && t.OrgId==learningCircleInfo.OrgId);
                if (smartTAInfo == null)
                    return null;
                //確認是否已經有smartTA存在於學習圈內
                var checkHadSmartTA = _uow.DbContext.SmartTAJoinGroupLog.Where(t => t.CircleKey.ToLower() == circlekey.ToLower());
                //要刪掉該smartTA的關聯
                if (checkHadSmartTA.FirstOrDefault() != null)
                {
                    foreach (var smartTA in checkHadSmartTA)
                    {
                        smartTA.Enabled = false;
                        smartTA.CreateUtcDate = DateTime.UtcNow;
                    }
                }
                var checkWasInserted = checkHadSmartTA.FirstOrDefault(t => t.SmartTAName.ToLower() == requestData.ClassRoomId.ToLower());
                //如果有加入過就修改為true 
                if (checkWasInserted != null)
                {
                    checkWasInserted.Enabled = true;
                    checkWasInserted.CreateUtcDate = DateTime.UtcNow;
                }
                //沒有就新增一筆關聯
                else
                {
                    var insertEntity = new SmartTAJoinGroupLog()
                    {
                        CircleKey = circlekey.ToLower(),
                        CreateUtcDate = DateTime.UtcNow,
                        SmartTAName = requestData.ClassRoomId.ToLower(),
                        Enabled = true,
                        ConnectionId = smartTAInfo.ConnectionId
                    };
                    _uow.DbContext.SmartTAJoinGroupLog.Add(insertEntity);
                }

                if (learningCircleInfo == null)
                    continue;
                /*更新學習圈課綱資料*/
                var weekTableDatas = _uow.DbContext.WeekTable.Where(t => t.LearningCircleId == learningCircleInfo.Id);
                foreach (var weektableData in weekTableDatas)
                {
                    weektableData.ClassRoomId = smartTAInfo.Name;
                    //weektableData.Place = smartTAInfo.Name;
                    weektableData.Updater = smartTAInfo.Id;
                    weektableData.UpdateUtcDate = DateTime.UtcNow;
                }
                var timeTableDatas = _uow.DbContext.TimeTable.Where(t => t.Course_No.ToLower() == circlekey.ToLower());
                foreach (var timeTableData in timeTableDatas)
                {
                    timeTableData.OriginClassRoomID = timeTableData.ClassRoomId;
                    timeTableData.OriginClassRoomName = timeTableData.ClassRoom;
                    timeTableData.ClassRoomId = smartTAInfo.Name;
                    //   timeTableData.ClassRoom = smartTAInfo.Name;
                    timeTableData.UpdateDate = DateTime.UtcNow;
                }
            }
            _uow.SaveChanges();
            return GetDataByClassRoomId(requestData.ClassRoomId, orgId.Value);
        }

        /// <summary>
        /// 取得學習圈相關的所有smartTA
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public List<SmartTAJoinGroupLog> GetSmartTAs(string circleKey, bool? enable = null)
        {
            var responseData = enable.HasValue ?
                _uow.DbContext.SmartTAJoinGroupLog.Where(t => t.CircleKey.ToLower() == circleKey.ToLower() && t.Enabled == enable.Value) :
                _uow.DbContext.SmartTAJoinGroupLog.Where(t => t.CircleKey.ToLower() == circleKey.ToLower());
            return responseData.FirstOrDefault() == null ? null : responseData.ToList();
        }

        /// <summary>
        /// 取得詳細關聯資料
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="smartTA"></param>
        /// <returns></returns>
        public SmartTAJoinGroupLog GetDetail(string circleKey, string smartTA)
        {
            var responseData = _uow.DbContext.SmartTAJoinGroupLog.FirstOrDefault(t => t.CircleKey.ToLower() == circleKey.ToLower() && t.SmartTAName.ToLower() == smartTA.ToLower());
            return responseData;
        }
    }
}
