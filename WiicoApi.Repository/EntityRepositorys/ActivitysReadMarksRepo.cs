using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using WiicoApi.Infrastructure.Entity;
using EntityRepository;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class ActivitysReadMarksRepo : GenericEntityRepository<ActivitysReadMark, WiicoDB>
    {
        public ActivitysReadMarksRepo(WiicoDB context) : base(context)
        {
        }

        public int SaveActivitysReadMarks(int memberId)
        {
            /* 傳入的SQL參數
declare @memberId int =4
declare @notice nvarchar(100)='myNotice'
declare @dt datetime=getdate()
 */
            #region //SQL
            string sql = @"declare @Bg int=0, @End int=0
                        select @End=max(id),@Bg=min(id) from [dbo].[ActivitysNotices] where [MemberId]=@memberId

                        IF(@End>0)
                        BEGIN
	                        UPDATE [dbo].[ActivitysReadMarks] SET [LastReadActivityIdEnd]=@End, [Time]=@now WHERE [ToRoomId]=@noticeKey and [memberId]=@memberId	
	                        IF @@ROWCOUNT = 0
	                        BEGIN
		                        INSERT [dbo].[ActivitysReadMarks]([ToRoomId],[memberId],[LastReadActivityIdBegin],[LastReadActivityIdEnd],[Time])
		                        VALUES(@noticeKey,@memberId,@bg,@end,@now)
	                        END
                        END";
            #endregion

            int result = _context.Database.ExecuteSqlCommand(sql,
                             new SqlParameter("@memberId", memberId),
                             new SqlParameter("@noticeKey", QueryCondition.noticeKey),
                             new SqlParameter("@now", DateTime.UtcNow));

            return result;
        }
    }
}
