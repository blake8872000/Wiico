using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Repository;
using static WiicoApi.Service.Utility.ParaCondition;

namespace WiicoApi.Service.SignalRService
{
    public class AttendanceRecordService
    {
        private readonly GenericUnitOfWork _uow;
        public AttendanceRecordService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 產生預設核心出缺勤資料(出席狀態:缺席)
        /// </summary>
        /// <param name="eventId">活動 Guid，可透過 Activity取得模組細節</param>
        /// <param name="memberIds">成員memberId陣列</param>
        public List<AttendanceRecord> GetInitAttendanceRecords(int circleId, Guid eventId, int[] memberIds)
        {
            // 產生成員預設出缺勤資料
            var records = new List<AttendanceRecord>();
            foreach (var man in memberIds)
            {
                var record = new AttendanceRecord()
                {
                    LearningId = circleId,
                    EventId = eventId,
                    StudId = man,
                    Status = AttendanceState.Absence,
                    UpdateTime = DateTime.UtcNow
                };
                records.Add(record);
            }
            return records;
        }


        /// <summary>
        /// 更新核心出缺勤資料表中的成員出席狀態
        /// </summary>
        /// <param name="eventId">活動 Guid，可透過 Activity取得模組細節</param>
        /// <param name="studId">成員 memberId</param>
        /// <param name="newState">新的出席狀態</param>
        public void UpdateLog(Guid eventId, int studId, string newState)
        {
            var log = (from arl in  _uow.DbContext.ActRollCallLog
                      join ar in _uow.DbContext.ActRollCall on arl.RollCallId equals ar.Id
                      where arl.StudId == studId && ar.EventId == eventId
                      select arl).FirstOrDefault();

            if (log != null)
            {
                log.Status = newState;
                log.Updated =TimeData.Create(DateTime.UtcNow);
                _uow.SaveChanges();
            }
        }
    }
}
