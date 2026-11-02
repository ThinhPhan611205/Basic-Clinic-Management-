using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project_65133370
{
    public class CheckRoleAttribute : ActionFilterAttribute
    {
        /// Danh sách các role được phép truy cập, phân cách bằng dấu phẩy
        /// Ví dụ: "admin,bacSi" hoặc "benhNhan"
        public string AllowedRoles { get; set; }
        /// Nếu true, chỉ cần đăng nhập là được (không cần kiểm tra role cụ thể)
        public bool RequireLoginOnly { get; set; } = false;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy session role hiện tại
            var session = filterContext.HttpContext.Session;
            var currentRole = session["role"] as string;
            var username = session["username"] as string;


            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(currentRole))
            {
                RedirectToLogin(filterContext);
                return;
            }

            if (RequireLoginOnly)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (!string.IsNullOrEmpty(AllowedRoles))
            {
                var allowedRolesList = AllowedRoles.Split(',')
                                                   .Select(r => r.Trim().ToLower())
                                                   .ToList();

                if (!allowedRolesList.Contains(currentRole.ToLower()))
                {
                    RedirectToAccessDenied(filterContext);
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private void RedirectToLogin(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "UserLogin_65133370" },
                    { "action", "DangNhap" }
                });
        }

        private void RedirectToAccessDenied(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
        }
    }
}
