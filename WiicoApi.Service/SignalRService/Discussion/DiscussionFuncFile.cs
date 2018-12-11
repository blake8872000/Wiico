using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SignalRService.Discussion
{
    public class DiscussionFuncFile
    {
        private readonly GenericUnitOfWork _uow = new GenericUnitOfWork();

        /// <summary>
        /// 查詢主題討論的檔案
        /// </summary>
        /// <param name="outerKey">活動代碼</param>
        /// <returns></returns>
        public List<FileStorageViewModel> GetDiscussionFileByOuterKey(string outerKey)
        {
            var eventId = Utility.OuterKeyHelper.PageTokenToGuid(outerKey);
            return GetDiscussionFileByEventId(eventId);
        }

        /// <summary>
        /// 查詢主題討論的檔案
        /// </summary>
        /// <param name="eventId">活動代碼</param>
        /// <returns></returns>
        public List<FileStorageViewModel> GetDiscussionFileByEventId(Guid eventId)
        {
            var fileService = new FileService();
            var db = _uow.DbContext;
            var discussionService = new DiscussionService();
            var discussionInfo = discussionService.GetDBDiscussionInfo(eventId.ToString());
            var sqlData = new List<FileStorage>();
            if (discussionInfo.GoogleDriveUrl == null)
            {
                sqlData = (from f in db.FileStorage
                           join df in db.DiscussionFile on f.Id equals df.FileId
                           join d in db.ActDiscussion on df.DiscussionId equals d.Id
                           where d.EventId == eventId && df.MessageId == null
                           select f).ToList();
            }

            if (sqlData.FirstOrDefault() != null)
                return fileService.ImageFileProcess(sqlData);
            else
                return null;
        }

        /// <summary>
        /// 刪除檔案關聯
        /// </summary>
        /// <param name="discussionId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool DeleteDiscussionFileReference(int discussionId, string[] files)
        {
            var db = _uow.DbContext;
            var checkDiscussionFile = db.DiscussionFile.Where(t => t.DiscussionId == discussionId);
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var file in files)
                    {
                        var fileId = Convert.ToInt32(file);
                        var dbData = checkDiscussionFile.FirstOrDefault(t => t.FileId == fileId);
                        if (dbData != null)
                            db.DiscussionFile.Remove(dbData);
                    }
                    db.SaveChanges();
                    dbTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// 建立主題討論/訊息 與檔案之間的關聯
        /// </summary>
        /// <param name="discussionId">主題討論編號</param>
        /// <param name="fileList">檔案列表</param>
        /// <param name="messageId">訊息編號 - 可NULL代表是要建立主題討論與檔案的關聯</param>
        /// <returns></returns>
        public bool DiscussionFileReference(int discussionId, List<FileStorageViewModel> fileList, int? messageId)
        {
            var db = _uow.DbContext;
            var checkDiscussionFile = db.DiscussionFile.Where(t => t.DiscussionId == discussionId);
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var file in fileList)
                    {
                        if (checkDiscussionFile.FirstOrDefault(t => t.FileId == file.Id) == null)
                        {
                            var addEntity = new DiscussionFile()
                            {
                                FileId = file.Id,
                                DiscussionId = discussionId,
                                Creator = file.Creator,
                                CreateUtcDate = DateTime.UtcNow,
                                MessageId = messageId
                            };
                            db.DiscussionFile.Add(addEntity);
                        }
                    }
                    db.SaveChanges();
                    dbTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    return false;
                }
            }
        }
    }
}
