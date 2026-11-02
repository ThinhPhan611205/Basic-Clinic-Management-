using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Project_65133370.Models;

namespace Project_65133370.Controllers
{
    // Bệnh án 
    [CheckRole(AllowedRoles = "bacSi")]
    public class BenhAn_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // GET: BenhAn_65133370
        public ActionResult Index()
        {
            var benhAns = db.benhAns.Include(b => b.bacSi).Include(b => b.tenBenh).Include(b => b.donThuoc).Include(b => b.lichKham).Include(b => b.benhNhan).Include(b => b.trangThaiLichHen);
            return View(benhAns.ToList());
        }

        // GET: BenhAn_65133370/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhAn benhAn = db.benhAns.Find(id);
            if (benhAn == null)
            {
                return HttpNotFound();
            }
            return View(benhAn);
        }

        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult DetailsBN(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhAn benhAn = db.benhAns.Find(id);
            if (benhAn == null)
            {
                return HttpNotFound();
            }
            return View(benhAn);
        }
        private string TangTuDong()
        {
            string maxId = db.benhAns
                             .Select(b => b.maBenhAn)
                             .OrderByDescending(id => id)
                             .FirstOrDefault();
            int number = int.Parse(maxId.Substring(2)) + 1;
            return "BA" + number.ToString("D2");
        }

        // GET: BenhAn_65133370/Create
        public ActionResult Create(string maLich = null)
        {
            benhAn ba = new benhAn();
            if (!string.IsNullOrEmpty(maLich))
            {
                var lichKham = db.lichKhams.Find(maLich);
                if (lichKham != null)
                {
                    // Tự động điền dữ liệu
                    ba.maLich = maLich;
                    ba.maSo = lichKham.maSo; // Mã BN
                    ba.ID = lichKham.ID;     // Mã BS
                    ba.ngayKham = DateTime.Now;

                    // Tạo ViewBag để hiển thị tên (thay vì mã số) bên View
                    ViewBag.TenBenhNhan = lichKham.benhNhan.hoBn + " " + lichKham.benhNhan.tenBn;
                    ViewBag.TenBacSi = lichKham.bacSi.hoBs + " " + lichKham.bacSi.tenBs;
                }
            }
            else
            {
                //tạo thủ công không qua lịch 
                ba.ngayKham = DateTime.Now;
            }
            ViewBag.ID = new SelectList(db.bacSis, "ID", "hoBs");
            ViewBag.maBenh = new SelectList(db.tenBenhs, "maBenh", "tenBenh1");
            ViewBag.maDonThuoc = new SelectList(db.donThuocs, "maDonThuoc", "maDonThuoc");
            ViewBag.maLich = new SelectList(db.lichKhams, "maLich", "maSo");
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn");
            ViewBag.trangThaiChuaTri = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai");
            return View(ba);
        }

        // POST: BenhAn_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maBenhAn,maSo,ID,ngayKham,trieuChung,chuanDoan,maLich,maBenh,trangThaiChuaTri,maDonThuoc")] benhAn benhAn)
        {
            if (ModelState.IsValid)
            {
                benhAn.maBenhAn = TangTuDong();
                db.benhAns.Add(benhAn);
                db.SaveChanges();
                return RedirectToAction("Index", "LichKham_65133370");
            }

            ViewBag.ID = new SelectList(db.bacSis, "ID", "hoBs", benhAn.ID);
            ViewBag.maBenh = new SelectList(db.tenBenhs, "maBenh", "tenBenh1", benhAn.maBenh);
            ViewBag.maDonThuoc = new SelectList(db.donThuocs, "maDonThuoc", "maDonThuoc", benhAn.maDonThuoc);
            ViewBag.maLich = new SelectList(db.lichKhams, "maLich", "maSo", benhAn.maLich);
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn", benhAn.maSo);
            ViewBag.trangThaiChuaTri = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai", benhAn.trangThaiChuaTri);
            if (!string.IsNullOrEmpty(benhAn.maLich))
            {
                var lich = db.lichKhams.Find(benhAn.maLich);
                if (lich != null)
                {
                    ViewBag.TenBenhNhan = lich.benhNhan.hoBn + " " + lich.benhNhan.tenBn;
                    ViewBag.TenBacSi = lich.bacSi.hoBs + " " + lich.bacSi.tenBs;
                }
            }
            return View(benhAn);
        }

        // GET: BenhAn_65133370/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhAn benhAn = db.benhAns.Find(id);
            if (benhAn == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.bacSis, "ID", "hoBs", benhAn.ID);
            ViewBag.maBenh = new SelectList(db.tenBenhs, "maBenh", "tenBenh1", benhAn.maBenh);
            ViewBag.maDonThuoc = new SelectList(db.donThuocs, "maDonThuoc", "maDonThuoc", benhAn.maDonThuoc);
            ViewBag.maLich = new SelectList(db.lichKhams, "maLich", "maSo", benhAn.maLich);
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn", benhAn.maSo);
            ViewBag.trangThaiChuaTri = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai", benhAn.trangThaiChuaTri);
            return View(benhAn);
        }

        // POST: BenhAn_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maBenhAn,maSo,ID,ngayKham,trieuChung,chuanDoan,maLich,maBenh,trangThaiChuaTri,maDonThuoc")] benhAn benhAn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(benhAn).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "LichKham_65133370");
            }
            ViewBag.ID = new SelectList(db.bacSis, "ID", "hoBs", benhAn.ID);
            ViewBag.maBenh = new SelectList(db.tenBenhs, "maBenh", "tenBenh1", benhAn.maBenh);
            ViewBag.maDonThuoc = new SelectList(db.donThuocs, "maDonThuoc", "maDonThuoc", benhAn.maDonThuoc);
            ViewBag.maLich = new SelectList(db.lichKhams, "maLich", "maSo", benhAn.maLich);
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn", benhAn.maSo);
            ViewBag.trangThaiChuaTri = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai", benhAn.trangThaiChuaTri);
            return View(benhAn);
        }

        // GET: BenhAn_65133370/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhAn benhAn = db.benhAns.Find(id);
            if (benhAn == null)
            {
                return HttpNotFound();
            }
            return View(benhAn);
        }

        // POST: BenhAn_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            benhAn benhAn = db.benhAns.Find(id);
            db.benhAns.Remove(benhAn);
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
