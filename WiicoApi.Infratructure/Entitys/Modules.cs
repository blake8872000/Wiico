using System.ComponentModel.DataAnnotations;

namespace WiicoApi.Infrastructure.Entity
{
    //模組資料表
    public class Modules : Base.EntityBase
    {
        /// <summary>
        /// 模組名稱
        /// </summary>
        [MaxLength(50)]
        public override string Name { get; set; }

        /// <summary>
        /// 是否在+號頁出現模組icon
        /// </summary>
        public bool ShowOnAddPage { get; set; }

        /// <summary>
        /// 在+號頁出現的排序
        /// </summary>
        public int OnAddPageSort { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 對外顯示的字串
        /// </summary>
        [MaxLength(100)]
        public string OutSideKey { get; set; }
        /// <summary>
        /// 需要計算分數
        /// </summary>
      //  public bool IsScore { get; set; }
        /// <summary>
        /// 需要被貼到活動牆
        /// </summary>
       // public bool IsModuleWall { get; set; }
    }
}
