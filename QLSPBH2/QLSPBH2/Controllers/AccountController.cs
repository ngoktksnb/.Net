using QLSPBH2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QLSPBH2.Controllers
{
    public class AccountController : Controller
    {
        private QLSP1Entities1 db = new QLSP1Entities1();

        private const string FixedEmail = "ngo147665@huce.edu.vn";
        private const string FixedHoTen = "Trần Văn Ngọ";
        private const string FixedMSSV = "147665";

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == FixedEmail && model.HoTen == FixedHoTen && model.MSSV == FixedMSSV)
                {
                    var nguoiDung = new tbl_NguoiDung
                    {
                        email = model.Email,
                        matkhau = model.MatKhau, // Bạn nên mã hóa mật khẩu trước khi lưu
                        hoten = model.HoTen,
                        mssv = model.MSSV,
                        isDelete = false
                    };

                    db.tbl_NguoiDung.Add(nguoiDung);
                    db.SaveChanges();

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Thông tin không khớp với sinh viên làm bài thi.");
                }
            }

            return View(model);
        }




        // Existing Register actions...

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.tbl_NguoiDung.FirstOrDefault(u => u.email == model.Email && u.matkhau == model.MatKhau && u.isDelete == false);
                if (user != null)
                {
                    // Lưu thông tin đăng nhập vào session hoặc cookie
                    Session["UserId"] = user.id;
                    Session["UserName"] = user.hoten;

                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
                }
            }

            return View(model);
        }









        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }


    }

}