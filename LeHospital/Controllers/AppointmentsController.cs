using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LeHospital.Models;
using Microsoft.AspNet.Identity;

namespace LeHospital.Controllers
{
    public class AppointmentsController : Controller
    {
        private LeHospitalEntities db = new LeHospitalEntities();

        // GET: Appointments
        public ActionResult Index()
        {
            var appointments = db.Appointments.Include(a => a.Clinic).Include(a => a.Patient);
            return View(appointments.ToList());
        }

        // GET: Appointments/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "ClinicNo");
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "Name");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PatientId,ClinicId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Id = Guid.NewGuid();
                // Oncethe clinic is reserved, change th boolen status to true, which means its no longer available

               // appointment.Clinic.Status = true;
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "ClinicNo", appointment.ClinicId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "Name", appointment.PatientId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "ClinicNo", appointment.ClinicId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "Name", appointment.PatientId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PatientId,ClinicId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "ClinicNo", appointment.ClinicId);
            ViewBag.PatientId = new SelectList(db.Patients, "Id", "Name", appointment.PatientId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //MAking appointments contains two methods to do so, the first one is to only ass the id of the clinic so oit can be bind
        // with the patient's file numberlater.
        // Method 1 fprmake appointment
        [Authorize]
        public ActionResult MakeAppointment(Guid? id)
        {
            //To get the current logged in user id in case user id has a foreign key from AspUser table.
            //Working for the case that the patient has to log in to make appointment by her/his self.
            //var userId = User.Identity.GetUserId();
            //var PatientUser = db.Patients.Where(s => s.AspNetUsersId == userId).FirstOrDefault<Patient>();
            //appointment.PatientId = PatientUser.Id;

            TempData["ClinicId"] = id;
            return RedirectToAction("AddAppointment");
        }

        //Methos 2 for make appointment
        [Authorize]
        public ActionResult AddAppointment()
        {
            MakeReservation mr = new MakeReservation();
            Guid id = Guid.Parse(TempData["ClinicId"].ToString());
            ViewBag.ip = id;
            mr.appId = id;

            var ClinicInfo = db.Clinics.Where(s => s.Id == id).FirstOrDefault<Clinic>();
                       
            return View(mr);
        }

        //To cancel appointment, by patients but via reeiptionist
        public ActionResult CancelAppoitment()
        {
            return View();
        }



        [Authorize]
        [HttpPost]
        public ActionResult AddAppointment(MakeReservation mr)
        {
            Guid id = mr.appId;
            int fileNum = mr.fileno;

            if (ModelState.IsValid)
            {
                Appointment appointment = new Appointment();
                appointment.Id = Guid.NewGuid();
                appointment.ClinicId = id;

                //Get the patient Id using the file number
                var PatientUser = db.Patients.Where(s => s.FileNumber == fileNum).FirstOrDefault<Patient>();
                appointment.PatientId = PatientUser.Id;

                //Get the receiptionist ID using the logged in user, P.s receiptionist and Admin are objects from AspUser table
                var userId = User.Identity.GetUserId();
                appointment.ReceiptionistId = userId;

                //Change the default value of status of clinic from false to true, meaning its now reserved
                var theClinic = db.Clinics.Where(a => a.Id == id).FirstOrDefault<Clinic>();
                theClinic.Status = true;
                

                db.Appointments.Add(appointment);
                db.SaveChanges();
            }
            return RedirectToAction("Index","Appointments");

        }

        // Takes the Patient Id to display all his/her appointments
        //public ActionResult ViewPatientAppointments(Guid? id)
        //{
        //    return View();
        //}

        // Using loggedin Docotor Id to disply the appointemnts that were reserved by Patients for his clinic
        public ActionResult ViewDoctorAppointments()
        {
            var userId = User.Identity.GetUserId();
            var DoctorUser = db.Doctors.Where(s => s.AspNetUsersId == userId).FirstOrDefault<Doctor>();
            Guid y = DoctorUser.Id;

            var appointments = db.Appointments.Include(c => c.Patient).Include( a => a.Clinic).Where(c => c.Clinic.DoctorId == y);
            return View(appointments.ToList());
            
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
