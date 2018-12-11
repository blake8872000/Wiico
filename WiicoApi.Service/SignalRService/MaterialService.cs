using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Repository;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Service.SignalRService
{
    public class MaterialService
    {
        private readonly GenericUnitOfWork _uow;

        private readonly CacheService cacheService = new CacheService();
        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();
        /// <summary>
        /// 存檔Server
        /// </summary>
        private string loginServer = ConfigurationManager.AppSettings["loginServer"].ToString();
        private readonly string systemManager = ConfigurationManager.AppSettings["googleDriveSystemManager"].ToString();

        /// <summary>
        /// 預設縮圖最大寬度
        /// </summary>       
        private readonly int maxImgWidth = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgWidth"]);
        /// <summary>
        /// 預設縮圖最大高度
        /// </summary>
        private readonly int maxImgHeight = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgHeight"]);
        public MaterialService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 判斷是否為雲端檔案
        /// </summary>
        /// <param name="fileOuterKey"></param>
        /// <returns></returns>
        public bool CheckFileIsColud(string fileOuterKey)
        {
            var db = _uow.DbContext;
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(fileOuterKey);
            if (eventId.HasValue == false)
                return false;
            return false;
        }

        public List<MaterialViewModel> GetFiles(string circleKey, int? pages = 1, int? rows = 20) {
            var response = new List<MaterialViewModel>();
            var moduleActs =(from a in  _uow.DbContext.Activitys
                            where a.ModuleKey.ToLower() == Infrastructure.ValueObject.activityEnum.Material.ToString().ToLower() && a.ToRoomId.ToLower() == circleKey.ToLower() && a.CardisShow == true
                            select a).OrderByDescending(t => t.Publish_Utc).Skip((pages.Value - 1) * rows.Value)
                            .Take(rows.Value);
            if (moduleActs.FirstOrDefault() == null)
                return null;

            var responseData = from ma in moduleActs.ToList()
                               join am in _uow.DbContext.ActMaterial on ma.OuterKey equals am.EventId
                               join fs in _uow.DbContext.FileStorage on am.EventId equals fs.FileGuid

                               select new MaterialViewModel
                               {
                                   ActivityId = am.Id,
                                   LearningId = am.LearningId,
                                   EventId = ma.OuterKey,
                                   OuterKey = ma.OuterKey,
                                   sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(ma.OuterKey),
                                   StartDate = am.Created.Local.Value,
                                   GoogleDriveFileId = fs.FileGuid.ToString("N"),
                                   Creator = ma.CreateUser.Value.ToString(),
                                   CreateDate = Convert.ToDateTime(am.Created.Local.ToString()),
                                   Created_Utc = Convert.ToDateTime(am.Created.Utc.Value),
                                   ModuleKey = Utility.ParaCondition.ModuleType.Material,
                             //      CreatorAccount = memberInfo.Account,
                             //      CreatorName = memberInfo.Name,
                                //   CreatorPhoto = memberInfo.Photo,
                                   Name = am.Name,
                                   FileType = am.FileType,
                                   FileLength = am.FileLength,
                                   ToRoomId = ma.ToRoomId.ToLower(),
                                   FileImgUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, fs.FileGuid.ToString("N"), maxImgWidth, maxImgHeight),
                                   FileDownLoadUrl = fs.FileUrl,
                                   FileWebViewUrl = fs.FileUrl,
                                   FolderId = null,
                                   Publish_Utc = ma.Publish_Utc,
                                   Rows = rows.Value,
                                   Pages = pages.Value
                               };
            //foreach (var act in moduleActs.ToList())
            //{
            //    if (act.OuterKey == null)
            //        continue;
            //    var material = GetFile(act.OuterKey.ToString());
            //    if (material != null)
            //        response.Add(material);
            //}
            return responseData.ToList();
     }

        public List<MaterialViewModel> GetList(string circleKey, int? pages = 1, int? rows = 20)
        {
            var db = _uow.DbContext;
            var response = new List<MaterialViewModel>();
            var moduleActs = db.Activitys.Where(t => t.ModuleKey.ToLower()==(Infrastructure.ValueObject.activityEnum.Material.ToString().ToLower()) && t.ToRoomId.ToLower()==circleKey.ToLower() && t.CardisShow == true).OrderByDescending(t => t.Publish_Utc).Skip((pages.Value - 1) * rows.Value).Take(rows.Value);
            if (moduleActs.FirstOrDefault() == null)
                return null;
            foreach (var act in moduleActs.ToList())
            {
                if (act.OuterKey == null)
                    continue;
                var material = GetFile(act.OuterKey.ToString());
                if (material != null)
                    response.Add(material);
            }
            return response;
        }

        /// <summary>
        /// 取得檔案資訊
        /// </summary>
        /// <param name="outerKey"></param>
        /// <returns></returns>
        public MaterialViewModel GetFile(string outerKey)
        {
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            if (eventId.HasValue)
            {
                var db = _uow.DbContext;
                var materialInfo = db.ActMaterial.FirstOrDefault(x => x.EventId == eventId);
                if (materialInfo == null)
                    return null;
                if (materialInfo.GoogleDriveFolder == null)
                    return GetFileFromFileStorage(eventId.Value);
                else
                    return GetFileInfoListForDB(eventId.Value);
            }
            else
                return null;
        }

        /// <summary>
        /// 從本地檔案抓檔案資訊
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.MaterialViewModel GetFileFromFileStorage(Guid eventId)
        {
            var _activityRep = _uow.EntityRepository<Activitys>();
            var db = _uow.DbContext;
            var mr = db.ActMaterial.FirstOrDefault(x => x.EventId == eventId);
            if (mr == null)
                return null;
            var ar = _activityRep.GetFirst(x => x.OuterKey == eventId);
            if (ar == null)
                return null;
            if (ar.CardisShow == true)
            {
                var memberInfo = _uow.DbContext.Members.FirstOrDefault(t => t.Id == (mr.CreateUser.Value));
                var fileInfo = db.FileStorage.FirstOrDefault(t => t.FileGuid == eventId);
                if (fileInfo != null)
                {
                    var vm = new Infrastructure.ViewModel.MaterialViewModel()
                    {
                        ActivityId = mr.Id,
                        LearningId = mr.LearningId,
                        EventId = eventId,
                        OuterKey = eventId,
                        sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(eventId),
                        StartDate = mr.Created.Local.Value,
                        GoogleDriveFileId = fileInfo.FileGuid.ToString("N"),
                        Creator = mr.CreateUser.Value.ToString(),
                        CreateDate = Convert.ToDateTime(mr.Created.Local.ToString()),
                        Created_Utc = Convert.ToDateTime(mr.Created.Utc.Value),
                        ModuleKey = Utility.ParaCondition.ModuleType.Material,
                        CreatorAccount = memberInfo.Account,
                        CreatorName = memberInfo.Name,
                        CreatorPhoto = memberInfo.Photo,
                        Name = mr.Name,
                        FileType = mr.FileType,
                        FileLength = mr.FileLength,
                        ToRoomId = ar.ToRoomId.ToLower(),
                        FileImgUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, fileInfo.FileGuid.ToString("N"), maxImgWidth, maxImgHeight),
                        FileDownLoadUrl = fileInfo.FileUrl,
                        FileWebViewUrl = fileInfo.FileUrl,
                        FolderId = null,
                        Publish_Utc = ar.Publish_Utc,
                        Rows = 10,
                        Pages = 1
                    };
                    return vm;
                }
                else
                { return null; }

            }
            else
            { return null; }
        }

        /// <summary>
        /// 從DB取得檔案列表
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.MaterialViewModel GetFileInfoListForDB(Guid eventId)
        {
            var _activityRep = _uow.EntityRepository<Activitys>();
            var mr = _uow.EntityRepository<ActMaterial>().GetFirst(x => x.EventId == eventId);
            var db = _uow.DbContext;
            if (mr == null)
                return null;
            var ar = _activityRep.GetFirst(x => x.OuterKey == eventId);
            if (ar == null)
                return null;
            if (ar.CardisShow == true)
            {
                var memberInfo = _uow.MembersRepo.MemberInfo(mr.CreateUser.Value);
                var fileInfo = db.GoogleFile.FirstOrDefault(t => t.FileId == mr.GoogleDriveFileId && t.ParentFileId == mr.GoogleDriveFolder);
                if (fileInfo != null)
                {
                    var vm = new MaterialViewModel()
                    {
                        sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(eventId),
                        ActivityId = mr.Id,
                        LearningId = mr.LearningId,
                        EventId = eventId,
                        OuterKey = eventId,
                        StartDate = mr.Created.Local.Value,
                        GoogleDriveFileId = mr.GoogleDriveFileId,
                        Creator = mr.CreateUser.Value.ToString(),
                        CreateDate = Convert.ToDateTime(mr.Created.Local.ToString()),
                        Created_Utc = Convert.ToDateTime(mr.Created.Utc.Value),
                        ModuleKey = Utility.ParaCondition.ModuleType.Material,
                        CreatorAccount = memberInfo.Account,
                        CreatorName = memberInfo.Name,
                        CreatorPhoto = memberInfo.Photo,
                        Name = mr.Name,
                        FileType = mr.FileType,
                        FileLength = mr.FileLength,
                        ToRoomId = ar.ToRoomId.ToLower(),
                        FileImgUrl = fileInfo.ImgUrl,
                        FileDownLoadUrl = fileInfo.DownLoadUrl,
                        FileWebViewUrl = fileInfo.WebViewUrl,
                        FolderId = mr.GoogleDriveFolder,
                        Publish_Utc = ar.Publish_Utc,
                        Rows = 10,
                        Pages = 1
                    };
                    return vm;
                }
                else
                { return null; }

            }
            else
            { return null; }

        }
        /// <summary>
        /// 刪除檔案資料
        /// </summary>
        /// <param name="outerKey"></param>
        public bool DeleteByOuterKey(string outerKey)
        {
            var eventId = Utility.OuterKeyHelper.CheckOuterKey(outerKey);
            var db = _uow.DbContext;
            var activityInfo = db.Activitys.FirstOrDefault(t => t.OuterKey == eventId);
            if (activityInfo == null)
                return false;
            var materialInfo = db.ActMaterial.FirstOrDefault(t => t.EventId == eventId);
            if (materialInfo == null)
                return false;
            try
            {
                db.Activitys.Remove(activityInfo);
                db.ActMaterial.Remove(materialInfo);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }


        }
        /// <summary>
        /// 新增一筆上傳檔案活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="name"></param>
        /// <param name="googleDriveFileId"></param>
        /// <param name="folderId"></param>
        /// <param name="fileType"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileShortImg"></param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.MaterialEvent Add(string circleKey, int memberId, string name, string googleDriveFileId, string folderId, string fileType, int fileSize, string fileShortImg)
        {
            Guid eveId = Guid.NewGuid();
            DateTime? dt = DateTime.UtcNow;
            int id = cacheService.GetCircle(circleKey).Id;
            using (var db = _uow.DbContext)
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region // 1. 建立上傳檔案活動物件                        
                        var objMaterial = new ActMaterial()
                        {
                            LearningId = id,
                            Name = name,
                            EventId = eveId,
                            CreateUser = memberId,
                            Created = Infrastructure.Property.TimeData.Create(dt),
                            Updated = Infrastructure.Property.TimeData.Create(null),
                            Deleted = Infrastructure.Property.TimeData.Create(null),
                            GoogleDriveFileId = googleDriveFileId,
                            FileImgUrl = string.Format("https://drive.google.com/a/g.scenet.pccu.edu.tw/uc?id={0}&export=download", googleDriveFileId),
                            FileLength = fileSize,
                            FileType = fileType,
                            GoogleDriveFolder = folderId


                        };
                        #endregion

                        #region // 2. 替上傳檔案活動建立一筆訊息物件

                        Activitys objAct = new Activitys()
                        {
                            Duration = 0,
                            ModuleKey = Utility.ParaCondition.ModuleType.Material,
                            ToRoomId = circleKey,
                            CreateUser = memberId,
                            Created = Infrastructure.Property.TimeData.Create(dt),
                            Updated = Infrastructure.Property.TimeData.Create(null),
                            Deleted = Infrastructure.Property.TimeData.Create(null),
                            OuterKey = eveId,
                            StartDate = dt,
                            IsActivity = true,
                            CardisShow = true,
                            Publish_Utc = dt
                        };
                        #endregion

                        #region // 3. 建立一筆Google檔案

                        var objGoogle = new GoogleFile()
                        {
                            DownLoadUrl = string.Format("https://drive.google.com/a/g.scenet.pccu.edu.tw/uc?id={0}&export=download", googleDriveFileId),
                            FileId = googleDriveFileId,
                            FileType = fileType,
                            ImgUrl = string.Format("https://drive.google.com/a/g.scenet.pccu.edu.tw/uc?id={0}&export=download", fileShortImg),
                            WebViewUrl = string.Format("https://drive.google.com/a/g.scenet.pccu.edu.tw/file/d/{0}/view?usp=drivesdk", googleDriveFileId),
                            Name = name,
                            ParentFileId = folderId,
                            Size = fileSize,
                            Create_User = memberId,
                            Create_Utc = Convert.ToDateTime(dt)
                        };
                        #endregion

                        // 3. 寫入DB
                        db.ActMaterial.Add(objMaterial);
                        db.Activitys.Add(objAct);
                        db.GoogleFile.Add(objGoogle);
                        db.SaveChanges();
                        dbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            #region // 回傳給APP的物件
            var data = new Infrastructure.BusinessObject.MaterialEvent()
            {
                ClassId = circleKey,
                OuterKey = eveId,
                Creator = memberId.ToString(),
                CreateDate = Convert.ToDateTime(dt)
            };
            #endregion
            return data;
        }

        /// <summary>
        /// 建立活動牆圖片活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <param name="files"></param>
        /// <param name="physicalStreams"></param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.MaterialEvent Insert(string circleKey,string token,List<FileStorage> files,Stream[] physicalStreams) {

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            var learningcircleService = new LearningCircleService();
            var learningcircleinfo = learningcircleService.GetDetailByOuterKey(circleKey);

            var createDateTime = DateTime.UtcNow;
            if (checkToken == null)
                return null;
            var fileService = new FileService();
            var newFiles = fileService.UploadFiles(checkToken.MemberId,files,physicalStreams);
            if (newFiles.FirstOrDefault() == null)
                return null;
            try
            {
                var db = _uow.DbContext;
                foreach (var file in newFiles) {
                    var materialEntity = new ActMaterial()
                    {
                        LearningId = learningcircleinfo.Id,
                        Name = file.Name,
                        EventId = file.FileGuid,
                        CreateUser = checkToken.MemberId,
                        Created = Infrastructure.Property.TimeData.Create(createDateTime),
                        Updated = Infrastructure.Property.TimeData.Create(null),
                        Deleted = Infrastructure.Property.TimeData.Create(null),
                        GoogleDriveFileId = file.FileGuid.ToString("N"),
                        FileImgUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, file.FileGuid.ToString("N"), maxImgWidth, maxImgHeight),
                        FileLength = file.FileSize,
                        FileType = file.FileContentType,
                        Visibility = true
                    };
                    var activityEntity = new Activitys()
                    {
                        CardisShow = true,
                        CreateUser = checkToken.MemberId,
                        Created = Infrastructure.Property.TimeData.Create(createDateTime),
                        Updated = Infrastructure.Property.TimeData.Create(null),
                        Deleted = Infrastructure.Property.TimeData.Create(null),
                        OuterKey = file.FileGuid,
                        Publish_Utc = createDateTime,
                        ToRoomId = circleKey,
                        ModuleKey = Utility.ParaCondition.ModuleType.Material,
                        StartDate = createDateTime,
                        IsActivity = true
                    };
                    db.ActMaterial.Add(materialEntity);
                    db.Activitys.Add(activityEntity);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
            var memberService = new MemberService();
            var memberInfo = memberService.UserIdToAccount(checkToken.MemberId);

            var response = new Infrastructure.BusinessObject.MaterialEvent()
            {
                ClassId = circleKey,
                CreateAccount = memberInfo.Account,
                Creator = memberInfo.Id.ToString(),
                OuterKey = newFiles.FirstOrDefault().FileGuid,
                CreateDate = createDateTime
            };
            return response;
        }

        /// <summary>
        /// 雲端版APP專用
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="token"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Infrastructure.BusinessObject.MaterialEvent AddFile(string circleKey, string token, HttpFileCollection files)
        {

            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            var learningcircleService = new LearningCircleService();
            var learningcircleinfo = learningcircleService.GetDetailByOuterKey(circleKey);

            var createDateTime = DateTime.UtcNow;
            if (checkToken == null)
                return null;
            var fileService = new FileService();
            var newFiles = fileService.UploadFile(checkToken.MemberId, files);
            var file = newFiles.FirstOrDefault();
            if (file == null)
                return null;
            try
            {
                var db = _uow.DbContext;
                var materialEntity = new Infrastructure.Entity.ActMaterial()
                {
                    LearningId = learningcircleinfo.Id,
                    Name = file.Name,
                    EventId = file.FileGuid,
                    CreateUser = checkToken.MemberId,
                    Created = Infrastructure.Property.TimeData.Create(createDateTime),
                    Updated = Infrastructure.Property.TimeData.Create(null),
                    Deleted = Infrastructure.Property.TimeData.Create(null),
                    GoogleDriveFileId = file.FileGuid.ToString("N"),
                    FileImgUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, file.FileGuid.ToString("N"), maxImgWidth, maxImgHeight),
                    FileLength = file.FileSize,
                    FileType = file.FileContentType,
                    Visibility = true
                };
                var activityEntity = new Activitys()
                {
                    CardisShow = true,
                    CreateUser = checkToken.MemberId,
                    Created = Infrastructure.Property.TimeData.Create(createDateTime),
                    Updated = Infrastructure.Property.TimeData.Create(null),
                    Deleted = Infrastructure.Property.TimeData.Create(null),
                    OuterKey = file.FileGuid,
                    Publish_Utc = createDateTime,
                    ToRoomId = circleKey,
                    ModuleKey = Utility.ParaCondition.ModuleType.Material,
                    StartDate = createDateTime,
                    IsActivity = true
                };
                db.ActMaterial.Add(materialEntity);
                db.Activitys.Add(activityEntity);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
            var memberService = new MemberService();
            var memberInfo = memberService.UserIdToAccount(checkToken.MemberId);

            var response = new Infrastructure.BusinessObject.MaterialEvent()
            {
                ClassId = circleKey,
                CreateAccount = memberInfo.Account,
                Creator = memberInfo.Id.ToString(),
                OuterKey = file.FileGuid,
                CreateDate = createDateTime
            };
            return response;
        }

        /// <summary>
        /// 在水道頁長出新增活動之後的檔案活動
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.ActivitysViewModel addPanelInfo(Guid eventId, int memberId, string circleKey)
        {
            var data = new Infrastructure.ViewModel.ActivitysViewModel();
            var db = _uow.DbContext;
            var activityInfo = db.Activitys.Where(t => t.OuterKey == eventId).FirstOrDefault();
            if (activityInfo != null)
            {

                DateTime dt = DateTime.UtcNow;
                // 回傳給APP的物件
                var me = db.Members.SingleOrDefault(x => x.Id == memberId);
                if (me != null)
                {
                    data = new Infrastructure.ViewModel.ActivitysViewModel()
                    {
                        Id = 0,
                        CreatorAccount = me.Account,
                        // CreatorPhoto = _uow.IThinkVmRepo.GetTempMemberByAccountId(memberId).Photo,
                        CreatorPhoto = me.Photo,
                        CreatorName = me.Name,
                        ToRoomId = circleKey.ToLower(),
                        ModuleKey = Utility.ParaCondition.ModuleType.Material,
                        Created_Utc = dt,
                        ReadMark = false,
                        PositionMark = false,
                        OuterKey = eventId,
                        Publish_Utc = activityInfo.Publish_Utc,
                        sOuterKey = Utility.OuterKeyHelper.GuidToPageToken(eventId)
                    };
                }
            }

            return data;
        }
    }
}
