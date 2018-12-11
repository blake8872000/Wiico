using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Service.ActivityModule.Provider;

namespace WiicoApi.Service.ActivityModule
{
    class ModuleFactory
    {
        public static ModuleProvider CreateModuleProvider(Infrastructure.DataTransferObject.ModuleParameter param)
        {
            switch (param.ModuleKey)
            {
             //   case "upload":
             //       return new HomeworkModuleProvider(param);
                case "material":
                    return new MaterialModuleProvider(param);
                case "signIn":
                    return new SigInModuleProvider(param);
                case "discussion":
                    return new DiscussionModuleProvider(param);
                case "group":
                    return new GroupModuleProvider(param);
                default:
                    return null;
            }
        }
    }
}
