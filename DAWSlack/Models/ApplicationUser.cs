using Microsoft.AspNetCore.Identity;

namespace DAWSlack.Models
{
    public class ApplicationUser: IdentityUser
    {
//un user poate trimite mai multe mesaje
    public virtual ICollection<Message> Messages { get; set; }
        //un user poate face parte din mai multe canale
        public virtual ICollection<UserChannel> UserChannels { get; set; }
    }
}
