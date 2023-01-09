using ExSignalR.Data;
using ExSignalR.Hubs;
using ExSignalR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace ExSignalR.Pages
{
    [Authorize]
    public class homeModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        IHubContext<ChatHub> _chatHubContext;
        private readonly ApplicationDbContext _db;
        public homeModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager, IHubContext<ChatHub> chatHubContext, ApplicationDbContext db)
        {
            _logger = logger;
            _userManager = userManager;
            _chatHubContext = chatHubContext;
            _db = db;
        }

        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {

                CurUserId = _userManager.GetUserId(User);
                var users = _userManager.Users.Where(x => x.UserName != User.Identity.Name).Select(x => new ApplicationUser(_db, CurUserId)
                {
                    Email = x.Email,
                    Id = x.Id
                }).ToList();
                //var list = users.OrderBy(x => x.LatestMsgDateTime).ToList();
                ListOfUsers = users.OrderByDescending(x => x.LatestMsgDateTime).ToList();
                //ListOfUsers = ListOfUsers.OrderBy(x => x.Messages.Max(s => s.MsgDateTime)).ToList();
                //Messages = _db.UserMessages.Where(x => x.ToUserId == CurUserId || x.FromUserId == CurUserId).ToList();
            }

        }
        public string CurUserId { get; set; }
        //public List<UserMessage> Messages { get; set; }
        public List<ApplicationUser> ListOfUsers { get; set; } = new List<ApplicationUser>();
        //public String SelectedUser { get; set; }
        //public string MessageText { get; set; } = string.Empty;
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    //if(SelectedUser != null)
        //    //{
        //    await _chatHubContext.Clients.All.SendAsync("TestMe", MessageText);
        //    //}

        //    return Page();
        //}

        //public IActionResult OnGetChatPartial(string toid)
        //{
        //    CurUserId = _userManager.GetUserId(User);
        //    List<String> userIds = new List<String>() { CurUserId, toid };
        //    Messages = _db.UserMessages.Where(x => userIds.Contains(x.ToUserId) || userIds.Contains(x.FromUserId)).ToList();
        //    if (Messages == null)
        //    {
        //        Messages = new List<UserMessage>();
        //    }
        //    return new PartialViewResult
        //    {
        //        ViewName = "_ChatPartial",
        //        ViewData = new ViewDataDictionary<List<UserMessage>>(ViewData, Messages)
        //    };
        //}

        public PartialViewResult OnGetChatPartial(string toid)
        {
            CurUserId = _userManager.GetUserId(User);
            //List<String> userIds = new List<String>() { CurUserId, toid };
            //Messages = _db.UserMessages.Where(x => userIds.Contains(x.ToUserId) || userIds.Contains(x.FromUserId)).ToList();
            var receivedMsgs = _db.UserMessages.Where(x => x.ToUserId == CurUserId && x.FromUserId == toid).ToList();
            if (receivedMsgs == null)
            {
                receivedMsgs = new List<UserMessage>();
            }
            var sentMsgs = _db.UserMessages.Where(x => x.ToUserId == toid && x.FromUserId == CurUserId).ToList();
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
            var model = new ChatViewModel { ToUserId = toid, FromUserId = CurUserId, Messages = TotalMessages };
            //var partialView = "_ChatPartial";
            //var myViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { { partialView,Messages } };
            //myViewData.Model = Messages;
            var myViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model,
            };

            return new PartialViewResult
            {
                ViewName = "_ChatPartial",
                ViewData = myViewData
                //ViewData = new ViewDataDictionary<List<UserMessage>>(ViewData, Messages)
                //ViewData = myViewData
            };
        }


    }
}
