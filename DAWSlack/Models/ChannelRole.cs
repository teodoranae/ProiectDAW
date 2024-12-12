using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class ChannelRole
    {
        [Key]
        public int Id { get; set; }

        public string ChannelRoleName { get; set; }
        public string ChannelRoleDescription { get; set; }

        public virtual ChatChannel Channel { get; set; }

    }
}
