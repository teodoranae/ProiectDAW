using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class ChannelRole
    {
        [Key]
        public int Id { get; set; }

        public string ChannelRoleName { get; set; }
        public string ChannelRoleDescription { get; set; }

        public int ChannelId { get; set; }
        public virtual ChatChannel Channel { get; set; }

        public string UserId { get; set; } 
        public virtual ApplicationUser User { get; set; }
    }
}
