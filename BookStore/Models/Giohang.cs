using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Giohang
{
    // Tạo đối tượng data chứa dữ liệu từ model dbBansach đã tạo.
    QLBANSACHEntities data = new QLBANSACHEntities();

    public int iMasach { set; get; }
    public string sTensach { set; get; }
    public string sAnhbia { set; get; }
    public Double dDongia { set; get; }
    public int iSoluong { set; get; }

    public Double dThanhtien
    {
        get { return iSoluong * dDongia; }
    }

    // Khởi tạo giỏ hàng theo Masach được truyền vào với Soluong mặc định là 1
    public Giohang(int iMasach)
    {
        this.iMasach = iMasach;
        SACH sach = data.SACH.Single(n => n.Masach == iMasach);
        sTensach = sach.Tensach;
        sAnhbia = sach.Anhbia;
        dDongia = double.Parse(sach.Giaban.ToString());
        iSoluong = 1;
    }
}