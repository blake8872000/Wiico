using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.Entity;
using EntityRepository;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class ActivitysNoticesRepo : GenericEntityRepository <ActivitysNotices, WiicoDB>
    {
        public ActivitysNoticesRepo(WiicoDB context) : base(context)
        {
        }

        public IEnumerable<ActivitysNoticeData> GetActivitysNoticeDatas(int memberId, int maxResult, int? ActivityNoticeId = 0)
        {
            /* 傳入的SQL參數
 declare @maxResult int =20
 declare @memberId int =941
 declare @noticeKey nvarchar(100) ='myNotice'
 declare @signIn nvarchar(100) ='signIn'
 declare @homework nvarchar(100) ='upload'
  */
            #region //SQL

            //沒有加入outerKey條件
            string sql = @"declare @lastRead int, @count int =0
                        --1.未讀燈號
                        select @lastRead=[LastReadActivityIdEnd]
                        from [dbo].[ActivitysReadMarks]
                        where [ToRoomId]=@noticeKey and [memberId]=@memberId
                        IF(@lastRead>0)
	                        BEGIN
		                        --1.1 有未讀紀錄-計算最後一筆未讀到最後一筆通知的數量
		                        select @count=count(*) from [dbo].[ActivitysNotices] a
		                        where [memberId]=@memberId and id>@lastRead
	                        END
                        ELSE
	                        BEGIN
		                        --1.2 沒有未讀紀錄-計算全部通知數量
		                        select @count=count(*) from [dbo].[ActivitysNotices] a
		                        where [memberId]=@memberId
	                        END

                        --2.回傳通知列表
                        select top(@maxResult) UnReadCount=@count, n.Id, n.ToRoomId, c.Id as CircleId, c.Name as CircleName
		                        , n.EventId, n.NoticeContent, n.HasClick, n.CreateTime
                                , a.ModuleKey
		                        , Title=case when a.ModuleKey=@signIn then (select '點名活動('+convert(varchar, a.Created_Utc,111)+' ' +convert(varchar(5), CONVERT(time, SWITCHOFFSET(CONVERT(datetimeoffset, a.Created_Utc), DATENAME(TzOffset, SYSDATETIMEOFFSET()))) ,114)+')') 
					                         when a.ModuleKey=@homework then (select Name from [dbo].[ActHomeWorks] where EventId=a.OuterKey) end
		                        , IsDelete=convert(bit,case when a.Deleted_Utc is null then 0 else 1 end)
                        from [dbo].[ActivitysNotices] n
                        join [dbo].[LearningCircles] c on n.ToRoomId=c.LearningOuterKey
                        join [dbo].[Activitys] a on n.EventId=a.OuterKey
                        left outer join [dbo].[Members] m on n.memberId=m.Id
                        where n.memberId=@memberId
                        order by CreateTime desc
						--OFFSET     @skipCount ROWS       -- skip 20 rows
						--FETCH NEXT @maxResult ROWS ONLY; -- take 20 rows
";

            //加入ActivityNoticesId的條件
            string outerKeySql = @"declare @lastRead int, @count int =0
                        --1.未讀燈號
                        select @lastRead=[LastReadActivityIdEnd]
                        from [dbo].[ActivitysReadMarks]
                        where [ToRoomId]=@noticeKey and [memberId]=@memberId
                        IF(@lastRead>0)
	                        BEGIN
		                        --1.1 有未讀紀錄-計算最後一筆未讀到最後一筆通知的數量
		                        select @count=count(*) from [dbo].[ActivitysNotices] a
		                        where [memberId]=@memberId and id>@lastRead
	                        END
                        ELSE
	                        BEGIN
		                        --1.2 沒有未讀紀錄-計算全部通知數量
		                        select @count=count(*) from [dbo].[ActivitysNotices] a
		                        where [memberId]=@memberId
	                        END

                        --2.回傳通知列表
                        select top(@maxResult) UnReadCount=@count, n.Id, n.ToRoomId, c.Id as CircleId, c.Name as CircleName
		                        , n.EventId, n.NoticeContent, n.HasClick, n.CreateTime
                                , a.ModuleKey
		                        , Title=case when a.ModuleKey=@signIn then (select '點名活動('+convert(varchar, a.Created_Utc,111)+' ' +convert(varchar(5), CONVERT(time, SWITCHOFFSET(CONVERT(datetimeoffset, a.Created_Utc), DATENAME(TzOffset, SYSDATETIMEOFFSET()))) ,114)+')') 
					                         when a.ModuleKey=@homework then (select Name from [dbo].[ActHomeWorks] where EventId=a.OuterKey) end
		                        , IsDelete=convert(bit,case when a.Deleted_Utc is null then 0 else 1 end)
                        from [dbo].[ActivitysNotices] n
                        join [dbo].[LearningCircles] c on n.ToRoomId=c.LearningOuterKey
                        join [dbo].[Activitys] a on n.EventId=a.OuterKey
                        left outer join [dbo].[Members] m on n.memberId=m.Id
                        where n.memberId=@memberId and n.Id < @anId
                        order by CreateTime desc
						--OFFSET     @skipCount ROWS       -- skip 20 rows
						--FETCH NEXT @maxResult ROWS ONLY; -- take 20 rows
";
            #endregion

            var data = new List<ActivitysNoticeData>();
            if (ActivityNoticeId != 0)
            {
                data = _context.Database.SqlQuery<ActivitysNoticeData>(outerKeySql,
                        new SqlParameter("@maxResult", maxResult),
                        new SqlParameter("@anId", ActivityNoticeId.Value),
                        new SqlParameter("@memberId", memberId),
                        new SqlParameter("@noticeKey", QueryCondition.noticeKey),
                        new SqlParameter("@signIn", QueryCondition.ModuleType.SignIn),
                        new SqlParameter("@discussion", QueryCondition.ModuleType.Discussion),
                        new SqlParameter("@homework", QueryCondition.ModuleType.Homework)).ToList();
            }
            else
            {

                data = _context.Database.SqlQuery<ActivitysNoticeData>(sql,
                          new SqlParameter("@maxResult", maxResult),
                          //  new SqlParameter("@skipCount", (pages.Value - 1) * maxResult),
                          new SqlParameter("@memberId", memberId),
                          new SqlParameter("@noticeKey", QueryCondition.noticeKey),
                          new SqlParameter("@signIn", QueryCondition.ModuleType.SignIn),
                          new SqlParameter("@discussion", QueryCondition.ModuleType.Discussion),
                          new SqlParameter("@homework", QueryCondition.ModuleType.Homework)).ToList();
            }

            return data;
        }
    }
}
