﻿using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityRepository;
using WiicoApi.Infrastructure.ValueObject;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class SystemRoleRepo : GenericEntityRepository<SystemRole,WiicoDB>
    {
        public SystemRoleRepo(WiicoDB context) : base(context)
        {
        }
    }
}
