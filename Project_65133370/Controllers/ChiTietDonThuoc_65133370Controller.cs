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
    [CheckRole(AllowedRoles = "bacSi")]
    public class ChiTietDonThuoc_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // GET: ChiTietDonThuoc_65133370
        public ActionResult Index()
        {
            var chiTietDonThuocs = db.chiTietDonThuocs.Include(c => c.donThuoc).Include(c => c.thuoc);
            return View(chiTietDonThuocs.ToList());
        }

        public ActionResult IndexEdit(string id)
        {
            var chiTietDonThuocs = db.chiTietDonThuocs.Include(c => c.donThuoc).Include(c => c.thuoc).Where(x => x.maDonThuoc == id);
            ViewBag.MaDonThuoc = id;

            return View(chiTietDonThuocs.ToList());
        }

        // GET: ChiTietDonThuoc_65133370/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chiTietDonThuoc chiTietDonThuoc = db.chiTietDonThuocs.Find(id);
            if (chiTietDonThuoc == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonThuoc);
        }

        // GET: ChiTietDonThuoc_65133370/Create/BA09
        // id = maBenhAn (tu URL /Create/BA09)
        public ActionResult Create(string id)
        {
            var model = new chiTietDonThuoc();

            if (!string.IsNullOrEmpty(id))
            {
                // id la maBenhAn, tim benh an de lay maDonThuoc
                var benhAn = db.benhAns.Find(id);
                if (benhAn != null)
                {
                    ViewBag.MaBenhAn = benhAn.maBenhAn;

                    if (!string.IsNullOrEmpty(benhAn.maDonThuoc))
                    {
                        model.maDonThuoc = benhAn.maDonThuoc;
                        ViewBag.MaDonThuocHienThi = benhAn.maDonThuoc;
                    }
                }
            }

            ViewBag.maThuoc = new SelectList(db.thuocs, "maThuoc", "tenThuoc");
            return View(model);
        }

        // POST: ChiTietDonThuoc_65133370/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maDonThuoc,maThuoc,soLuong,lieuDung")] chiTietDonThuoc chiTietDonThuoc, string maBenhAnLuu)
        {
            if (ModelState.IsValid)
            {
                db.chiTietDonThuocs.Add(chiTietDonThuoc);
                db.SaveChanges();

                // Redirect ve trang chi tiet benh an
                if (!string.IsNullOrEmpty(maBenhAnLuu))
                {
                    return RedirectToAction("Details", "BenhAn_65133370", new { id = maBenhAnLuu });
                }

                // Neu khong co maBenhAn, tim tu maDonThuoc
                var benhAn = db.benhAns.FirstOrDefault(b => b.maDonThuoc == chiTietDonThuoc.maDonThuoc);
                if (benhAn != null)
                {
                    return RedirectToAction("Details", "BenhAn_65133370", new { id = benhAn.maBenhAn });
                }

                return RedirectToAction("Index");
            }

            ViewBag.maThuoc = new SelectList(db.thuocs, "maThuoc", "tenThuoc", chiTietDonThuoc.maThuoc);
            ViewBag.MaDonThuocHienThi = chiTietDonThuoc.maDonThuoc;
            ViewBag.MaBenhAn = maBenhAnLuu;

            return View(chiTietDonThuoc);
        }

        // GET: ChiTietDonThuoc_65133370/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chiTietDonThuoc chiTietDonThuoc = db.chiTietDonThuocs.Find(id);
            if (chiTietDonThuoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.maDonThuoc = new SelectList(db.donThuocs, "maDonThuoc", "maDonThuoc", chiTietDonThuoc.maDonThuoc);
            ViewBag.maThuoc = new SelectList(db.thuocs, "maThuoc", "tenThuoc", chiTietDonThuoc.maThuoc);
            return View(chiTietDonThuoc);
        }

        // POST: ChiTietDonThuoc_65133370/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maDonThuoc,maThuoc,soLuong,lieuDung")] chiTietDonThuoc chiTietDonThuoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietDonThuoc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maDonThuoc = new SelectList(db.donThuocs, "maDonThuoc", "maDonThuoc", chiTietDonThuoc.maDonThuoc);
            ViewBag.maThuoc = new SelectList(db.thuocs, "maThuoc", "tenThuoc", chiTietDonThuoc.maThuoc);
            return View(chiTietDonThuoc);
        }

        // GET: ChiTietDonThuoc_65133370/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chiTietDonThuoc chiTietDonThuoc = db.chiTietDonThuocs.Find(id);
            if (chiTietDonThuoc == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonThuoc);
        }

        // POST: ChiTietDonThuoc_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            chiTietDonThuoc chiTietDonThuoc = db.chiTietDonThuocs.Find(id);
            db.chiTietDonThuocs.Remove(chiTietDonThuoc);
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
