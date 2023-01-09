using ExSignalR.Data;
using ExSignalR.Hubs;
using ExSignalR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;

namespace ExSignalR.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        IHubContext<ChatHub> _chatHubContext;
        private readonly ApplicationDbContext _db;
        public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager, IHubContext<ChatHub> chatHubContext, ApplicationDbContext db)
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
                var CurUserId = _userManager.GetUserId(User);
                ListOfUsers = _userManager.Users.Where(x => x.UserName != User.Identity.Name).ToList();
                Messages = _db.UserMessages.Where(x => x.ToUserId == CurUserId || x.FromUserId == CurUserId).ToList();
            }

        }
        public string CurUserId { get; set; }
        public List<UserMessage> Messages { get; set; }
        public List<IdentityUser> ListOfUsers { get; set; } = new List<IdentityUser>();
        public String SelectedUser { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public async Task<IActionResult> OnPostAsync()
        {
            //if(SelectedUser != null)
            //{
            await _chatHubContext.Clients.All.SendAsync("TestMe", MessageText);
            //}

            return Page();
        }

    }
}