using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityRepository;
using WiicoApi.Infrastructure.ValueObject;

namespace WiicoApi.Repository.EntityRepositorys
{
    public class UserTokensRepo : GenericEntityRepository<UserToken,WiicoDB>
    {
        public UserTokensRepo(WiicoDB _context) : base(_context)
        {

        }

        /// <summary>
        /// 新增登入紀錄
        /// </summary>
        /// <param name="requestSystem"></param>
        /// <param name="phoneId"></param>
        /// <param name="loginMemberId"></param>
        /// <param name="simulatorId"></param>
        /// <returns></returns>
        public UserToken InsertUserTokenByOrganization(string requestSystem, string phoneId, Member loginMemberInfo, string pushToken,string orgToken, int? simulatorId = null)
        {
            var checkDevice = requestSystem.Split(';');
            var requestStartText = checkDevice[0].ToLower();
            //假設是手機端token
            if (!requestStartText.ToLower().StartsWith("desktop")) {
                var tokens = _context.UserToken.Where(t => t.MemberId == loginMemberInfo.Id && 
                                                                                                     t.OrgId == loginMemberInfo.OrgId && (
                                                                                                        t.RequestSystem.ToLower().StartsWith("ios") || 
                                                                                                        t.RequestSystem.ToLower().StartsWith("android")
                                                                                                      )
                                                                                           ).ToList();
                if (tokens.FirstOrDefault() != null)
                    _context.UserToken.RemoveRange(tokens);
            }
        
            var entity = new UserToken()
            {
                Created = Infrastructure.Property.TimeData.Create(DateTime.UtcNow),
                Deleted = Infrastructure.Property.TimeData.Create(null),
                Updated = Infrastructure.Property.TimeData.Create(null),
                CreateUser = loginMemberInfo.Id,
                DeviceKey = phoneId,
                Enable = true,
                MemberId = loginMemberInfo.Id,
                OrgId = loginMemberInfo.OrgId,
                PushToken = pushToken,
                RequestSystem = requestSystem,
                Token = orgToken,
                TokenMark = true,
                SimulationMember = null
            };
            if (simulatorId.HasValue)
                entity.SimulationMember = simulatorId.Value;

            _context.UserToken.Add(entity);
            _context.SaveChanges();
            
            return entity;
        }

        /// <summary>
        /// 新增登入紀錄
        /// </summary>
        /// <param name="requestSystem"></param>
        /// <param name="phoneId"></param>
        /// <param name="loginMemberId"></param>
        /// <param name="simulatorId"></param>
        /// <returns></returns>
        public UserToken InsertUserToken(string requestSystem, string phoneId, Member loginMemberInfo, string pushToken, int? simulatorId = null)
        {
            var token = Guid.NewGuid().ToString();
            var entity = new UserToken()
            {
                Created = Infrastructure.Property.TimeData.Create(DateTime.UtcNow),
                Deleted = Infrastructure.Property.TimeData.Create(null),
                Updated = Infrastructure.Property.TimeData.Create(null),
                CreateUser = loginMemberInfo.Id,
                DeviceKey = phoneId,
                Enable = true,
                MemberId = loginMemberInfo.Id,
                OrgId = loginMemberInfo.OrgId,
                PushToken = pushToken,
                RequestSystem = requestSystem,
                Token = token.ToString(),
                TokenMark = true,
                SimulationMember = simulatorId,
                IsOrgAdmin = false
            };

            _context.UserToken.Add(entity);
            _context.SaveChanges();

            //新增ican5 ManToken
            /*   #region 未來如果沒接iCan5會拔掉
               string sql = @" DECLARE @check bit = 0
                                            Begin Transaction addLoginLog 
                                               insert into [ICAN5].[dbo].[ManToken](man_no,device,token) values(@man_no,@device,@token)
                                               IF @@Error <> 0 BEGIN SET @check = 1 END 
                                               IF @check <> 0 BEGIN
                                                   Rollback Transaction addLoginLog -- 復原
                                               END
                                               ELSE BEGIN
                                                   Commit Transaction addLoginLog -- 寫入變更
                                               END";

               var data = _context.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@man_no", loginMemberInfo.Account),
                    new SqlParameter("@token", token),
                    new SqlParameter("@device", phoneId));
               #endregion*/
            return entity;
        }

        public UserToken GetiCan5Token(string token)
        {
            string sql = @" select man_no from [ican5].[dbo].[mantoken] where token=@token ";
            var tokenAccount = _context.Database.SqlQuery<string>(sql, new SqlParameter("@token", token)).FirstOrDefault();
            if (tokenAccount != null || tokenAccount != string.Empty) {
                var accountData = _context.Members.FirstOrDefault(t => t.Account == tokenAccount);
                if (accountData == null)
                    return null;
                var responseData = new UserToken()
                {
                    Created = TimeData.Create(DateTime.UtcNow),
                    CreateUser = accountData.Id,
                    Updated = TimeData.Create(null),
                    Deleted = TimeData.Create(null),
                    Enable = true,
                    Token = token,
                    MemberId = accountData.Id
                };
                _context.UserToken.Add(responseData);
                _context.SaveChanges();
                return responseData;
            }
            return null;
        }
    }
}
