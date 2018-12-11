using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity.Base
{

    public abstract class SyncableEntity : EntityBase, Interface.ISyncable
    {
        [MaxLength(100)]
        public string OutsideKey { get; set; }

        public byte[] RowVersion { get; set; }
    }

}
