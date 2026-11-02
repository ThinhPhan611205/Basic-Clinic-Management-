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
    // Thuốc - Chỉ admin mới được quản lý danh mục thuốc
    [CheckRole(AllowedRoles = "admin")]
    public class Thuoc_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // GET: Thuoc_65133370
        public ActionResult Index()
        {
            return View(db.thuocs.ToList());
        }

        // Tim kiem thuoc cho Admin
        public ActionResult TimKiemThuoc(string maThuoc, string tenThuoc)
        {
            var query = db.thuocs.AsQueryable();

            if (!string.IsNullOrEmpty(maThuoc))
            {
                query = query.Where(t => t.maThuoc.Contains(maThuoc));
            }

            if (!string.IsNullOrEmpty(tenThuoc))
            {
                query = query.Where(t => t.tenThuoc.Contains(tenThuoc));
            }

            ViewBag.maThuoc = maThuoc;
            ViewBag.tenThuoc = tenThuoc;

            return View("Index", query.ToList());
        }

        // GET: Thuoc_65133370/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            thuoc thuoc = db.thuocs.Find(id);
            if (thuoc == null)
            {
                return HttpNotFound();
            }
            return View(thuoc);
        }

        // GET: Thuoc_65133370/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Thuoc_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maThuoc,tenThuoc,donGia")] thuoc thuoc)
        {
            if (ModelState.IsValid)
            {
                db.thuocs.Add(thuoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thuoc);
        }

        // GET: Thuoc_65133370/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            thuoc thuoc = db.thuocs.Find(id);
            if (thuoc == null)
            {
                return HttpNotFound();
            }
            return View(thuoc);
        }

        // POST: Thuoc_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maThuoc,tenThuoc,donGia")] thuoc thuoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thuoc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuoc);
        }

        // GET: Thuoc_65133370/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            thuoc thuoc = db.thuocs.Find(id);
            if (thuoc == null)
            {
                return HttpNotFound();
            }
            return View(thuoc);
        }

        // POST: Thuoc_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            thuoc thuoc = db.thuocs.Find(id);
            db.thuocs.Remove(thuoc);
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
