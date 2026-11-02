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
    // Hóa đơn - Chỉ bác sĩ mới được quản lý (dùng _LayoutBacSi)
    [CheckRole(AllowedRoles = "bacSi")]
    public class HoaDon_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // GET: HoaDon_65133370
        public ActionResult Index()
        {
            var hoaDons = db.hoaDons.Include(h => h.benhAn);
            return View(hoaDons.ToList());
        }

        // GET: HoaDon_65133370/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hoaDon hoaDon = db.hoaDons.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            return View(hoaDon);
        }
        //ACTION XU LY THANH TOAN
        // Action xu ly thanh toan (Mo phong thanh toan thanh cong)
        [CheckRole(AllowedRoles = "bacSi,benhNhan")]
        public ActionResult ThanhToan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 1. Tim hoa don theo ma
            var hoaDon = db.hoaDons.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // 2. Cap nhat trang thai thanh toan
            hoaDon.trangThaiThanhToan = true; // Da thanh toan
            db.Entry(hoaDon).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            // 3. Xac dinh role de redirect dung trang
            var currentRole = Session["role"] as string;
            if (currentRole == "benhNhan")
            {
                // Benh nhan -> quay ve DetailsBN
                return RedirectToAction("DetailsBN", "BenhAn_65133370", new { id = hoaDon.maBenhAn });
            }
            else
            {
                // Bac si -> quay ve Details
                return RedirectToAction("Details", "BenhAn_65133370", new { id = hoaDon.maBenhAn });
            }
        }
        // GET: HoaDon_65133370/Create
        // GET: Tạo hóa đơn cho bệnh án cụ thể
        public ActionResult Create(string maBenhAn)
        {
            if (string.IsNullOrEmpty(maBenhAn)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var benhAn = db.benhAns.Find(maBenhAn);
            if (benhAn == null) return HttpNotFound();

            // 1. TÍNH TIỀN THUỐC (Tự động)
            double tienThuoc = 0;
            if (!string.IsNullOrEmpty(benhAn.maDonThuoc))
            {
                var donThuoc = db.donThuocs.Find(benhAn.maDonThuoc);
                if (donThuoc != null && donThuoc.chiTietDonThuocs != null)
                {
                    // Lưu ý: handle null cẩn thận
                    tienThuoc = donThuoc.chiTietDonThuocs.Sum(ct => (double)((ct.soLuong ?? 0) * (ct.thuoc.donGia ?? 0)));
                }
            }

            // 2. CHUẨN BỊ DỮ LIỆU VIEW
            ViewBag.TienThuoc = tienThuoc;
            ViewBag.MaBenhAn = maBenhAn;

            // Tạo mã hóa đơn tự động
            ViewBag.MaHdGoiY = "HD" + DateTime.Now.ToString("ddHHmmss");

            // Lấy danh sách dịch vụ để hiển thị checkbox
            ViewBag.ListDichVu = db.DichVus.ToList();

            return View();
        }

        // POST: HoaDon_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // maBenhAn, maHd: lấy từ form
        // selectedDichVu: mảng chứa các mã dịch vụ được tick chọn (checkbox)
        public ActionResult Create(string maBenhAn, string maHd, string[] selectedDichVu)
        {
            var benhAn = db.benhAns.Find(maBenhAn);

            // 1. TẠO HÓA ĐƠN MASTER
            hoaDon newHD = new hoaDon();
            newHD.maHd = maHd;
            newHD.maBenhAn = maBenhAn;
            newHD.ngayLap = DateTime.Now;
            newHD.trangThaiThanhToan = false; // Mặc định chưa thanh toán

            // 2. TÍNH TIỀN THUỐC (Tính lại ở server để bảo mật)
            double tongTien = 0;
            if (!string.IsNullOrEmpty(benhAn.maDonThuoc))
            {
                var ctDonThuoc = db.chiTietDonThuocs.Where(ct => ct.maDonThuoc == benhAn.maDonThuoc).ToList();
                tongTien += ctDonThuoc.Sum(ct => (double)((ct.soLuong ?? 0) * (ct.thuoc.donGia ?? 0)));
            }

            // 3. XỬ LÝ DỊCH VỤ & CHI TIẾT HÓA ĐƠN
            db.hoaDons.Add(newHD); // Lưu Hóa đơn trước để có ID làm khóa ngoại

            if (selectedDichVu != null)
            {
                foreach (var maDV in selectedDichVu)
                {
                    // Tạo chi tiết hóa đơn
                    chiTietHoaDon ct = new chiTietHoaDon();
                    ct.maChiTiet = "CT" + Guid.NewGuid().ToString().Substring(0, 8); // Tạo mã ngẫu nhiên
                    ct.maHd = maHd;
                    ct.maDichVu = maDV;

                    db.chiTietHoaDons.Add(ct);

                    // Cộng tiền dịch vụ vào tổng
                    var dichVu = db.DichVus.Find(maDV);
                    if (dichVu != null)
                    {
                        tongTien += dichVu.giaTien;
                    }
                }
            }

            // 4. CẬP NHẬT TỔNG TIỀN CUỐI CÙNG
            newHD.tongTien = tongTien;

            db.SaveChanges();

            return RedirectToAction("Details", "BenhAn_65133370", new { id = maBenhAn });
        }

        // GET: HoaDon_65133370/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hoaDon hoaDon = db.hoaDons.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            ViewBag.maBenhAn = new SelectList(db.benhAns, "maBenhAn", "maSo", hoaDon.maBenhAn);
            return View(hoaDon);
        }

        // POST: HoaDon_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maHd,maBenhAn,ngayLap,tongTien,trangThaiThanhToan")] hoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoaDon).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maBenhAn = new SelectList(db.benhAns, "maBenhAn", "maSo", hoaDon.maBenhAn);
            return View(hoaDon);
        }

        // GET: HoaDon_65133370/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            hoaDon hoaDon = db.hoaDons.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            return View(hoaDon);
        }

        // POST: HoaDon_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            hoaDon hoaDon = db.hoaDons.Find(id);
            db.hoaDons.Remove(hoaDon);
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
