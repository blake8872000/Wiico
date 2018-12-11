using WiicoApi.Infrastructure.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.ViewModel.Base;
namespace WiicoApi.Infrastructure.ViewModel.Backend
{
    public class SemesterGradePostRequest : BackendBaseRequest
    {
        [JsonProperty("semesterCode")]
        public string SemesterCode { get; set; }

        [JsonProperty("gradeYears")]
        public int GradeYears { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("semesters")]
        public List<SemesterGrade> Semesters { get; set; }
    }
}
