using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class CourseMember
    {
        public int MemberId { get; set; }
        [MaxLength(50)]
        public string MemberName { get; set; }
        [MaxLength(100)]
        public string MemberAccount { get; set; }
        public int SectionRoleId { get; set; }
        [MaxLength(50)]
        public string RoleName { get; set; }
    }
}
