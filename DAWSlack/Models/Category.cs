using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Denumirea categoriei este necesara.")]
        [StringLength(50, ErrorMessage = "Category Name cannot exceed 50 characters.")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Descrierea categoriei este necesara.")] 
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string CategoryDescription { get; set; }

        public virtual ICollection<ChatChannel>? Channels { get; set; }
    }
}
