using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class GioHangController : Controller
    {    // GET: Giohang
        QLBANSACHEntities data = new QLBANSACHEntities();

        public ActionResult Index()
        {
            return View();
        }
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                // Neu gio hang chua ton tai thi khoi tao listGiohang
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGiohang(int iMasach, string strURL)
        {
            // Lay ra Session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            // Kiem tra sach nà y ton tai trong Session["Giohang"] chua?
            Giohang sanpham = lstGiohang.Find(n => n.iMasach == iMasach);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMasach);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        // Tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        // Tính tổng tiền
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        // Xây dựng trang Giỏ hàng
        public ActionResult GioHang()
        {
            List<Giohang> lstGioHang = Laygiohang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            // Lay ra Session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            // Kiem tra sach nà y ton tai trong Session["Giohang"] chua?
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasach == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            // Lay ra Session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "BookStore");
        }

        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            // Lay ra Session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            // Kiem tra sach nà y ton tai trong Session["Giohang"] chua?
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSouong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
        [HttpGet]    
        public ActionResult DatHang()
        {
            // Check if the user is logged in
            if (Session["Taikhoan"] == null || string.IsNullOrEmpty(Session["Taikhoan"].ToString()))
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Check if the cart exists
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "BookStore");
            }

            // Retrieve cart data from Session
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }

        public ActionResult DatHang(FormCollection collection)
        {
            // Create new order
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();

            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;

            var ngaygiaoStr = collection["Ngaygiao"];
            ddh.Ngaygiao = DateTime.TryParse(ngaygiaoStr, out DateTime ngaygiao) ? ngaygiao : DateTime.Now;
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;

            data.DONDATHANG.Add(ddh);
            data.SaveChanges();

            // Add order details
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG
                {
                    MaDonHang = ddh.MaDonHang,
                    Masach = item.iMasach,
                    Soluong = item.iSoluong,
                    Dongia = (decimal)item.dDongia
                };

                data.CHITIETDONTHANG.Add(ctdh);
            }

            data.SaveChanges();
            Session["Giohang"] = null;

            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang()
        {
           return View();
        }

    }
}