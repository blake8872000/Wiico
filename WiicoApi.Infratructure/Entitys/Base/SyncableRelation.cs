using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity.Base
{
    public abstract class SyncableRelation : Base.RelationBase, Interface.ISyncable
    {
        public byte[] RowVersion { get; set; }
    }
}
