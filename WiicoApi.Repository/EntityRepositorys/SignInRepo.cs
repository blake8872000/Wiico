using EntityRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.ValueObject;
using WiicoApi.Infrastructure.ViewModel;
using static WiicoApi.Repository.QueryCondition;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class SignInRepo : GenericEntityRepository<ActRollCall, WiicoDB>
    {

        public SignInRepo(WiicoDB _context) : base(_context)    {        }

        /// <summary>
        /// 發起一個點名活動
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="circleId"></param>
        /// <param name="memberId"></param>
        /// <param name="beaconKey"></param>
        /// <param name="duration"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActivitysViewModel InsertSignIn(string circleKey, int circleId, int memberId, string beaconKey, int duration, string name)
        {
            var dt = DateTime.UtcNow;
            var eveId = Guid.NewGuid();
            string newPwd = GetRandomPwd(RandomPwd.Number, 4);

            if (string.IsNullOrEmpty(beaconKey))
                beaconKey = GetRandomKey();

            #region //1. SQL - 新增Activity & 點名活動 
            string sql = @"	/* 1.新增活動*/
	                        insert into [dbo].[Activitys]([ToRoomId],[ModuleKey],[OuterKey],[CreateUser],[Created_Utc],[Duration],[IsActivity],[ActivityDate],[CardisShow],[Publish_Utc])
	                        values(@circleKey,@moduleKey,@eveId,@creater,@now,@duration,1,@now,1,@now)
                            
	                        /* 2.新增點名*/
	                        insert into [dbo].[ActRollCalls]([LearningId],[Name],[CreateUser],[Created_Utc],[EventId],[SignInKey],[SignInPwd],[Visibility])
	                        values(@learningId,@name,@creater,@now,@eveId,@signInKey,@signInPwd,1);

                            DECLARE @rollCallId int=(Select SCOPE_IDENTITY());

	                        /* 3.準備回傳資料*/
	                        select RollCallId=@rollCallId,
                                   CreatorAccount=Account,
	                               CreatorName=Name,
                                   CreatorPhoto=Photo,
	                               ModuleKey=@moduleKey,
                                   ToRoomId=@circleKey,
                                   OuterKey=@eveId,
	                               Created_Utc=@now,
                                   ReadMark=CAST(0 AS BIT)
                            from [dbo].[Members]
                            where Id=@creater";
            #endregion

            var data = _context.Database.SqlQuery<ActivitysViewModel>(sql,
                 new SqlParameter("@circleKey", circleKey),
                            new SqlParameter("@learningId", circleId),
                            new SqlParameter("@moduleKey", ModuleType.SignIn),
                            new SqlParameter("@name", name),
                            new SqlParameter("@duration", duration),
                            new SqlParameter("@creater", memberId),
                            new SqlParameter("@eveId", eveId),
                            new SqlParameter("@signInKey", beaconKey),
                            new SqlParameter("@signInPwd", newPwd),
                            new SqlParameter("@moduleFun", SignInFunction.Member),
                            new SqlParameter("@state", AttendanceState.Absence),
                            new SqlParameter("@now", dt)).FirstOrDefault();
            data.Publish_Utc = dt;
            _context.SaveChanges();
           return data;
        }
        private string GetRandomKey()
        {

            var random = new Random();
            int first = random.Next(ushort.MaxValue);
            int secod = random.Next(ushort.MaxValue);
            var signinKey = ConfigurationManager.AppSettings["signInKeyGuid"].ToString().ToLower();
            return string.Format("{0}_{1}_{2}", signinKey, first.ToString(), secod.ToString());
        }
        private string GetRandomPwd(RandomPwd type, int length)
        {
            Random random = new Random();
            string chars = "";

            switch (type)
            {
                case RandomPwd.Number:
                    chars = "0123456789";
                    break;
                case RandomPwd.Uppercase:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case RandomPwd.Lowercase:
                    chars = "abcdefghijklmnopqrstuvwxyz";
                    break;
            }

            var data = Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray();

            var rtn = new string(data);

            return rtn;
        }
    }
}
