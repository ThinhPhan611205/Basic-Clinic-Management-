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
    // Đơn thuốc - Chỉ bác sĩ mới được quản lý (dùng _LayoutBacSi)
    [CheckRole(AllowedRoles = "bacSi")]
    public class DonThuoc_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // GET: DonThuoc_65133370
        public ActionResult Index()
        {
            return View(db.donThuocs.ToList());
        }

        private string TaoMaDonThuocTuDong()
        {
            // Get all prescription codes and find the max numeric value
            var allMaDonThuoc = db.donThuocs
                                  .Select(d => d.maDonThuoc)
                                  .ToList();

            if (allMaDonThuoc == null || allMaDonThuoc.Count == 0)
            {
                return "DT001";
            }

            // Extract numeric part and find max
            int maxSo = 0;
            foreach (var ma in allMaDonThuoc)
            {
                if (!string.IsNullOrEmpty(ma) && ma.StartsWith("DT") && ma.Length > 2)
                {
                    int so;
                    if (int.TryParse(ma.Substring(2), out so))
                    {
                        if (so > maxSo)
                        {
                            maxSo = so;
                        }
                    }
                }
            }

            maxSo++;
            return "DT" + maxSo.ToString("D3");
        }

        public ActionResult IndexTaoDon(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var benhAn = db.benhAns.Find(id);
            if (benhAn == null)
            {
                return HttpNotFound("Không tìm thấy mã bệnh án này.");
            }
            ViewBag.MaBenhAnLuu = id;

            donThuoc model;
            if (string.IsNullOrEmpty(benhAn.maDonThuoc))
            {
                model = new donThuoc();

                model.maDonThuoc = TaoMaDonThuocTuDong();
                model.ngayKeDon = DateTime.Now;

                ViewBag.TrangThai = "Tạo mới";
            }
            else
            {
                model = db.donThuocs.Find(benhAn.maDonThuoc);

                if (model == null)
                {
                    model = new donThuoc();
                    model.maDonThuoc = benhAn.maDonThuoc;
                    model.ngayKeDon = DateTime.Now;
                }

                ViewBag.TrangThai = "Cập nhật";
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IndexTaoDon(donThuoc model, string maBenhAnLuu)
        {
            if (ModelState.IsValid)
            {
                var existingDT = db.donThuocs.Find(model.maDonThuoc);

                if (existingDT != null)
                {
                    // Update existing prescription
                    existingDT.ngayKeDon = model.ngayKeDon;
                    db.Entry(existingDT).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    // Create new prescription
                    db.donThuocs.Add(model);
                }

                // Always update benhAn.maDonThuoc if maBenhAnLuu is provided
                if (!string.IsNullOrEmpty(maBenhAnLuu))
                {
                    var benhAn = db.benhAns.Find(maBenhAnLuu);
                    if (benhAn != null && benhAn.maDonThuoc != model.maDonThuoc)
                    {
                        benhAn.maDonThuoc = model.maDonThuoc;
                        db.Entry(benhAn).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                db.SaveChanges();

                if (!string.IsNullOrEmpty(maBenhAnLuu))
                {
                    return RedirectToAction("Details", "BenhAn_65133370", new { id = maBenhAnLuu });
                }
                else
                {
                    return RedirectToAction("Index", "BenhAn_65133370");
                }
            }
            ViewBag.MaBenhAnLuu = maBenhAnLuu;
            ViewBag.TrangThai = "Kiem tra lai";
            return View(model);
        }

        // GET: DonThuoc_65133370/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            donThuoc donThuoc = db.donThuocs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }
            return View(donThuoc);
        }

        // GET: DonThuoc_65133370/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DonThuoc_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maDonThuoc,ngayKeDon")] donThuoc donThuoc)
        {
            if (ModelState.IsValid)
            {
                db.donThuocs.Add(donThuoc);
                db.SaveChanges();
                return RedirectToAction("IndexTaoDon");
            }

            return View(donThuoc);
        }

        // GET: DonThuoc_65133370/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            donThuoc donThuoc = db.donThuocs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }
            return View(donThuoc);
        }

        // POST: DonThuoc_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maDonThuoc,ngayKeDon")] donThuoc donThuoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donThuoc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(donThuoc);
        }

        // GET: DonThuoc_65133370/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            donThuoc donThuoc = db.donThuocs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }
            return View(donThuoc);
        }

        // POST: DonThuoc_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            donThuoc donThuoc = db.donThuocs.Find(id);
            db.donThuocs.Remove(donThuoc);
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
