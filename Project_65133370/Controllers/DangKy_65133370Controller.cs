using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65133370.Models;
using static Project_65133370.Models.DangKyTaiKhoan_65133370Model;

namespace Project_65133370.Controllers
{
    public class DangKy_65133370Controller : Controller
    {
        // GET: DangKy_65133370
        private Project_65133370Entities db = new Project_65133370Entities();
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(string username,string password,string confirmPassword,string hoBn,string tenBn,DateTime ngaySinh, string soLienLac)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            if (db.benhNhans.Any(x => x.userN == username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }
            if (soLienLac.Length > 10 && soLienLac.Length < 10)
            {
                ViewBag.Error = "Số điện thoại không hợp lệ!";
                return View();
            }
            var lastBN = db.benhNhans
                           .OrderByDescending(x => x.maSo)
                           .FirstOrDefault();

            string maBN = "BN01";
            if (lastBN != null)
            {
                int so = int.Parse(lastBN.maSo.Substring(2)) + 1;
                maBN = "BN" + so.ToString("D2");
            }

            benhNhan bn = new benhNhan{maSo = maBN,hoBn = hoBn,tenBn = tenBn,ngaySinh = ngaySinh,soLienLac = soLienLac,userN = username,passW = password,IDR = 2};

            db.benhNhans.Add(bn);
            db.SaveChanges();

            ViewBag.Success = "Đăng ký thành công!";
            return RedirectToAction("DangNhap");
        }
    }
}