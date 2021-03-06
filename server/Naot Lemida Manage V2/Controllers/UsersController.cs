﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Naot_Lemida_Manage_V2.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI.WebControls;
using System.IO;
using ClosedXML.Excel;

namespace Naot_Lemida_Manage_V2.Controllers
{
    public class UsersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(r => r.Roles).Include(s => s.School).Include(y => y.YearOfStudy).OrderBy(r=>r.Name);
            ViewBag.numOfManagers = db.Users.Include(r => r.Roles).Count(u => u.IdentityRole.Name.Equals("מנהל"));
            ViewBag.numOfCoordinators = db.Users.Include(r => r.Roles).Count(u => u.IdentityRole.Name.Equals("רכז"));
            ViewBag.numOfTeachers = db.Users.Include(r => r.Roles).Count(u => u.IdentityRole.Name.Equals("מורה"));
            ViewBag.numOfPupils = db.Users.Include(r => r.Roles).Count(u => u.IdentityRole.Name.Equals("תלמיד"));
            return View(users.ToList());
        }


        // GET: Cities/Details/5
        public ActionResult Details(String id)
        {
            if (id==null||id.Equals(""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView(user);
        }

        
        // GET: Cities/Edit/5
        public ActionResult Edit(String id)
        {
            if (id == null || id.Equals(""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.SchoolID= new SelectList(db.Schools, "ID", "Name", user.SchoolID);
            ViewBag.IdentityRoleID = new SelectList(db.Roles, "ID", "Name", user.IdentityRoleID);
            ViewBag.YearOfStudyID = new SelectList(db.YearOfStudies, "ID", "Year", user.YearOfStudyID);
            return PartialView(user);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name, LastName,Phone, Mail, SchoolID, IdentityRoleID, YearOfStudyID")]ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                ApplicationUser Model = UserManager.FindById(user.Id);
                Model.Name = user.Name;
                Model.LastName = user.LastName;
                Model.Phone = user.Phone;
                Model.Mail = user.Mail;
                Model.SchoolID = user.SchoolID;
                Model.IdentityRoleID = user.IdentityRoleID;
                Model.YearOfStudyID = user.YearOfStudyID;
                IdentityResult result = await UserManager.UpdateAsync(Model);
                return PartialView("Success");
            }
            return View(user);
        }

        // GET: Cities/Delete/5
        public ActionResult Delete(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.SchoolID = new SelectList(db.Schools, "ID", "Name", user.SchoolID);
            ViewBag.IdentityRoleID = new SelectList(db.Roles, "ID", "Name", user.IdentityRoleID);
            ViewBag.YearOfStudyID = new SelectList(db.YearOfStudies, "ID", "Year", user.YearOfStudyID);
            return PartialView(user);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(String id)
        {
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser Model = UserManager.FindById(id);
            IdentityResult result = await UserManager.DeleteAsync(Model);
            return PartialView("Success");
        }
        public ActionResult ExportData()
        {
            
            DataSet ds = new DataSet();
            ds.Tables.Add(ApplicationUser.getManagersTable());
            ds.Tables.Add(ApplicationUser.getCoordinatorsTable());
            ds.Tables.Add(ApplicationUser.getTeachersTable());
            ds.Tables.Add(ApplicationUser.getPupilsTable());

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= רשימת משתמשים.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            } 

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