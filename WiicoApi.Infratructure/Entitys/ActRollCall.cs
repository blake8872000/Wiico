using System;
using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity
{
    public class ActRollCall : Base.ChangeTimeBase
    {
        /// <summary>
        /// 點名活動id - 流水號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 學習圈編號 - 流水號[外來鍵]
        /// </summary>
        public int LearningId { get; set; }

        /// <summary>
        /// 點名活動 Guid
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// 活動名稱
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// beacon用的簽到Key(user端傳入的key值與此相同才可簽到)
        /// </summary>
        public string SignInKey { get; set; }

        /// <summary>
        /// 簽到Key(user端傳入的key值與此相同才可簽到)
        /// </summary>
        [MaxLength(50)]
        public string SignInPwd { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Visibility { get; set; }
    }
}
