using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.SignalR.SignIn
{
    public class ServerCheckItem
    {
        /// <summary>
        /// 成員編號，權限確認後可取用
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 活動Guid，須給OuterKey，權限確認後才可取用
        /// </summary>
        public Guid EventId { get; set; }
        /// <summary>
        /// 活動outerKey，權限確認後，可透過 EventId 屬性取回對應值
        /// </summary>
        public string OuterKey { get; set; }

        /// <summary>
        /// 學習圈編號，須給CircleKey，權限確認後才可取用
        /// </summary>
        public int CircleId { get; set; }
        /// <summary>
        /// 學習圈outerKey，權限確認後，可透過 CircleId 屬性取回對應值
        /// </summary>
        public string CircleKey { get; set; }

        /// <summary>
        /// 模組權限，須給ModuleFun、CircleKey，權限確認後才可取用
        /// </summary>
        public bool ModuleAuth { get; set; }
        /// <summary>
        /// 模組方法outerKey，權限確認後，可透過 ModuleAuth 屬性取回對應值
        /// </summary>
        public string ModuleFun { get; set; }

    }
}