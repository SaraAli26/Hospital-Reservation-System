using System;
using System.Collections;
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
    public class ClinicsController : Controller
    {
        private LeHospitalEntities db = new LeHospitalEntities();

        // GET: Clinics
        public ActionResult Index()
        {
            var clinics = db.Clinics.Include(c => c.Doctor).Include(c => c.Specialty);
            return View(clinics.ToList());
        }

        // GET: Clinics/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clinic clinic = db.Clinics.Find(id);
            if (clinic == null)
            {
                return HttpNotFound();
            }
            return View(clinic);
        }

        // GET: Clinics/Create
        public ActionResult Create()
        {

            //Newly create list for AM or PM
            List<string> amPm = new List<string>();
            amPm.Add("AM");
            amPm.Add("PM");
            //Then add it to a viewbag s it can be added to the view when adding the availble clinics by the admin
            ViewBag.AmPm = new SelectList(amPm);

            //Newly created list for the availble times in the clinic
            List<string> timesAvailable = new List<string>();
            timesAvailable.Add("7");
            timesAvailable.Add("7:30");
            timesAvailable.Add("8");
            timesAvailable.Add("8:30");
            timesAvailable.Add("9");
            timesAvailable.Add("9:30");
            timesAvailable.Add("10");
            timesAvailable.Add("10:30");
            //Then add it to a viewbag s it can be added to the view when adding the availble clinics by the admin
            ViewBag.Time = new SelectList(timesAvailable);
           

            ViewBag.DoctorId = new SelectList(db.Doctors, "Id", "Name");
            ViewBag.SpecialityId = new SelectList(db.Specialties, "Id", "Name");
            return View();
        }

        // POST: Clinics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClinicNo,DateTime,DoctorId,SpecialityId,AmPm,Time")] Clinic clinic)
        {
            if (ModelState.IsValid)
            {
                clinic.Id = Guid.NewGuid();
                db.Clinics.Add(clinic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DoctorId = new SelectList(db.Doctors, "Id", "Name", clinic.DoctorId);
            ViewBag.SpecialityId = new SelectList(db.Specialties, "Id", "Name", clinic.SpecialityId);
            return View(clinic);
        }

        //Getting corresponding doctors for the specilaity in cascading list when adding availble clinics by admin
        [HttpPost]
        public JsonResult GetDoctors(string SpecialtiesId)
        {
            Guid SpecialtId;
            List<SelectListItem> doctorsNames = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(SpecialtiesId))
            {
                SpecialtId = Guid.Parse(SpecialtiesId);
                List<Doctor> doctors = db.Doctors.Where(x => x.SpecialityId == SpecialtId).ToList();
                doctors.ForEach(x =>
                {
                    doctorsNames.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                });
            }
            return Json(doctorsNames, JsonRequestBehavior.AllowGet);
        }
    

        // GET: Clinics/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clinic clinic = db.Clinics.Find(id);
            if (clinic == null)
            {
                return HttpNotFound();
            }
            List<string> amPm = new List<string>();
            amPm.Add("AM");
            amPm.Add("PM");
            //Then add it to a viewbag s it can be added to the view when adding the availble clinics by the admin
            ViewBag.AmPm = new SelectList(amPm);

            //Newly created list for the availble times in the clinic
            List<string> timesAvailable = new List<string>();
            timesAvailable.Add("7");
            timesAvailable.Add("7:30");
            timesAvailable.Add("8");
            timesAvailable.Add("8:30");
            timesAvailable.Add("9");
            timesAvailable.Add("9:30");
            timesAvailable.Add("10");
            timesAvailable.Add("10:30");
            //Then add it to a viewbag s it can be added to the view when adding the availble clinics by the admin
            ViewBag.Time = new SelectList(timesAvailable);
           

            ViewBag.DoctorId = new SelectList(db.Doctors, "Id", "Name", clinic.DoctorId);
            ViewBag.SpecialityId = new SelectList(db.Specialties, "Id", "Name", clinic.SpecialityId);
            return View(clinic);
        }

        // POST: Clinics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClinicNo,DateTime,DoctorId,SpecialityId,,AmPm,Time")] Clinic clinic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clinic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DoctorId = new SelectList(db.Doctors, "Id", "Name", clinic.DoctorId);
            ViewBag.SpecialityId = new SelectList(db.Specialties, "Id", "Name", clinic.SpecialityId);
            return View(clinic);
        }

        // GET: Clinics/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clinic clinic = db.Clinics.Find(id);
            if (clinic == null)
            {
                return HttpNotFound();
            }
            return View(clinic);
        }

        // POST: Clinics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Clinic clinic = db.Clinics.Find(id);
            db.Clinics.Remove(clinic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Viewing all avaible appointments that hasn't been reserved yet
        public ActionResult ShowClinics(Guid? id)
        {
            var clinics = db.Clinics.Include(c => c.Doctor).Where(c => c.SpecialityId == id).Where(c => c.Status == false);
            return View(clinics.ToList());
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
