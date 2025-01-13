using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class UserChannel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int ChannelId { get; set; }
        public virtual ChatChannel Channel { get; set; }
        public string Roles { get; set; } 

    }
}
