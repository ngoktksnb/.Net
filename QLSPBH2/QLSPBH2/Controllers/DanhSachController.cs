using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using QLSPBH2.Models;  // Đảm bảo bạn đang sử dụng đúng namespace của dự án

namespace QLSPBH2.Controllers
{
    public class DanhSachController : Controller
    {
        private QLSP1Entities1 db = new QLSP1Entities1(); // Sử dụng đúng context của bạn

        // GET: DanhSach
        public ActionResult Index(string searchString, int? page, int? pageSize, string sortOrder)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            int pageSizeValue = pageSize ?? 10;
            ViewBag.CurrentPageSize = pageSizeValue;

            var danhSachs = db.tbl_DanhSach.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                danhSachs = danhSachs.Where(ds =>
                    ds.IDDS.ToString().Contains(searchString) ||
                    ds.IDDanhSachCha.ToString().Contains(searchString) ||
                    ds.TenDanhSach.Contains(searchString) ||
                    ds.MoTa.Contains(searchString)
                );
            }

            switch (sortOrder)
            {
                case "id_asc":
                    danhSachs = danhSachs.OrderBy(ds => ds.IDDS);
                    break;
                case "id_desc":
                    danhSachs = danhSachs.OrderByDescending(ds => ds.IDDS);
                    break;
                case "name_asc":
                    danhSachs = danhSachs.OrderBy(ds => ds.TenDanhSach);
                    break;
                case "name_desc":
                    danhSachs = danhSachs.OrderByDescending(ds => ds.TenDanhSach);
                    break;
                default:
                    danhSachs = danhSachs.OrderBy(ds => ds.IDDS);
                    break;
            }

            int pageNumber = (page ?? 1);
            var danhSachsPaged = danhSachs.ToPagedList(pageNumber, pageSizeValue);

            return View(danhSachsPaged);
        }

        public ActionResult Create()
        {
            var danhSachs = db.tbl_DanhSach.ToList();
            ViewBag.DanhSachChaList = new SelectList(danhSachs, "IDDS", "TenDanhSach");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_DanhSach danhSach)
        {
            if (ModelState.IsValid)
            {
                db.tbl_DanhSach.Add(danhSach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var danhSachs = db.tbl_DanhSach.ToList();
            ViewBag.DanhSachChaList = new SelectList(danhSachs, "IDDS", "TenDanhSach");
            return View(danhSach);
        }


        public ActionResult Edit(int id)
        {
            tbl_DanhSach ds1 = db.tbl_DanhSach.FirstOrDefault(x => x.IDDS == id);
            if (ds1 == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách các danh mục trừ danh mục hiện tại
            var danhSachs = db.tbl_DanhSach.Where(ds => ds.IDDS != id).ToList();
            ViewBag.DanhSachChaList = new SelectList(danhSachs, "IDDS", "TenDanhSach");

            return View(ds1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tbl_DanhSach ds1)
        {
            if (ModelState.IsValid)
            {
                tbl_DanhSach uds = db.tbl_DanhSach.FirstOrDefault(x => x.IDDS == ds1.IDDS);
                if (uds != null)
                {
                    uds.TenDanhSach = ds1.TenDanhSach;
                    uds.MoTa = ds1.MoTa;
                    uds.IDDanhSachCha = ds1.IDDanhSachCha;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, lấy lại danh sách các danh mục để hiển thị trong dropdown list
            var danhSachs = db.tbl_DanhSach.Where(ds => ds.IDDS != ds1.IDDS).ToList();
            ViewBag.DanhSachChaList = new SelectList(danhSachs, "IDDS", "TenDanhSach");

            return View(ds1);
        }

        public ActionResult Delete(int id)
        {
            tbl_DanhSach dm1 = db.tbl_DanhSach.FirstOrDefault(x => x.IDDS == id);
            db.tbl_DanhSach.Remove(dm1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
