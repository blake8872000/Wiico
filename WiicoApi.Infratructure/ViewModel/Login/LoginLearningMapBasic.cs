using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.Login
{
    public class LoginLearningMapBasic
    {
        /// <summary>
        /// 院系名稱
        /// </summary>
        public string Name_coll { get; set; }
        /// <summary>
        /// 學生年級
        /// </summary>
        public int? Stud_Grade { get; set; }
        /// <summary>
        ///  模組代碼
        /// </summary>
        public string ID_MainDoma { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
        public string Name_MainDoma { get; set; }

        /// <summary>
        /// 在學/畢業狀態
        /// </summary>
        public string StudStatus { get; set; }
        /// <summary>
        /// 入學年
        /// </summary>
        public int? Stud_SchlInYear { get; set; }
        /// <summary>
        /// 所屬學年級
        /// </summary>
        public int? Coll_SemeGrade { get; set; }
        /// <summary>
        /// 學制代碼
        /// </summary>
        public string ID_syst { get; set; }
        /// <summary>
        /// 學制要讀幾年
        /// </summary>
        public int GradeYears { get; set; }
        /// <summary>
        /// 總共有幾學期(目前固定為3)
        /// </summary>
        public int SemesterLength { get; set; }
        /// <summary>
        /// 目前學年
        /// </summary>
        public int? cour_year { get; set; }
        /// <summary>
        /// 目前學期
        /// </summary>
        public int? cour_seme { get; set; }

        /// <summary>
        /// 目標達成學分
        /// </summary>
        public decimal goalCredit { get; set; }

        /// <summary>
        /// 目前達成學分
        /// </summary>
        public decimal correspondCredit { get; set; }

        /// <summary>
        /// 以學制對應要讀幾年
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int IdToGradeYears(string id)
        {
            switch (id)
            {
                case "10":
                    //二專
                    return 5;
                case "30":
                //進學
                case "43":
                //碩專
                case "44":
                //產碩
                case "47":
                    //境外專班
                    return 4;
                default:
                    return 4;
            }
        }
    }
}
