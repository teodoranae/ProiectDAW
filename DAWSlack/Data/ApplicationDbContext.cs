using DAWSlack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAWSlack.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ChatChannel> Channels { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ChannelRole> ChannelRoles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserChannel> UserChannels { get; set; }

        public DbSet<JoinRequest> JoinRequests { get; set; }
    }
}
