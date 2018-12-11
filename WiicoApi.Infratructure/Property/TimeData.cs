using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Property
{
    [ComplexType]
    /// <summary>
    /// 此類別處理資料庫中儲存UTC時間，但讀取資料時可顯示本地時間
    /// </summary>
    public class TimeData
    {
        public static TimeData Create(DateTime? utcTime)
        {
            var t = new TimeData();
            t.Utc = utcTime;
            return t;
        }

        public static TimeData CreateEmpty()
        {
            return new TimeData();
        }
        string isoformat = "yyyy-MM-ddTHH:mm:ss";

        /*
		using System.Web.Mvc;
		[HiddenInput(DisplayValue = false)]
		*/
        public virtual DateTime? Utc { get; set; }

        [NotMapped, Display(Name = "")]
        public DateTime? Local
        {
            get { return Utc?.ToLocalTime(); }
            set { Utc = value?.ToUniversalTime(); }
        }
        public override string ToString()
        {
            return Local?.ToString(isoformat);
        }
    }
}
