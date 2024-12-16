using DAWSlack.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DAWSlack.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
               
                if (context.Roles.Any())
                {
                    return; 
                }

                context.Roles.AddRange(

                new IdentityRole
                {
                    Id = "b06c5910-4b67-4d62-9a89-d92a0e875590",
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },


                new IdentityRole
                {
                    Id = "b06c5910-4b67-4d62-9a89-d92a0e875591",
                    Name = "Moderator",
                    NormalizedName = "Moderator".ToUpper()
                },


                new IdentityRole
                {
                    Id = "b06c5910-4b67-4d62-9a89-d92a0e875592",
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                }


                );

                var hasher = new PasswordHasher<ApplicationUser>();

                context.Users.AddRange(
                new ApplicationUser

                {

                    Id = "b129ac90-dcde-499d-83a5-db6b00e63833",

                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },

                new ApplicationUser
                {

                    Id = "b129ac90-dcde-499d-83a5-db6b00e63834",

                    UserName = "moderator@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "MODERATOR@TEST.COM",
                    Email = "moderator@test.com",
                    NormalizedUserName = "MODERATOR@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Moderator1!")
                },
                new ApplicationUser

                {

                    Id = "b129ac90-dcde-499d-83a5-db6b00e63835",

                    UserName = "user@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@TEST.COM",
                    Email = "user@test.com",
                    NormalizedUserName = "USER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "User1!")
                }
            );

                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {

                    RoleId = "b06c5910-4b67-4d62-9a89-d92a0e875590",
                    UserId = "b129ac90-dcde-499d-83a5-db6b00e63833"
                },

                new IdentityUserRole<string>

                {

                    RoleId = "b06c5910-4b67-4d62-9a89-d92a0e875591",
                    UserId = "b129ac90-dcde-499d-83a5-db6b00e63834"
                },

                new IdentityUserRole<string>

                {

                    RoleId = "b06c5910-4b67-4d62-9a89-d92a0e875592",
                    UserId = "b129ac90-dcde-499d-83a5-db6b00e63835"
                }
                );
                context.SaveChanges();
            }
        }
    }
}
    