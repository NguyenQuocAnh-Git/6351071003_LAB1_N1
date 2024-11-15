using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Web.UI.WebControls;

namespace BookStore.Controllers
{
    public class AdminController : Controller
    {
        private QLBANSACHEntities db = new QLBANSACHEntities();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SACH(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            var pagedList = db.SACH.OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize);
            if (!pagedList.Any())
            {
                ViewBag.Message = "Không có sách nào để hiển thị.";
            }
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = pagedList.PageCount;
            return View(pagedList);
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var username = collection["username"];
            var password = collection["password"];


            // Kiểm tra xem có bỏ trống username không
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "Username is required");
            }

            // Kiểm tra xem có bỏ trống password không
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "Password is required");
            }

            // Nếu cả hai trường đều được nhập và hợp lệ
            if (ModelState.IsValid)
            {
                Admin ad = db.Admin.SingleOrDefault(n => n.UserAdmin == username && n.PassAdmin == password);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("SACH", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Invalid username or password!";
                }
            }

            return View();
        }


        [HttpGet]
        public ActionResult ThemmoiSach()
        {
            ViewBag.MaCD = new SelectList(db.CHUDE.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBAN.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisach(SACH sach, HttpPostedFileBase fileUpload)
        {
            // Đưa dữ liệu vào dropdownlist
            ViewBag.MaCD = new SelectList(db.CHUDE.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBAN.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Kiểm tra đường dẫn file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            // Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    // Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);

                    // Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);

                    // Kiểm tra hình ảnh tồn tại chưa?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // Lưu hình ảnh vào đường dẫn
                        fileUpload.SaveAs(path);
                    }

                    sach.Anhbia = fileName;

                    // Lưu vào CSDL
                    db.SACH.Add(sach);
                    db.SaveChanges();
                }
                return RedirectToAction("Sach");
            }
        }

        public ActionResult Chitietsach(int id)
        {
            SACH sach = db.SACH.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            SACH sach = db.SACH.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            SACH sach = db.SACH.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACH.Remove(sach);
            db.SaveChanges();
            return RedirectToAction("SACH");
        }

        [HttpGet]
        public ActionResult Suasach(int id)
        {
            SACH sach = db.SACH.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDE.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBAN.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            // Đưa dữ liệu vào dropdownlist
            ViewBag.MaCD = new SelectList(db.CHUDE.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBAN.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Kiểm tra đường dẫn file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            // Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    // Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);

                    // Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);

                    // Kiểm tra hình ảnh tồn tại chưa?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // Lưu hình ảnh vào đường dẫn
                        fileUpload.SaveAs(path);
                    }

                    sach.Anhbia = fileName;

                    // Lưu vào CSDL
                    db.Entry(sach).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Sach");
            }
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

    }
}