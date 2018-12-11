using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Repository;

namespace WiicoApi.Service.CommenService
{
    /// <summary>
    /// 整理檔案資訊
    /// </summary>
    public class FileService
    {
        private readonly GenericUnitOfWork _uow;
        private string locks = "";
        //  private readonly PdfService pdfService = new PdfService();

        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();
        /// <summary>
        /// 存檔Server
        /// </summary>
        private string loginServer = ConfigurationManager.AppSettings["loginServer"].ToString();

        /// <summary>
        /// 預設縮圖最大寬度
        /// </summary>       
        private readonly int maxImgWidth = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgWidth"]);
        /// <summary>
        /// 預設縮圖最大高度
        /// </summary>
        private readonly int maxImgHeight = Convert.ToInt32(ConfigurationManager.AppSettings["maxImgHeight"]);

        /// <summary>
        /// 建立檔案接口
        /// </summary>
        public FileService()
        {
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 取得特定檔案接口
        /// </summary>
        /// <param name="guid"></param>
        public FileService(string guid)
        {
            this.fileGuid = guid;
            _uow = new GenericUnitOfWork();
        }

        /// <summary>
        /// 縮圖檔案接口
        /// </summary>
        /// <param name="guid">原圖 guidName</param>
        /// <param name="thumbnailFileName">縮圖 fileName</param>
        /// <param name="width">縮圖寬度</param>
        /// <param name="height">縮圖高度</param>
        public FileService(string guid, string thumbnailFileName = null, int? width = null, int? height = null)
        {
            _uow = new GenericUnitOfWork();
            this.fileGuid = guid;
            if (String.IsNullOrEmpty(thumbnailFileName) == false)
            {
                this.ThumbnailFileName = thumbnailFileName;
            }
            if (width.HasValue)
            {
                this.Width = width.Value;
            }
            if (height.HasValue)
            {
                this.Height = height.Value;
            }
        }

        /// <summary>
        /// 檔案代碼
        /// </summary>
        public string fileGuid { get; private set; }

        /// <summary>
        /// 縮圖fileName 系統自動編輯
        /// </summary>
        public string ThumbnailFileName { get; private set; }

        /// <summary>
        /// 圖片寬度 - 如果需要縮圖的寬度
        /// </summary>
        public int? Width { get; private set; }

        /// <summary>
        /// 圖片高度 - 如果需要縮圖的高度
        /// </summary>
        public int? Height { get; private set; }


        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="deleter">刪除者編號</param>
        /// <param name="fileGuid">檔案代碼</param>
        /// <returns></returns>
        public bool DeleteFiles(int deleter, string[] files)
        {
            var result = false;
            var db = _uow.DbContext;
            try
            {
                foreach (var fileId in files)
                {
                    var file = db.FileStorage.Find(Convert.ToInt32(fileId));
                    if (file != null)
                    {
                        file.DeleteUtcDate = DateTime.UtcNow;
                        file.Deleter = deleter;
                    }
                }
                result = true;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;

        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="deleter">刪除者編號</param>
        /// <param name="fileGuid">檔案代碼</param>
        /// <returns></returns>
        public bool DeleteFile(int deleter, Guid fileGuid)
        {
            var result = false;
            var db = _uow.DbContext;
            var fileEntity = db.FileStorage.FirstOrDefault(t=>t.FileGuid==fileGuid);
            if (fileEntity != null)
            {
                fileEntity.DeleteUtcDate = DateTime.UtcNow;
                fileEntity.Deleter = deleter;
                db.SaveChanges();
                result = true;
                return result;
            }
            else
                return result;
        }

        /// <summary>
        /// 取得特定的FileStorage資料
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <returns></returns>
        public FileStorageViewModel GetFileInfoByFileGuidName(Guid fileGuid)
        {
            var db = _uow.DbContext;
            
            var fileInfo =(from fs in db.FileStorage
                          where fs.FileGuid == fileGuid
                          select fs).FirstOrDefault();
            if (fileInfo != null) {
                var imgFile_g = fileGuid.ToString("N");
                var responseData = new FileStorageViewModel() {
                    CreateUtcDate = fileInfo.CreateUtcDate,
                    Creator = fileInfo.Creator,
                    FileContentType = fileInfo.FileContentType,
                    Deleter = fileInfo.Deleter,
                    DeleteUtcDate = fileInfo.DeleteUtcDate,
                    FileGuid = fileInfo.FileGuid,
                    FileImageHeight = fileInfo.FileImageHeight,
                    FileImageUrl = fileInfo.FileContentType.ToLower().Contains("image") ?
                     string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, fileInfo.FileGuid.ToString("N"), maxImgWidth, maxImgHeight) :
                     null,
                    FileImageWidth = fileInfo.FileImageWidth,
                    FileSize = fileInfo.FileSize,
                    FileUrl = fileInfo.FileUrl,
                    Name = fileInfo.Name
                };

                responseData.FileImageUrl = fileInfo.FileContentType.ToLower().Contains("image") ? string.Format("{0}api/imgFile/{1}", loginServer, imgFile_g, maxImgWidth, maxImgHeight) : null;
                return responseData;
            }

            else
                return null;
        }

        /// <summary>
        /// 產生縮圖 (只有指定最大寬度)
        /// </summary>
        /// <param name="sourceImagePath">原始圖檔路徑</param>
        /// <param name="widthMaxPixel">最大寬度</param>
        /// <param name="thumbnailImagePath">縮圖圖檔路徑</param>
        public void ImageResize(string sourceImagePath, int widthMaxPixel, string thumbnailImagePath, string mineType)
        {
            ImageResize(sourceImagePath, widthMaxPixel, 0, thumbnailImagePath, mineType);
        }

        /// <summary>
        /// 產生縮圖 (同時指定最大寬度與高度)
        /// </summary>
        /// <param name="sourceImagePath">原始圖檔路徑</param>
        /// <param name="widthMaxPixel">最大寬度</param>
        /// <param name="heightMaxPixel">最大高度</param>
        /// <param name="thumbnailImagePath">縮圖圖檔路徑</param>
        public void ImageResize(string sourceImagePath, int widthMaxPixel, int heightMaxPixel, string thumbnailImagePath, string mineType)
        {
            try
            {
                // 讀取原始圖片
                using (var fs = new FileStream(sourceImagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var originalBitmap = new Bitmap(fs))
                    {
                        SetPhotoFileResizeInfo(originalBitmap, sourceImagePath, widthMaxPixel, heightMaxPixel, thumbnailImagePath);
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Infrastructure.ViewModel.MemberManage.MemberPhotoResponse SaveMemberPhotoFile(string token, string account, Infrastructure.ViewModel.FileViewModel photo)
        {
            var db = _uow.DbContext;
            var memberService = new MemberService();
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            if (checkToken == null)
                return null;

            var memberData = db.Members.Find(checkToken.MemberId);
            //    var memberInfo = memberService.UserIdToAccount(checkToken.MemberId);
            if (memberData == null)
                return null;
            int? fileImageWidth = null;
            int? fileImageHeight = null;
            var fileGuidName = Guid.NewGuid();
            var resourcePath = Path.Combine(drivePath, fileGuidName.ToString("N"));

            var photoStream = photo.InputStream;
            var photoBitmap = new Bitmap(photoStream);

            //原始圖片檔
            SetPhotoFileInfo(photoBitmap, resourcePath);

            var imgResizePath = Path.Combine(drivePath, string.Format("{0}_w{1}_h{2}", fileGuidName.ToString("N"), maxImgWidth, maxImgHeight));
            //縮圖檔
            SetPhotoFileResizeInfo(photoBitmap, resourcePath, maxImgWidth, maxImgHeight, imgResizePath);
            var image = System.Drawing.Image.FromStream(photoStream);

            //實體圖片寬高
            fileImageWidth = image.Width;
            fileImageHeight = image.Width;
            var imgContentType = "image/png";
            var fileServer = WebConfigurationManager.AppSettings["loginServer"];
            var fileEntity = new Infrastructure.Entity.FileStorage()
            {
                Name = memberData.Name + "照片",
                FileGuid = fileGuidName,
                FileSize = photo.ContentLength, //Byte
                FileContentType = imgContentType,
                Creator = memberData.Id,
                CreateUtcDate = DateTime.UtcNow,
                FileUrl = string.Format("{0}api/file/{1}", fileServer, fileGuidName.ToString("N")),
                FileImageHeight = fileImageHeight,
                FileImageWidth = fileImageWidth
            };

            // Db File
            var responseImgData =  Create(fileEntity);
            memberData.Photo = responseImgData.FileImageUrl;
            memberData.Updated = TimeData.Create(DateTime.UtcNow);
            memberData.UpdateUser = memberData.Id;
            db.SaveChanges();

            var response = new Infrastructure.ViewModel.MemberManage.MemberPhotoResponse();
            response.AcpdId = memberData.Account;
            response.Photo = responseImgData.FileImageUrl;
            return response;
        }

        /// <summary>
        /// 儲存個人照片檔案
        /// </summary>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.MemberManage.MemberPhotoResponse SaveMemberPhotoFile(string token, string account, string photo)
        {
            var db = _uow.DbContext;
            var memberService = new MemberService();
            var tokenService = new TokenService();
            var checkToken = tokenService.GetTokenInfo(token).Result;
            if (checkToken == null)
                return null;

            var memberData = db.Members.Find(checkToken.MemberId);
            //    var memberInfo = memberService.UserIdToAccount(checkToken.MemberId);
            if (memberData == null)
                return null;
            int? fileImageWidth = null;
            int? fileImageHeight = null;
            var fileGuidName = Guid.NewGuid();
            var resourcePath = Path.Combine(drivePath, fileGuidName.ToString("N"));

            var photoData = Convert.FromBase64String(photo);
            var photoStream = new System.IO.MemoryStream(photoData);
            var photoBitmap = new Bitmap(new MemoryStream(photoData));

            //原始圖片檔
            SetPhotoFileInfo(photoBitmap, resourcePath);

            var imgResizePath = Path.Combine(drivePath, string.Format("{0}_w{1}_h{2}", fileGuidName.ToString("N"), maxImgWidth, maxImgHeight));
            //縮圖檔
            SetPhotoFileResizeInfo(photoBitmap, resourcePath, maxImgWidth, maxImgHeight, imgResizePath);
            var image = System.Drawing.Image.FromStream(photoStream);

            //實體圖片寬高
            fileImageWidth = image.Width;
            fileImageHeight = image.Width;
            var imgContentType = "image/png";
            var fileServer = WebConfigurationManager.AppSettings["loginServer"];
            var fileEntity = new Infrastructure.Entity.FileStorage()
            {
                Name = memberData.Name + "照片",
                FileGuid = fileGuidName,
                FileSize = photoData.Length, //Byte
                FileContentType = imgContentType,
                Creator = memberData.Id,
                CreateUtcDate = DateTime.UtcNow,
                FileUrl = string.Format("{0}api/file/{1}", fileServer, fileGuidName.ToString("N")),
                
                FileImageHeight = fileImageHeight,
                FileImageWidth = fileImageWidth
            };

            // Db File
            var fileImgData = Create(fileEntity);
            memberData.Photo = fileImgData.FileImageUrl;
            memberData.Updated = TimeData.Create(DateTime.UtcNow);
            memberData.UpdateUser = memberData.Id;
            db.SaveChanges();

            var response = new Infrastructure.ViewModel.MemberManage.MemberPhotoResponse();
            response.AcpdId = account;
            response.Photo = fileImgData.FileImageUrl;
            return response;
        }

        /// <summary>
        /// 圖片存檔
        /// </summary>
        /// <param name="photo"></param>
        /// <param name="sourceImagePath"></param>
        public void SetPhotoFileInfo(Bitmap photo, string sourceImagePath)
        {
            photo.Save(sourceImagePath);
            var img = photo;
            /*    var rotate = GetOrientType(sourceImagePath);
                if (rotate > 1)
                    img = GetOrientBitmap(photo, rotate);
                    */
            var width = img.Width * 1.0;
            var height = img.Height * 1.0;

            using (Graphics g = Graphics.FromImage(photo))
            {
                //經測試後採用HighQuality = 2,
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //g.PixelOffsetMode = (PixelOffsetMode)Convert.ToInt32(mode2Text.Text);
                //經測試後採用Bicubic = 4,
                g.InterpolationMode = InterpolationMode.Bicubic;
                //g.InterpolationMode = (InterpolationMode)Convert.ToInt32(mode3Text.Text);
                g.DrawImage(img, 0, 0, (float)width, (float)height);
            }

            //設定圖片類型編碼
            ImageCodecInfo myImageCodecInfo;
            //設定圖片編碼
            System.Drawing.Imaging.Encoder myEncoder;
            //設定編碼參數為圖片深度
            myEncoder = System.Drawing.Imaging.Encoder.ColorDepth;
            //設定加入的編碼參數
            EncoderParameter myEncoderParameter;
            //設定要修改的編碼參數列表
            var EncoderParameters = new EncoderParameters(1);

            //設定圖片深度為24
            myEncoderParameter = new EncoderParameter(myEncoder, 24);
            myImageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == "image/jpeg");
            //加入參數
            EncoderParameters.Param[0] = myEncoderParameter;
            // 將縮圖存檔
            //     photo.Save(sourceImagePath, myImageCodecInfo, EncoderParameters);
        }

        /// <summary>
        /// 設定縮圖資訊
        /// </summary>
        /// <param name="photo"></param>
        /// <param name="sourceImagePath"></param>
        /// <param name="widthMaxPixel"></param>
        /// <param name="heightMaxPixel"></param>
        /// <param name="thumbnailImagePath"></param>
        public void SetPhotoFileResizeInfo(Bitmap photo, string sourceImagePath, int widthMaxPixel, int heightMaxPixel, string thumbnailImagePath)
        {
            var img = photo;
            var rotate = GetOrientType(sourceImagePath);
            if (rotate > 1)
                img = GetOrientBitmap(photo, rotate);

            var width = img.Width * 1.0;
            var height = img.Height * 1.0;
            var ratio = 0.0;
            // 縮圖規則
            // widthMaxPixel 為指定最大寬度, heightMaxPixel 為指定最大高度
            // 縮圖大小以不超過指定的最大寬度及高度為限
            // 如果圖片尺寸小於指定寬度及高度，則不會影響原圖尺寸

            // 如果尺寸寬高都比指定尺寸小，完全不需要操作縮圖重繪的話
            // 就直接將原始檔案複製為縮圖，避免重繪之後反而檔案大小變大的問題
            if (width <= widthMaxPixel && (heightMaxPixel == 0 || height <= heightMaxPixel))
            {
                System.IO.File.Copy(sourceImagePath, thumbnailImagePath);
                return;
            }
            ratio = (width >= height) ? (widthMaxPixel / width) : (heightMaxPixel / height);
            if (ratio < 1)
            {
                width = width * ratio;
                height = height * ratio;
            }
            var thumbnailImage = new Bitmap((int)width, (int)height);
            using (Graphics g = Graphics.FromImage(thumbnailImage))
            {
                //經測試後採用HighQuality = 2,
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //g.PixelOffsetMode = (PixelOffsetMode)Convert.ToInt32(mode2Text.Text);
                //經測試後採用Bicubic = 4,
                g.InterpolationMode = InterpolationMode.Bicubic;
                //g.InterpolationMode = (InterpolationMode)Convert.ToInt32(mode3Text.Text);
                g.DrawImage(img, 0, 0, (float)width, (float)height);
            }

            //設定圖片類型編碼
            ImageCodecInfo myImageCodecInfo;
            //設定圖片編碼
            System.Drawing.Imaging.Encoder myEncoder;
            //設定編碼參數為圖片深度
            myEncoder = System.Drawing.Imaging.Encoder.ColorDepth;
            //設定加入的編碼參數
            EncoderParameter myEncoderParameter;
            //設定要修改的編碼參數列表
            var EncoderParameters = new EncoderParameters(1);

            //設定圖片深度為24
            myEncoderParameter = new EncoderParameter(myEncoder, 24);
            myImageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == "image/jpeg");
            //加入參數
            EncoderParameters.Param[0] = myEncoderParameter;
            // 將縮圖存檔
            thumbnailImage.Save(thumbnailImagePath, myImageCodecInfo, EncoderParameters);
            //thumbnailImage.Save(thumbnailImagePath);
        }

        /// <summary>
        /// 取得實體檔案資訊
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public FileStream ExecuteAsync(CancellationToken cancellationToken)
        {

            Guid newGuid;
            //DB
            var fileInfo = new Infrastructure.Entity.FileStorage();

            if (Guid.TryParse(fileGuid, out newGuid))
                fileInfo = GetFileInfoByFileGuidName(newGuid);

            //如果資料庫中檔案沒被刪除，或者資料庫中有該檔案
            if (fileInfo == null || fileInfo.DeleteUtcDate != null)
                // 資料庫紀錄不存在
                return null;


            var filePath = Path.Combine(drivePath, this.fileGuid.Replace("-", ""));

            if (System.IO.File.Exists(filePath) == false)
                return null;


            if (String.IsNullOrEmpty(this.ThumbnailFileName))
            {
                // 沒有縮圖的 filename，直接輸出原檔案內容
                var fileData = System.IO.File.Open(filePath, FileMode.Open);
                return fileData;
            }
            else
            {
                // 如果有縮圖的 fileName
                // 組合縮圖 filename 的路徑
                var thumbPath = Path.Combine(drivePath, this.ThumbnailFileName);

                // 查看是否有縮圖檔案
                if (System.IO.File.Exists(thumbPath))
                {
                    var fileData = System.IO.File.Open(thumbPath, FileMode.Open);
                    return fileData;
                }
                else
                {
                    try
                    {
                        // 拆解縮圖長寬
                        if (this.Width.HasValue && this.Height.HasValue)
                            ImageResize(filePath, this.Width.Value, this.Height.Value, thumbPath, fileInfo.FileContentType);
                        else
                            ImageResize(filePath, this.Width.Value, thumbPath, fileInfo.FileContentType);

                        var fileSize = new FileInfo(thumbPath).Length;
                        var fileData = System.IO.File.Open(thumbPath, FileMode.Open);

                        return fileData;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 透過mutipart Form Data傳檔案進來
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public List<FileStorageViewModel> UploadFiles(int memberId, List<FileStorage> files, Stream[] physicalStreams)
        {
            var fileList = new List<FileStorageViewModel>();

            //預設存檔位置
            var drivePath = WebConfigurationManager.AppSettings["DrivePath"];
            //存檔server
            var fileServer = WebConfigurationManager.AppSettings["loginServer"];
            //確認實體檔案資料夾是否存在
            /*      if (!Directory.Exists(drivePath))
                      //建一個新的資料夾
                      Directory.CreateDirectory(drivePath);*/
            var streamIndex = 0;
            foreach (var file in files)
            {
                var fileStream = physicalStreams[streamIndex];
                var responseFileData = new FileStorageViewModel();
                int? fileImageWidth = null;
                int? fileImageHeight = null;
                //  var fileGuidName = Guid.NewGuid();
                var resourcePath = Path.Combine(drivePath, file.FileGuid.ToString("N"));

                if (file.FileContentType.Contains("image"))
                {
                    var imgPath = Path.Combine(drivePath, string.Format("{0}_w{1}_h{2}", file.FileGuid.ToString("N"), maxImgWidth, maxImgHeight));
                    var image = Image.FromStream(fileStream);

                    //實體圖片寬高
                    fileImageWidth = image.Width;
                    fileImageHeight = image.Width;
                    //建立一個縮圖
                    ImageResize(resourcePath, maxImgWidth, maxImgHeight, imgPath, file.FileContentType);
                    responseFileData.FileImageUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", fileServer, file.FileGuid.ToString("N"), maxImgWidth, maxImgHeight);
                    responseFileData.FileImageHeight = fileImageHeight;
                    responseFileData.FileImageWidth = fileImageWidth;
                    file.FileImageHeight = fileImageHeight;
                    file.FileImageWidth = fileImageWidth;
                }
                file.Name = HttpUtility.UrlDecode(file.Name);
                // file.FileGuid = fileGuidName;
                file.Creator = memberId;
                file.CreateUtcDate = DateTime.UtcNow;
                file.FileUrl = string.Format("{0}api/file/{1}", fileServer, file.FileGuid.ToString("N"));

                responseFileData.FileGuid = file.FileGuid;
                responseFileData.Creator = memberId;
                responseFileData.FileUrl = string.Format("{0}api/file/{1}", fileServer, file.FileGuid.ToString("N"));
                responseFileData.CreateUtcDate = DateTime.UtcNow;
                responseFileData.Name = HttpUtility.UrlDecode(file.Name);
                responseFileData.Size = file.FileSize;
                responseFileData.FileSize = file.FileSize;
                responseFileData.FileType = file.FileContentType;
                responseFileData.FileContentType = file.FileContentType;
                fileList.Add(responseFileData);
                fileStream.Close();
                streamIndex++;
            }
            var db = _uow.DbContext;
            db.FileStorage.AddRange(files);
            db.SaveChanges();
            fileList = (from fl in fileList
                        join f in files on fl.FileGuid equals f.FileGuid
                        select new FileStorageViewModel {
                            Id = f.Id,
                            CreateUtcDate = fl.CreateUtcDate,
                            Creator = fl.Creator,
                            Deleter = fl.Deleter,
                            DeleteUtcDate = fl.DeleteUtcDate,
                            FileContentType = fl.FileContentType,
                            DownLoadUrl=fl.DownLoadUrl,
                            FileGuid=fl.FileGuid,
                            FileImageHeight=fl.FileImageHeight,
                            FileImageUrl=fl.FileImageUrl,
                            FileImageWidth=fl.FileImageWidth,
                            FileSize=fl.FileSize,
                            FileType=fl.FileType,
                            FileUrl=fl.FileUrl,
                            ImgUrl=fl.ImgUrl,
                            Name=fl.Name,
                            Size=fl.Size,
                            WebViewUrl=fl.WebViewUrl
                        }).ToList();
            return fileList;
        }




        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public List<FileStorageViewModel> UploadFile(int memberId, HttpFileCollection files)
        {
            var fileList = new List<FileStorageViewModel>();

            //預設存檔位置
            var drivePath = WebConfigurationManager.AppSettings["DrivePath"];
            //存檔server
            var fileServer = WebConfigurationManager.AppSettings["loginServer"];
            //確認實體檔案資料夾是否存在
            if (!Directory.Exists(drivePath))
                //建一個新的資料夾
                Directory.CreateDirectory(drivePath);


            //開始上傳檔案
            for (int i = 0; i < files.Count; i++)
            {
                var fileInfo = new FileStorageViewModel();
                HttpPostedFile file = files[i];

                int? fileImageWidth = null;
                int? fileImageHeight = null;
                var fileGuidName = Guid.NewGuid();
                var resourcePath = Path.Combine(drivePath, fileGuidName.ToString("N"));

                // Physical File
                file.SaveAs(resourcePath);
                var fileEntity = new Infrastructure.Entity.FileStorage()
                {
                    Name = HttpUtility.UrlDecode(file.FileName),
                    FileGuid = fileGuidName,
                    FileSize = file.ContentLength, //Byte
                    FileContentType = file.ContentType,
                    Creator = memberId,
                    CreateUtcDate = DateTime.UtcNow,
                    FileUrl = string.Format("{0}api/file/{1}", fileServer, fileGuidName.ToString("N")),
                    // FileImageUrl = string.Format("{0}api/imgFile/{1}/{2}/{3}", fileServer, fileGuidName.ToString("N"), maxImgWidth, maxImgHeight),
                    FileImageHeight = fileImageHeight,
                    FileImageWidth = fileImageWidth
                };
                if (file.ContentType.Contains("image"))
                {
                    var imgPath = Path.Combine(drivePath, string.Format("{0}_w{1}_h{2}", fileGuidName.ToString("N"), maxImgWidth, maxImgHeight));

                    System.Drawing.Image image = System.Drawing.Image.FromStream(file.InputStream);

                    //實體圖片寬高
                    fileImageWidth = image.Width;
                    fileImageHeight = image.Width;
                    //建立一個縮圖
                    ImageResize(resourcePath, maxImgWidth, maxImgHeight, imgPath, file.ContentType);
                }

      

                // Db File
                fileInfo = Create(fileEntity);
                fileList.Add(fileInfo);
            }
            var db = _uow.DbContext;
            db.SaveChanges();
            return fileList;
        }


        public FileStorageViewModel UploadFile(int memberId, string fileName, string contentType, int size, int? fileImageHeight = null, int? fileImageWidth = null)
        {
            var fileInfo =new FileStorageViewModel();
            var fileGuidName = Guid.NewGuid();

            var fileEntity = new Infrastructure.Entity.FileStorage()
            {
                Name = HttpUtility.UrlDecode(fileName),
                //  Name =file.FileName,
                FileGuid = fileGuidName,
                FileSize = size,
                FileContentType = contentType,
                Creator = memberId,
                CreateUtcDate = DateTime.UtcNow,
                FileUrl = string.Format("{0}api/file/{1}", loginServer, fileGuidName.ToString("N"))
            };
            if (contentType.Contains("image"))
            {
                fileEntity.FileImageHeight = fileImageHeight;
                fileEntity.FileImageWidth = fileImageWidth;
            }
            // Db File 
            fileInfo = Create(fileEntity);
            _uow.SaveChanges();
            return fileInfo;
        }



        /// <summary>
        /// 寫實體檔案
        /// </summary>
        /// <param name="size">檔案大小</param>
        /// <param name="path">檔案路徑</param>
        /// <param name="stream">原始檔案stream</param>
        /// <param name="byteStream">原始檔案byte</param>
        public void FileProxy(int size, string path, Stream stream, byte[] byteStream)
        {
            //確認實體檔案資料夾是否存在
            if (!Directory.Exists(drivePath))
                //建一個新的資料夾
                Directory.CreateDirectory(drivePath);
            //建立檔案
            var fileStream = System.IO.File.Create(path, size);
            lock (locks)
            {
                using (fileStream)
                {
                    stream.Read(byteStream, 0, byteStream.Length);
                    fileStream.Write(byteStream, 0, byteStream.Length);

                }
            }
        }

        /// <summary>
        /// 圖片檔案處理
        /// </summary>
        /// <param name="sqlData"></param>
        /// <returns></returns>
        public List<FileStorageViewModel> ImageFileProcess(List<Infrastructure.Entity.FileStorage> sqlData)
        {
            var fileServerUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["loginServer"];

            var response = (from fs in sqlData
                           select new FileStorageViewModel() {
                               Id = fs.Id,
                               CreateUtcDate = fs.CreateUtcDate,
                               Creator = fs.Creator,
                               Deleter = fs.Deleter,
                               DeleteUtcDate=fs.DeleteUtcDate,
                               FileContentType=fs.FileContentType,
                               FileSize=fs.FileSize,
                               FileGuid = fs.FileGuid,
                               FileImageHeight=fs.FileImageHeight,
                               FileImageWidth = fs.FileImageWidth,
                               FileUrl = fs.FileUrl,
                               Name=fs.Name,
                               FileImageUrl = fs.FileContentType.ToLower().Contains("image") ?
                               string.Format("{0}api/imgFile/{1}/{2}/{3}", fileServerUrl, fs.FileGuid.ToString("N"), maxImgWidth, maxImgHeight) :
                               null
                           }).ToList();

            return response;
        }


        /// <summary>
        /// 檔案上傳處理
        /// </summary>
        /// <param name="checkMember"></param>
        /// <returns></returns>
        public List<FileStorageViewModel> GetFileStorages(int memberId, List<Infrastructure.DataTransferObject.RequestFile> files)
        {
            var fileResult = new List<FileStorageViewModel>();

            foreach (var file in files)
            {
                //建立檔案
                var dbFile = UploadFile(memberId, file.FileName, file.ContentType, file.Size);
                fileResult.Add(dbFile);
                var path = Path.Combine(drivePath, dbFile.FileGuid.ToString("N"));

                //實際檔案處理 
                FileProxy(file.Size, path, file.Stream, file.ByteStream);
                if (file.ContentType.Contains("image"))
                {
                    var imgPath = Path.Combine(drivePath, string.Format("{0}_w{1}_h{2}", dbFile.FileGuid.ToString("N"), maxImgWidth, maxImgHeight));
                    var image = System.Drawing.Image.FromStream(file.Stream);
                    //建立一個縮圖
                    ImageResize(path, maxImgWidth, maxImgHeight, imgPath, file.ContentType);
                }
            }
            return fileResult;
        }
        /// <summary>
        /// 新增資訊
        /// </summary>
        /// <param name="fileEntity">檔案實體</param>
        /// <returns></returns>
        private FileStorageViewModel Create(FileStorage fileEntity)
        {
            var db = _uow.DbContext;
            if (GetFileInfoByFileGuidName(fileEntity.FileGuid) == null)
            {
                db.FileStorage.Add(fileEntity);
                var respponseData = new FileStorageViewModel()
                {
                    CreateUtcDate=fileEntity.CreateUtcDate,
                    Creator = fileEntity.Creator,
                    FileContentType = fileEntity.FileContentType,
                    Deleter = fileEntity.Deleter,
                    DeleteUtcDate = fileEntity.DeleteUtcDate,
                    FileGuid = fileEntity.FileGuid,
                    FileImageHeight = fileEntity.FileImageHeight,
                    FileImageUrl = fileEntity.FileContentType.ToLower().Contains("image") ?
                     string.Format("{0}api/imgFile/{1}/{2}/{3}", loginServer, fileEntity.FileGuid.ToString("N"), maxImgWidth, maxImgHeight) :
                     null,
                    FileImageWidth = fileEntity.FileImageWidth,
                    FileSize = fileEntity.FileSize,
                    FileUrl = fileEntity.FileUrl,
                    Name = fileEntity.Name
                };
                
                return respponseData;
            }
            else
                return null;
        }

        /// <summary>
        /// 將資料存成檔案
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="fileName"></param>
        private void SaveFile(MemoryStream memoryStream, string fileName)
        {
            var filePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Content/PDF/"),
                                        Path.GetFileName(fileName));

            var temp = new MemoryStream(memoryStream.ToArray());
            using (var fileStream = System.IO.File.Create(filePath))
            {
                temp.Seek(0, SeekOrigin.Begin);
                temp.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// 將資料直接輸出至前端
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="downloadFileName"></param>
        private void DownloadFile(MemoryStream memoryStream, string downloadFileName)
        {
            MemoryStream tempMemoryStream = null;
            try
            {
                //清除buffer
                System.Web.HttpContext.Current.Response.Clear();
                //清除 buffer 表頭
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.Buffer = true;

                // 檔案類型還有下列幾種"application/pdf"、"application/vnd.ms-excel"、"text/xml"、"text/HTML"、"image/JPEG"、"image/GIF"
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";

                System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;

                // FileName需要經過編碼, 否則中文會呈現亂碼                
                downloadFileName = System.Web.HttpUtility.UrlEncode(downloadFileName, System.Text.Encoding.UTF8);

                System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + downloadFileName);
                // 透過Response.OutputStream.Write慢慢將MemoryStream傳給Client端
                // 由於原始的MemoryStream已經關閉, 所以使用Temp的MemoryStream.
                tempMemoryStream = new MemoryStream(memoryStream.ToArray());
                tempMemoryStream.Seek(0, SeekOrigin.Begin);

                var buffer = new byte[10000];
                int length;
                long dataToRead = tempMemoryStream.Length;
                while (dataToRead > 0)
                {
                    if (System.Web.HttpContext.Current.Response.IsClientConnected)
                    {
                        length = tempMemoryStream.Read(buffer, 0, 10000);

                        System.Web.HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        System.Web.HttpContext.Current.Response.Flush();
                        buffer = new byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }

                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            finally
            {
                if (tempMemoryStream != null)
                {
                    tempMemoryStream.Close();
                    tempMemoryStream.Dispose();
                }
            }
        }


        /// <summary>
        /// 取得翻轉過的圖片
        /// </summary>
        /// <param name="img"></param>
        /// <param name="orient"></param>
        /// <returns></returns>
        private Bitmap GetOrientBitmap(Bitmap img, int orient)
        {
            switch (orient)
            {
                case 1:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    break;
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                default:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    break;
            }
            return img;
        }

        /// <summary>
        /// 確認圖片是否翻轉
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private int GetOrientType(string filePath)
        {
            int rotate = 0;
            using (var image = System.Drawing.Image.FromFile(filePath))
            {
                foreach (var prop in image.PropertyItems)
                {
                    if (prop.Id == 0x112)
                        rotate = prop.Value[0];
                }
            }
            return rotate;
        }

        /// <summary>
        /// 取得Imagecode裡面的MineType編碼
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// 文檔輸出
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        //public string SignInForm(string circleKey)
        //{
        //    string fileUrl = "";
        //    MemoryStream memoryStream = null;
        //    try
        //    {
        //        string fileName = "";
        //        bool isGen = pdfService.GenSignInForm(circleKey, ref memoryStream, ref fileName);
        //        if (isGen)
        //        {
        //            SaveFile(memoryStream, fileName);
        //            fileUrl = "Content/PDF/" + fileName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        if (memoryStream != null)
        //        {
        //            memoryStream.Close();
        //            memoryStream.Dispose();
        //        }
        //    }
        //    return fileUrl;
        //}

    }
}
