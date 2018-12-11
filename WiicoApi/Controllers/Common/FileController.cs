using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Service.CommenService;

namespace WiicoApi.Controllers.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class FileController : ApiController
    {

        /// <summary>
        /// 檔案處理服務
        /// </summary>
        private FileService service = new FileService();
        private TokenService tokenService = new TokenService();
        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();

        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost, Route("api/file/uploadFiles")]
        public async Task< IHttpActionResult> Post(Guid token) {

            var addFiles = new List<Infrastructure.Entity.FileStorage>();
            var streamProvider = new MultipartFormDataStreamProvider(Path.Combine(drivePath,"upload"));
            var dataFromRequest = await Request.Content.ReadAsMultipartAsync(streamProvider);
            var fileStreams = new List<Stream>();
   
            var fileStreamIndex = 0;
            foreach (var fileData in streamProvider.FileData) {
                var datas = new Infrastructure.Entity.FileStorage()
                {
                    Name = fileData.Headers.ContentDisposition.FileName.Trim('\"'),
                //    FileSize = fileData.Headers.ContentLength.HasValue ? (int)fileData.Headers.ContentDisposition.Size.Value : 0,
                    FileContentType = fileData.Headers.ContentType.MediaType,
                    FileGuid = Guid.NewGuid(),
                };
                //建立實體檔案                
                File.Copy(fileData.LocalFileName, Path.Combine(drivePath,datas.FileGuid.ToString("N")));
                File.Delete(fileData.LocalFileName);
                var physicalFiles = File.OpenRead(Path.Combine(drivePath, datas.FileGuid.ToString("N")));

                datas.FileSize = Convert.ToInt32(physicalFiles.Length);
                addFiles.Add(datas);
                fileStreams.Add(physicalFiles);
                fileStreamIndex++;
            }
            var memberInfo = tokenService.GetTokenInfo(token.ToString()).Result;
            var responseData = service.UploadFiles(memberInfo.MemberId, addFiles, fileStreams.ToArray());
            return responseData!=null ? Ok(true) : Ok(false);
        }

        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost, Route("api/file/upload/{token}")]
        public async Task<IHttpActionResult> Upload(Guid token)
        {
            try
            {
                var files = HttpContext.Current.Request.Files;
                // check upload file is exist
                if (HttpContext.Current.Request.Files.Count.Equals(0))
                    return Content(HttpStatusCode.NotFound, "No file upload");
                var memberInfo = tokenService.GetTokenInfo(token.ToString()).Result;
                if (memberInfo != null)
                {
                    var responseData = service.UploadFile(memberInfo.MemberId, files);
                    //從 request content 中讀取檔案並以 BodyPart_{GUID} 格式寫至上方定義的路徑中
                    //   var files = await Request.Content.ReadAsMultipartAsync();
                    var result = new List<FileStorageViewModel>();
                    if (responseData.FirstOrDefault() != null)
                        result = responseData;
                    // 取得實際檔案內容
                    /*   foreach (var httpContent in files.Contents)
                       {
                           var fileName = httpContent.Headers.ContentDisposition.FileName.ToString().Replace("\"","");
                           var contentType = httpContent.Headers.ContentType.MediaType;
                           var size = httpContent.Headers.ContentLength;
                           //建立檔案
                           var dbFile = service.UploadFile(memberInfo.MemberId, fileName, contentType, (int)size);
                           result.Add(dbFile);
                           var path = Path.Combine(drivePath, dbFile.FileGuid.ToString("N"));
                           var stream = httpContent.ReadAsStreamAsync().Result;
                           var bytesInStream = httpContent.ReadAsByteArrayAsync().Result;
                           //實際檔案處理 
                           service.FileProxy((int)size, path, stream, bytesInStream);
                           if (contentType.Contains("image"))
                           {
                               var imgPath = Path.Combine(drivePath, string.Format("{0}_w{1}_h{2}", dbFile.FileGuid.ToString("N"), 640, 480));
                               var image = System.Drawing.Image.FromStream(stream);

                               //建立一個縮圖
                               service.ImageResize(path, 640,480, imgPath, contentType);
                           }
                       }*/
                    return Ok(result);
                }
                else
                    return Content(HttpStatusCode.Forbidden, "error member");

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, "API InternalServerError");
            }
        }

        /// <summary>
        /// 為了取得ican5人員頭像而做 - 之後要批次把ican5頭像轉到ican6中
        /// </summary>
        /// <param name="pictureid"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpGet, Route("api/file/getimg/{pictureid}/{account}")]
        public async Task<HttpResponseMessage> GetiCan5Img(Guid pictureid, string account)
        {
            var response = new HttpResponseMessage();
            using (var hc = new HttpClient())
            {
                //取得ican5圖像
                var img = await hc.GetStreamAsync(string.Format("http://file.sce.pccu.edu.tw/HostingService/api/direct/{0}/{1}.jpg", pictureid, account));
                response.Content = new StreamContent(img);
                //response.Content.Headers.ContentLength = img.Length;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                return response;
            }
        }
        /// <summary>
        /// 取得原始檔案
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        [HttpGet, Route("api/file/{g}")]
        public HttpResponseMessage Get(string g)
        {
            var response = new HttpResponseMessage();
            // NOTE: If there was any other 'async' stuff here, then you would need to return
            // a Task<IHttpActionResult>, but for this simple case you need not.
            var result = new FileService(g);
            Guid fileGuid;
            var cancelToken = new CancellationToken(true);

            //查詢FILE DB容器
            var fileInfo = new Infrastructure.Entity.FileStorage();

            if (Guid.TryParse(g, out fileGuid))
                fileInfo = service.GetFileInfoByFileGuidName(fileGuid);

            if (fileInfo == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            //查出實體檔案
            var fileData = result.ExecuteAsync(cancelToken);
            if (fileData == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            else
            {
                response.Content = new StreamContent(fileData);
                response.Content.Headers.ContentLength = fileInfo.FileSize;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(fileInfo.FileContentType);

                return response;
            }
        }

        /// <summary>
        /// 取得縮圖檔案
        /// </summary>
        /// <param name="g">檔案代碼</param>
        /// <param name="w">縮圖寬度</param>
        /// <param name="h">縮圖高度</param>
        /// <returns></returns>
        [HttpGet, Route("api/imgFile/{g}/{w}/{h}")]
        public Task<HttpResponseMessage> GetImage(string g, int w, int h)
        {
            var result = new FileService(g, string.Format("{0}_w{1}_h{2}", g, w, h), w, h);
            var cancelToken = new CancellationToken(true);
            var response = new HttpResponseMessage();
            Guid fileGuid;
            //查詢FILE DB容器
            var fileInfo = new Infrastructure.Entity.FileStorage();

            if (Guid.TryParse(g, out fileGuid))
                fileInfo = service.GetFileInfoByFileGuidName(fileGuid);

            if (fileInfo == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;

                return Task.FromResult(response);
            }

            //查出實體檔案
            var fileData = result.ExecuteAsync(cancelToken);
            if (fileData == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return Task.FromResult(response);
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StreamContent(fileData);
                response.Content.Headers.ContentLength = fileData.Length;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(fileInfo.FileContentType);

                return Task.FromResult(response);
            }
        }
    }
}
