using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_65133370.Models;

namespace Project_65133370.Controllers
{
    // Danh mục bệnh
    [CheckRole(AllowedRoles = "admin")]
    public class Benh_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // GET: Benh_65133370
        public ActionResult Index()
        {
            return View(db.tenBenhs.ToList());
        }

        // GET: Benh_65133370/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tenBenh tenBenh = db.tenBenhs.Find(id);
            if (tenBenh == null)
            {
                return HttpNotFound();
            }
            return View(tenBenh);
        }

        // GET: Benh_65133370/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Benh_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maBenh,tenBenh1")] tenBenh tenBenh)
        {
            if (ModelState.IsValid)
            {
                db.tenBenhs.Add(tenBenh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tenBenh);
        }

        // GET: Benh_65133370/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tenBenh tenBenh = db.tenBenhs.Find(id);
            if (tenBenh == null)
            {
                return HttpNotFound();
            }
            return View(tenBenh);
        }

        // POST: Benh_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maBenh,tenBenh1")] tenBenh tenBenh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tenBenh).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tenBenh);
        }

        // GET: Benh_65133370/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tenBenh tenBenh = db.tenBenhs.Find(id);
            if (tenBenh == null)
            {
                return HttpNotFound();
            }
            return View(tenBenh);
        }

        // POST: Benh_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tenBenh tenBenh = db.tenBenhs.Find(id);
            db.tenBenhs.Remove(tenBenh);
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
