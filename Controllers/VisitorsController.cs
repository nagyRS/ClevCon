using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClevConWebUI.Models;
using PagedList;

namespace ClevConWebUI.Controllers
{
    public class VisitorsController : Controller
    {
        private CCDbEntities1 db = new CCDbEntities1();
   


        // GET: Visitors
        public ActionResult Index(string searchString, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParam = string.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewBag.NameSortParam = sortOrder == "name_asc" ? "name_desc" : "name_asc";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var visitors = from v in db.Visitors.Include(v => v.Host)
                           select v;


            if (!string.IsNullOrEmpty(searchString))
            {
                visitors = visitors.Where(v => v.Name.Contains(searchString) || v.Host.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    visitors = visitors.OrderByDescending(v => v.Name);
                    break;

                case "name_asc":
                    visitors = visitors.OrderBy(v => v.Name);
                    break;

                case "date_asc":
                    visitors = visitors.OrderBy(v => v.Expected);
                    break;

                default:
                    visitors = visitors.OrderByDescending(v => v.Expected);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(visitors.ToPagedList(pageNumber, pageSize));
        }



        public ActionResult Checkin(int id)
        {
            Visitor visitor = db.Visitors.Find(id);

            DateTime serverTime = DateTime.Now;
            DateTime localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "GMT Standard Time");             // same for Daylight Saving Time, but not in different timezones

            visitor.Arrived = localTime;

            if (ModelState.IsValid)
            {
                db.Entry(visitor).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }


        public ActionResult Checkout(int id)
        {
            Visitor visitor = db.Visitors.Find(id);

            DateTime serverTime = DateTime.Now;
            DateTime localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "GMT Standard Time");   // same for Daylight Saving Time, but not in different timezones

            visitor.Departed = localTime;

            if (ModelState.IsValid)
            {
                db.Entry(visitor).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        // GET: Visitors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visitor visitor = db.Visitors.Find(id);
            if (visitor == null)
            {
                return HttpNotFound();
            }
            return View(visitor);
        }

        // GET: Visitors/Create
        public ActionResult Create()
        {
            ViewBag.HostId = new SelectList(db.Hosts, "HostId", "Name");
            return View();
        }

        // POST: Visitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VisitorId,Name,Arrived,Departed,Notes,HostId,Expected")] Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                db.Visitors.Add(visitor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HostId = new SelectList(db.Hosts, "HostId", "Name", visitor.HostId);
            return View(visitor);
        }

        // GET: Visitors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visitor visitor = db.Visitors.Find(id);
            if (visitor == null)
            {
                return HttpNotFound();
            }
            ViewBag.HostId = new SelectList(db.Hosts, "HostId", "Name", visitor.HostId);
            return View(visitor);
        }

        // POST: Visitors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VisitorId,Arrived,Departed,Name,Notes,HostId,Expected")] Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(visitor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HostId = new SelectList(db.Hosts, "HostId", "Name", visitor.HostId);
            return View(visitor);
        }

        // GET: Visitors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visitor visitor = db.Visitors.Find(id);
            if (visitor == null)
            {
                return HttpNotFound();
            }
            return View(visitor);
        }

        // POST: Visitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Visitor visitor = db.Visitors.Find(id);
            db.Visitors.Remove(visitor);
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
