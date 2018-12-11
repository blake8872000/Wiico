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
    public class ActivitysRepo : GenericEntityRepository<Activitys,WiicoDB>
    {
        public ActivitysRepo(WiicoDB _context) : base(_context)
        {

        }
        public Activitys ActivityCreate(Activitys entity) {
            Insert(entity);
            _context.SaveChanges();
            return entity;
        }


        /// <summary>
        /// 取得該使用者每個課程最後一筆活動資訊
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="circlekey"></param>
        /// <returns></returns>
        public IEnumerable<ActivitysLatest> GetActivitysLatestByLinQ(int memberId,string circlekey) {
            var response = new List<ActivitysLatest>();
            //取得修課紀錄
            var learningCircleSqlResult =from lc in _context.LearningCircle
                                     join cm in _context.CircleMemberRoleplay on lc.Id equals cm.CircleId
                                     where cm.MemberId == memberId && lc.Enable==true
                                     group lc by new { lc } into g
                                         select g.Key.lc;

            if (learningCircleSqlResult.FirstOrDefault() == null)
                return null;

            var learningcircleList = new List<LearningCircle>();
        //    if (circlekey!=null && circlekey!=string.Empty)
          //      learningcircleList = learningCircleSqlResult.Where(t=>t.LearningOuterKey== circlekey).ToList();
          //  else
                learningcircleList = learningCircleSqlResult.ToList();

            foreach (var learningCircleInfo in learningcircleList)
            {
                //最後活動資訊
                var latestActivitySqlData = _context.Activitys.Where(t => t.ToRoomId.ToLower() == learningCircleInfo.LearningOuterKey.ToLower() && t.Publish_Utc.HasValue && t.IsActivity==true && t.CardisShow==true).OrderByDescending(t => t.Publish_Utc).FirstOrDefault();
                if (latestActivitySqlData != null)
                {
                    //使用者已讀資訊
                    var memberReadInfo = _context.ActivitysReadMark.FirstOrDefault(t => t.memberId == memberId && t.ToRoomId.ToLower() == learningCircleInfo.LearningOuterKey.ToLower());
          
                    if (memberReadInfo == null)
                    {
                       var firstActivityData = _context.Activitys.Where(t => t.ToRoomId == learningCircleInfo.LearningOuterKey && t.Publish_Utc.HasValue && t.IsActivity == true && t.CardisShow == true).OrderBy(t => t.Publish_Utc).FirstOrDefault();
                        var actReadMarkEntity = new Infrastructure.Entity.ActivitysReadMark();
                        //如果沒有的話幫他建立一筆已讀資訊
                        if (firstActivityData != null)
                        {
                            actReadMarkEntity.Enabled = true;
                            actReadMarkEntity.LastReadActivityIdBegin =1;
                                actReadMarkEntity.LastReadActivityIdEnd = firstActivityData.Id;
                                actReadMarkEntity.memberId = memberId;
                               actReadMarkEntity.ToRoomId = learningCircleInfo.LearningOuterKey.ToLower();
                                actReadMarkEntity.Time = DateTime.UtcNow;
                            _context.ActivitysReadMark.Add(actReadMarkEntity);
                        }
                        else {
                            actReadMarkEntity.Enabled = true;
                            actReadMarkEntity.LastReadActivityIdBegin = 1;
                            actReadMarkEntity.LastReadActivityIdEnd = 1;
                            actReadMarkEntity.memberId = memberId;
                            actReadMarkEntity.ToRoomId = learningCircleInfo.LearningOuterKey.ToLower();
                            actReadMarkEntity.Time = DateTime.UtcNow;
                            _context.ActivitysReadMark.Add(actReadMarkEntity);
                        }
                        _context.SaveChanges();
                        memberReadInfo = actReadMarkEntity;
                    }
                    //使用者最後已讀活動資訊
                    var memberLastestReadActivity = _context.Activitys.FirstOrDefault(t => t.Id == memberReadInfo.LastReadActivityIdEnd);
                    if (memberLastestReadActivity == null)
                        continue;
                    //使用者未讀活動列表
                    var memberUnReadSql = (from a in _context.Activitys
                                           join rm in _context.ActivitysReadMark on a.ToRoomId equals rm.ToRoomId
                                           where rm.ToRoomId == learningCircleInfo.LearningOuterKey && rm.memberId == memberId && a.Id > memberLastestReadActivity.Id  && a.IsActivity == true && a.CardisShow == true
                                           select a).ToList();
                   
                    var unReadActivityCount = memberUnReadSql.Count();
                    var unReadActivityInfo = memberUnReadSql.OrderByDescending(t => t.Publish_Utc).FirstOrDefault();
                    
                    if (unReadActivityInfo !=null && unReadActivityInfo.Id == memberLastestReadActivity.Id)
                        unReadActivityCount = unReadActivityCount - 1;
                    var memberCount = _context.CircleMember.Where(t => t.CircleId == learningCircleInfo.Id).Count();
                    //活動建立者資訊
                    var memberInfo = _context.Members.FirstOrDefault(t => t.Id == latestActivitySqlData.CreateUser.Value);
                    //如果沒有未讀資料又是刪除狀態，就不顯示
                    if (unReadActivityCount <= 0 && memberReadInfo.Enabled == false)
                        continue;
                    else if(unReadActivityCount > 0 && memberReadInfo.Enabled == false)
                    {
                        memberReadInfo.Enabled = true;
                        _context.SaveChanges();
                    }
             

                    var responseData = new Infrastructure.ValueObject.ActivitysLatest()
                    {
                        ActType = latestActivitySqlData.ModuleKey,
                        CircleId = learningCircleInfo.Id,
                        CircleKey = learningCircleInfo.LearningOuterKey.ToLower(),
                        CircleName = learningCircleInfo.Name,
                        Publish_Utc = latestActivitySqlData.Publish_Utc.Value,
                        MemberName = memberInfo.Name,
                        MemberCount = memberCount,
                        Count = unReadActivityCount,
                        EventId = latestActivitySqlData.OuterKey
                    };
                    response.Add(responseData);
                }
                else
                    continue;
            }
            
            return response;
        }
        public IEnumerable<ActivitysLatest> GetActivitysLatest(int memberId, string circleKey)
        {

            #region //SQL
            string sql = @"declare @checkActivityId int, @lastRead int, @key uniqueidentifier
                        declare @circleId int, @circleKey nvarchar(50), @circleName nvarchar(50), @count int, @creatorId int, @name nvarchar(50), @actType nvarchar(100), @_context nvarchar(max), @date datetime , @memberCount int
                        declare @result Table (
	                        [CircleId] int,
	                        [CircleKey] nvarchar(50), 
	                        [CircleName] nvarchar(50), 
	                        [Count] int, 
	                        [MemberName] nvarchar(50),
	                        [ActType] nvarchar(100),
	                        [Content] nvarchar(max),
	                       -- [Created_Utc] datetime,
                            [Publish_Utc] datetime,
                            [MemberCount] int);

                        --取出我所有的學習圈+學習圈人數(TODO)
                        DECLARE circleKeys CURSOR FOR
                        select lc.LearningOuterKey, lc.Name, lc.Id
                        from [dbo].[CircleMembers] cm
                        left outer join [dbo].[LearningCircles] lc on cm.CircleId=lc.Id
                        where cm.MemberId=@memberId

                        OPEN circleKeys
                        FETCH NEXT FROM circleKeys INTO @circleKey, @circleName, @circleId
                        WHILE @@FETCH_STATUS = 0
                        BEGIN

	                        --清空判斷參數
	                        set @key=null;
	                        set @_context='';

                            select @memberCount = count(*)
							from dbo.LearningCircles lc
							inner join dbo.circlemembers cm on lc.id = cm.circleId
							where lc.LearningOuterKey = @circleKey

	                        --2.取得學習圈內最後一筆活動
	                        select top 1 @actType=[ModuleKey],@checkActivityId = a.Id, @date=a.[Publish_Utc], @key=[OuterKey], @creatorId=a.CreateUser, @name=m.Name 
	                        from [dbo].[Activitys] a
	                        left outer join [dbo].[Members] m on a.CreateUser = m.Id
	                        where [ToRoomId]=@circleKey and IsActivity=1 and a.CardisShow=1
	                        order by IsNull(a.Publish_Utc,a.created_utc) desc
					
	                        --學習圈有活動紀錄才繼續往下做，否則就換下一筆
	                        IF(@key is not null)
	                        BEGIN
		                        --1.取得活動細節
                                IF(@actType=@messageKey)
			                        BEGIN
				                        select @_context=[Content] from [dbo].[ActMessages] where [EventId]=@key
                                        select @name='' where @creatorId=@memberId /*2017-01-09 add by sophiee 如果是自己發話，不顯示姓名*/
			                        END
                                IF(@actType=@discussion)
                                    BEGIN
                                         select @_context=[Name] from [dbo].[ActDiscussions] where [EventId]=@key
                                    END
                                IF(@actType='group')
                                    BEGIN
                                         select @_context=[Name] from [dbo].[ActGroupCategories] where [EventId]=@key
                                    END
                                IF(@actType='general')
                                    BEGIN
                                         select @_context=[ActType]+'活動：「'+[Title]+'」' from [dbo].[ActGenerals] where [EventId]=@key
                                    END



		                        --2.取得未讀數量
		                        set @count=0
		                        --如果不是指定排除的學習圈，進行未讀活動計算
		                        IF(@notCircleKey='' or (@notCircleKey != @circleKey))
		                        BEGIN
			                        select @lastRead=[LastReadActivityIdEnd]
			                        from [dbo].[ActivitysReadMarks]
			                        where [ToRoomId]=@circleKey and [memberId]=@memberId

			                        IF(@lastRead>0)
				                        BEGIN
					                        --2.1 有未讀紀錄-計算最後一筆未讀到最後一筆活動的數量
					                        select @count=count(*) from [dbo].[Activitys] a
					                        where ToRoomId=@circleKey and id>@lastRead  and IsActivity=1 and a.CardisShow=1
				                        END
			                        ELSE
				                        BEGIN
					                        --2.2 沒有未讀紀錄-全部活動都當作未讀
					                        select @count=count(*) from [dbo].[Activitys] a
					                        where ToRoomId=@circleKey and IsActivity=1 and a.CardisShow=1
				                        END
		                        END
								--判断是否需要show
								if(exists(select * from [dbo].[ActivitysReadMarks] where [ToRoomId]=@circleKey and [memberId]=@memberId and (LastReadActivityIdEnd< @checkActivityId or enabled =1)))
								begin
									update [dbo].[ActivitysReadMarks] set enabled=1 , UpdateDate_Utc = GETUTCDATE() where [ToRoomId]=@circleKey and [memberId]=@memberId
									--將最後一筆活動資訊寫入回傳物件
									insert into @result	values(@circleId, @circleKey, @circleName, @count, @name, @actType, @_context, @date, @memberCount)
								end
	                        END
	                        --換查看下一筆學習圈
	                        FETCH NEXT FROM circleKeys INTO @circleKey, @circleName, @circleId
                        END

                        CLOSE circleKeys
                        DEALLOCATE circleKeys --將cursor物件從記憶體移除

                        select * from @result
                        order by Publish_Utc desc";
            #endregion

            var data = _context.Database.SqlQuery<ActivitysLatest>(sql,
                             new SqlParameter("@memberId", memberId),
                             new SqlParameter("@notCircleKey", circleKey),
                             new SqlParameter("@messageKey", QueryCondition.ModuleType.Message),
                             new SqlParameter("@discussion", QueryCondition.ModuleType.Discussion)).ToList();
            return data;
        }

        public IEnumerable<Infrastructure.ViewModel.ActivitysViewModel> GetQueryDateList(string circleKey, int memberId, int maxResult, Guid pageToken, DateTime? queryDate = null)
        {
            // 最後select的欄位，必須與方法GetListByDirection一致，若有增減請一併調整

            /* 傳入的SQL參數
                declare @groupId nvarchar(max) ='TestClass'
                declare @userId int = 6
                declare @maxResult int =20
                declare @pageToken uniqueidentifier='00000000-0000-0000-0000-000000000000'
             */
            #region // 產生SQL
            string sql = @"declare @mt int = (@maxResult+1)/2
                        declare @result Table (
	                        [Id] int identity(1,1), 
	                        [RowNum] bigint, 
	                        [ActivityId] int, 
	                        [RowNumTemp] int);
	
	                        -- 預塞
	                        if (not exists(select 0 from [dbo].[ActivitysReadMarks] where [memberId] = @userId and [ToRoomId] = @groupId))
	                        begin
		                        insert into [dbo].[ActivitysReadMarks]
		                        ([memberId], [ToRoomId], [LastReadActivityIdBegin], [LastReadActivityIdEnd])
		                        values
		                        (@userId,@groupId,0,0)
	                        end

                        -- 1. 依照id取前後特定筆數資料
	                        declare @middleId int = 0;
	                        declare @posId int = 0;
	                        declare @lastReadBegin int = 0;
	                        declare @lastReadEnd int = 0;

	                        select @lastReadBegin = [LastReadActivityIdBegin], @lastReadEnd = [LastReadActivityIdEnd]
	                        from [dbo].[ActivitysReadMarks]
	                        where [memberId] = @userId and [ToRoomId] = @groupId;

                        -- 1-1. 如果有傳guid，以此guid查id
	                        if (@pageToken != '00000000-0000-0000-0000-000000000000')
	                        begin
		                        select @middleId = [Id] from [dbo].[Activitys] where [OuterKey] = @pageToken and IsActivity=1;
		                        set @posId = @middleId;
	                        end
                        -- 1-2. 如果未傳guid，自動查讀取紀錄拿最後已讀的項目id
	                        else
	                        begin
		                        if (@lastReadEnd > 0)
		                        begin -- 有已讀
			                        set @middleId = @lastReadEnd
			                        set @posId = @lastReadEnd;

			                        -- 萬一全部讀完或者讀到剩一兩筆，將指標往上調
			                        declare @temp Table ([r] int, [Id] int)
			                        insert into @temp
			                        select * from (
				                        select ROW_NUMBER() over (order by [Id] asc) as [r] ,[Id]
				                        from [dbo].[Activitys] where [ToRoomId] = @GroupId and IsActivity=1
										                             and isnull(Publish_utc,Created_Utc) >=@queryDate
			                        ) x where x.[r] > @mt -- @maxResult-1
			
			                        if ( exists( select 0 from @temp where [Id] = @posId))
			                        select @middleId = min([Id]) from @temp
		                        end
		                        else
		                        begin -- 沒已讀
			                        select @middleId = min([Id]) from [dbo].[Activitys] where [ToRoomId] = @GroupId and IsActivity=1 and Created_Utc >= @queryDate
			                        set @posId = @middleId;
		                        end
	                        end

                        ;with [TempResult] 
                        as (
	                        select * from (
		                        select 0 as [RowNum], 1 as [PrevMark], ROW_NUMBER() over (order by [Id] desc) as [RowNumTemp],
		                        r.Id
		                        from [dbo].[Activitys] r
		                        where r.[ToRoomId] = @GroupId and r.[Id] <= @middleId and IsActivity=1 
	                        ) X where [RowNumTemp] between @mt+1 and @mt+1

	                        union
	                        select ROW_NUMBER() over (order by [PrevMark] desc, [Id]) as [RowNum],* from (
		                        select
		                        1 as [PrevMark],
		                        ROW_NUMBER() over (order by r.[Created_Utc] desc) as [RowNumTemp], 
		                        r.[Id]
		                        from [dbo].[Activitys] r
		                        where r.[ToRoomId] = @GroupId and IsActivity=1
		                        and r.[Id] <= @middleId
								and r.Created_Utc >= @queryDate
		                        union

		                        select 
		                        0 as [PrevMark],
		                        ROW_NUMBER() over (order by r.[Created_Utc]) as [RowNumTemp], 
		                        r.[Id]
		                        from [dbo].[Activitys] r
		                        where r.[ToRoomId] = @GroupId and IsActivity=1
		                        and r.[Id] > @middleId
								and r.Created_Utc >= @queryDate
	                        ) x
	                        where [PrevMark] = 1 and RowNumTemp <= @mt or [PrevMark] = 0 and  RowNumTemp <= @maxResult
                        )

                        -- 為了做更多處理，這裡再將結果暫存(with語句做出來的[TempResult]只能用一次)
                        insert into @result ([RowNum],[ActivityId],[RowNumTemp])
                        select [RowNum],[Id],[RowNumTemp] as [ActivityId] from [TempResult];

                        -- ***更新已讀

                        declare @readBegin int = 0, @readEnd int = 0
                        select @readBegin = min([ActivityId]), @readEnd = max([ActivityId]) from @result where [RowNum] between 1 and @maxResult

                        IF(@readBegin is not NULL and @readEnd is not NULL)
                        BEGIN
	                        update [dbo].[ActivitysReadMarks]
	                        set [LastReadActivityIdBegin] = case when ISNULL([LastReadActivityIdBegin],0) = 0 then @readBegin 
										                         when ISNULL([LastReadActivityIdBegin],0) < @readBegin then [LastReadActivityIdBegin] else @readBegin end
	                        , [LastReadActivityIdEnd] = case when ISNULL([LastReadActivityIdEnd],0) = 0 then @readEnd
									                         when ISNULL([LastReadActivityIdEnd],0) > @readEnd then [LastReadActivityIdEnd] else @readEnd end
	                        , [Time] = getUtcDate()
	                        where [memberId] = @userId and [ToRoomId] = @groupId
                        END

                        -- ***輸出結果
                        select
                        tr.RowNum,
                        (select count(*) from [dbo].[ActivitysReadMarks] where r.[Id] between [LastReadActivityIdBegin] and [LastReadActivityIdEnd]) as ReadCount,
                        Convert(bit, case when tr.[ActivityId] = @posId then 1 else 0 end) as [PositionMark], 
                        Convert(bit, case when tr.[ActivityId] between @lastReadBegin and @lastReadEnd then 1 else 0 end) as [ReadMark], 
                        tr.RowNumTemp,
                        m.Account as CreatorAccount,
                        m.Name as CreatorName,
                        m.Photo as CreatorPhoto,
                        [Text]=ISNULL(msg.Content,''),
                        r.*
                        from @result tr
                        inner join [dbo].[Activitys] r on r.Id = tr.[ActivityId]
                        left outer join [dbo].[ActMessages] msg on r.OuterKey=msg.EventId and msg.[Type]='text'
                        left outer join [dbo].[MembersView] m on m.Id = r.CreateUser
                        where [RowNum] between 0 and @maxResult-1 and IsActivity=1
";
            #endregion

            var data = _context.Database.SqlQuery<Infrastructure.ViewModel.ActivitysViewModel>(sql,
                 new SqlParameter("@groupId", circleKey),
                 new SqlParameter("@userId", memberId),
                 new SqlParameter("@maxResult", maxResult),
                 new SqlParameter("@pageToken", pageToken),
                 new SqlParameter("@queryDate", queryDate)).ToList();

            return data;
        }


        /// <summary>
        /// 取得活動列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="maxResult"></param>
        /// <param name="pageToken"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.ActivitysViewModel> GetInitActivityListByLinQ(string circleKey, int memberId, int maxResult, Guid pageToken)
        {
            var activitys = from a in _context.Activitys
                            join lc in _context.LearningCircle on a.ToRoomId equals lc.LearningOuterKey
                            join m in _context.Members on a.CreateUser equals m.Id
                            where lc.LearningOuterKey.ToLower() == circleKey.ToLower() && a.CardisShow == true && a.IsActivity
                            select new Infrastructure.ViewModel.ActivitysViewModel()
                            {
                                ReadCount = 0,
                                ToRoomId = circleKey,
                                
                                Id = a.Id,
                                ModuleKey = a.ModuleKey,
                                CreatorAccount = m.Account,
                                CreatorName = m.Name,
                                CreatorPhoto = m.Photo,
                                Created_Utc = a.Created.Utc.Value,
                                StartDate = a.StartDate.Value,
                                Duration = a.Duration,
                                Publish_Utc = a.Publish_Utc,
                                OuterKey = a.OuterKey,
                                Deleted = a.Deleted,
                            };
            if (pageToken != null && pageToken != Guid.Empty)
            {
                var middleActivity = _context.Activitys.FirstOrDefault(t => t.OuterKey == pageToken);
                if (middleActivity != null)
                {
                    maxResult = maxResult / 2;
                    var olderActivitys =  activitys.Where(t => t.Publish_Utc <= middleActivity.Publish_Utc).OrderByDescending(x => x.Publish_Utc).Take(maxResult).ToList();//舊訊息:前端UI會往上長，因此是由大到小排序(最後長的是最舊的訊息)    
                    var newerActivitys = activitys.Where(t => t.Publish_Utc > middleActivity.Publish_Utc).OrderBy(t => t.Publish_Utc).Take(maxResult).ToList();
                    var resultActivitys = new List<Infrastructure.ViewModel.ActivitysViewModel>();
                    resultActivitys.AddRange(olderActivitys);
                    resultActivitys.AddRange(newerActivitys);
                    return resultActivitys;
                }
                else
                    return null;
            }
            else
            {
                var activityList = activitys.OrderByDescending(t => t.Publish_Utc).Take(maxResult).ToList();
                if (activityList.FirstOrDefault() == null)
                    return null;
                return activityList.OrderBy(t => t.Publish_Date);
            }
        }

        /// <summary>
        /// 取得活動列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <param name="memberId"></param>
        /// <param name="maxResult"></param>
        /// <param name="pageToken"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.ActivitysViewModel> GetListByLinQ(bool goBack,string circleKey, int memberId, int maxResult, Guid pageToken) {

            var activitys = from a in _context.Activitys
                            join lc in _context.LearningCircle on a.ToRoomId equals lc.LearningOuterKey
                            join m in _context.Members on a.CreateUser equals m.Id
                            where lc.LearningOuterKey == circleKey && a.CardisShow ==true && a.IsActivity
                            select new Infrastructure.ViewModel.ActivitysViewModel()
                            {
                                ReadCount=0,
                                ToRoomId = circleKey,
                                ReadMark = true,
                                Id = a.Id,
                                ModuleKey = a.ModuleKey,
                                CreatorAccount=m.Account,
                                CreatorName = m.Name,
                                CreatorPhoto=m.Photo,
                                Created_Utc=a.Created.Utc.Value,
                                StartDate = a.StartDate.Value,
                                Duration = a.Duration,
                                Publish_Utc = a.Publish_Utc,
                                OuterKey = a.OuterKey,
                                Deleted = a.Deleted,
                                
                            };
            if (pageToken != null && pageToken != Guid.Empty)
            {
                var middleActivity = _context.Activitys.FirstOrDefault(t => t.OuterKey == pageToken);
                if (middleActivity != null)
                {
                    var activityList = activitys.ToList();
                    var publishDate = middleActivity.Publish_Utc;
                    if (goBack)
                    {
                        var response = activityList.Where(t => t.Publish_Utc <= publishDate).OrderByDescending(x => x.Publish_Date).Take(maxResult).ToList();//舊訊息:前端UI會往上長，因此是由大到小排序(最後長的是最舊的訊息)    
                        return response;
                    }
                    else {
                        var response = activityList.Where(t => t.Publish_Utc >= publishDate).OrderBy(t => t.Publish_Utc).Take(maxResult).ToList();
                        return response;
                    }
                }else
                    return null;
            }
            else {
                var activityList = activitys.OrderByDescending(t => t.Publish_Utc).Take(maxResult).ToList();
                if (activityList.FirstOrDefault() == null)
                    return null;
                return activityList.OrderBy(t=>t.Publish_Date);
            }
        }
        public IEnumerable<Infrastructure.ViewModel.ActivitysViewModel> GetList(string circleKey, int memberId, int maxResult, Guid pageToken)
        {
            // 最後select的欄位，必須與方法GetListByDirection一致，若有增減請一併調整

            /* 傳入的SQL參數
                declare @groupId nvarchar(max) ='TestClass'
                declare @userId int = 6
                declare @maxResult int =20
                declare @pageToken uniqueidentifier='00000000-0000-0000-0000-000000000000'
             */
            #region // 產生SQL
            string sql = @"declare @mt int = (@maxResult+1)/2
                        declare @result Table (
	                        [Id] int identity(1,1), 
	                        [RowNum] bigint, 
	                        [ActivityId] int, 
	                        [RowNumTemp] int);
	
	                        -- 預塞
	                        if (not exists(select 0 from [dbo].[ActivitysReadMarks] where [memberId] = @userId and [ToRoomId] = @groupId))
	                        begin
		                        insert into [dbo].[ActivitysReadMarks]
		                        ([memberId], [ToRoomId], [LastReadActivityIdBegin], [LastReadActivityIdEnd])
		                        values
		                        (@userId,@groupId,0,0)
	                        end

                        -- 1. 依照id取前後特定筆數資料
	                        declare @middleId int = 0;
	                        declare @posId int = 0;
	                        declare @lastReadBegin int = 0;
	                        declare @lastReadEnd int = 0;
                            declare @middlePublish datetime;
                            declare @lastReadEndPublish datetime;
                            declare @lastReadBeginPublish datetime;
                            declare @postPublish datetime;
                            select @lastReadBeginPublish = (select Publish_Utc from [Activitys] where Id = ar.[LastReadActivityIdBegin]), @lastReadEndPublish = (select Publish_Utc from [Activitys] where Id = ar.[LastReadActivityIdEnd])
	                        from [dbo].[ActivitysReadMarks] ar
	                        where [memberId] = @userId and [ToRoomId] = @groupId;

	                        select @lastReadBegin = [LastReadActivityIdBegin], @lastReadEnd = [LastReadActivityIdEnd]
	                        from [dbo].[ActivitysReadMarks]
	                        where [memberId] = @userId and [ToRoomId] = @groupId;

                        -- 1-1. 如果有傳guid，以此guid查id
	                        if (@pageToken != '00000000-0000-0000-0000-000000000000')
	                        begin
                                select @middlePublish = [Publish_Utc]  from [dbo].[Activitys] where [OuterKey] = @pageToken and IsActivity=1 and CardisShow = 1;
		                        select @middleId = [Id] from [dbo].[Activitys] where [OuterKey] = @pageToken and IsActivity=1 and CardisShow = 1;
                                set @postPublish = @middlePublish
		                        set @posId = @middleId;
	                        end
                        -- 1-2. 如果未傳guid，自動查讀取紀錄拿最後已讀的項目id
	                        else
	                        begin
		                        if (@lastReadEnd > 0)
		                        begin -- 有已讀
                                    set @middlePublish = @lastReadEndPublish
			                        set @middleId = @lastReadEnd
			                        set @posId = @lastReadEnd;
                                    set @postPublish = @lastReadEndPublish
			                        -- 萬一全部讀完或者讀到剩一兩筆，將指標往上調
			                        declare @temp Table ([r] int, [Id] int,[Publish_Utc] datetime)
			                        insert into @temp
			                        select * from (
				                        select ROW_NUMBER() over (order by isnull([publish_utc],[Created_Utc]) desc) as [r] ,[Id],[Publish_Utc]
				                        from [dbo].[Activitys] where [ToRoomId] = @GroupId and IsActivity=1 and CardisShow = 1
			                        ) x where x.[r] < @mt -- @maxResult-1
			
			                        if ( exists( select 0 from @temp where [Id] = @posId))
                                        begin
                                            select @middlePublish = min([Publish_Utc]) from @temp
			                                select @middleId = min([Id]) from @temp
		                                end
                                    end
		                        else
		                        begin -- 沒已讀
                                    select @middlePublish = min([Publish_Utc]) from [dbo].[Activitys] where [ToRoomId] = @GroupId and IsActivity=1 and CardisShow = 1
			                        select @middleId = min([Id]) from [dbo].[Activitys] where [ToRoomId] = @GroupId and IsActivity=1 and CardisShow = 1
                                    set @postPublish = @middlePublish
			                        set @posId = @middleId;
		                        end
	                        end

                        ;with [TempResult] 
                        as (
	                        select * from (
		                       -- select 0 as [RowNum], 1 as [PrevMark], ROW_NUMBER() over (order by [Id] desc) as [RowNumTemp],
                                  select 0 as [RowNum], 1 as [PrevMark], ROW_NUMBER() over (order by isnull(r.Publish_utc,r.Created_Utc) desc) as [RowNumTemp],
		                        r.Id,isnull(r.Publish_utc,r.Created_Utc) as Publish_utc
		                        from [dbo].[Activitys] r
		                       -- where r.[ToRoomId] = @GroupId and r.[Id] <= @middleId and IsActivity=1 and CardisShow = 1 -- 這裡需要修改
		                        where r.[ToRoomId] = @GroupId and r.[Publish_Utc] <= @middlePublish and IsActivity=1 and CardisShow = 1 -- 這裡需要修改
	                        ) X where [RowNumTemp] between @mt+1 and @mt+1

	                        union
	                        select ROW_NUMBER() over (order by [PrevMark] desc, x.[Publish_Utc]) as [RowNum],* from (
		                        select
		                        1 as [PrevMark],
		                        ROW_NUMBER() over (order by isnull(r.Publish_utc,r.Created_Utc) desc) as [RowNumTemp], 
		                        r.[Id],isnull(r.Publish_utc,r.Created_Utc) as Publish_Utc
		                        from [dbo].[Activitys] r
		                        where r.[ToRoomId] = @GroupId and IsActivity=1 and CardisShow = 1
		                        --and r.[Id] <= @middleId
                                and r.[Publish_Utc] <= @middlePublish

		                        union

		                        select 
		                        0 as [PrevMark],
		                        ROW_NUMBER() over (order by isnull(r.Publish_utc,r.Created_Utc) ) as [RowNumTemp], 
		                        r.[Id], isnull(r.Publish_utc,r.Created_Utc) as Publish_Utc
		                        from [dbo].[Activitys] r
		                        where r.[ToRoomId] = @GroupId and IsActivity=1 and CardisShow = 1
		                        --and r.[Id] > @middleId
                                and r.[Publish_Utc] > @middlePublish
	                        ) x
	                        where [PrevMark] = 1 and RowNumTemp <= @mt or [PrevMark] = 0 and  RowNumTemp <= @maxResult
                        )

                        -- 為了做更多處理，這裡再將結果暫存(with語句做出來的[TempResult]只能用一次)
                        insert into @result ([RowNum],[ActivityId],[RowNumTemp])
                        select [RowNum],[Id],[RowNumTemp] as [ActivityId] from [TempResult];

                        -- ***更新已讀

                        declare @readBegin int = 0, @readEnd int = 0
                        select @readBegin = min([ActivityId]), @readEnd = max([ActivityId]) from @result where [RowNum] between 1 and @maxResult

                        IF(@readBegin is not NULL and @readEnd is not NULL)
                        BEGIN
	                        update [dbo].[ActivitysReadMarks]
	                        set [LastReadActivityIdBegin] = case when ISNULL([LastReadActivityIdBegin],0) = 0 then @readBegin 
										                         when ISNULL([LastReadActivityIdBegin],0) < @readBegin then [LastReadActivityIdBegin] else @readBegin end
	                        , [LastReadActivityIdEnd] = case when ISNULL([LastReadActivityIdEnd],0) = 0 then @readEnd
									                         when ISNULL([LastReadActivityIdEnd],0) > @readEnd then [LastReadActivityIdEnd] else @readEnd end
	                        , [Time] = getUtcDate()
	                        where [memberId] = @userId and [ToRoomId] = @groupId
                        END

                        -- ***輸出結果
                        select
                        tr.RowNum,
                        (select count(*) from [dbo].[ActivitysReadMarks] where r.[Id] between [LastReadActivityIdBegin] and [LastReadActivityIdEnd]) as ReadCount,
                        Convert(bit, case when tr.[ActivityId] = @posId then 1 else 0 end) as [PositionMark], 
                        Convert(bit, case when tr.[ActivityId] between @lastReadBegin and @lastReadEnd then 1 else 0 end) as [ReadMark], 
                        tr.RowNumTemp,
                        m.Account as CreatorAccount,
                        m.Name as CreatorName,
                        m.Photo as CreatorPhoto,
                        [Text]=ISNULL(msg.Content,''),
                        r.*
                        from @result tr
                        inner join [dbo].[Activitys] r on r.Id = tr.[ActivityId]
                        left outer join [dbo].[ActMessages] msg on r.OuterKey=msg.EventId and msg.[Type]='text'
                        left outer join [dbo].[MembersView] m on m.Id = r.CreateUser
                        where [RowNum] between 0 and @maxResult+1 and IsActivity=1 and IsActivity=1 and r.CardisShow=1
						order by isnull(r.Publish_utc,r.Created_Utc)";
            #endregion

            var data = _context.Database.SqlQuery<Infrastructure.ViewModel.ActivitysViewModel>(sql,
                 new SqlParameter("@groupId", circleKey),
                 new SqlParameter("@userId", memberId),
                 new SqlParameter("@maxResult", maxResult),
                 new SqlParameter("@pageToken", pageToken)).ToList();

            return data;
        }

        public IEnumerable<Infrastructure.ViewModel.ActivitysViewModel> GetListByDirection(bool goback, string circleKey, int memberId, int maxResult, Guid pageToken)
        {
            // 最後select的欄位，必須與方法GetList一致，若有增減請一併調整

            /* 傳入的SQL參數
                declare @groupId nvarchar(max) ='TestClass'
                declare @userId int =4
                declare @maxResult int =20
                declare @pageToken uniqueidentifier ='211A6B4E-08A7-4DC2-9D12-71FCB2B08819'
                declare @goback bit =1 --1:查更舊的活動，0:查更新的活動
             */
            #region // 產生SQL
            string sql = @"declare @result Table (
	                        [Id] int identity(1,1), 
	                        [RowNum] bigint, 
	                        [ActivityId] int);

                        declare @oldest int=0, @newest int=0, @nowId int=0, @otherId int=0, @tempId int=0;
                        declare @lastReadBegin int = 0, @lastReadEnd int = 0;

                        --找出現在的編號
                        select @nowId=Id from [dbo].[Activitys] where ToRoomId=@groupId and [OuterKey]=@pageToken and IsActivity=1 and CardisShow =1

                        --有資料才繼續
                        IF @nowId >0
                        BEGIN
	                        --查出活動編號列表
	                        ;With [TempResult]
	                        as (	
		                        select ROW_NUMBER() over (order by IsNull(r.[Publish_Utc],r.[Created_Utc])) as [RowNum], 
		                        r.*
		                        from [dbo].[Activitys] r
		                        where r.[ToRoomId] = @GroupId and r.Id >= @nowId and @goback = 0 and IsActivity=1 and CardisShow =1
		                        union
		                        select ROW_NUMBER() over (order by IsNull(r.[Publish_Utc],r.[Created_Utc]) desc) as RowNum, 
		                        r.*
		                        from [dbo].[Activitys] r
		                        where r.[ToRoomId] = @GroupId and r.Id <= @nowId and @goback = 1 and IsActivity=1 and CardisShow =1
	                        )

	                        insert into @result ([RowNum],[ActivityId])
	                        select [RowNum],[Id] as [ActivityId] from [TempResult];

	                        --查出標的編號
	                        select @tempId=MAX(RowNum) from @result
	                        IF @goback=1
		                        BEGIN
			                        --找出最舊的編號
			                        IF @tempId>@maxResult+1
				                        select @oldest=ActivityId from @result where RowNum=@maxResult+1
			                        ELSE
				                        select @oldest=ActivityId from @result where RowNum=@tempId	

			                        --找出起始的編號		
			                        IF @tempId>@maxResult
				                        select @otherId=ActivityId from @result where RowNum=@maxResult
			                        ELSE
				                        select @otherId=ActivityId from @result where RowNum=@tempId

			                        --找出最新的編號
			                        IF @oldest=0 and @otherId=0
				                        set @newest=0
			                        ELSE
				                        SELECT @newest=ISNULL(MIN(Id),@nowId) from [dbo].[Activitys] where Id>@nowId and [ToRoomId]=@groupId and IsActivity=1 and CardisShow =1
		                        END
	                        ELSE
		                        BEGIN
			                        --找出最舊的編號
			                        select @oldest=ISNULL(MAX(Id),@nowId) from [dbo].[Activitys] where Id<@nowId and [ToRoomId]=@groupId and IsActivity=1 and CardisShow =1

			                        --找出結束的編號			
			                        IF @tempId>@maxResult
				                        select @otherId=ActivityId from @result where RowNum=@maxResult
			                        ELSE
				                        select @otherId=ActivityId from @result where RowNum=@tempId

			                        --找出最新的編號
			                        select @newest=ActivityId from @result where RowNum=@maxResult+1
			                        IF @newest=0
				                        set @newest=@otherId
		                        END

	                        --更新已讀
	                        declare @readBegin int = 0, @readEnd int = 0
	                        select @readBegin = min([ActivityId]), @readEnd = max([ActivityId]) from @result where [RowNum] between 1 and @maxResult

	                        update [dbo].[ActivitysReadMarks]
	                        set [LastReadActivityIdBegin] = case when [LastReadActivityIdBegin] = 0 then @readBegin when [LastReadActivityIdBegin] < @readBegin then [LastReadActivityIdBegin] else @readBegin end
	                        , [LastReadActivityIdEnd] = case when [LastReadActivityIdEnd] = 0 then @readEnd when [LastReadActivityIdEnd] > @readEnd then [LastReadActivityIdEnd] else @readEnd end
	                        , [Time] = getUtcDate()
	                        where [memberId] = @userId and [ToRoomId] = @groupId
                            
	                        --取出未讀起訖編號
	                        select @lastReadBegin = [LastReadActivityIdBegin], @lastReadEnd = [LastReadActivityIdEnd]
	                        from [dbo].[ActivitysReadMarks]
	                        where [memberId] = @userId and [ToRoomId] = @groupId;	
	
	                        -- ***輸出結果
	                        print @nowId
	                        print @otherId
	                        print @goback
	                        select	RowNum=case when a.Id=@oldest and (@oldest<>@otherId and @oldest<>@nowId)  then 0
						                        when a.Id=@newest and (@newest<>@otherId and @newest<>@nowId) then @maxResult+1
						                        else r.RowNum end
			                        , (select count(*) from [dbo].[ActivitysReadMarks] where ToRoomId=@groupId and (r.[Id] between [LastReadActivityIdBegin] and [LastReadActivityIdEnd])) as [ReadCount]
			                        , Convert(bit, case when r.[ActivityId] = @nowId then 1 else 0 end) as [PositionMark]
			                        , Convert(bit, case when r.[ActivityId] between @lastReadBegin and @lastReadEnd then 1 else 0 end) as [ReadMark]
			                        , m.Account as CreatorAccount
			                        , m.Name as CreatorName
			                        , m.Photo as CreatorPhoto
			                        , [Text]=ISNULL(msg.Content,'')
			                        , a.*
	                        from [dbo].[Activitys] a
	                        left outer join @result r on a.Id=r.ActivityId
	                        left outer join [dbo].[ActMessages] msg on a.OuterKey=msg.EventId and msg.[Type]='text'
	                        left outer join [dbo].[MembersView] m on m.Id = a.CreateUser
	                        where a.ToRoomId=@groupId and (a.Id between @oldest and @newest) and IsActivity=1 and CardisShow =1
	                        order by [RowNum]
                        END";
            #endregion

            var data = _context.Database.SqlQuery<Infrastructure.ViewModel.ActivitysViewModel>(sql,
                new SqlParameter("@groupId", circleKey),
                new SqlParameter("@userId", memberId),
                new SqlParameter("@maxResult", maxResult),
                new SqlParameter("@pageToken", pageToken),
                new SqlParameter("@goback", goback)).ToList();

            return data;
        }

        public IEnumerable<SignInData> GetSignInData(Infrastructure.DataTransferObject.SignInEventParam param,bool isSce = false)
        {
      
            #region //SQL
            var sql = new StringBuilder();
            if (isSce)
                sql.Append(@"select m.*, Sort=CAST(ROW_NUMBER() OVER(ORDER BY ms.manscore_type,ms.manscore_collno,ms.manscore_grade DESC,ms.manscore_grp,isnull(ms.manscore_subgrp,''),ms.man_no) AS INT)");
            else
                sql.Append(@"select m.*, Sort = CAST(ROW_NUMBER() OVER(ORDER BY m.StudId) AS INT)");

            sql.Append(@"from
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
                      --  left outer join [dbo].[MembersView] m on a.CreateUser=m.Id
            left outer join [dbo].[Members] m on a.CreateUser=m.Id
                        left outer join [LearningRoles] AS lr ON lr.LearningId = s.LearningId
                        left outer join [CircleMemberRoleplays] AS cr ON cr.RoleId=lr.Id
                        left outer join [LearningAuths] AS la ON cr.RoleId = la.[LearningRoleId]
                        left outer join [ModuleFuntions] AS mf ON la.[FunctionId] = mf.[Id]
                   --     left outer join [dbo].[MembersView] mem on cr.MemberId = mem.Id
            left outer join [dbo].[Members] mem on cr.MemberId = mem.Id
                        left outer join [dbo].[ActRollCallLogs] l on l.RollCallId=s.Id and l.StudId=mem.Id
                      --left outer join dbo.AttendanceLeaves al on s.LearningId = al.LearningId and al.studid=l.studid and a.ActivityDate >=al.LeaveDate and a.ActivityDate < dateadd(d,1,al.LeaveDate) and al.status !=40
                        left outer join [dbo].[AttendanceLeaves] al on s.LearningId = al.LearningId and al.studid=l.studid and Convert(varchar(10),a.ActivityDate,111) =Convert(varchar(10),al.LeaveDate,111) and al.status !=40
                        left outer join [dbo].[Members] c on l.CreateUser=c.Id
                        where mf.OutSideKey=@moduleFun
                        and cr.Enable=1 and la.Enable=1 and mf.Enable=1 and a.CardisShow=1  and lr.IsAdminRole =0");
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
                sqlParam.Add(new SqlParameter("@eventId", param.EventIds.FirstOrDefault()));
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
            if (isSce) {
                sql.Append(" left outer join [ICAN5].[dbo].[ManScore] ms on m.ToRoomId=ms.course_no and m.StudId=ms.man_no");
                sql.Append(" where ms.manscore_mark='10'");
            }
                    

                var list = _context.Database.SqlQuery<SignInData>(sql.ToString(), sqlParam.ToArray()).ToList();

            return list;
        }

        public IEnumerable<Infrastructure.ViewModel.GroupListViewModel> GetGroupListViewModel(string circleKey, Guid eventId)
        {
            string sqlList = @"
                                    select a.OuterKey as EventId,agc.Name as GroupTitle, agc.Created_Utc as CreateDateUtc ,a.Publish_Utc as PublishDateUtc
                                    from [dbo].[Activitys] a
                                    inner join [dbo].[ActGroupCategories] agc on a.OuterKey = agc.EventId
                                    inner join [dbo].[ActGroups] ag on agc.Id = ag.CategoryId
                                    where a.ModuleKey = 'group' and 
                                          a.ToRoomId = @circleKey and
                                          a.OuterKey = @eventId and 
                                          a.Deleted_Utc is null
                ";
            var sqlResult = _context.Database.SqlQuery<Infrastructure.ViewModel.GroupListViewModel>(sqlList,
                     new SqlParameter("@circleKey", circleKey),
                     new SqlParameter("@eventId", eventId)).ToList();

            return sqlResult;
        }

        public IEnumerable<Infrastructure.ViewModel.GroupListViewModel> GetGroupListViewModelByNongroup(string circleKey, Guid eventId)
        {
            string sqlNonList = @"
                                    select a.OuterKey as EventId,agc.Name as GroupTitle,'未分組' as GroupName,0 as GroupSort,a.Publish_Utc as PublishDateUtc
                                    from [dbo].[Activitys] a
                                    inner join [dbo].[ActGroupCategories] agc on a.OuterKey = agc.EventId
                                    where ModuleKey = 'group' and 
                                          ToRoomId = @circleKey and
                                          a.OuterKey = @eventId and 
                                          a.Deleted_Utc is null
                ";
            var sqlNonResult = _context.Database.SqlQuery<Infrastructure.ViewModel.GroupListViewModel>(sqlNonList,
                     new SqlParameter("@circleKey", circleKey),
                     new SqlParameter("@eventId", eventId)).ToList();

            return sqlNonResult;
        }

        public IEnumerable<Infrastructure.ViewModel.GroupListViewModel> GetGroupListViewModel(string circleKey, int memberId, Guid eventId)
        {
            string sqlList = @"
                                    select a.OuterKey as EventId,agc.Name as GroupTitle,ag.Name as GroupName,ag.Sort as GroupSort, agc.Created_Utc as CreateDateUtc ,a.Publish_Utc as PublishDateUtc 
                                    from [dbo].[Activitys] a
                                    inner join [dbo].[ActGroupCategories] agc on a.OuterKey = agc.EventId
                                    inner join [dbo].[ActGroups] ag on agc.Id = ag.CategoryId
                                    inner join [dbo].[ActGroupMembers] agm on ag.Id = agm.GroupId
                                    where ModuleKey = 'group' and 
                                          ToRoomId = @circleKey and
                                          agm.MemberId = @memberId and
                                          a.OuterKey = @eventId and 
                                          a.Deleted_Utc is null
                ";
            var sqlResult = _context.Database.SqlQuery<Infrastructure.ViewModel.GroupListViewModel>(sqlList,
                     new SqlParameter("@circleKey", circleKey),
                     new SqlParameter("@memberId", memberId),
                     new SqlParameter("@eventId", eventId)).ToList();

            return sqlResult;
        }


    }
}
