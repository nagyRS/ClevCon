using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClevConWebUI.Models;

namespace ClevConWebUI.Controllers
{
    public class HostsController : Controller
    {
        private CCDbEntities1 db = new CCDbEntities1();

        // GET: Hosts
        public ActionResult Index(string searchString)
        {

            var hosts = from h in db.Hosts select h;

            if (!String.IsNullOrEmpty(searchString))
            {
               hosts = hosts.Where(h => h.Name.Contains(searchString));
            }

            return View(hosts.ToList());
        }

        // GET: Hosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // GET: Hosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HostId,Name,Company,Email,Phone")] Host host)
        {
            if (ModelState.IsValid)
            {
                db.Hosts.Add(host);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(host);
        }

        // GET: Hosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // POST: Hosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HostId,Name,Company,Email,Phone")] Host host)
        {
            if (ModelState.IsValid)
            {
                db.Entry(host).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(host);
        }

        // GET: Hosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // POST: Hosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            db.Visitors.Where(v => v.HostId == id)
               .ToList().ForEach(v => db.Visitors.Remove(v));

            Host host = db.Hosts.Find(id);
            db.Hosts.Remove(host);
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
