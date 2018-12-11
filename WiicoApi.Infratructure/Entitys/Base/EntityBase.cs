using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure;

namespace WiicoApi.Infrastructure.Entity.Base
{
    
        /// <summary>
        /// 具有基本的Id主鍵欄位，名稱、建立日期與更新日期的標準基底類
        /// </summary>

    public abstract class EntityBase : ChangeTimeBase
    {
        public EntityBase()
        {
        }

        [Key]
        [JsonProperty("id")]
        public virtual int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Localization))]
        [JsonProperty("name")]
        public virtual string Name { get; set; }



    }
}
