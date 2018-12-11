using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;

namespace WiicoApi.Repository
{

    public class MigrationsContextFactory : IDbContextFactory<WiicoDB>
    {
        public WiicoDB Create()
        {
            return new WiicoDB("name=AzureHkxfDB");
        }
    }
    /// <summary>
    /// DB連接
    /// </summary>
    public class WiicoDB : DbContext
    {
        /// <summary>
        /// DB連接
        /// </summary>
        /// <param name="connectString">連接DB字串名稱 目前有name=AzureDB,iCan6DB,FlipusDB,iThinkDB,AzureTestDB,AzureUnitTestDB</param>
        public WiicoDB(string connectString = null)
       : base(connectString != null ? connectString : "name=AzureHkxfDB")
        {
        }

        // 針對您要包含在模型中的每種實體類型新增 DbSet。如需有關設定和使用
        // Code First 模型的詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=390109。
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<ExternalResource> ExtResources { get; set; }
        public virtual DbSet<ExternalResType> ExtResTypes { get; set; }
        public virtual DbSet<Dept> Depts { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<ModuleFuntions> ModuleFunction { get; set; }
        public virtual DbSet<SystemRole> SystemRole { get; set; }
        public virtual DbSet<Modules> Modules { get; set; }
        public virtual DbSet<UserToken> UserToken { get; set; }
        public virtual DbSet<SqlConnectionString> SqlConnectionString { get; set; }
        public virtual DbSet<LikeLog> LikeLog { get; set; }
        public virtual DbSet<Activitys> Activitys { get; set; }
        public virtual DbSet<ActivitysReadMark> ActivitysReadMark { get; set; }
        public virtual DbSet<ActivitysNotices> ActivitysNotice { get; set; }
        /// <summary>
        /// 出缺勤 - 請假主檔
        /// </summary>
        public virtual DbSet<AttendanceLeave> AttendanceLeave { get; set; }
        /// <summary>
        /// 出缺勤 - 請假歷程檔
        /// </summary>
        public virtual DbSet<AttendanceLeaveLog> AttendanceLeaveLog { get; set; }
        /// <summary>
        /// 出缺勤 - 學生log檔
        /// </summary>
        public virtual DbSet<AttendanceRecord> AttendanceRecord { get; set; }
        /// <summary>
        /// 點名模組用
        /// </summary>
        public virtual DbSet<ActRollCall> ActRollCall { get; set; }
        public virtual DbSet<ActRollCallLog> ActRollCallLog { get; set; }
        /// <summary>
        /// 訊息模組用
        /// </summary>
        public virtual DbSet<ActMessage> ActMessage { get; set; }
        /// <summary>
        /// 作業模組用
        /// </summary>
        public virtual DbSet<ActHomeWork> ActHomeWork { get; set; }
        public virtual DbSet<ActHomeWorkLog> ActHomeWorkLog { get; set; }
        public virtual DbSet<ActHomeWorkScore> ActHomeWorkScore { get; set; }
        /// <summary>
        /// 教材模組用
        /// </summary>
        public virtual DbSet<ActMaterial> ActMaterial { get; set; }
        public virtual DbSet<LearningCircle> LearningCircle { get; set; }
        public virtual DbSet<LearningAuth> LearningAuth { get; set; }
        public virtual DbSet<LearningRole> LearningRole { get; set; }
        /// <summary>
        /// 學習圈與人與角色關聯用
        /// </summary>
        public virtual DbSet<CircleMember> CircleMember { get; set; }
        public virtual DbSet<CircleMemberRoleplay> CircleMemberRoleplay { get; set; }

        public virtual DbSet<ExtensionValue> ExtensionValue { get; set; }

        public virtual DbSet<LCExtensionValue> LCExtensionValue { get; set; }
        public virtual DbSet<ExtensionColumn> ExtensionColumn { get; set; }

        public virtual DbSet<ExtensionColumnCustomization> ExtensionColumnCustomization { get; set; }

        /// <summary>
        /// 用於放目前上課教室跟時間日期
        /// </summary>
        public virtual DbSet<TimeTable> TimeTable { get; set; }
        /// <summary>
        /// 用於同步iCan5的比對資料表
        /// </summary>
        public virtual DbSet<SyncLog> SyncLog { get; set; }
        /// <summary>
        /// 同步iCan5目前課程的進度表
        /// </summary>
        public virtual DbSet<Syllabus> Syllabus { get; set; }

        /// <summary>
        /// 分組主表
        /// </summary>
        public virtual DbSet<ActGroupCategory> ActGroupCategory { get; set; }
        /// <summary>
        /// 第二層分組
        /// </summary>
        public virtual DbSet<ActGroup> ActGroup { get; set; }
        /// <summary>
        /// 分組成員
        /// </summary>
        public virtual DbSet<ActGroupMember> ActGroupMember { get; set; }
        /// <summary>
        /// 分組活動關聯表
        /// </summary>
        public virtual DbSet<ModuleGroupCategory> ModuleGroupCategory { get; set; }
        /// <summary>
        /// 部門角色成員關聯表
        /// </summary>
        public virtual DbSet<DepartmentAdmin> DepartmentAdmin { get; set; }
        /// <summary>
        /// 主題討論
        /// </summary>
        public virtual DbSet<ActDiscussion> ActDiscussion { get; set; }
        /// <summary>
        /// 主題討論 - 留言
        /// </summary>
        public virtual DbSet<ActDiscussionMsg> ActDiscussionMsg { get; set; }
        /// <summary>
        /// 活動模組留言
        /// </summary>
        public virtual DbSet<ActModuleMessage> ActModuleMessage { get; set; }


        #region iCan5介接iThinkAPI專用
        /// <summary>
        /// 公版活動資料表
        /// </summary>
        public virtual DbSet<ActGeneral> ActGeneral { get; set; }

        #endregion

        #region 第二階段新增的資料表
        /// <summary>
        /// 學習圈模組管理
        /// </summary>
        public virtual DbSet<LearningCircleModuleManage> LearningCircleModuleManages { get; set; }

        /// <summary>
        /// 課程同步基本資訊
        /// </summary>
        public virtual DbSet<SyncCourseLog> SyncCourseLog { get; set; }


        /// <summary>
        /// 規則
        /// </summary>
        public virtual DbSet<Rules> Rules { get; set; }

        /// <summary>
        /// 給分規則
        /// </summary>
        public virtual DbSet<RuleScore> RuleScore { get; set; }

        /// <summary>
        /// 請假類別
        /// </summary>
        public virtual DbSet<LeaveCategory> LeaveCategory { get; set; }

        /// <summary>
        /// 請假檔案
        /// </summary>
        public virtual DbSet<LeaveFile> LeaveFile { get; set; }

        /// <summary>
        /// 討論檔案
        /// </summary>
        public virtual DbSet<DiscussionFile> DiscussionFile { get; set; }

        /// <summary>
        /// google檔案資訊
        /// </summary>
        public virtual DbSet<GoogleFile> GoogleFile { get; set; }

        /// <summary>
        /// 推播合法token - 每日更新一次
        /// </summary>
        public virtual DbSet<PushAccessToken> PushAccessToken { get; set; }

        #endregion



        #region 2018-02-05以後開的資料表 沒特別意義，單純分階段而已

        ///檔案倉庫資料表
        public virtual DbSet<FileStorage> FileStorage { get; set; }
        ///程式版本資料表
        public virtual DbSet<AppVersion> AppVersion { get; set; }

        /// <summary>
        /// 錯誤資訊紀錄表
        /// </summary>
        public virtual DbSet<SystemErrorLog> SystemErrorLog { get; set; }

        /// <summary>
        /// 錯誤資訊類型
        /// </summary>
        public virtual DbSet<SystemErrorType> SystemErrorType { get; set; }

        /// <summary>
        /// 投票活動資料表
        /// </summary>
        public virtual DbSet<ActVote> ActVote { get; set; }

        /// <summary>
        /// 投票活動項目資料表
        /// </summary>
        public virtual DbSet<ActVoteItem> ActVoteitem { get; set; }

        /// <summary>
        /// 課程周次資料表
        /// </summary>
        public virtual DbSet<WeekTable> WeekTable { get; set; }

        /// <summary>
        /// 學習圈管理者資料表
        /// </summary>
        public virtual DbSet<LearningCircleManager> LearningCircleManager { get; set; }

        /// <summary>
        /// 學制資料表
        /// </summary>
        public virtual DbSet<SemesterGrade> SemesterGrade { get; set; }

        /// <summary>
        /// 活動進度資料表
        /// </summary>
        public virtual DbSet<ActivitySyllabus> ActivitySyllabus { get; set; }

        /// <summary>
        /// 行事曆資料表
        /// </summary>
        public virtual DbSet<Calendar> Calendar { get; set; }


        public virtual DbSet<SchoolRole> SchoolRole { get; set; }

        public virtual DbSet<OrganizationRole> OrganizationRole { get; set; }

        public virtual DbSet<CalendarSemester> CalendarSemester { get; set; }

        public virtual DbSet<CalendarOrganizationRole> CalendarOrganizationRole { get; set; }

        public virtual DbSet<ActBulletin> ActBulletin { get; set; }
        public virtual DbSet<LearningTemplateRoles> LearningTemplateRoles { get; set; }
        public virtual DbSet<CalendarDept> CalendarDept { get; set; }
        /// <summary>
        /// 組織登入API參數
        /// </summary>
        public virtual DbSet<OrganizationLoginColumn> OrganizationLoginColumn { get; set; }

        public virtual DbSet<PushData> PushData { get; set; }

        public virtual DbSet<PushLog> PushLog { get; set; }

        /// <summary>
        /// 邀請碼
        /// </summary>
        public virtual DbSet<MemberInvite> MemberInvite { get; set; }

        /// <summary>
        /// 問題回報
        /// </summary>
        public virtual DbSet<FeedBack> FeedBack { get; set; }
        #endregion


        #region 2018-11-23新階段
        /// <summary>
        /// SmartTA JoinGroup紀錄表
        /// </summary>
       public virtual DbSet<SmartTAJoinGroupLog> SmartTAJoinGroupLog { get; set; }
        
        #endregion  

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
