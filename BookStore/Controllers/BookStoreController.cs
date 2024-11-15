using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;

using PagedList;
using PagedList.Mvc;
namespace BookStore.Controllers
{
    public class BookStoreController : Controller
    {
        QLBANSACHEntities data = new QLBANSACHEntities();

        private List<SACH> Laysachmoi(int cout)
        {
            return data.SACH.OrderByDescending(a => a.Ngaycapnhat).Take(cout).ToList();
        }
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1); 
            var sachmoi = Laysachmoi(15); 
            return View(sachmoi.ToPagedList(pageNum, pageSize)); 
        }
        public ActionResult Chude()
        {
            var chude = from cd in data.CHUDE select cd;
            return PartialView(chude);
        }

        public ActionResult Nhaxuatban()
        {
            var chude = from cd in data.NHAXUATBAN select cd;
            return PartialView(chude);
        }

        public ActionResult SPTheochude(int id)
        {
            var sach = from s in data.SACH where s.MaCD == id select s;
            return View(sach);
        }
        public ActionResult SPTheoNXB(int id)
        {
            var sach = from s in data.NHAXUATBAN where s.MaNXB == id select s;
            return View(sach);
        }
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACH where s.Masach == id select s;
            return View(sach.Single());
        }
    }
}