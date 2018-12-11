using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity.Base
{
    public class TimeMarkBase : Interface.ITimeMark
    {
        //[Editable(false, AllowInitialValue = false)]
        //[Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [JsonIgnore]
        [ReadOnly(true), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Property.TimeData Created { get; set; }
        [JsonIgnore]
        [ReadOnly(true), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public Property.TimeData Updated { get; set; }
        [JsonIgnore]
        [ReadOnly(true), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public Property.TimeData Deleted { get; set; }
    }
}
