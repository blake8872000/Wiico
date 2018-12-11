using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;

using EntityRepository;
using WiicoApi.Infrastructure.ValueObject;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class CoursesRepo : GenericEntityRepository<Course,WiicoDB>
    {

        public CoursesRepo(WiicoDB _context) : base(_context)
        {
        }

       public IEnumerable<CourseInfo> GetCourseInfoList(int learningId)
        {
            string sql = @" select c.*, d.Name as DeptName,lc.Id as CircleId from courses c
                            inner join LearningCircles lc on c.CourseCode = lc.LearningOuterKey
                            inner join Depts d on c.DeptId = d.Id
                            where lc.Id=@learningId";

            var data = _context.Database.SqlQuery<CourseInfo>(sql,
                 new SqlParameter("@learningId", learningId)).ToList();

            return data;
        }

        public IEnumerable<ICan5Course> GetICan5CourseList(string cour_year)
        {
            string sql = @" select course_no,
                                  cour_name_1000,
                                  cour_name_2000,
                                  cour_year,
                                  coll_no,
                                  cour_comment_1000,
                                  cour_comment_2000,
                                  cour_teachitem,
                                  cour_otheritem,
                                  startdate,
                                  enddate,
                                  createdate from iCan5.dbo.Course
                            where cour_year = @cour_year";

            var data = _context.Database.SqlQuery<ICan5Course>(sql,
                 new SqlParameter("@cour_year", cour_year)).ToList();

            return data;
        }

        public void CheckCourseInfo(string course_no)
        {
            _context.Database.SqlQuery<int>("usp_SynciCan5 @course_no", new SqlParameter("course_no", course_no));
        }
    }
}
