using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class JoinRequest
    {
        [Key]
        public int RequestId { get; set; }
        public string UserId { get; set; }
        public int ChannelId { get; set; }

    }
}
