using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class learningMapColls
    {
        public string ID_CredType { get; set; }
        public string NAME_CredType { get; set; }
        public string Memo_Cred { get; set; }
        public decimal? Cred_Coll { get; set; }
        public decimal? Stud_SumCredit { get; set; }
        public bool CombinedDoma { get; set; }

    }
}
