using DAWSlack.Data;
using DAWSlack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

                db.Channels.Add(channel);
                db.SaveChanges();
                TempData["message"] = "Canalul a fost creat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
                return View();
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

        // Se afiseaza un singur articol in functie de id-ul sau 
        // impreuna cu categoria din care face parte
        // In plus sunt preluate si toate comentariile asociate unui articol
        // Se afiseaza si userul care a postat articolul respectiv
        // [HttpGet] se executa implicit implicit
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            ChatChannel channel = db.Channels
                              .Where(art => art.Id == id)
                              .First();

            // Adaugam bookmark-urile utilizatorului pentru dropdown

            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(channel);
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
