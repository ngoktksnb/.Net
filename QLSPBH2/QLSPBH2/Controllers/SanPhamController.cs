using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace QLSPBH2.Controllers
{
    public class SanPhamController : Controller
    {
        private QLSP1Entities1 db = new QLSP1Entities1();

        // GET: SanPham
        public ActionResult Index(string searchString, int? page, int? pageSize, string sortOrder)
        {
            // Lưu trữ từ khóa tìm kiếm và sắp xếp vào ViewBag để giữ lại giá trị khi phân trang
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            // Số lượng mục hiển thị trên mỗi trang (mặc định là 10 nếu không được cung cấp)
            int pageSizeValue = pageSize ?? 10;
            ViewBag.CurrentPageSize = pageSizeValue;

            // Lấy danh sách sản phẩm từ database
            var sanPhams = db.tbl_SanPham.AsQueryable();

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                sanPhams = sanPhams.Where(sp =>
                    sp.IDSP.ToString().Contains(searchString) ||
                    sp.MaSP.Contains(searchString) ||
                    sp.MoTa.Contains(searchString) ||
                    sp.HinhAnh.Contains(searchString) ||
                    (sp.tbl_DanhSach != null && sp.tbl_DanhSach.TenDanhSach.Contains(searchString))
                );
            }

            // Xử lý sắp xếp
            switch (sortOrder)
            {
                case "id_asc":
                    sanPhams = sanPhams.OrderBy(sp => sp.IDSP);
                    break;
                case "id_desc":
                    sanPhams = sanPhams.OrderByDescending(sp => sp.IDSP);
                    break;
                case "name_asc":
                    sanPhams = sanPhams.OrderBy(sp => sp.MaSP);
                    break;
                case "name_desc":
                    sanPhams = sanPhams.OrderByDescending(sp => sp.MaSP);
                    break;
                default:
                    sanPhams = sanPhams.OrderBy(sp => sp.IDSP);
                    break;
            }

            // Trang hiện tại (nếu không được cung cấp thì mặc định là trang 1)
            int pageNumber = (page ?? 1);

            // Sử dụng PagedList để phân trang
            var sanPhamsPaged = sanPhams.ToPagedList(pageNumber, pageSizeValue);

            // Trả về view với danh sách đã phân trang
            return View(sanPhamsPaged);
        }


        public ActionResult CreateSanPham()
        {
            // Lấy danh sách các danh mục từ cơ sở dữ liệu
            var danhMucs = db.tbl_DanhSach.ToList();
            ViewBag.DanhMucChaList = new SelectList(danhMucs, "IDDS", "TenDanhSach");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSanPham(tbl_SanPham sanPham, IEnumerable<HttpPostedFileBase> imageFiles)
        {
            if (ModelState.IsValid)
            {
                // Đường dẫn lưu trữ hình ảnh
                var imagePath = Server.MapPath("~/Images/");
                var imageFileNames = new List<string>();

                // Lưu từng ảnh lên server và thu thập tên file
                if (imageFiles != null)
                {
                    foreach (var file in imageFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(imagePath, fileName);
                            file.SaveAs(path);
                            imageFileNames.Add(fileName);
                        }
                    }
                }

                // Lưu tên các tệp vào thuộc tính HinhAnh của sản phẩm, cách nhau bằng dấu |
                sanPham.HinhAnh = string.Join("|", imageFileNames);

                // Lưu sản phẩm vào cơ sở dữ liệu
                db.tbl_SanPham.Add(sanPham);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu không thành công, hiển thị lại form với các thông tin đã nhập
            var danhMucs = db.tbl_DanhSach.ToList();
            ViewBag.DanhMucChaList = new SelectList(danhMucs, "IDDS", "TenDanhSach");
            return View(sanPham);
        }

        // GET: SanPham/Edit/5
        public ActionResult Edit(int id)
        {
            // Lấy thông tin sản phẩm cần sửa từ database
            var sp1 = db.tbl_SanPham.FirstOrDefault(x => x.IDSP == id);

            if (sp1 == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách các danh mục từ cơ sở dữ liệu
            var danhMucs = db.tbl_DanhSach.ToList();
            ViewBag.DanhMucChaList = new SelectList(danhMucs, "IDDS", "TenDanhSach");

            // Chuyển đến view sửa sản phẩm và truyền thông tin sản phẩm vào view
            return View(sp1);
        }

        // POST: SanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tbl_SanPham sp1, IEnumerable<string> deleteImages, IEnumerable<HttpPostedFileBase> newImageFiles)
        {
            if (ModelState.IsValid)
            {
                var sanPham = db.tbl_SanPham.FirstOrDefault(x => x.IDSP == sp1.IDSP);
                if (sanPham != null)
                {
                    sanPham.MaSP = sp1.MaSP;
                    sanPham.MoTa = sp1.MoTa;
                    sanPham.IDDS = sp1.IDDS;

                    // Xử lý xóa các hình ảnh đã chọn
                    if (deleteImages != null)
                    {
                        foreach (var image in deleteImages)
                        {
                            var imagePath = Path.Combine(Server.MapPath("~/Images/"), image);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                                sanPham.HinhAnh = sanPham.HinhAnh.Replace(image + "|", "").Replace(image, ""); // Xóa tên hình ảnh khỏi danh sách HinhAnh trong SQL
                            }
                        }
                    }

                    // Xử lý thêm mới hình ảnh
                    if (newImageFiles != null)
                    {
                        var imageFileNames = new List<string>();
                        foreach (var file in newImageFiles)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                                file.SaveAs(path);
                                imageFileNames.Add(fileName);
                            }
                        }
                        // Cộng thêm hình ảnh mới vào danh sách hình ảnh cũ
                        var currentImages = sanPham.HinhAnh?.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                        currentImages.AddRange(imageFileNames);
                        sanPham.HinhAnh = string.Join("|", currentImages);
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound();
                }
            }

            // Nếu có lỗi, lấy lại danh sách các danh mục để hiển thị trong dropdown list
            var danhMucs = db.tbl_DanhSach.ToList();
            ViewBag.DanhMucChaList = new SelectList(danhMucs, "IDDS", "TenDanhSach");
            return View(sp1);
        }


        public ActionResult Delete(int id)
        {
            tbl_SanPham sp1 = db.tbl_SanPham.FirstOrDefault(x => x.IDSP == id);
            db.tbl_SanPham.Remove(sp1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
