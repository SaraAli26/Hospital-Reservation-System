using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LeHospital.Models;

namespace LeHospital.Controllers
{
    public class SpecialtiesController : Controller
    {
        private LeHospitalEntities db = new LeHospitalEntities();

        // GET: Specialties
        public ActionResult Index()
        {
            return View(db.Specialties.ToList());
        }

        // GET: Specialties/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specialty specialty = db.Specialties.Find(id);
            if (specialty == null)
            {
                return HttpNotFound();
            }
            return View(specialty);
        }

        // GET: Specialties/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                specialty.Id = Guid.NewGuid();
                db.Specialties.Add(specialty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(specialty);
        }

        // GET: Specialties/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specialty specialty = db.Specialties.Find(id);
            if (specialty == null)
            {
                return HttpNotFound();
            }
            return View(specialty);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(specialty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(specialty);
        }

        // GET: Specialties/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specialty specialty = db.Specialties.Find(id);
            if (specialty == null)
            {
                return HttpNotFound();
            }
            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Specialty specialty = db.Specialties.Find(id);
            db.Specialties.Remove(specialty);
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
