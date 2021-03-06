﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAL.Context;
using DAL.Entities;
using DAL.Services;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace MyMVCBookStore.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public ActionResult Index(string searchString, string sortOption, int page = 1)
        {
            int pageSize = 5;
            var service = new BookService(HttpContext.GetOwinContext().Get<ApplicationDbContext>());
            var result = service.CreateBook().AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                result = service.Search(searchString).AsQueryable();
            }
            switch (sortOption)
            {
                case "name_acs":
                    result = result.OrderBy(p => p.Author.FirstName);
                    break;
                case "name_desc":
                    result = result.OrderByDescending(p => p.Author.FirstName);
                    break;
                case "country_acs":
                    result = result.OrderBy(p => p.Country.Name);
                    break;
                case "country_desc":
                    result = result.OrderByDescending(p => p.Country.Name);
                    break;
                case "title_acs":
                    result = result.OrderBy(p => p.Title);
                    break;
                case "title_desc":
                    result = result.OrderByDescending(p => p.Title);
                    break;
                case "price_acs":
                    result = result.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    result = result.OrderByDescending(p => p.Price);
                    break;
                case "date_acs":
                    result = result.OrderBy(p => p.PublishedDay);
                    break;
                case "date_desc":
                    result = result.OrderByDescending(p => p.PublishedDay);
                    break;
                case "page_acs":
                    result = result.OrderBy(p => p.PageCount);
                    break;
                case "page_desc":
                    result = result.OrderByDescending(p => p.PageCount);
                    break;
                default:
                    result = result.OrderBy(p => p.Id);
                    break;

            }
            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("BookList", result.ToPagedList(page, pageSize))
                : View(result.ToPagedList(page, pageSize));
        }
           

        //public ActionResult Search(string searchString)
        //{
        //    var service = new BookService(HttpContext.GetOwinContext().Get<ApplicationDbContext>());
        //    var result = service.Search(searchString);

        //    return View("Index", result);
        //}
        // GET: Books/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return Content("Siktir");
            }
            var service = new BookService(HttpContext.GetOwinContext().Get<ApplicationDbContext>());
            var result = service.EditDetalisDelObject(book);
            return View(result);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FullName");
            ViewBag.CountryId = new SelectList(db.Countires, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CountryId,AuthorId,Title,Price,PublishedDay,Description,PageCount,Image")] Book book, HttpPostedFileBase upimage)
        {


            if (ModelState.IsValid)
            {
                if (upimage != null)
                {
                    book.Image = new byte[upimage.ContentLength];
                    upimage.InputStream.Read(book.Image, 0, upimage.ContentLength);
                }
                db.Books.Add(book);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FullName", book.AuthorId);
            ViewBag.CountryId = new SelectList(db.Countires, "Id", "Name", book.CountryId);
            return View();
        }

        // GET: Books/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            var service = new BookService(HttpContext.GetOwinContext().Get<ApplicationDbContext>());
            var result = service.EditDetalisDelObject(book);
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FullName", book.AuthorId);
            ViewBag.CountryId = new SelectList(db.Countires, "Id", "Name", book.CountryId);
            return View(result);
        }

        // POST: Books/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CountryId,AuthorId,Title,Price,PublishedDay,Description,PageCount,Image")] Book book, HttpPostedFileBase upimage)
        {
            if (ModelState.IsValid)
            {
                if (upimage != null)
                {
                    book.Image = new byte[upimage.ContentLength];
                    upimage.InputStream.Read(book.Image, 0, upimage.ContentLength);
                }
                db.Books.Attach(book);
                db.Entry(book).State = EntityState.Modified;
                if (upimage == null)
                {
                    db.Entry(book).Property(m => m.Image).IsModified = false;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FirstName", book.AuthorId);
            ViewBag.CountryId = new SelectList(db.Countires, "Id", "Name", book.CountryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            var service = new BookService(HttpContext.GetOwinContext().Get<ApplicationDbContext>());
            var result = service.EditDetalisDelObject(book);
            return View(result);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Book book = await db.Books.FindAsync(id);
            db.Books.Remove(book);
            await db.SaveChangesAsync();
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
