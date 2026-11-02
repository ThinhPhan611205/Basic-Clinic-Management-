using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65133370.Models;

namespace Project_65133370.Controllers
{
    public class UserLogin_65133370Controller : Controller
    {
        // GET: UserLogin_65133370
        Project_65133370Entities db = new Project_65133370Entities();
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string username, string password)
        {
            var bn = db.benhNhans.FirstOrDefault(x => x.userN == username && x.passW == password);
            if (bn != null)
            {
                Session["username"] = bn.userN;
                Session["role"] = "benhNhan";
                Session["IDR"] = bn.IDR;
                Session["maSo"] = bn.maSo;
                Session["Ten"] = "BN. " + bn.hoBn + " " + bn.tenBn;
                return RedirectToAction("TrangChuBN", "BenhNhan_65133370");
            }

            var bs = db.bacSis.FirstOrDefault(x => x.userN == username && x.passW == password);
            if (bs != null)
            {
                if (bs.IDR == 3)
                {
                    Session["username"] = bs.userN;
                    Session["role"] = "bacSi";
                    Session["IDR"] = bs.IDR;
                    Session["maSo"] = bs.ID;
                    Session["Ten"] = "BS. " + bs.hoBs + " " + bs.tenBs;
                    return RedirectToAction("TrangChuBS", "BacSi_65133370");
                }
                else if (bs.IDR == 1)
                {
                    Session["username"] = bs.userN;
                    Session["role"] = "admin";
                    Session["IDR"] = bs.IDR;
                    Session["maSo"] = bs.ID;
                    Session["Ten"] = "BS. " + bs.hoBs + " " + bs.tenBs;
                    return RedirectToAction("GioiThieuBS_Admin", "BacSi_65133370");
                }
            }
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap");
        }
    }
}