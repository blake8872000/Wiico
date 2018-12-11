using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class GetLearningMapDataResponse
    {
        public string ID_syst { get; set; }
        public string ID_coll { get; set; }
        public string Name_coll { get; set; }
        public string BriefName_coll { get; set; }
        public int? Stud_Grade { get; set; }
        public string ID_MainDoma { get; set; }
        public decimal? GradCredit { get; set; }
        public string StudStatus { get; set; }
        public int? Stud_SchlInYear { get; set; }
        public int? Coll_SemeGrade { get; set; }
        [JsonProperty("colls")]
        public List<learningMapColls> Colls { get; set; }

        [JsonProperty("domas")]
        public List<LearningMapDomas> Domas { get; set; }
        [JsonProperty("curss")]
        public List<LearningMapCurss>Curss { get; set; }
    }
}
