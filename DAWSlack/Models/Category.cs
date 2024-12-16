using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Denumirea categoriei este necesara.")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Descrierea categoriei este necesara.")]
        public string CategoryDescription { get; set; }

        public virtual ICollection<ChatChannel>? Channels { get; set; }
    }
}
