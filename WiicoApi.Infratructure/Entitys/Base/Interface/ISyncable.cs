using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity.Base.Interface
{
    /// <summary>
	/// 定義為可與外部資料同步的資料，必須有外部鍵值
	/// </summary>
	interface ISyncable
    {
        [Timestamp]
        byte[] RowVersion { get; set; }
    }
}
