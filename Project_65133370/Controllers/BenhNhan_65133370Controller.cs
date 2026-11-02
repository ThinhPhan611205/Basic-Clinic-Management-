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
    public class BenhNhan_65133370Controller : Controller
    {
        private Project_65133370Entities db = new Project_65133370Entities();

        // Trang chủ bệnh nhân - Dashboard
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult TrangChuBN()
        {
            string maBenhNhan = Session["maSo"]?.ToString();
            if (string.IsNullOrEmpty(maBenhNhan))
                return RedirectToAction("DangNhap", "UserLogin_65133370");

            // Tổng lịch khám đã đặt
            ViewBag.TongLichKham = db.lichKhams.Count(l => l.maSo == maBenhNhan);

            // Tổng bệnh án
            ViewBag.TongBenhAn = db.benhAns.Count(b => b.maSo == maBenhNhan);

            // Lịch đang chờ khám (trạng thái 1 hoặc 2)
            ViewBag.LichChoKham = db.lichKhams
                .Count(l => l.maSo == maBenhNhan && (l.trangThaiKham == 1 || l.trangThaiKham == 2));

            // Lịch khám sắp tới (5 lịch gần nhất)
            ViewBag.LichKhamSapToi = db.lichKhams
                .Include(l => l.bacSi)
                .Where(l => l.maSo == maBenhNhan && l.ngayKham >= DateTime.Now)
                .OrderBy(l => l.ngayKham)
                .Take(5)
                .ToList();

            return View();
        }

        // GET: BenhNhan_65133370 - 
        public ActionResult GioiThieuBN()
        {
            return View();
        }

        // Xem thông tin cá nhân -
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult ThongTinCTBN()
        {
            string id = Session["maSo"]?.ToString();

            if (id == null)
                return RedirectToAction("DangNhap", "UserLogin_65133370");

            var bn = db.benhNhans.SingleOrDefault(x => x.maSo == id);

            return View(bn);
        }

        // Tìm kiếm bệnh nhân 
        [CheckRole(AllowedRoles = "bacSi,admin")]
        public ActionResult TimKiemBenhNhan(string maSo, string hoTen, string maBenhAn)
        {
            var query = db.benhNhans
                          .Include(b => b.benhAns)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(maSo))
            {
                query = query.Where(b => b.maSo.Contains(maSo));
            }

            if (!string.IsNullOrEmpty(hoTen))
            {
                query = query.Where(b =>
                    (b.hoBn + " " + b.tenBn).Contains(hoTen));
            }

            if (!string.IsNullOrEmpty(maBenhAn))
            {
                query = query.Where(b =>
                    b.benhAns.Any(ba => ba.maBenhAn.Contains(maBenhAn)));
            }

            ViewBag.maSo = maSo;
            ViewBag.hoTen = hoTen;
            ViewBag.maBenhAn = maBenhAn;

            return View("IndexBN_BS", query.ToList());
        }

        // Danh sach benh nhan 
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Index()
        {
            var benhNhans = db.benhNhans.Include(b => b.Role);
            return View(benhNhans.ToList());
        }

        // Tim kiem benh nhan cho Admin
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult TimKiemBenhNhan_Admin(string maSo, string hoTen)
        {
            var query = db.benhNhans.Include(b => b.Role).AsQueryable();

            if (!string.IsNullOrEmpty(maSo))
            {
                query = query.Where(b => b.maSo.Contains(maSo));
            }

            if (!string.IsNullOrEmpty(hoTen))
            {
                query = query.Where(b => (b.hoBn + " " + b.tenBn).Contains(hoTen));
            }

            ViewBag.maSo = maSo;
            ViewBag.hoTen = hoTen;

            return View("Index", query.ToList());
        }

        // Bác sĩ xem danh sách bệnh nhân 
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult IndexBN_BS()
        {
            var benhNhans = db.benhNhans.Include(b => b.Role);
            return View(benhNhans.ToList());
        }

        // GET: BenhNhan_65133370/Details/5 - Bác sĩ xem chi tiết 
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhNhan benhNhan = db.benhNhans.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }
            return View(benhNhan);
        }
        // Chi tiết cho Admin
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Details_Admin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhNhan benhNhan = db.benhNhans.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }
            return View(benhNhan);
        }

        // Ham tu dong tao ma so benh nhan
        private string TaoMaSoBenhNhanTuDong()
        {
            var allMaSo = db.benhNhans.Select(b => b.maSo).ToList();

            if (allMaSo == null || allMaSo.Count == 0)
            {
                return "BN001";
            }

            int maxSo = 0;
            foreach (var ma in allMaSo)
            {
                if (!string.IsNullOrEmpty(ma) && ma.StartsWith("BN") && ma.Length > 2)
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
            return "BN" + maxSo.ToString("D3");
        }

        // GET: BenhNhan_65133370/Create - Bac si - 
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Create()
        {
            var model = new benhNhan();
            model.maSo = TaoMaSoBenhNhanTuDong();
            model.IDR = 2; // Mac dinh la benh nhan
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", model.IDR);
            return View(model);
        }

        // POST: BenhNhan_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Create([Bind(Include = "maSo,hoBn,tenBn,ngaySinh,soLienLac,userN,passW,IDR")] benhNhan benhNhan)
        {
            if (ModelState.IsValid)
            {
                db.benhNhans.Add(benhNhan);
                db.SaveChanges();
                return RedirectToAction("IndexBN_BS");
            }

            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }
        // Tao benh nhan tu Admin
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Create_Admin()
        {
            var model = new benhNhan();
            model.maSo = TaoMaSoBenhNhanTuDong();
            model.IDR = 2; // Mac dinh la benh nhan
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", model.IDR);
            return View(model);
        }

        // POST: BenhNhan_65133370/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Create_Admin([Bind(Include = "maSo,hoBn,tenBn,ngaySinh,soLienLac,userN,passW,IDR")] benhNhan benhNhan)
        {
            if (ModelState.IsValid)
            {
                db.benhNhans.Add(benhNhan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }
        // GET: BenhNhan_65133370/Edit/5 - Bác sĩ - dùng _LayoutBacSi
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhNhan benhNhan = db.benhNhans.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }

        // POST: BenhNhan_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Edit([Bind(Include = "maSo,hoBn,tenBn,ngaySinh,soLienLac,userN,passW,IDR")] benhNhan benhNhan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(benhNhan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "BenhNhan_65133370", new { id = benhNhan.maSo });
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }
        // Edit cho Admin
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Edit_Admin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhNhan benhNhan = db.benhNhans.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }

        // POST: BenhNhan_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Edit_Admin([Bind(Include = "maSo,hoBn,tenBn,ngaySinh,soLienLac,userN,passW,IDR")] benhNhan benhNhan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(benhNhan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "BenhNhan_65133370");
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }
        // Bệnh nhân tự sửa thông tin cá nhân
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult EditBN(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhNhan benhNhan = db.benhNhans.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }

        // POST: BenhNhan_65133370/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "benhNhan")]
        public ActionResult EditBN([Bind(Include = "maSo,hoBn,tenBn,ngaySinh,soLienLac,userN,passW,IDR")] benhNhan benhNhan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(benhNhan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ThongTinCTBN", "BenhNhan_65133370");
            }
            ViewBag.IDR = new SelectList(db.Roles, "IDR", "IDR", benhNhan.IDR);
            return View(benhNhan);
        }

        // GET: BenhNhan_65133370/Delete/5 - Bác sĩ - dùng _LayoutBacSi
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhNhan benhNhan = db.benhNhans.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }
            return View(benhNhan);
        }

        // POST: BenhNhan_65133370/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "bacSi")]
        public ActionResult DeleteConfirmed(string id)
        {
            benhNhan benhNhan = db.benhNhans.Find(id);
            db.benhNhans.Remove(benhNhan);
            db.SaveChanges();
            return RedirectToAction("IndexBN_BS");
        }
        // Xóa cho Admin
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult Delete_Admin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            benhNhan benhNhan = db.benhNhans.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }
            return View(benhNhan);
        }

        // POST: BenhNhan_65133370/Delete/5
        [HttpPost, ActionName("Delete_Admin")]
        [ValidateAntiForgeryToken]
        [CheckRole(AllowedRoles = "admin")]
        public ActionResult DeleteConfirmed_Admin(string id)
        {
            benhNhan benhNhan = db.benhNhans.Find(id);
            db.benhNhans.Remove(benhNhan);
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
