using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAWSlack.Models
{
    public class ChatChannel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Denumirea canalului este necesară.")]
        [MaxLength(100, ErrorMessage = "Denumirea nu poate depăși 100 de caractere.")]
        public string ChannelName { get; set; }
        [Required(ErrorMessage = "Descrierea canalului este necesară.")]
        [MaxLength(1000, ErrorMessage = "Descrierea nu poate depăși 1000 de caractere")]
        public string? ChannelDescription { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Category? Category { get; set; }
        // cheie externa (FK) - un canal are asociata una sau mai multe categorii
        public int? CategoryId { get; set; }
        public virtual ICollection<ChannelRole>? ChannelRoles { get; set; }
        public virtual ICollection<UserChannel>? UserChannels { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<ApplicationUser>? Users { get; set; }
        public virtual ICollection<ApplicationUser>? Mods { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }
    }
}
