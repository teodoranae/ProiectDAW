using DAWSlack.Data;
using DAWSlack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAWSlack.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public MessagesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
            public IActionResult New(Message mes)
            {
                mes.Date = DateTime.Now;

                try
                {
                    db.Messages.Add(mes);
                    db.SaveChanges();
                    return Redirect("/Articles/Show/" + mes.ChannelId);
                }

                catch (Exception)
                {
                    return Redirect("/Articles/Show/" + mes.ChannelId);
                }

            }

            // Stergerea unui comentariu asociat unui articol din baza de date
            [HttpPost]
            public IActionResult Delete(int id)
            {
                Message mes = db.Messages.Find(id);
                db.Messages.Remove(mes);
                db.SaveChanges();
                return Redirect("/Channels/Show/" + mes.ChannelId);
            }


            public IActionResult Edit(int id)
            {
                Message mes = db.Messages.Find(id);
                ViewBag.Message = mes;
                return View();
            }

            [HttpPost]
            public IActionResult Edit(int id, Message requestComment)
            {
                Message mes = db.Messages.Find(id);
                try
                {

                    mes.Content = requestComment.Content;

                    db.SaveChanges();

                    return Redirect("/Channels/Show/" + mes.ChannelId);
                }
                catch (Exception e)
                {
                    return Redirect("/Channels/Show/" + mes.ChannelId);
                }


            }
        }
    }

