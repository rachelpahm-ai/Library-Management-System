using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class LoanDetailsController : Controller
    {
        private DBLibraryManagementEntities db = new DBLibraryManagementEntities();

        // GET: LoanDetails
        public ActionResult Index()
        {
            var loanDetail = db.LoanDetail.Include(l => l.Loan).Include(l => l.Book);
            return View(loanDetail.ToList());
        }

        // GET: LoanDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanDetail loanDetail = db.LoanDetail.Find(id);
            if (loanDetail == null)
            {
                return HttpNotFound();
            }
            return View(loanDetail);
        }

        // GET: LoanDetails/Create
        public ActionResult Create()
        {
            ViewBag.LoanID = new SelectList(db.Loan, "LoanID", "Status");
            ViewBag.BookID = new SelectList(db.Book, "BookID", "Title");
            return View();
        }

        // POST: LoanDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoanID,BookID,ReturnDate,FineAmount")] LoanDetail loanDetail)
        {
            if (ModelState.IsValid)
            {
                // 1. Dùng LINQ tìm cuốn sách độc giả muốn mượn trong kho (Bỏ chữ 's' ở db.Book)
                var book = db.Book.Find(loanDetail.BookID);

                // 2. NGHIỆP VỤ BẮT BUỘC: Kiểm tra số lượng
                if (book.AvailableQuantity <= 0)
                {
                    // Báo lỗi đỏ chót trên màn hình, chặn không cho lưu
                    ModelState.AddModelError("", "Lỗi nghiệp vụ: Cuốn sách này đã hết trong kho, không thể cho mượn!");
                    ViewBag.BookID = new SelectList(db.Book, "BookID", "Title", loanDetail.BookID);
                    ViewBag.LoanID = new SelectList(db.Loan, "LoanID", "Status", loanDetail.LoanID);
                    return View(loanDetail);
                }

                // 3. Nếu sách còn -> Cho mượn và TỰ ĐỘNG TRỪ số lượng đi 1
                book.AvailableQuantity -= 1;

                // 4. Lưu phiếu mượn và cập nhật lại kho sách
                db.LoanDetail.Add(loanDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookID = new SelectList(db.Book, "BookID", "Title", loanDetail.BookID);
            ViewBag.LoanID = new SelectList(db.Loan, "LoanID", "Status", loanDetail.LoanID);
            return View(loanDetail);
        }

        // GET: LoanDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanDetail loanDetail = db.LoanDetail.Find(id);
            if (loanDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.LoanID = new SelectList(db.Loan, "LoanID", "Status", loanDetail.LoanID);
            ViewBag.BookID = new SelectList(db.Book, "BookID", "Title", loanDetail.BookID);
            return View(loanDetail);
        }

        // POST: LoanDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoanID,BookID,ReturnDate,FineAmount")] LoanDetail loanDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loanDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LoanID = new SelectList(db.Loan, "LoanID", "Status", loanDetail.LoanID);
            ViewBag.BookID = new SelectList(db.Book, "BookID", "Title", loanDetail.BookID);
            return View(loanDetail);
        }

        // GET: LoanDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanDetail loanDetail = db.LoanDetail.Find(id);
            if (loanDetail == null)
            {
                return HttpNotFound();
            }
            return View(loanDetail);
        }

        // POST: LoanDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoanDetail loanDetail = db.LoanDetail.Find(id);
            db.LoanDetail.Remove(loanDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // ==========================================
        // BỔ SUNG CHO NHÁNH: feature-lichsu-thongke
        // ==========================================

        // 1. GET: LoanDetails/History (Lịch sử mượn/trả của độc giả)
        public ActionResult History()
        {
            var loanDetails = db.LoanDetail.Include(l => l.Loan).Include(l => l.Book);
            return View(loanDetails.ToList());
        }

        // 2. GET: LoanDetails/Dashboard (Dashboard thống kê cho Admin)
        public ActionResult Dashboard()
        {
            // Thống kê tổng số sách đang cho mượn (ReturnDate chưa có/chưa trả)
            ViewBag.TotalBooksBorrowed = db.LoanDetail.Count(ld => ld.ReturnDate == null);

            // Thống kê tổng tiền phạt thu được
            ViewBag.TotalFinesCollected = db.LoanDetail.Sum(ld => (decimal?)ld.FineAmount) ?? 0;

            return View();
        }
    }
}
