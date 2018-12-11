using WiicoApi.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
namespace WiicoApi.Repository.EntityRepositorys
{
    public class ActVoteRepo : GenericEntityRepository<ActVote,WiicoDB>
    {
        public ActVoteRepo(WiicoDB _context) : base(_context)
        {

        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ViewModel.ActivityFunction.Vote.VoteListResponse> GetList(string groupId) {
            var list = (from av in _context.ActVote
                       join a in _context.Activitys on av.EventId equals a.OuterKey
                       where a.ToRoomId == groupId && a.ModuleKey=="vote" && a.CardisShow==true 
                       select new Infrastructure.ViewModel.ActivityFunction.Vote.VoteListResponse {
                           Content=av.Content,
                           EventId = av.EventId,
                           Title=av.Title,
                           PublishTime = a.Publish_Utc.Value,
                           GroupId = groupId,
                           PresentCount=av.PresentCount,
                           VoteId = av.Id,
                           IsStart = av.IsStart
                       }).ToList();
            if (list.FirstOrDefault() != null)
                return list;
            else
                return null;
        }
        /// <summary>
        /// 建立投票
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActVote VoteCreate(ActVote entity)
        {
                Insert(entity);
                _context.SaveChanges();
                return entity;
        }
    }
}
