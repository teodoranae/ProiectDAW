
using DAWSlack.Data;
using DAWSlack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DAWSlack.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CategoriesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public ActionResult Index()
        {
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            return View();
        }

        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Category cat)
        {
            try
            {
                db.Categories.Add(cat);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(cat);
            }
        }

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, Category requestCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(requestCategory); 
            }

            try
            {
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return NotFound(); 
                }
                category.CategoryName = requestCategory.CategoryName;
                category.CategoryDescription = requestCategory.CategoryDescription;

                db.SaveChanges();

                TempData["message"] = "Categoria a fost modificată cu succes.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["error"] = "A apărut o eroare. Te rugăm să încerci din nou.";
                return View(requestCategory);
            }
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
