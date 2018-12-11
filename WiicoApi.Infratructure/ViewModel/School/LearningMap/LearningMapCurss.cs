using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class LearningMapCurss
    {
        public string ID_CredType { get; set; }
        public string Name_Clop{ get; set; }
        public string ID_Doma { get; set; }
        public string ID_Curs { get; set; }
        public string Name_Curs { get; set; }
        public decimal? Credit { get; set; }
        public decimal? SumCredit { get; set; }
        public bool Passed { get; set; }
        public decimal? OrderNo_Curs { get; set; }
        public string Memo { get; set; }
    }
}
