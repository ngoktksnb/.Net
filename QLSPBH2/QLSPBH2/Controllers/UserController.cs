using QLSPBH2;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;

namespace QLSPBH2.Controllers
{
    public class UserController : Controller
    {
        private QLSP1Entities1 db = new QLSP1Entities1();

        // GET: User
        public ActionResult Index(string searchString, int? page, int? pageSize, string sortOrder)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            int pageSizeValue = pageSize ?? 10;
            ViewBag.CurrentPageSize = pageSizeValue;
            var users = db.tbl_NguoiDung.Where(u => u.isDelete == false).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.email.Contains(searchString) || u.hoten.Contains(searchString) || u.mssv.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "email_asc":
                    users = users.OrderBy(u => u.email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.email);
                    break;
                case "name_asc":
                    users = users.OrderBy(u => u.hoten);
                    break;
                case "name_desc":
                    users = users.OrderByDescending(u => u.hoten);
                    break;
                default:
                    users = users.OrderBy(u => u.id);
                    break;
            }

            int pageNumber = (page ?? 1);
            var usersPaged = users.ToPagedList(pageNumber, pageSizeValue);
            return View(usersPaged);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbl_NguoiDung user = db.tbl_NguoiDung.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,email,matkhau,hoten,mssv,isDelete")] tbl_NguoiDung user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbl_NguoiDung user = db.tbl_NguoiDung.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_NguoiDung user = db.tbl_NguoiDung.Find(id);
            if (user != null)
            {
                user.isDelete = true; // Đánh dấu là đã xóa thay vì xóa khỏi cơ sở dữ liệu
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "email,matkhau,hoten,mssv,isDelete")] tbl_NguoiDung user, string status)
        {
            if (ModelState.IsValid)
            {
                user.isDelete = status == "active" ? false : true;
                db.tbl_NguoiDung.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
    }
}
