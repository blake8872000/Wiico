using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActHomeWorkLog : Base.ChangeTimeBase
    {
        /// <summary>
        /// 作業上傳紀錄編號 - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 作業活動Id - 流水號
        /// </summary>
        public int HomeWorkId { get; set; }

        /// <summary>
        /// 學生作業的說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 上傳時間
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 上傳作業的學生MemId
        /// </summary>
        public int StudId { get; set; }

        /// <summary>
        /// 上傳的狀態:1已繳交,2未繳交,3遲交
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 上傳檔案數量
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 學生作業上傳草稿區
        /// </summary>
        [MaxLength(50)]
        public string TempGoogleDriveFolder { get; set; }

        /// <summary>
        /// 是否允許上傳
        /// </summary>
        public bool AllowSend { get; set; }

        /// <summary>
        /// 是否需要參與
        /// </summary>
        public bool Participated { get; set; }




    }
}
