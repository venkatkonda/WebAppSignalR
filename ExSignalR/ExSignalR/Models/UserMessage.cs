using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExSignalR.Models
{
    public class UserMessage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime MsgDateTime { get; set; }
        public bool IsSeen { get; set; } = false;

    }

    public class TotalUserMessage
    {
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime MsgDateTime { get; set; }
        public bool IsSeen { get; set; } = false;
        public bool IsReceived { get; set; }
    }
    public class ChatViewModel
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public List<TotalUserMessage> Messages { get; set; }
    }
}
