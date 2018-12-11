using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using WiicoApi.Repository.EntityRepositorys;

namespace WiicoApi.Repository
{
    public class GenericUnitOfWork  : GenericEntityUnitOfWork<WiicoDB>
    {
        public GenericUnitOfWork(string connectionString)
        {
            connectionString = (connectionString != string.Empty && connectionString!=null) ? 
                string.Format("name={0}", connectionString) :
                null;
            _dbContext = new WiicoDB(connectionString);
        }
        public GenericUnitOfWork()
        {
            _dbContext = new WiicoDB();
        }

        private ActivitysNoticesRepo activitysNoticesRepo = null;
        private ActivitysReadMarksRepo activitysReadMarksRepo = null;
        private ActivitysRepo activitysRepo = null;
        private MembersRepo membersRepo = null;
        private CoursesRepo coursesRepo = null;
        private UserTokensRepo userTokensRepo = null;
        private CircleMemberRoleplaysRepo circleMemberRoleplaysRepo = null;
        private ActVoteRepo actVoteRepo = null;
        private ActVoteItemRepo actVoteItemRepo = null;
        private OrganizationRepo organizationRepo = null;
        private SystemRoleRepo systemRoleRepo = null;
        private DeptRepo deptRepo = null;
        private OrganizationRoleRepo organizationRoleRepo = null;
        private ExternalResourceRepo externalResourceRepo = null;
        private LearningTemplateRoleRepo learningTemplateRoleRepo = null;
        private SemesterGradeRepo semesterGradeRepo = null;
        private MemberInviteRepo memberInviteRepo = null;
        private CalendarRepo calendarRepo = null;
        private FeedBackRepo feedBackRepo = null;
        private LearningCircleRepo learningCircleRepo = null;
        private SignInRepo signInRepo = null;

        public LearningCircleRepo LearningCircleRepo { get {
                return learningCircleRepo ?? new LearningCircleRepo(_dbContext);
            }
        }

        public ActivitysNoticesRepo ActivitysNoticesRepo
        {
            get
            {
                return activitysNoticesRepo ?? new ActivitysNoticesRepo(_dbContext);
            }
        }

        public OrganizationRepo OrganizationRepo
        {
            get
            {
                return organizationRepo ?? new OrganizationRepo(_dbContext);
            }
        }

        public SystemRoleRepo SystemRoleRepo
        {
            get
            {
                return systemRoleRepo ?? new SystemRoleRepo(_dbContext);
            }
        }

        public ActivitysReadMarksRepo ActivitysReadMarksRepo
        {
            get
            {
                return activitysReadMarksRepo ?? new ActivitysReadMarksRepo(_dbContext);
            }
        }



        public ActivitysRepo ActivitysRepo
        {
            get
            {
                return activitysRepo ?? new ActivitysRepo(_dbContext);
            }
        }
        
        public MembersRepo MembersRepo
        {
            get
            {
                return membersRepo ?? new MembersRepo(_dbContext);
            }
        }


        public UserTokensRepo UserTokensRepo
        {
            get
            {
                return userTokensRepo ?? new UserTokensRepo(_dbContext);
            }
        }

        public CoursesRepo CoursesRepo
        {
            get
            {
                return coursesRepo ?? new CoursesRepo(_dbContext);
            }
        }
        public CircleMemberRoleplaysRepo CircleMemberRoleplaysRepo
        {
            get
            {
                return circleMemberRoleplaysRepo ?? new CircleMemberRoleplaysRepo(_dbContext);
            }
        }
        /// <summary>
        /// 投票DA
        /// </summary>
        public ActVoteRepo ActVoteRepo
        {
            get
            {
                return actVoteRepo ?? new ActVoteRepo(_dbContext);
            }
        }

        /// <summary>
        /// 投票DA
        /// </summary>
        public ActVoteItemRepo ActVoteItemRepo
        {
            get
            {
                return actVoteItemRepo ?? new ActVoteItemRepo(_dbContext);
            }
        }

        /// <summary>
        /// 投票DA
        /// </summary>
        public OrganizationRoleRepo OrganizationRoleRepo
        {
            get
            {
                return organizationRoleRepo ?? new OrganizationRoleRepo(_dbContext);
            }
        }
        /// <summary>
        /// 分類DA
        /// </summary>
        public DeptRepo DeptRepo
        {
            get
            {
                return deptRepo ?? new DeptRepo(_dbContext);
            }
        }
        public ExternalResourceRepo ExternalResourceRepo
        {
            get
            {
                return externalResourceRepo ?? new ExternalResourceRepo(_dbContext);
            }
        }

        public LearningTemplateRoleRepo LearningTemplateRoleRepo
        {
            get
            {
                return learningTemplateRoleRepo ?? new LearningTemplateRoleRepo(_dbContext);
            }
        }
        public SemesterGradeRepo SemesterGradeRepo
        {
            get
            {
                return semesterGradeRepo ?? new SemesterGradeRepo(_dbContext);
            }
        }


        public MemberInviteRepo MemberInviteRepo { get { return memberInviteRepo ?? new MemberInviteRepo(_dbContext); } }

        public CalendarRepo CalendarRepo { get { return calendarRepo ?? new CalendarRepo(_dbContext); } }

        public FeedBackRepo FeedBackRepo { get { return feedBackRepo ?? new FeedBackRepo(_dbContext); } }

        public SignInRepo SignInRepo { get { return signInRepo ?? new SignInRepo(_dbContext); } }
    }
}
