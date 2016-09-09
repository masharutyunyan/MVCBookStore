﻿using System;
using System.Collections.Generic;
using System.Data;
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

namespace MyMVCBookStore.Controllers
{
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        public  ActionResult Index()
        {

            var books = db.Books.Include(b => b.Author).Include(b => b.Country);
            List<Book> bookResult = new List<Book>();
            foreach (var item in books)
            {
                bookResult.Add(new Book
                {
                    Title = item.Title,
                    PageCount = item.PageCount,
                    Description = item.Description,
                    PublishedDay = item.PublishedDay,
                    Author = item.Author,
                    Image = item.Image,
                    Price = item.Price,
                    Country = item.Country
                });
            }
            //var service = new BookService(HttpContext.GetOwinContext().Get<ApplicationDbContext>());
            //service.CreateBook(model.Title,model.PageCount,model.Description,model.PublishedDay,model.Author,model.Image,model.Price);
            return View(bookResult);
        }

        // GET: Books/Details/5
        public async Task<ActionResult> Details(int? id)
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
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
           
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FirstName");
            ViewBag.CountryId = new SelectList(db.Countires, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CountryId,AuthorId,Title,Price,PublishedDay,Description,PageCount,Image")] Book book,HttpPostedFileBase upimage)
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
            
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FirstName", book.AuthorId);
            ViewBag.CountryId = new SelectList(db.Countires, "Id", "Name", book.CountryId);
            return View(book);
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
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "FirstName", book.AuthorId);
            ViewBag.CountryId = new SelectList(db.Countires, "Id", "Name", book.CountryId);
            return View(book);
        }

        // POST: Books/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CountryId,AuthorId,Title,Price,PublishedDay,Description,PageCount,Image")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
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
            return View(book);
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
