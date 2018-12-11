using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public  class SceLearningMapResponse
    {
        public string ID_syst { get; set; }
        public string ID_coll { get; set; }
        public string Name_coll { get; set; }
        public string BriefName_coll { get; set; }
        public string Stud_Grade { get; set; }
        public string ID_MainDoma { get; set; }
        public string GradCredit { get; set; }
        public string StudStatus { get; set; }
        public string Stud_SchlInYear { get; set; }
        public string Coll_SemeGrade { get; set; }
        [JsonProperty("colls")]
        public List<SceLearningMapColls> Colls { get; set; }

        [JsonProperty("domas")]
        public List<SceLearningMapDomas> Domas { get; set; }
        [JsonProperty("curss")]
        public List<SceLearningMapCurss> Curss { get; set; }
    }
}
