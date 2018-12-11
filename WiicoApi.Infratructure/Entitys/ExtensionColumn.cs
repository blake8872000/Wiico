using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    /// <summary>
    /// 擴充欄位
    /// </summary>
    public class ExtensionColumn : Base.EntityBase
    {
        [JsonProperty("name")]
        [MaxLength(100)]
        /// <summary>
        /// 姓名
        /// </summary>
        public override string Name{ get;  set;}

        /// <summary>
        /// 顯示名稱
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 組織代碼
        /// </summary>
        [JsonProperty("orgId")]
        public int OrgId { get; set; }

        /// <summary>
        /// 編輯換行
        /// </summary>
        [JsonProperty("editorMultiLine")]
        public int EditorMultiLine { get; set; }

        /// <summary>
        /// 顯示多行
        /// </summary>
        [JsonProperty("displayMultiLine")]
        public bool DisplayMultiLine { get; set; }

        /// <summary>
        /// 限制輸入
        /// </summary>
        [JsonProperty("editorMaxLength")]
        public int EditorMaxLength { get; set; }

        /// <summary>
        /// 輔助連結
        /// </summary>
        [JsonProperty("helpLink")]
        public string HelpLink { get; set; }

        /// <summary>
        /// 輔助連結的字串
        /// </summary>
        [JsonProperty("helpText")]
        public string HelpText { get; set; }

        /// <summary>
        /// 是否可編輯
        /// </summary>
        [JsonProperty("editable")]
        public bool Editable { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        /// <summary>
        /// 是否顯示
        /// </summary>
        [JsonProperty("visibility")]
        public bool Visibility { get; set; }

        //public virtual Organization Organization { get; set;}

    }
}
