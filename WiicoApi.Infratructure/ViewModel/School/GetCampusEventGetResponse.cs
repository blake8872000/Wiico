using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class GetCampusEventGetResponse
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string BoarderID { get; set; }
        public string UpDateMan { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateMan { get; set; }
        public DateTime? CreateDate { get; set; }
        public string TitleC { get; set; }
        public string TitleE { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBigEvent { get; set; }
        public string Url{ get; set; }
        public string PeriodWeek { get; set; }
        [JsonIgnore]
        public int? Updater { get; set; }
        [JsonIgnore]
        public int? FileId { get; set; }
        [JsonIgnore]
        public int? DeptId { get; set; }
        [JsonIgnore]
        public int? SemesterGradeId { get; set; }
        [JsonIgnore]
        public int? OrganizationRoleId { get; set; }
    }
}
