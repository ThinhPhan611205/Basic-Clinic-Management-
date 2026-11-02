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
    public class LichKham_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // GET: LichKham_65133370 - Bác sĩ xem lịch khám của mình - dùng _LayoutBacSi
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Index()
        {
            if (Session["maSo"] == null)
                return RedirectToAction("DangNhap", "UserLogin_65133370");
            string maBacSi = Session["maSo"].ToString();
            var lichKhams = db.lichKhams.Include(l => l.bacSi).Include(l => l.benhNhan).Include(l => l.trangThaiLichHen).Where(l => l.bacSi.ID == maBacSi).ToList();
            return View(lichKhams);
        }

        // Bệnh nhân xem lịch khám của mình
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult IndexLK_BN()
        {
            if (Session["maSo"] == null)
                return RedirectToAction("DangNhap", "UserLogin_65133370");
            string maBenhNhan = Session["maSo"].ToString();
            var lichKhams = db.lichKhams.Include(l => l.benhNhan).Include(l => l.benhNhan).Include(l => l.trangThaiLichHen).Where(l => l.benhNhan.maSo == maBenhNhan).ToList();
            return View(lichKhams);
        }

        // GET: LichKham_65133370/Details/5 - Bác sĩ xem - dùng _LayoutBacSi
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lichKham lichKham = db.lichKhams.Find(id);
            if (lichKham == null)
            {
                return HttpNotFound();
            }
            return View(lichKham);
        }

        // GET: LichKham_65133370/Create - Bác sĩ tạo - dùng _LayoutBacSi
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.bacSis, "ID", "hoBs");
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn");
            ViewBag.trangThaiKham = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai");
            return View();
        }

        // POST: LichKham_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Create([Bind(Include = "maLich,maSo,lyDoKham,ID,ngayDatLich,ngayKham,trangThaiKham")] lichKham lichKham)
        {
            if (ModelState.IsValid)
            {
                db.lichKhams.Add(lichKham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.bacSis, "ID", "hoBs", lichKham.ID);
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn", lichKham.maSo);
            ViewBag.trangThaiKham = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai", lichKham.trangThaiKham);
            return View(lichKham);
        }
        // GET: Hiển thị form đăng ký - Bệnh nhân đặt lịch
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult CreateLK_BN(string id)
        {
            if (Session["maSo"] == null)
            {
                return RedirectToAction("DangNhap", "UserLogin_65133370");
            }

            if (string.IsNullOrEmpty(id))
            {
                return Content("Không tìm thấy ID bác sĩ.");
            }

            // Tìm bác sĩ
            var bacSi = db.bacSis.Find(id);
            if (bacSi == null)
            {
                return HttpNotFound();
            }
            ViewBag.DoctorId = id;
            ViewBag.hoBs = bacSi.hoBs;
            ViewBag.tenBs = bacSi.tenBs;
            ViewBag.maKhoa = bacSi.maKhoa;
            ViewBag.email = bacSi.email;
            ViewBag.soLienLac = bacSi.soLienLac;

            return View();
        }

        // POST: LichKham_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult CreateLK_BN([Bind(Include = "maLich,maSo,lyDoKham,ID,ngayDatLich,ngayKham,trangThaiKham")] lichKham lichKham)
        {
            if (ModelState.IsValid)
            {
                db.lichKhams.Add(lichKham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.bacSis, "ID", "hoBs", lichKham.ID);
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn", lichKham.maSo);
            ViewBag.trangThaiKham = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai", lichKham.trangThaiKham);
            return View(lichKham);
        }
        // API lấy khung giờ đã đặt - Cần đăng nhập
        [HttpGet]
        [CheckRole(RequireLoginOnly = true)]
        public JsonResult GetBookedSlots(string doctorId, string date)
        {
            try
            {
                DateTime searchDate = DateTime.Parse(date);

                var appointments = db.lichKhams
                    .Where(l => l.ID == doctorId)
                    .ToList()
                    .Where(l => l.ngayKham.HasValue && l.ngayKham.Value.Date == searchDate.Date)
                    .Select(l => l.ngayKham.Value.ToString("h:mm tt"))
                    .ToList();

                return Json(appointments, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }

        // 3. POST: Xử lý lưu lịch khám (Gọi từ Ajax) - Bệnh nhân đặt lịch
        [HttpPost]
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult BookAppointment(DatLich_65133370Model model)
        {
            try
            {
                // 1. Kiểm tra đăng nhập
                if (Session["maSo"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để đặt lịch!" });
                }
                string maBenhNhan = Session["maSo"].ToString();

                var lastLichKham = db.lichKhams
                                     .OrderByDescending(x => x.maLich)
                                     .FirstOrDefault();

                string autoMaLich = "LK01";

                if (lastLichKham != null)
                {
                    string maCu = lastLichKham.maLich;
                    if (int.TryParse(maCu.Substring(2), out int soHienTai))
                    {
                        int soMoi = soHienTai + 1;
                        autoMaLich = "LK" + soMoi.ToString("D2");
                    }
                    else
                    {
                        autoMaLich = "LK" + (db.lichKhams.Count() + 1).ToString("D2");
                    }
                }

                DateTime fullDate = DateTime.Parse(model.NgayKham + " " + model.GioKham);

                bool isBooked = db.lichKhams.Any(l => l.ID == model.BacSiId && l.ngayKham == fullDate);
                if (isBooked)
                {
                    return Json(new { success = false, message = "Khung giờ này vừa có người đặt. Vui lòng chọn giờ khác!" });
                }
                lichKham lk = new lichKham();
                lk.maLich = autoMaLich;           // Mã tự động
                lk.maSo = maBenhNhan;             // Mã bệnh nhân từ Session
                lk.ID = model.BacSiId;            // Mã bác sĩ
                lk.ngayDatLich = DateTime.Now;    // Ngày hiện tại
                lk.ngayKham = fullDate;           // Ngày giờ khám user chọn
                lk.lyDoKham = model.LyDoKham;     // Lý do

                lk.trangThaiKham = 1;

                db.lichKhams.Add(lk);
                db.SaveChanges();

                return Json(new { success = true, message = "Đăng ký lịch khám thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        // GET: LichKham_65133370/Edit/5 - Bác sĩ sửa - dùng _LayoutBacSi
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lichKham lichKham = db.lichKhams.Find(id);
            if (lichKham == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.bacSis.Select(bs => new
            {
                bs.ID,
                FullName = bs.hoBs + " " + bs.tenBs
            }), "ID", "FullName", lichKham.ID);
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn", lichKham.maSo);
            ViewBag.trangThaiKham = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai", lichKham.trangThaiKham);
            return View(lichKham);
        }

        // POST: LichKham_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Edit([Bind(Include = "maLich,maSo,lyDoKham,ID,ngayDatLich,ngayKham,trangThaiKham")] lichKham lichKham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lichKham).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.bacSis.Select(bs => new
            {
                bs.ID,
                FullName = bs.hoBs + " " + bs.tenBs
            }), "ID", "FullName", lichKham.ID);
            ViewBag.maSo = new SelectList(db.benhNhans, "maSo", "hoBn", lichKham.maSo);
            ViewBag.trangThaiKham = new SelectList(db.trangThaiLichHens, "maTrangThai", "trangThai", lichKham.trangThaiKham);
            return View(lichKham);
        }

        // GET: LichKham_65133370/Delete/5 - Bác sĩ xóa - dùng _LayoutBacSi
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lichKham lichKham = db.lichKhams.Find(id);
            if (lichKham == null)
            {
                return HttpNotFound();
            }
            return View(lichKham);
        }

        // POST: LichKham_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult DeleteConfirmed(string id)
        {
            lichKham lichKham = db.lichKhams.Find(id);
            db.lichKhams.Remove(lichKham);
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
