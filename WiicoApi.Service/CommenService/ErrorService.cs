using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    public class ErrorService
    {
        private readonly GenericUnitOfWork _uow;

        public ErrorService()
        {
            _uow = new GenericUnitOfWork();
        }
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="seachString"></param>
        /// <param name="errorType"></param>
        /// <param name="isFix"></param>
        /// <returns></returns>
        public IEnumerable<SystemErrorLog> GetList(string seachString, int? errorType = null, bool? isFix = null)
        {
            var db = _uow.DbContext;
            var result = from el in db.SystemErrorLog
                         join st in db.SystemErrorType on el.ErrorType equals st.Id
                         select el;
            if (seachString != null && seachString != string.Empty)
                result = result.Where(t => t.Description.StartsWith(seachString));

            if (errorType.HasValue)
                result = result.Where(t => t.ErrorType == errorType.Value);

            if (isFix.HasValue)
                result = result.Where(t => t.IsFix == isFix.Value);

            if (result.FirstOrDefault() == null)
                return null;

            return result.ToList();
        }

        /// <summary>
        /// 建立errorLog
        /// </summary>
        /// <param name="errorType"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public SystemErrorLog InsertError(int errorType, string description)
        {
            var db = _uow.DbContext;
            var entity = new Infrastructure.Entity.SystemErrorLog()
            {
                CreateUtcDate = DateTime.UtcNow,
                Description = description,
                ErrorType = errorType,
                IsFix = false
            };
            db.SystemErrorLog.Add(entity);
            db.SaveChanges();
            return entity;
        }
        /// <summary>
        /// 更新錯誤紀錄狀態
        /// </summary>
        /// <param name="errorId"></param>
        /// <param name="isfix"></param>
        /// <returns></returns>
        public bool UpdateFixState(int[] errorIds, bool isfix)
        {
            var db = _uow.DbContext;
            foreach (var errorId in errorIds)
            {
                var updateEntity = db.SystemErrorLog.FirstOrDefault(t => t.Id == errorId);
                updateEntity.IsFix = isfix;
            }
            db.SaveChanges();
            return true;
        }
    }
}