using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QLSPBH2.Controllers
{
    public class AnhBiaController : Controller
    {
        private QLSP1Entities1 db = new QLSP1Entities1();

        //-----------------------------
        public ActionResult Index(string searchString, int? page, int? pageSize, string sortOrder)
        {
            // Lưu trữ từ khóa tìm kiếm và sắp xếp vào ViewBag để giữ lại giá trị khi phân trang
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            // Số lượng mục hiển thị trên mỗi trang (mặc định là 10 nếu không được cung cấp)
            int pageSizeValue = pageSize ?? 10;
            ViewBag.CurrentPageSize = pageSizeValue;

            // Lấy danh sách ảnh bìa từ database
            var anhBias = db.tbl_AnhBia.AsQueryable();

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                anhBias = anhBias.Where(ab => ab.IDAB.ToString().Contains(searchString) || ab.HinhAnhBia.Contains(searchString));
            }

            // Xử lý sắp xếp
            switch (sortOrder)
            {
                case "id_asc":
                    anhBias = anhBias.OrderBy(ab => ab.IDAB);
                    break;
                case "id_desc":
                    anhBias = anhBias.OrderByDescending(ab => ab.IDAB);
                    break;
                default:
                    anhBias = anhBias.OrderBy(ab => ab.IDAB);
                    break;
            }

            // Trang hiện tại (nếu không được cung cấp thì mặc định là trang 1)
            int pageNumber = (page ?? 1);

            // Sử dụng PagedList để phân trang
            var anhBiasPaged = anhBias.ToPagedList(pageNumber, pageSizeValue);

            // Trả về view với danh sách đã phân trang
            return View(anhBiasPaged);
        }
        //----------------------------




        public ActionResult CreateAnhBia()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAnhBia(tbl_AnhBia anhBia, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Đường dẫn lưu trữ hình ảnh
                var imagePath = Server.MapPath("~/Images/");
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(imagePath, fileName);
                    imageFile.SaveAs(path);
                    anhBia.HinhAnhBia = fileName;
                }

                // Lưu ảnh bìa vào cơ sở dữ liệu
                db.tbl_AnhBia.Add(anhBia);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(anhBia);
        }





        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2
        public ActionResult CreateAnhBia2()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAnhBia2(tbl_AnhBia anhBia, IEnumerable<HttpPostedFileBase> imageFiles)
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

                // Lưu tên các tệp vào thuộc tính HinhAnhBia của ảnh bìa, cách nhau bằng dấu |
                anhBia.HinhAnhBia = string.Join("|", imageFileNames);

                // Lưu ảnh bìa vào cơ sở dữ liệu
                db.tbl_AnhBia.Add(anhBia);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(anhBia);
        }

        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2
        // cách thêm mới ảnh bìa thứ 2


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_AnhBia anhBia = db.tbl_AnhBia.Find(id);
            if (anhBia == null)
            {
                return HttpNotFound();
            }
            return View(anhBia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tbl_AnhBia anhBia, IEnumerable<string> deleteImages, IEnumerable<HttpPostedFileBase> newImageFiles)
        {
            if (ModelState.IsValid)
            {
                var anhBiaToUpdate = db.tbl_AnhBia.Find(anhBia.IDAB);
                if (anhBiaToUpdate != null)
                {
                    // Xử lý xóa các hình ảnh đã chọn
                    if (deleteImages != null)
                    {
                        foreach (var image in deleteImages)
                        {
                            var imagePath = Path.Combine(Server.MapPath("~/Images/"), image);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                                anhBiaToUpdate.HinhAnhBia = anhBiaToUpdate.HinhAnhBia.Replace(image + "|", "").Replace(image, ""); // Xóa tên hình ảnh khỏi danh sách HinhAnh trong SQL
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
                        var currentImages = anhBiaToUpdate.HinhAnhBia?.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                        currentImages.AddRange(imageFileNames);
                        anhBiaToUpdate.HinhAnhBia = string.Join("|", currentImages);
                    }

                    db.Entry(anhBiaToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(anhBia);
        }




        public ActionResult Delete(int id)
        {
            tbl_AnhBia sp1 = db.tbl_AnhBia.FirstOrDefault(x => x.IDAB == id);
            db.tbl_AnhBia.Remove(sp1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Slider()
        {
            var anhBias = db.tbl_AnhBia.ToList();
            return View(anhBias);
        }


    }

}