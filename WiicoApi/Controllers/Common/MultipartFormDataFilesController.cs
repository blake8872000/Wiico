using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WiicoApi.Controllers.Common
{
    /// <summary>
    /// 處理httpRequest Multipart Form-Data檔案上傳資訊
    /// </summary>
    /// <typeparam name="T">multipart Form-Data其他參數的Model</typeparam>
    [EnableCors("*","*","*")]
    public class MultipartFormDataFilesController<T> : ApiController
    {
        /// <summary>
        /// 預設存檔位置
        /// </summary>
        private string drivePath = ConfigurationManager.AppSettings["DrivePath"].ToString();
        /// <summary>
        /// 檔案Stream索引
        /// </summary>
        public int fileStreamIndex = 0;
        /// <summary>
        /// 暫存的Stream檔案
        /// </summary>
        public List<Stream> fileStreams ;
        /// <summary>
        /// 取得multipartFormData的檔案
        /// </summary>
        public List<Infrastructure.Entity.FileStorage> multipartFormDataFiles ;
        /// <summary>
        /// multipartFormData的參數
        /// </summary>
        public T multipartFormDataModel { get; set; }
        /// <summary>
        /// 設定存檔實體位置
        /// </summary>
        public MultipartFormDataStreamProvider streamProvider { get {
                return new MultipartFormDataStreamProvider(Path.Combine(drivePath, "upload"));
            }
        }
        /// <summary>
        /// 用於暫存multipartFormData的request資料
        /// </summary>
        public MultipartFormDataStreamProvider multipartFormDataStreamProvider { get; set; }

        /// <summary>
        /// 取得multipartFormData的檔案，並存入預設位置，同時設定FileStorage以及Stream
        /// </summary>
        public MultipartFormDataFilesController() {
            multipartFormDataFiles = new List<Infrastructure.Entity.FileStorage>();
            fileStreams = new List<Stream>();
        }
        /// <summary>
        /// 設定檔案
        /// </summary>
        [HttpDelete]
        public async Task SetFileData() {
             multipartFormDataStreamProvider = await Request.Content.ReadAsMultipartAsync(streamProvider);
            //設定檔案資訊
            foreach (var fileData in multipartFormDataStreamProvider.FileData)
            {
                var datas = new Infrastructure.Entity.FileStorage()
                {
                    Name = fileData.Headers.ContentDisposition.FileName.Trim('\"'),
                    FileContentType = fileData.Headers.ContentType.MediaType,
                    FileGuid = Guid.NewGuid(),
                };
                //建立實體檔案                
                File.Copy(fileData.LocalFileName, Path.Combine(drivePath, datas.FileGuid.ToString("N")));
                File.Delete(fileData.LocalFileName);
                var physicalFiles = File.OpenRead(Path.Combine(drivePath, datas.FileGuid.ToString("N")));
                datas.FileSize = Convert.ToInt32(physicalFiles.Length);
                multipartFormDataFiles.Add(datas);
                fileStreams.Add(physicalFiles);
                fileStreamIndex++;
            }
        }
    }
}
