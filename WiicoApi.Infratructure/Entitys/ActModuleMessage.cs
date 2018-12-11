using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    ///  用於讓活動共用該聊天室的結構，去長出屬於特定活動的聊天室
    /// </summary>
    public class ActModuleMessage : Base.ChangeTimeBase
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Display(Name = "留言流水號")]
        public int Id { get; set; }
        /// <summary>
        /// 所屬活動代碼
        /// </summary>
        [Display(Name = "所屬活動代碼")]
        public int ActivityId { get; set; }
        /// <summary>
        /// 留言類型 - 含圖片[GoogleUrl]+[text]
        /// </summary>
        [Display(Name = "留言類型")]
        public string MsgType { get; set; }
        /// <summary>
        /// 留言內容 - 含圖片[GoogleUrl]+[text]
        /// </summary>
        [Display(Name = "留言內容")]
        public string Content { get; set; }
        /// <summary>
        /// 所屬活動模組
        /// </summary>
        [Display(Name = "所屬活動模組")]
        public string ModuleType { get; set; }

        /// <summary>
        /// 留言代碼
        /// </summary>
        [Display(Name = "留言代碼")]
        public Guid OuterKey { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        [Display(Name = "是否有效")]
        public bool Visibility { get; set; }

        /// <summary>
        /// 回覆哪則留言
        /// </summary>
        [Display(Name = "回覆哪則留言")]
        public int? Parent { get; set; }

        /// <summary>
        /// Tag哪個回覆
        /// </summary>
        [Display(Name = "Tag哪個回覆")]
        public int? TagActModuleMessageId { get; set; }
    }
}
