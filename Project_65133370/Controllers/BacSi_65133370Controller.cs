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
    public class BacSi_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // Trang chủ bác sĩ - Dashboard
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult TrangChuBS()
        {
            string maBacSi = Session["maSo"]?.ToString();
            if (string.IsNullOrEmpty(maBacSi))
                return RedirectToAction("DangNhap", "UserLogin_65133370");

            // Thống kê
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Lịch khám hôm nay
            ViewBag.LichKhamHomNay = db.lichKhams
                .Count(l => l.ID == maBacSi && l.ngayKham.HasValue &&
                       DbFunctions.TruncateTime(l.ngayKham) == today);

            // Tổng bệnh nhân đã khám
            ViewBag.TongBenhNhan = db.benhAns.Count(b => b.ID == maBacSi);

            // Bệnh án tháng này
            ViewBag.BenhAnThangNay = db.benhAns
                .Count(b => b.ID == maBacSi && b.ngayKham.HasValue &&
                       b.ngayKham >= startOfMonth);

            // Chờ xác nhận
            ViewBag.ChoXacNhan = db.lichKhams
                .Count(l => l.ID == maBacSi && l.trangThaiKham == 1);

            // Lịch khám sắp tới (5 lịch gần nhất)
            ViewBag.LichKhamSapToi = db.lichKhams
                .Include(l => l.benhNhan)
                .Where(l => l.ID == maBacSi && l.ngayKham >= DateTime.Now)
                .OrderBy(l => l.ngayKham)
                .Take(5)
                .ToList();

            return View();
        }

        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult GioiThieuBS()
        {
            return View();
        }

        [CheckRole(AllowedRoles = "admin")]
        public ActionResult GioiThieuBS_Admin()
        {
            return View();
        }


        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Index()
        {
            var bacSis = db.bacSis.Include(b => b.Role).Include(b => b.chuyenKhoa);
            return View(bacSis.ToList());
        }

        // Tim kiem bac si cho Admin
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult TimKiemBacSi(string maBS, string hoTen, string chuyenKhoa)
        {
            var query = db.bacSis.Include(b => b.Role).Include(b => b.chuyenKhoa).AsQueryable();

            if (!string.IsNullOrEmpty(maBS))
            {
                query = query.Where(b => b.ID.Contains(maBS));
            }

            if (!string.IsNullOrEmpty(hoTen))
            {
                query = query.Where(b => (b.hoBs + " " + b.tenBs).Contains(hoTen));
            }

            if (!string.IsNullOrEmpty(chuyenKhoa))
            {
                query = query.Where(b => b.chuyenKhoa.tenKhoa.Contains(chuyenKhoa));
            }

            ViewBag.maBS = maBS;
            ViewBag.hoTen = hoTen;
            ViewBag.chuyenKhoa = chuyenKhoa;

            return View("Index", query.ToList());
        }

        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult IndexBS_BN()
        {
            var bacSis = db.bacSis.Include(b => b.Role).Include(b => b.chuyenKhoa);
            return View(bacSis.ToList());
        }

        // GET: BacSi_65133370/Details/5 
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bacSi bacSi = db.bacSis.Find(id);
            if (bacSi == null)
            {
                return HttpNotFound();
            }
            return View(bacSi);
        }

        // GET: BacSi_65133370/Create 
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Create()
        {
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR");
            ViewBag.maKhoa = new SelectList(db.chuyenKhoas, "maKhoa", "tenKhoa");
            return View();
        }

        // POST: BacSi_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Create([Bind(Include = "ID,hoBs,tenBs,ngaySinh,maKhoa,soLienLac,email,chucVu,userN,passW,IDR")] bacSi bacSi)
        {
            if (ModelState.IsValid)
            {
                db.bacSis.Add(bacSi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", bacSi.IDR);
            ViewBag.maKhoa = new SelectList(db.chuyenKhoas, "maKhoa", "tenKhoa", bacSi.maKhoa);
            return View(bacSi);
        }

        // GET: BacSi_65133370/Edit/5 
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bacSi bacSi = db.bacSis.Find(id);
            if (bacSi == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", bacSi.IDR);
            ViewBag.maKhoa = new SelectList(db.chuyenKhoas, "maKhoa", "tenKhoa", bacSi.maKhoa);
            return View(bacSi);
        }

        // POST: BacSi_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Edit([Bind(Include = "ID,hoBs,tenBs,ngaySinh,maKhoa,soLienLac,email,chucVu,userN,passW,IDR")] bacSi bacSi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bacSi).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", bacSi.IDR);
            ViewBag.maKhoa = new SelectList(db.chuyenKhoas, "maKhoa", "tenKhoa", bacSi.maKhoa);
            return View(bacSi);
        }

        // GET: BacSi_65133370/Delete/5 
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bacSi bacSi = db.bacSis.Find(id);
            if (bacSi == null)
            {
                return HttpNotFound();
            }
            return View(bacSi);
        }

        // POST: BacSi_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            bacSi bacSi = db.bacSis.Find(id);
            db.bacSis.Remove(bacSi);
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
