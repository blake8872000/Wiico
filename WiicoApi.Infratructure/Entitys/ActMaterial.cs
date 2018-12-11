using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActMaterial : Base.ChangeTimeBase
    {
         /// <summary>
        /// 教材活動id - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        public int LearningId { get; set; }

        /// <summary>
        /// 教材活動 Guid
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 活動名稱 - 教材檔案名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所屬googleDriveFileId
        /// </summary>
        public string GoogleDriveFileId { get; set; }
        /// <summary>
        /// 所屬活動的資料夾
        /// </summary>
        public string GoogleDriveFolder { get; set; }
        /// <summary>
        /// 檔案類型
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 檔案大小
        /// </summary>
        public int FileLength { get; set; }
        /// <summary>
        /// 檔案縮圖
        /// </summary>
        public string FileImgUrl { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Visibility { get; set; }

    }
}
