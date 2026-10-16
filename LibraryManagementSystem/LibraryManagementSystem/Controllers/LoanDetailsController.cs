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
                db.LoanDetail.Add(loanDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LoanID = new SelectList(db.Loan, "LoanID", "Status", loanDetail.LoanID);
            ViewBag.BookID = new SelectList(db.Book, "BookID", "Title", loanDetail.BookID);
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
    }
}
