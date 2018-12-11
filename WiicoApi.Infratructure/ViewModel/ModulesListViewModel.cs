using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel
{
    public class ModulesListViewModel
    {
        /// <summary>
        /// 目前要顯示的moduleKey
        /// </summary>
        [JsonProperty("moduleKey")]
        public string ModuleKey { get; set; }
        /// <summary>
        /// 顯示要回去的learningId
        /// </summary>
        [JsonProperty("learningId")]
        public int LearningId { get; set; }
        /// <summary>
        /// 學習圈的key
        /// </summary>
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        /// <summary>
        /// 學習圈的名稱
        /// </summary>
        [JsonProperty("circleName")]
        public string CircleName { get; set; }
        /// <summary>
        /// 是否為模組管理者角色
        /// </summary>
        [JsonProperty("isAdminRole")]
        public bool IsAdminRole { get; set; }
        /// <summary>
        /// 作業列表用
        /// </summary>
        [JsonProperty("homeWorkList")]
        public List<HomeWorkViewModel> HomeWorkList { get; set; }
        /// <summary>
        /// 點名列表用
        /// </summary>        
        [JsonProperty("signInList")]
        public List<ValueObject.SignInEvent> SignInList { get; set; }
        /// <summary>
        /// 教材列表用
        /// </summary>
        [JsonProperty("materialList")]
        public List<MaterialViewModel> MaterialList { get; set; }
        /// <summary>
        /// 主題討論列表用
        /// </summary>
        [JsonProperty("discussionList")]
        public List<DiscussionViewModel> DiscussionList { get; set; }
        /// <summary>
        /// 分組列表用
        /// </summary>
        [JsonProperty("groupList")]
        public List<GroupListViewModel> GroupList { get; set; }
    }
}
