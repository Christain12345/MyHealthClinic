using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using MyHealthClinic.Models;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace MyHealthClinic.Controllers
{
    public class AvailableTimesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        // GET: AvailableTimes
        public ActionResult Index()
        {
            return View(db.AvailableTimes.Include(at => at.GeneralPractioner).ToList());
        }

        // GET: AvailableTimes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AvailableTime availableTime = db.AvailableTimes.Find(id);
            if (availableTime == null)
            {
                return HttpNotFound();
            }
            return View(availableTime);
        }

        // GET: AvailableTimes/Create
        public ActionResult Create()
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var gpRole = rm.FindByName("General Practitioner");
            var gpList = UserManager.Users
                .Where(u => u.Roles.Any(r => r.RoleId == gpRole.Id))
                .Select(u => new ProfileViewModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    About = u.About,
                }).ToList();
            ViewBag.GeneralPractitioner = new SelectList(gpList, "Id", "FirstName");
            return View();
        }

        // POST: AvailableTimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StartTime,EndTime")] AvailableTime availableTime)
        {
            if (ModelState.IsValid)
            {
                db.AvailableTimes.Add(availableTime);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(availableTime);
        }

        // GET: AvailableTimes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AvailableTime availableTime = db.AvailableTimes.Find(id);
            if (availableTime == null)
            {
                return HttpNotFound();
            }
            return View(availableTime);
        }

        // POST: AvailableTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StartTime,EndTime")] AvailableTime availableTime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(availableTime).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(availableTime);
        }

        // GET: AvailableTimes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AvailableTime availableTime = db.AvailableTimes.Find(id);
            if (availableTime == null)
            {
                return HttpNotFound();
            }
            return View(availableTime);
        }

        // POST: AvailableTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AvailableTime availableTime = db.AvailableTimes.Find(id);
            db.AvailableTimes.Remove(availableTime);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
