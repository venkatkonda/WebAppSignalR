using ExSignalR.Data;
using ExSignalR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ExSignalR.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task SendMessage(string fromid, string toid, string message)
        {
            var msg = new UserMessage
            {
                FromUserId = fromid,
                ToUserId = toid,
                Message = message,
                MsgDateTime = DateTime.UtcNow,
                IsSeen = false
            };
            _db.UserMessages.Add(msg);
            await _db.SaveChangesAsync();
            //await Clients.User(toid).SendAsync("ReceiveMessage", fromusername, message);
            await Clients.User(toid).SendAsync("ReceiveMessage", message);
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task TestMe(string someRandomText)
        {
            await Clients.All.SendAsync(
                $"{this.Context.User.Identity.Name} : {someRandomText}",
                CancellationToken.None);
        }


    }
}
