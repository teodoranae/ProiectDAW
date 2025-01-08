using DAWSlack.Data;
using DAWSlack.Models;
using Microsoft.AspNetCore.Authorization;
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
                    return Redirect("/Messages/Show/" + mes.ChannelId);
                }

                catch (Exception)
                {
                    return Redirect("/Messages/Show/" + mes.ChannelId);
                }

            }

        //// Stergerea unui comentariu asociat unui articol din baza de date
        //[HttpPost]
        //public IActionResult Delete(int id)
        //{
        //    Message message = db.Messages.Find(id);
        //    if (message == null)
        //    {
        //        TempData["message"] = "Message not found.";
        //        TempData["messageType"] = "alert-danger";
        //        return RedirectToAction("Show", "ChatChannels"); // Redirect back to the channel
        //    }

        //    db.Messages.Remove(message);
        //    db.SaveChanges();

        //    TempData["message"] = "Message deleted successfully.";
        //    TempData["messageType"] = "alert-success";
        //    return RedirectToAction("Show", "ChatChannels"); // Redirect back to the channel
        //}

            [HttpPost]
            public IActionResult Delete(int id)
            {
                var message = db.Messages.FirstOrDefault(m => m.Id == id);

                if (message == null)
                {
                    TempData["message"] = "Message not found.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Show", "ChatChannels");
                }

                db.Messages.Remove(message);
                db.SaveChanges();

                TempData["message"] = "Message deleted successfully.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "ChatChannels", new { id = message.ChannelId });
            }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Message comm = db.Messages.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Articles");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Message requestComment)
        {
            Message comm = db.Messages.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comm.Content = requestComment.Content;

                    db.SaveChanges();

                    return Redirect("/ChatChannels/Show/" + comm.ChannelId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", "ChatChannels", new { id = requestComment.ChannelId });
            }
        }
    }
    }

