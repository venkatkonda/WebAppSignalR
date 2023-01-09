using ExSignalR.Data;
using ExSignalR.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ExSignalR.Models
{
    public class ApplicationUser : IdentityUser
    {
        private readonly ApplicationDbContext _db;
        private readonly string _curUserId;
        public ApplicationUser(ApplicationDbContext db,string curuserId)
        {
            _db = db;
            _curUserId = curuserId;
        }
        public DateTime LatestMsgDateTime { get; set; }
        public List<TotalUserMessage> Messages
        {
            get
            {
                //var msgs = _db.UserMessages.Where(x => x.ToUserId == this.Id || x.FromUserId == this.Id).ToList();
                var msgs = GetUserMessages();
                if (msgs.Count > 0)
                {
                    LatestMsgDateTime = msgs.OrderBy(x => x.MsgDateTime).FirstOrDefault().MsgDateTime;
                }
                else
                {
                    msgs = new List<TotalUserMessage>();
                    LatestMsgDateTime = DateTime.MinValue;
                }
                return msgs;
            }
        }

        public List<TotalUserMessage> GetUserMessages()
        {
            var receivedMsgs = _db.UserMessages.Where(x => x.ToUserId == _curUserId && x.FromUserId == this.Id).ToList();
            if (receivedMsgs == null)
            {
                receivedMsgs = new List<UserMessage>();
            }
            var sentMsgs = _db.UserMessages.Where(x => x.ToUserId == this.Id && x.FromUserId == _curUserId).ToList();
            if (sentMsgs == null)
            {
                sentMsgs = new List<UserMessage>();
            }
            var TotalMessages = new List<TotalUserMessage>();
            TotalMessages.AddRange(receivedMsgs.Select(x => new TotalUserMessage
            {
                Id = x.Id,
                FromUserId = x.FromUserId,
                IsReceived = true,
                IsSeen = x.IsSeen,
                Message = x.Message,
                MsgDateTime = x.MsgDateTime,
                ToUserId = x.ToUserId
            }));
            TotalMessages.AddRange(sentMsgs.Select(x => new TotalUserMessage
            {
                Id = x.Id,
                FromUserId = x.FromUserId,
                IsReceived = false,
                IsSeen = x.IsSeen,
                Message = x.Message,
                MsgDateTime = x.MsgDateTime,
                ToUserId = x.ToUserId
            }));
            TotalMessages = TotalMessages.OrderBy(x => x.MsgDateTime).ToList();
            return TotalMessages;
        }
    }
}
