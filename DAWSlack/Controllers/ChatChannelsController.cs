using DAWSlack.Data;
using DAWSlack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Channels;


namespace DAWSlack.Controllers
{
    public class ChatChannelsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ChatChannelsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //public ActionResult Index()
        //{
        //    var channels = from channel in db.Channels
        //                   orderby channel.ChannelName
        //                   select channel;
        //    ViewBag.Channels = channels;
        //    return View();
        //}

        //public ActionResult Show(int id)
        //{
        //    ChatChannel channel = db.Channels.Find(id);
        //    ViewBag.Channels = channel;
        //    return View();
        //}



        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName
                });
            }
            /* Sau se poate implementa astfel: 
             * 
            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.Id.ToString();
                listItem.Text = category.CategoryName;

                selectList.Add(listItem);
             }*/


            // returnam lista de categorii
            return selectList;
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public ActionResult New()
        {
            ChatChannel channel = new ChatChannel();

            channel.Categ = GetAllCategories();

            return View(channel);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New(ChatChannel channel)
        {
            if (ModelState.IsValid)
            {
                channel.UserId= _userManager.GetUserId(User);
                db.Channels.Add(channel);
                db.SaveChanges();
                TempData["message"] = "Canalul a fost creat";
                TempData["messageType"] = "alert-success";
                int idChannel = channel.Id;
                string idUser = channel.UserId;

                
                UserChannel userChannel = new UserChannel();
                userChannel.ChannelId = idChannel;
                userChannel.UserId = idUser;
                if (User.IsInRole("Admin"))
                {
                    userChannel.Roles = "Admin";
                }
                else
                {
                    userChannel.Roles = "Moderator";
                    UserChannel addAdmin = new UserChannel();
                    addAdmin.ChannelId = idChannel;
                    addAdmin.Roles = "Admin";

                    var roleId = db.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                    var adminUserId = db.UserRoles
                        .Where(ur => ur.RoleId == roleId)
                        .Select(uc => uc.UserId)
                        .FirstOrDefault();
                    addAdmin.UserId = adminUserId;
                    db.UserChannels.Add(addAdmin);
                    db.SaveChanges();
                }
                db.UserChannels.Add(userChannel);
                db.SaveChanges();
                return RedirectToAction("Index");
                //return View();
            }
            else
            {
                channel.Categ = GetAllCategories();
                return View(channel);
            }
        }

        //public ActionResult New(ChatChannel channel)
        //{
        //    try
        //    {
        //        db.Channels.Add(channel);
        //        db.SaveChanges();
        //        TempData["message"] = "Canalul a fost adaugata";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception e)
        //    {
        //        return View(channel);
        //    }
        //}

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index()
        {
            var channels = db.Channels.OrderByDescending(a => a.ChannelName);

            // ViewBag.OriceDenumireSugestiva
            // ViewBag.Articles = articles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // MOTOR DE CAUTARE

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in articol (Title si Content)

                List<int> channelsIds = db.Channels.Where
                                        (
                                         at => at.ChannelName.Contains(search)
                                        ).Select(a => a.Id).ToList();




                // Lista articolelor care contin cuvantul cautat
                // fie in articol -> Title si Content
                // fie in comentarii -> Content
                //articles = db.Channels.Where(article => mergedIds.Contains(article.Id))

                channels = db.Channels.Where(channels => channelsIds.Contains(channels.Id))
                    .OrderByDescending(a => a.ChannelName);

            }

            ViewBag.SearchString = search;

            // AFISARE PAGINATA

            // Alegem sa afisam 3 articole pe pagina
            int _perPage = 3;

            // Fiind un numar variabil de articole, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = channels.Count();

            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedArticles = channels.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Channels = paginatedArticles;

            // DACA AVEM AFISAREA PAGINATA IMPREUNA CU SEARCH

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/ChatChannels/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/ChatChannels/Index/?page";
            }

            return View();
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult MyChannels()
        {
            // Obține UserId-ul utilizatorului curent
            var userId = _userManager.GetUserId(User);

            // Selectează doar canalele unde utilizatorul este "Creator" sau "Moderator"
            var channels = db.UserChannels
                .Where(uc => uc.UserId == userId && (uc.Roles == "Creator" || uc.Roles == "Moderator" || uc.Roles == "Admin"))
                .Select(uc => uc.ChannelId) // Selectăm doar ID-urile canalelor
                .ToList();

            // Preluăm datele canalelor asociate acestor ID-uri
            var myChannels = db.Channels
                .Where(ch => channels.Contains(ch.Id))
                .OrderBy(ch => ch.ChannelName)
                .ToList();

            // Transmitem canalele către View
            ViewBag.MyChannels = myChannels;

            return View();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult EditChannel(int id)
        {
            // Găsește canalul după ID
            var channel = db.Channels.FirstOrDefault(c => c.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            // JOIN între UserChannels și AspNetUsers
            var participants = (from uc in db.UserChannels
                                join user in db.Users
                                on uc.UserId equals user.Id
                                where uc.ChannelId == id
                                select new
                                {
                                    UserId = user.Id,
                                    UserName = user.UserName, // Numele utilizatorului
                                    Role = uc.Roles // Rolul utilizatorului
                                }).ToList();

            if (!participants.Any())
            {
                Console.WriteLine($"Nu au fost găsiți participanți pentru canalul cu ID {id}");
            }

            ViewBag.Participants = participants;

            var joinRequests = db.JoinRequests
                         .Where(jr => jr.ChannelId == id)
                         .Join(db.Users,
                               jr => jr.UserId,
                               user => user.Id,
                               (jr, user) => new
                               {
                                   RequestId = jr.RequestId,
                                   UserId = user.Id,
                                   UserName = user.UserName
                               }).ToList();

            ViewBag.JoinRequests = joinRequests;

            return View(channel);
        }



        [HttpPost]
        public IActionResult AddRole(int ChannelId, string UserId)
        {
            var userChannel = db.UserChannels
                                .FirstOrDefault(uc => uc.ChannelId == ChannelId && uc.UserId == UserId);

            if (userChannel == null)
            {
                TempData["message"] = "Utilizatorul nu este asociat acestui canal.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("EditChannel", new { id = ChannelId });
            }

            // Actualizează rolul utilizatorului
            userChannel.Roles = "Moderator"; // Sau alt rol dorit
            db.SaveChanges();

            TempData["message"] = $"Rolul utilizatorului {UserId} a fost actualizat cu succes.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("EditChannel", new { id = ChannelId });
        }


        [HttpPost]
        public IActionResult RevokeRole(int ChannelId, string UserId)
        {
            var userChannel = db.UserChannels
                                .FirstOrDefault(uc => uc.ChannelId == ChannelId && uc.UserId == UserId);

            if (userChannel == null)
            {
                TempData["message"] = "Utilizatorul nu este asociat acestui canal.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("EditChannel", new { id = ChannelId });
            }

            // Actualizează rolul utilizatorului la "Member"
            userChannel.Roles = "Member";
            db.SaveChanges();

            TempData["message"] = $"Rolul utilizatorului {UserId} a fost actualizat la 'Member'.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("EditChannel", new { id = ChannelId });
        }


        [HttpPost]
        public IActionResult RemoveParticipant(int ChannelId, string UserId)
        {
            var userChannel = db.UserChannels.FirstOrDefault(uc => uc.ChannelId == ChannelId && uc.UserId == UserId);

            if (userChannel == null)
            {
                TempData["message"] = "Participantul nu a fost găsit.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("EditChannel", new { id = ChannelId });
            }

            db.UserChannels.Remove(userChannel);
            db.SaveChanges();

            return RedirectToAction("EditChannel", new { id = ChannelId });
        }
        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult EditChannel(int id, ChatChannel updatedChannel)
        {
            if (ModelState.IsValid)
            {
                
                var channel = db.Channels.FirstOrDefault(c => c.Id == id);
                if (channel == null)
                {
                    return NotFound(); 
                }

                channel.ChannelName = updatedChannel.ChannelName;
                channel.ChannelDescription = updatedChannel.ChannelDescription;
                channel.CategoryId = updatedChannel.CategoryId;

                db.SaveChanges();

                TempData["message"] = "Canalul a fost actualizat cu succes.";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Index");
            }

            return View(updatedChannel);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult DeleteChannel(int id)
        {
            
            var channel = db.Channels.FirstOrDefault(c => c.Id == id);

            if (channel == null)
            {
                TempData["message"] = "Canalul nu a fost găsit.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("MyChannels");
            }

            var messages = db.Messages.Where(m => m.ChannelId == id).ToList();

            if (messages.Any())
            {
                db.Messages.RemoveRange(messages);
            }

            var userChannels = db.UserChannels.Where(uc => uc.ChannelId == id).ToList();

            
            if (userChannels.Any())
            {
                db.UserChannels.RemoveRange(userChannels);
            }

            db.Channels.Remove(channel);

            db.SaveChanges();

            TempData["message"] = "Canalul și datele asociate au fost șterse cu succes.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("MyChannels");
        }



        // Se afiseaza un singur articol in functie de id-ul sau 
        // impreuna cu categoria din care face parte
        // In plus sunt preluate si toate comentariile asociate unui articol
        // Se afiseaza si userul care a postat articolul respectiv
        // [HttpGet] se executa implicit implicit
        //[Authorize(Roles = "User,Moderator,Admin")]
        //public IActionResult Show(int id)
        //{
        //    ChatChannel channel = db.Channels.Include("Category").Include("User")
        //                          .Where(ch => ch.Id == id)
        //                          .FirstOrDefault();

        //    var messages = from message in db.Messages
        //                   where message.ChannelId == channel.Id
        //                   orderby message.Date
        //                   select message;
        //    ViewBag.Messages = messages;


        //    var messagesWithUsers = (from mess in db.Messages
        //                             join user in db.Users on mess.UserId equals user.Id
        //                             where mess.ChannelId == channel.Id
        //                             select new
        //                             {
        //                                 mess.Content,
        //                                 mess.UserId,
        //                                 user.UserName
        //                             }).ToList();

        //    // Pass the combined data to ViewBag
        //    ViewBag.MessagesWithUsers = messagesWithUsers;


        //    SetAccessRights();

        //    if (TempData.ContainsKey("message"))
        //    {
        //        ViewBag.Message = TempData["message"];
        //    }

        //    if (TempData.ContainsKey("messageType"))
        //    {
        //        ViewBag.Alert = TempData["messageType"];
        //    }

        //    //return View(channel);
        //    return PartialView("Channellnfo", channel);
        //}


        //[HttpPost]
        //[Authorize(Roles = "User,Editor,Admin")]
        //public IActionResult Show([FromForm] Message message)
        //{
        //    message.Date = DateTime.Now;

        //    // preluam Id-ul utilizatorului care posteaza comentariul
        //    message.UserId = _userManager.GetUserId(User);
        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var modelState in ModelState.Values)
        //        {
        //            foreach (var error in modelState.Errors)
        //            {
        //                Console.WriteLine(error.ErrorMessage); // Log or debug this line
        //            }
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        message.Type = "1";
        //        db.Messages.Add(message);
        //        db.SaveChanges();

        //        ChatChannel channel = db.Channels.Include("Category")
        //                                 .Where(channel => channel.Id == message.ChannelId)
        //                                 .First();


        //        //
        //        //return Redirect("/ChatChannels/Show/"+message.ChannelId.ToString());
        //        var messages = from mess in db.Messages
        //                       where mess.ChannelId == channel.Id
        //                       orderby mess.Date
        //                       select mess;
        //        ViewBag.Messages = messages;
        //        var channels = db.Channels.OrderByDescending(a => a.ChannelName);
        //        ViewBag.Channels = channels;
        //        //return PartialView("Channellnfo", channel);
        //        return View("Index");
        //    }
        //    else
        //    {
        //        var channel = db.Channels.Find(message.ChannelId); // Ex

        //        SetAccessRights();

        //        var messages = from mess in db.Messages
        //                       where mess.ChannelId == channel.Id
        //                       orderby mess.Date
        //                       select mess;
        //        ViewBag.Messages = messages;


        //        var channels = db.Channels.OrderByDescending(a => a.ChannelName);
        //        ViewBag.Channels = channels;
        //        ViewBag.NewID = channel.Id;
        //        //return View("Index");
        //        return View();
        //    }
        //}

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            var channels = db.Channels.OrderByDescending(a => a.ChannelName);

            var userId = _userManager.GetUserId(User);

            // Verifică dacă utilizatorul este participant la canal
            var isParticipant = db.UserChannels.Any(uc => uc.ChannelId == id && uc.UserId == userId);
            ViewBag.IsParticipant = isParticipant;
            if (!isParticipant)
            {
                TempData["message"] = "Nu aveți permisiunea de a accesa acest canal.";
                TempData["messageType"] = "alert-danger";
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // MOTOR DE CAUTARE

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in articol (Title si Content)

                List<int> channelsIds = db.Channels.Where
                                        (
                                         at => at.ChannelName.Contains(search)
                                        ).Select(a => a.Id).ToList();




                // Lista articolelor care contin cuvantul cautat
                // fie in articol -> Title si Content
                // fie in comentarii -> Content
                //articles = db.Channels.Where(article => mergedIds.Contains(article.Id))

                channels = db.Channels.Where(channels => channelsIds.Contains(channels.Id))
                    .OrderByDescending(a => a.ChannelName);

            }

            ViewBag.SearchString = search;

            // AFISARE PAGINATA

            // Alegem sa afisam 3 articole pe pagina
            int _perPage = 3;

            // Fiind un numar variabil de articole, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = channels.Count();

            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedArticles = channels.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Channels = paginatedArticles;

            // DACA AVEM AFISAREA PAGINATA IMPREUNA CU SEARCH

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/ChatChannels/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/ChatChannels/Index/?page";
            }

            ////////
            ChatChannel channel = db.Channels.Include("Category").Include("User")
                                  .Where(ch => ch.Id == id)
                                  .FirstOrDefault();

            if (channel == null)
            {
                return NotFound();
            }

            var messagesWithUsers = db.Messages
            .Where(m => m.ChannelId == id)
            .Select(m => new
            {
                MessageId = m.Id,
                UserName = m.User.UserName,
                UserId = m.UserId,
                Content = m.Content,
                Date = m.Date
            })
            .ToList();

            
            ViewBag.MessagesWithUsers = messagesWithUsers;
            ViewBag.UserCurent = User.Identity.Name; 


            ViewBag.MessagesWithUsers = messagesWithUsers;

            var participants = (from uc in db.UserChannels
                                join user in db.Users
                                on uc.UserId equals user.Id
                                where uc.ChannelId == id
                                select new
                                {
                                    UserName = user.UserName,
                                    Role = uc.Roles
                                }).ToList();

            ViewBag.Participants = participants;

            SetAccessRights();

            bool esteAdmin = db.UserChannels.Any(uc => uc.ChannelId == id && uc.UserId == userId && uc.Roles == "Admin");

            bool esteModerator = db.UserChannels.Any(uc => uc.ChannelId == id && uc.UserId == userId && uc.Roles == "Moderator");

            ViewBag.EsteAdmin = esteAdmin;
            ViewBag.EsteModerator = esteModerator;


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            if (TempData.ContainsKey("messageType"))
            {
                ViewBag.Alert = TempData["messageType"];
            }

            // Return partial view for channel information
            //return PartialView("Channellnfo", channel);
            return View(channel);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Message message)
        {
            message.Type = "1";
            message.Date = DateTime.Now;

            message.UserId = _userManager.GetUserId(User);
            if (message.Content == null || message.Content.Length > 500)
            {
                TempData["message"] = "Content is too large or empty.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", new { id = message.ChannelId });
            }
           
            message.Content = ConvertToEmbeddedMedia(message.Content);

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return View("Error");
            }

            db.Messages.Add(message);
            db.SaveChanges();

            ChatChannel channel = db.Channels.Include("Category")
                                     .Where(channel => channel.Id == message.ChannelId)
                                     .First();


            //
            //return Redirect("/ChatChannels/Show/"+message.ChannelId.ToString());
            var messages = from mess in db.Messages
                           where mess.ChannelId == channel.Id
                           orderby mess.Date
                           select mess;
            ViewBag.Messages = messages;
            var channels = db.Channels.OrderByDescending(a => a.ChannelName);
            ViewBag.Channels = channels;
            //return PartialView("Channellnfo", channel);
            //return View("Index");
            //return View(channel);
            return RedirectToAction("Show", new { id = channel.Id });
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult CreateRequest(int ChannelId)
        {
            // Obține UserId-ul utilizatorului curent
            var userId = _userManager.GetUserId(User);

            var existingRequest = db.JoinRequests
                                    .FirstOrDefault(jr => jr.ChannelId == ChannelId && jr.UserId == userId);

            if (existingRequest != null)
            {
                TempData["message"] = "You have already sent a request to join this channel.";
                TempData["messageType"] = "alert-warning";
                return RedirectToAction("Index", "Home"); 
            }

            var joinRequest = new JoinRequest
            {
                ChannelId = ChannelId,
                UserId = userId
            };

            db.JoinRequests.Add(joinRequest);
            db.SaveChanges();

            TempData["message"] = "Your request to join this channel has been sent.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index", "Home"); 
        }

        [HttpPost]
        public IActionResult AcceptJoinRequest(int RequestId)
        {
            var joinRequest = db.JoinRequests.FirstOrDefault(jr => jr.RequestId == RequestId);
            if (joinRequest == null)
            {
                TempData["message"] = "Cererea de aderare nu a fost găsită.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("EditChannel", new { id = joinRequest?.ChannelId });
            }

            var existingParticipant = db.UserChannels
                .FirstOrDefault(uc => uc.ChannelId == joinRequest.ChannelId && uc.UserId == joinRequest.UserId);

            if (existingParticipant != null)
            {
                TempData["message"] = "Utilizatorul este deja participant în canal.";
                TempData["messageType"] = "alert-warning";
                return RedirectToAction("EditChannel", new { id = joinRequest.ChannelId });
            }

            var newUserChannel = new UserChannel
            {
                ChannelId = joinRequest.ChannelId,
                UserId = joinRequest.UserId,
                Roles = "Member" 
            };

            db.UserChannels.Add(newUserChannel);

            db.JoinRequests.Remove(joinRequest);

            db.SaveChanges();

            TempData["message"] = "Utilizatorul a fost acceptat și adăugat în canal.";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("EditChannel", new { id = joinRequest.ChannelId });
        }

        [HttpPost]
        public IActionResult RejectJoinRequest(int RequestId)
        {
            var joinRequest = db.JoinRequests.FirstOrDefault(jr => jr.RequestId == RequestId);
            
            db.JoinRequests.Remove(joinRequest);

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public string ConvertToEmbeddedMedia(string? content)
        {
            content = Regex.Replace(content, @"https?://(?:www\.)?(?:youtube\.com/watch\?v=|youtu\.be/)([\w\-]+)",
                "<iframe width='560' height='315' src='https://www.youtube.com/embed/$1' frameborder='0' allowfullscreen></iframe>", RegexOptions.IgnoreCase);

            content = Regex.Replace(content, @"https?://(?:www\.)?vimeo\.com/(\d+)", 
                "<iframe width='560' height='315' src='https://player.vimeo.com/video/$1' frameborder='0' allowfullscreen></iframe>", RegexOptions.IgnoreCase);

            content = Regex.Replace(content, @"(https?://[^\s]+(\.(jpg|jpeg|png|gif|bmp|svg|webp)))", 
                "<img src='$1' style='max-width: 100%; max-height: 300px;' />", RegexOptions.IgnoreCase);

            return content;
        }



        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Moderator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

    }
}
