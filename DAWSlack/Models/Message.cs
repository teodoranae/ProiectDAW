using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public string ?Type { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public string Content { get; set; }

        public DateTime Date { get; set; }
        
        public string? UserId { get; set; } 
        public virtual ApplicationUser? User { get; set; }  
        public int? ChannelId { get; set; }

        public virtual ChatChannel? Channel { get; set; }
    }
}
