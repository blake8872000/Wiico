using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using WiicoApi.Infrastructure.ValueObject;
using EntityRepository;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class MembersRepo : GenericEntityRepository<Member,WiicoDB>
    {

        public MembersRepo(WiicoDB _context) : base(_context)
        {
        }
        /// <summary>
        /// 從iCan取Token資訊 但是對應到 iCan6 Entity中
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public LoginTokenInfo CheckiCanToken(Guid token)
        {
            string strSQL = @"
SELECT m.Id as memberId , m.orgId,m.IsOrgAdmin as IsOrgAdmin
                        FROM [ICAN5].[dbo].[ManToken] t
                        left outer join [dbo].[Members] m on t.man_no=m.Account
                        where token=@token
";

            var data = _context.Database.SqlQuery<LoginTokenInfo>(strSQL,
                             new SqlParameter("@token", token)).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 取得某個Member詳細資訊，包含相片
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public TempMember GetSingleMember(string account)
        {
            string strSQL = @"select m.Id,m.Name,m.OrgId,m.Email,m.Account,m.Photo,lr.[Enable], lr.Name as RoleName, lr.IsAdminRole
                        from [dbo].[MembersView] m 
                        join [dbo].[CircleMemberRoleplays] cmr on m.Id=cmr.MemberId
                        join [dbo].[LearningRoles] lr on cmr.RoleId=lr.Id
                        join [dbo].[LearningCircles] lc on lr.LearningId=lc.Id
                        left outer join [ICAN5].[dbo].[ManScore] ms on m.Account=ms.man_no and ms.course_no=lc.LearningOuterKey
                        where ms.manscore_mark='10' and ms.man_no=@account";
            var data =  _context.Database.SqlQuery<TempMember>(strSQL,
                             new SqlParameter("@account", account)).FirstOrDefault();
            return data;
        }

        public TempMember MemberInfo(int accountId)
        {
            //2016/9/12 mark by sophiee:為顯示舊ican個人大頭照，不讀取members table
            //iThinkDB db = new iThinkDB();
            //var sqlMemberInfo = db.Members.Find(accountId);
            //return sqlMemberInfo;

            //2016/9/12 add by sophiee:為顯示舊ican個人大頭照，讀取membersview
            string strSQL = @"select Id,Name,OrgId,Email,Account,Photo
                        from [dbo].[MembersView]
                        where Id=@accountId";

            var data = _context.Database.SqlQuery<TempMember>(strSQL,
                             new SqlParameter("@account", accountId)).FirstOrDefault();
            return data;
        }

        public IEnumerable<SignInData> GetSignInData(Infrastructure.DataTransferObject.SignInEventParam param)
        {
            #region //SQL
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select m.*, Sort=CAST(ROW_NUMBER() OVER(ORDER BY ms.manscore_type,ms.manscore_collno,ms.manscore_grade DESC,ms.manscore_grp,isnull(ms.manscore_subgrp,''),ms.man_no) AS INT)
                        from
                        (
                        select distinct s.Id, a.ToRoomId, a.ModuleKey, OuterKey=s.EventId, CreatorAccount=m.Account, CreatorName=m.Name,CreatorPhoto=m.Photo, a.Created_Utc, a.Updated_Utc, a.Deleted_Utc,a.ActivityDate
		                        , Convert(bit, case when s.id=(SELECT max(id) FROM [dbo].[ActRollCalls] where LearningId=s.LearningId) then 1 else 0 end) as 'IsNewest'
                                , a.StartDate, a.Duration, ActivityId=a.Id, s.Name, s.SignInKey, s.SignInPwd, StuId=mem.Id, StudId=mem.Account, StudName=mem.Name, StudPhoto=mem.Photo
		                        , case when l.[Status] is null then '-1' else l.[Status] end [Status]
                                , al.[Status] as LeaveStatus
                                , al.[EventId] as LeaveEventId
		                        , l.[Time], LogCreator=c.Account, LogUpdateDate=l.Updated_Utc
                                , a.[Publish_Utc]
                        from [dbo].[Activitys] a 
                        left outer join [dbo].[ActRollCalls] s on a.OuterKey=s.EventId
                        left outer join [dbo].[MembersView] m on a.CreateUser=m.Id
                        left outer join [LearningRoles] AS lr ON lr.LearningId = s.LearningId
                        left outer join [CircleMemberRoleplays] AS cr ON cr.RoleId=lr.Id
                        left outer join [LearningAuths] AS la ON cr.RoleId = la.[LearningRoleId]
                        left outer join [ModuleFuntions] AS mf ON la.[FunctionId] = mf.[Id]
                        left outer join [dbo].[MembersView] mem on cr.MemberId = mem.Id
                        left outer join [dbo].[ActRollCallLogs] l on l.RollCallId=s.Id and l.StudId=mem.Id
                      --left outer join dbo.AttendanceLeaves al on s.LearningId = al.LearningId and al.studid=l.studid and a.ActivityDate >=al.LeaveDate and a.ActivityDate < dateadd(d,1,al.LeaveDate) and al.status !=40
                        left outer join [dbo].[AttendanceLeaves] al on s.LearningId = al.LearningId and al.studid=l.studid and Convert(varchar(10),a.ActivityDate,111) =Convert(varchar(10),al.LeaveDate,111) and al.status !=40
                        left outer join [dbo].[Members] c on l.CreateUser=c.Id
                        where mf.OutSideKey=@moduleFun
                        and cr.Enable=1 and la.Enable=1 and mf.Enable=1 and a.CardisShow=1");
            #endregion

            #region // 參數準備

            List<object> sqlParam = new List<object>();

            // 模組動作代碼
            sqlParam.Add(new SqlParameter("@moduleFun", QueryCondition.SignInFunction.Member));

            #region //查詢特定點名活動
            // 單筆
            if (param.EventIds.Count() == 1)
            {
                sql.Append(" and a.OuterKey=@eventId");
                sqlParam.Add(new SqlParameter("@eventId", param.EventIds[0]));
            }
            // 多筆
            else if (param.EventIds.Count() > 1)
            {
                sql.Append(" and a.OuterKey in (");
                for (int i = 0; i < param.EventIds.Count; i++)
                {
                    string pKey = "@p" + i.ToString();
                    sql.Append(pKey);
                    sql.Append(",");
                    sqlParam.Add(new SqlParameter(pKey, param.EventIds[i]));
                }
                if (sql.ToString().EndsWith(","))
                    sql.Remove(sql.Length - 1, 1);
                sql.Append(")");
            }
            #endregion

            #region //查詢特定學習圈內的點名活動
            if (!string.IsNullOrEmpty(param.CircleKey))
            {
                sql.Append(" and a.ToRoomId=@circleKey");
                sqlParam.Add(new SqlParameter("@circleKey", param.CircleKey));
            }
            #endregion

            #region //是否有權限看所有人的紀錄(只能看自己的記錄)
            if (!param.IsAdminRole)
            {
                sql.Append(" and mem.Id=@memberId");
                sqlParam.Add(new SqlParameter("@memberId", param.MemberId));
            }
            #endregion

            #region //查詢結果是否包含已刪除的點名活動
            if (param.NotDeleted)
            {
                sql.Append(" and a.deleted_Utc is null");
            }
            #endregion

            #endregion

            sql.Append(" ) m");
            sql.Append(" left outer join [ICAN5].[dbo].[ManScore] ms on m.ToRoomId=ms.course_no and m.StudId=ms.man_no");
            sql.Append(" where ms.manscore_mark='10'");

            var list = _context.Database.SqlQuery<SignInData>(sql.ToString(), sqlParam.ToArray()).ToList();

            return list;
        }

        /// <summary>
        /// 取得學習圈成員 - 依據學習圈代碼與是否為老師
        /// </summary>
        /// <param name="circleKey">學習圈代碼</param>
        /// <param name="isTeacher">是否為老師</param>
        /// <returns></returns>
        public IEnumerable<Member> GetLearningCircleMemberRoleList(string circleKey, bool isTeacher)
        {
            var db = _context;
            var list = from cmr in db.CircleMemberRoleplay
                       join m in db.Members on cmr.MemberId equals m.Id
                       join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                       join lr in db.LearningRole on cmr.RoleId equals lr.Id
                       where lc.LearningOuterKey == circleKey && lr.IsAdminRole == isTeacher
                       select m;
            return list;
        }
    }
}
