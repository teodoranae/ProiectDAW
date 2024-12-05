using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class Channel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Denumirea canalului este necesară.")]
        [MaxLength(100, ErrorMessage = "Denumirea nu poate depăși 100 de caractere.")]
        public string ChannelName { get; set; }
        [Required(ErrorMessage = "Descrierea canalului este necesară.")]
        [MaxLength(1000, ErrorMessage = "Descrierea nu poate depăși 1000 de caractere")]
        public string ChannelDescription { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ChannelRole> ChannelRoles  { get; set; }
        public virtual ICollection<UserChannel> UserChannels { get; set; }
    }
}
