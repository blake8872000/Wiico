using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.School
{
    public class LearningMapDomas
    {
        public string ID_CredType { get; set; }
        public string ID_Doma { get; set; }
        public string Name_Doma { get; set; }

        public decimal? Cred_Doma { get; set; }
        public decimal? Stud_Doma_SumCredit { get; set; }
        public decimal? OrderNo_Doma { get; set; }
    }
}
