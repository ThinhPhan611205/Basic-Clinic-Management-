using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_65133370.Controllers
{
    public class LayID_65133370Controller : Controller
    {
        // GET: LayID_65133370
        public string GetCurrentUserID()
        {
            return Session["maSo"]?.ToString();
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string role = Session["role"] as string;

            if (role == "bacSi")
            {
                ViewBag.Layout = "~/Views/Shared/_LayoutBacSi.cshtml";
            }
            else if (role == "benhNhan")
            {
                ViewBag.Layout = "~/Views/Shared/_LayoutBenhNhan.cshtml";

            }
            else if (role == "admin")
            {
                ViewBag.Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
            }
            else
            {
                ViewBag.Layout = "~/Views/Shared/_LayoutGioiThieu.cshtml";
            }

            base.OnActionExecuted(filterContext);
        }
    }
}