using System.ComponentModel.DataAnnotations;

namespace DAWSlack.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }


        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public virtual ICollection<Channel> Channels { get; set; }
    }
}
