using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QMS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Dynamic;
using RnDCore3;
using System.Text;



namespace QMS.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly Datas.ApplicationDbContext _db;
        private readonly Interfaces.IFunc _IFunc;
        private readonly string cS;
        public HomeController(Datas.ApplicationDbContext db, ILogger<HomeController> logger, Interfaces.IFunc ifunc)
        {
            _logger = logger;
            _db = db;
            _IFunc = ifunc; cS = _IFunc.connStr;
        }

        //public IActionResult login(int id = 0)
        //{
        //    ViewBag.status = "";
        //    ViewBag.id = id;
        //    //RnDCore3.email.emailSend("donotreply@collins.com", "novi.susanti@collins.com", "", "Document Request", "Please Review this Document");
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult login(int id = 0, string login__username = "", string login__password = "")
        //{
        //    bool stat = false;
        //    try
        //    {
        //        // ldapConnection.Path = "";
        //        // Pass the username and password to the DirectoryEntry for authentication.
        //        DirectoryEntry myLdapConnection = new DirectoryEntry("LDAP://YPH011GUAIN102:389/DC=utcain,DC=com", login__username, login__password);

        //        DirectorySearcher search = new DirectorySearcher(myLdapConnection);
        //        search.Filter = "(SAMAccountName=" + login__username + ")";
        //        SearchResult result = search.FindOne();

        //        if (result != null)
        //        {
        //            stat = true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }


        //    //if (RnDCore3.login.WindowsLogin(login__username, login__password, "utcain")) {
        //    if (stat)
        //    {
        //        RnDCore3.web.cookiesSet(Response, "qmsuser", login__username, 7200);
        //        var temp = _db.DOCREQS.Find(id);
        //        if (temp != null)
        //        {
        //            return Redirect("../Approval/" + id);
        //        }
        //        //Models.DOCREQ d = new DOCREQ();
        //        //d.Reqtype = "test";
        //        //_db.Add(d);
        //        //_db.SaveChanges();
        //        return RedirectToAction("List");

        //    }
        //    else
        //    {
        //        ViewBag.status = "wrong user name or password";
        //        return View();
        //    }


        //    //RnDCore3.email.emailSend("donotreply@collins.com", "novi.susanti@collins.com", "", "Document Request", "Please Review this Document");

        //}
        public IActionResult login(int id = 0) {
            ViewBag.status = TempData["status"] as string ?? "";
            ViewBag.id = id;
            ViewBag.cs = cS;
            //RnDCore3.email.emailSend("donotreply@collins.com", "novi.susanti@collins.com", "", "Document Request", "Please Review this Document");
            return View();
        }

        [HttpPost]
        public IActionResult login(int id = 0, string login__username = "", string login__password = "")
        {
            //var AccountName = GetAccountName(login__username, "LDAP://YPH011GUAIN102:389/DC=utcain,DC=com"); Ini coba-coba get Nama
            if (RnDCore3.login.WindowsLogin(login__username, login__password, "utcain"))
            {
                var nmKry = _db.EMPL.FirstOrDefault(x => x.WinAccount == login__username)?.NM_KRY;
                RnDCore3.web.cookiesSet(Response, "qmsuser", login__username, 7200);
                RnDCore3.web.cookiesSet(Response, "qmsuserfullname", nmKry ?? login__username, 7200);
                var temp = _db.DOCREQS.Find(id);
                if (temp != null)
                {
                    return Redirect("../Approval/" + id);
                }
                //Models.DOCREQ d = new DOCREQ();
                //d.Reqtype = "test";
                //_db.Add(d);
                //_db.SaveChanges();
                return RedirectToAction("List");

            }
            else
            {
                TempData["status"] = "wrong user name or password";
                return RedirectToAction("login", new { id = id });
            }


            //RnDCore3.email.emailSend("donotreply@collins.com", "novi.susanti@collins.com", "", "Document Request", "Please Review this Document");

        }
        public static string GetAccountName(string strUserName, string ldapPath)
        {
            using (DirectoryEntry entry = new DirectoryEntry(ldapPath))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = $"(sAMAccountName={strUserName})";
                    searcher.PropertiesToLoad.Add("displayName");
                    searcher.PageSize = 1; // Improve performance for single result

                    SearchResult result = searcher.FindOne();
                    if (result != null && result.Properties.Contains("displayName") && result.Properties["displayName"].Count > 0)
                    {
                        return result.Properties["displayName"][0].ToString();
                    }
                }
            }
            return null;
        }

        public IActionResult newpage (int id = 0) {
            return View();
        }
        public IActionResult request(int id = 0) {

            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser"))) {
                return RedirectToAction("login");
            }
            ViewBag.docown = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER ORDER BY Name", cS);
            ViewBag.docitc = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE ITCapprovalaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.docqms = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE AS9100approvalaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.docnadcap = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE NADCAPapprovalaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.docon = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE Doccontrollaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));

            if (id <= 0) {
                return View(new Models.DOCREQ());
            } else {
                var data = _db.DOCREQS.Find(id);
                return View(data);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult request(int id,  [Bind("Id,Reqtype,LinkDoc,Functgroup,FunctgroupOthers,Docnumber,Associateddoc,Doctype,Superseededdoc,Reqinitiatedate,Prioritydoc,AuditNo,Docowner,Docownerdate,Finalapprovername,SMEname,ITCname,QMSASname,QMSNADCAPname,Doccontrollername,Listdoc1,Listdocaffected1,Listdoc2,Listdocaffected2,Listdoc3,Listdocaffected3,Listdoc4,Listdocaffected4,Listdoc5,Listdocaffected5,Listdoc6,Listdocaffected6,Listdoc7,Listdocaffected7,Listdoc8,Listdocaffected8,Listdoc9,Listdocaffected9,Listdoc10,Listdocaffected10,Finalapproverdecision,Finalapproverdate,SMEapproverdecision,SMEdate,ITCapproverdecision,ITCdate,QMSASapproverdecision,QMSASdate,QMSNADCAPapproverdecision,QMSNADCAPdate,Doccontrolleapproverdecision,Doccontrollerdate,documentReason")] DOCREQ dt, IFormFile Fileupload = null)
        {
            // Check authentication again on POST
            var currentUser = RnDCore3.web.cookiesGet(Request, "qmsuser");
            if (string.IsNullOrEmpty(currentUser))
            {
                // Assign the session expired message to ViewBag
                ViewBag.SessionExpired = "Your session has expired. Please log in again.";
                return View("login");
            }

            ViewBag.docown = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER ORDER BY Name", cS);
            ViewBag.docitc = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE ITCapprovalaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.docqms = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE AS9100approvalaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.docnadcap = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE NADCAPapprovalaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.docon = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE Doccontrollaccess = 'Yes' ORDER BY Name", cS);
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
            if (ModelState.IsValid) {
                if(string.IsNullOrEmpty(dt.Docowner))
                    dt.Docowner = RnDCore3.web.cookiesGet(Request, "qmsuser");
                dt.Docownerdate = DateTime.Now;
                if (id > 0) {
                    if(Fileupload != null) {
                        string fileName = Fileupload.FileName;
                        fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetFileName(fileName);
                        string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", fileName);
                        if(!string.IsNullOrEmpty(dt.LinkDoc)) {
                            string uploadpath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", dt.LinkDoc);
                            System.IO.File.Delete(uploadpath2);
                        }
                        dt.LinkDoc = fileName;
                        var stream = new FileStream(uploadpath, FileMode.Create);

                        Fileupload.CopyToAsync(stream);
                    }
                    _db.DOCREQS.Update(dt);
                    _db.SaveChanges();
                    sendemail(cS, dt.Id);
                    
                    return RedirectToAction("List");
                } else {
                    if (Fileupload != null) {
                        string fileName = Fileupload.FileName;
                        fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetFileName(fileName);
                        string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", fileName);
                        if (!string.IsNullOrEmpty(dt.LinkDoc)) {
                            string uploadpath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", dt.LinkDoc);
                            System.IO.File.Delete(uploadpath2);
                        }
                        dt.LinkDoc = fileName;
                        var stream = new FileStream(uploadpath, FileMode.Create);

                        Fileupload.CopyToAsync(stream);
                    }
                    dt.Reqinitiatedate = DateTime.Now;
                    _db.Add(dt);
                    _db.SaveChanges();
                    sendemail(cS, dt.Id);
                    
                    return RedirectToAction("newpage");
                }
            } else {
                return View(dt);
            }
        }
        public IActionResult List(string id = "") {
            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser"))) {
                return RedirectToAction("login");
            }
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
            //ViewBag.cs = cS;
            //ViewBag.usr = RnDCore3.web.cookiesGet(Request, "qmsuser");
            IEnumerable<DOCREQ> data = _db.DOCREQS;

            // Build a dictionary of document owner/requestor username to displayName
            var ListEMPL = _db.EMPL.Where(e => e.WinAccount != null).Select(e => e.WinAccount).Distinct().ToList();

            var displayNameDict = new Dictionary<string, string>();
            foreach (var username in ListEMPL)
            {
                var displayName = _db.EMPL
                    .Where(e => e.WinAccount == username)
                    .Select(e => e.NM_KRY)
                    .FirstOrDefault();
                displayNameDict[username] = displayName ?? username;
            }
            ViewBag.DisplayNames = displayNameDict;

            return View(data);
        }

        public IActionResult Deletelist(string id = "") {
            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser"))) {
                return RedirectToAction("login");
            }
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));

            IEnumerable<DOCREQ> data = _db.DOCREQS;
            return View(data);
        }

        public IActionResult delete(int id = 0) {
            var data = _db.DOCREQS.Find(id);
            if(data == null) {
                return Ok("error");
            } else {
                _db.Remove(data);
                _db.SaveChanges();
                return RedirectToAction("Deletelist");
            }
        }

        //COBAAN DULU JALAN G YA UNTUK CCR REQUEST
        //public IActionResult CCRNew(string id = "")
        //{
        //    //if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser")))
        //    //{
        //    //    return RedirectToAction("login");
        //    //}
        //    //ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));

        //    //IEnumerable<CCRREQ> data = _db.CCRREQS;
          
        //    return View();
        //}

        public IActionResult Approval(int id = 0) {
            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser"))) {
                return RedirectToAction("login");
            }
            var data = _db.DOCREQS.Find(id);
            ViewBag.loginuser = RnDCore3.web.cookiesGet(Request, "qmsuser");
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
            if (data == null) return NotFound();
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approval(int id, [Bind("Finalapproverdecision,Finalapprovernote,SMEapproverdecision,SMEapprovernote,ITCapproverdecision,ITCapprovernote,QMSASapproverdecision,QMSASapprovernote,QMSNADCAPapproverdecision,QMSNADCAPapprovernote,Doccontrolleapproverdecision,Doccontrolleapprovernote,TechnicalData,SmartsolveUpload")] DOCREQ dt)
        {
            DOCREQ data = _db.DOCREQS.Find(id);
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
            if (data == null) return NotFound();
            if (data.Finalapproverdecision != dt.Finalapproverdecision) {
                data.Finalapproverdecision = dt.Finalapproverdecision;
                data.Finalapprovername = RnDCore3.web.cookiesGet(Request, "qmsuser");
                data.Finalapproverdate = DateTime.Now;
            }
            data.Finalapprovernote = dt.Finalapprovernote;

            if (data.SMEapproverdecision != dt.SMEapproverdecision) {
                data.SMEapproverdecision = dt.SMEapproverdecision;
                data.SMEname = RnDCore3.web.cookiesGet(Request, "qmsuser");
                data.SMEdate = DateTime.Now;
            }
            data.SMEapprovernote = dt.SMEapprovernote;
            if (data.ITCapproverdecision != dt.ITCapproverdecision) {
                data.ITCapproverdecision = dt.ITCapproverdecision;
                data.ITCname = RnDCore3.web.cookiesGet(Request, "qmsuser");
                data.ITCdate = DateTime.Now;
            }
            data.ITCapprovernote = dt.ITCapprovernote;
            data.TechnicalData = dt.TechnicalData;
            data.SmartsolveUpload = dt.SmartsolveUpload;

            if (data.QMSASapproverdecision != dt.QMSASapproverdecision) {
                data.QMSASapproverdecision = dt.QMSASapproverdecision;
                data.QMSASname = RnDCore3.web.cookiesGet(Request, "qmsuser");
                data.QMSASdate = DateTime.Now;
            }
            data.QMSASapprovernote = dt.QMSASapprovernote;

            if (data.QMSNADCAPapproverdecision != dt.QMSNADCAPapproverdecision) {
                data.QMSNADCAPapproverdecision = dt.QMSNADCAPapproverdecision;
                data.QMSNADCAPname = RnDCore3.web.cookiesGet(Request, "qmsuser");
                data.QMSNADCAPdate = DateTime.Now;
            }
            data.QMSNADCAPapprovernote = dt.QMSNADCAPapprovernote;

            if (data.Doccontrolleapproverdecision != dt.Doccontrolleapproverdecision) {
                data.Doccontrolleapproverdecision = dt.Doccontrolleapproverdecision;
                data.Doccontrollername = RnDCore3.web.cookiesGet(Request, "qmsuser");
                data.Doccontrollerdate = DateTime.Now;
            }
            data.Doccontrolleapprovernote = dt.Doccontrolleapprovernote;
            if (data.Finalapproverdecision == "reject" || data.SMEapproverdecision == "reject" || data.ITCapproverdecision == "reject" || data.QMSASapproverdecision == "reject" || data.QMSNADCAPapproverdecision == "reject" || data.Doccontrolleapproverdecision == "reject")
            {
                data.Closestatus = "Reject";
                if (data.Closedate == null) {
                    data.Closedate = DateTime.Now;
                }

            } else if (data.Finalapproverdecision == "approved" && data.SMEapproverdecision == "approved" && data.ITCapproverdecision == "approved" && data.QMSASapproverdecision == "approved" && data.QMSNADCAPapproverdecision == "approved" && data.Doccontrolleapproverdecision == "approved")
            {
                data.Closestatus = "approved";
                if (data.Closedate == null) {
                    data.Closedate = DateTime.Now;
                }
            } else {
                data.Closestatus = "On Progress";
                data.Closedate = null;
            }
            _db.DOCREQS.Update(data);
            _db.SaveChanges();
            sendemail(cS, id);
            //return Ok("Approved -" + data.Finalapproverdecision);
            return RedirectToAction("List");
        }
        public IActionResult UPDATE() {
            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser"))) {
                return RedirectToAction("login");
            }
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
            var modelawal = new Models.DOCREQ();
            return View(modelawal);
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Edituser(string id = "") {
            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser"))) {
                return RedirectToAction("login");
            }
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
            if (string.IsNullOrEmpty(id)) {
                return View(new Models.DOCREQUSER());
            } else {
                var data = _db.DOCREQUSERs.Find(id);
                return View(data);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edituser(string id, DOCREQUSER dt) {
            if (ModelState.IsValid) {
                if (!string.IsNullOrEmpty(id)) {
                    _db.DOCREQUSERs.Update(dt);
                    _db.SaveChanges();
                    return RedirectToAction("Listusername");
                } else {
                    var nv = _db.DOCREQUSERs.Find(dt.Userid);
                    ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
                    if (nv == null) {
                        _db.Add(dt);
                        _db.SaveChanges();
                        return View(new DOCREQUSER());
                    } else {
                        return View(dt);
                    }
                }
            } else {
                return View(dt);
            }
        }
        //Deleteuser
        [HttpGet]
        public IActionResult Deleteuser(string id = "") {
            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser"))) {
                return RedirectToAction("login");
            }
            DOCREQUSER dt = _db.DOCREQUSERs.Find(id);
            if (dt == null) {
                return NotFound();
            }
            _db.DOCREQUSERs.Remove(dt);
            _db.SaveChanges();
            return RedirectToAction("Listusername");

        }
        public IActionResult Listusername(string id = "")
        {
            if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser")))
            {
                return RedirectToAction("login");
            }
            ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
            IEnumerable<DOCREQUSER> data = _db.DOCREQUSERs;
            return View(data);
        }
        public IActionResult Chart(string id = "")
        {
            try
            {
                string tendo = "";
                for (int i = 1; i < 13; i++)
                {
                    string query = "SELECT COUNT(Id) FROM DOCREQ WHERE YEAR(Reqinitiatedate) = '" + DateTime.Now.ToString("yyyy") + "' AND MONTH(Reqinitiatedate) = '" + i + "'";
                    if (!string.IsNullOrEmpty(tendo)) tendo += "|";
                    var result = RnDCore3.db.readSingle(query, cS);
                    tendo += string.IsNullOrEmpty(result) ? "0" : result;
                }
                ViewBag.year = DateTime.Now.ToString("yyyy");
                ViewBag.chart1 = tendo;

                string novis = "";
                var closed = RnDCore3.db.readSingle("SELECT count(Id) FROM DOCREQ WHERE UPPER(Closestatus) = 'CLOSED'", cS);
                var onprogress = RnDCore3.db.readSingle("SELECT count(Id) FROM DOCREQ WHERE UPPER(Closestatus) <> 'REJECT' AND UPPER(Closestatus) <> 'CLOSED'", cS);
                var reject = RnDCore3.db.readSingle("SELECT count(Id) FROM DOCREQ WHERE UPPER(Closestatus) = 'REJECT'", cS);
                novis = (string.IsNullOrEmpty(closed) ? "0" : closed) + "|";
                novis += (string.IsNullOrEmpty(onprogress) ? "0" : onprogress) + "|";
                novis += (string.IsNullOrEmpty(reject) ? "0" : reject);
                ViewBag.chart2 = novis;

                // Use consistent casing for Closestatus in LINQ
                IEnumerable<DOCREQ> data = _db.DOCREQS
                    .Where(b => b.Closestatus == null
                        || (b.Closestatus.ToUpper() != "REJECT" && b.Closestatus.ToUpper() != "CLOSED"))
                    .ToList();

                ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));

                // Build a dictionary of document owner/requestor username to displayName
                var ListEMPL = _db.EMPL.Where(e => e.WinAccount != null).Select(e => e.WinAccount).Distinct().ToList();

                var displayNameDict = new Dictionary<string, string>();
                foreach (var username in ListEMPL)
                {
                    var displayName = _db.EMPL
                        .Where(e => e.WinAccount == username)
                        .Select(e => e.NM_KRY)
                        .FirstOrDefault();
                    displayNameDict[username] = displayName ?? username;
                }
                ViewBag.DisplayNames = displayNameDict;

                ViewBag.ChartError = null;
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Chart action");
                ViewBag.ChartError = ex.ToString();
                RnDCore3.email.emailSend("donotreply@collins.com", "AhmadLuthfi.Fahrizi@collins.com", "AhmadLuthfi.Fahrizi@collins.com", "Error", ex.ToString());
                // Return empty data to avoid further errors in the view
                return View(Enumerable.Empty<DOCREQ>());
            }
        }
        public static string cekadmin(string cs, string user)
        {
            string rest = "";
            string query = "SELECT Userid, Adminaccess FROM DOCREQUSER where Userid = '" + user + "' and Adminaccess = 'Yes'";
            if (RnDCore3.db.noRow(query, cs) > 0)
            {
                rest = "admin";
            }
            return rest;
        }
        public static string sendemail(string cs, int id)
        {
            
            string temporary = "";
            string query = "SELECT Id, Reqtype, LinkDoc, Functgroup, Docowner, Finalapprovername, SMEname, ITCname, QMSASname, QMSNADCAPname, Doccontrollername FROM DOCREQ WHERE Id = '" + id + "'";
            
            if (RnDCore3.db.noRow(query,cs) > 0)
            {
                
                string[,] data = RnDCore3.db.read(query, cs);
                string to = "";
                string status = "Requested";
                
                string bodyemail = "Please Review this Document Request, and click the link for more detail action : <br><a href = 'http://gidbnd02:8011/Home/login/" + id + "'>Document Request Aprroval</a>'";
                if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "' AND (Finalapproverdecision = 'approved' AND SMEapproverdecision = 'approved' AND ITCapproverdecision = 'approved' AND QMSASapproverdecision = 'approved' AND QMSNADCAPapproverdecision = 'approved' AND Doccontrolleapproverdecision = 'approved')", cs) > 0)
                {
                    
                    status = "CLOSED";
                    for (int i = 4; i < data.GetLength(1); i++)
                    {
                        query = "SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, i] + "'";
                        if (RnDCore3.db.noRow(query, cs) > 0)
                        {
                            if (!string.IsNullOrEmpty(to)) to += ";";
                            to += RnDCore3.db.readSingle(query, cs);
                        } else if (RnDCore3.db.noRow("SELECT AL_EML FROM EMPL WHERE WinAccount = '" + data[0, i] + "'", cs) > 0)
                        {
                            if (!string.IsNullOrEmpty(to)) to += ";";
                            to += RnDCore3.db.readSingle("SELECT AL_EML FROM EMPL WHERE WinAccount = '" + data[0, i] + "'", cs);
                        }
                    }
                }
                else if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "' and (Finalapproverdecision = 'reject' OR SMEapproverdecision = 'reject' OR ITCapproverdecision = 'reject' OR QMSASapproverdecision = 'reject' OR QMSNADCAPapproverdecision = 'reject' OR Doccontrolleapproverdecision = 'reject')", cs) > 0)
                {
                    
                    status = "Reject";
                    query = "SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, 4] + "'";
                    if (RnDCore3.db.noRow(query, cs) > 0) to = RnDCore3.db.readSingle(query, cs);
                    else to = RnDCore3.db.readSingle("SELECT AL_EML FROM EMPL WHERE WinAccount = '" + data[0, 4] + "'", cs);

                    query = "SELECT Email FROM DOCREQUSER WHERE Doccontrollaccess = 'Yes'";
                    string[,] ctrl = RnDCore3.db.read(query, cs);
                    for(int i = 0; i<ctrl.GetLength(0); i++)
                    {
                        to += ";" + ctrl[i, 0];
                    }
                }
                else if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "' AND (Finalapproverdecision = 'approved' AND SMEapproverdecision = 'approved' AND ITCapproverdecision = 'approved' AND QMSASapproverdecision = 'approved' AND QMSNADCAPapproverdecision = 'approved')", cs) > 0)
                {
                    status = "DOCUMENT CONTROL APPROVAL";
                    to = RnDCore3.db.readSingle("SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, 10] + "'", cs);
                }
                else if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "' AND (Finalapproverdecision = 'approved' AND SMEapproverdecision = 'approved' AND ITCapproverdecision = 'approved' AND QMSASapproverdecision = 'approved')", cs) > 0)
                {
                    status = "NADCAP APPROVAL";
                    to = RnDCore3.db.readSingle("SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, 9] + "'", cs);
                }
                else if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "' AND (Finalapproverdecision = 'approved' AND SMEapproverdecision = 'approved' AND ITCapproverdecision = 'approved')", cs) > 0)
                {
                    status = "QMS APPROVAL";
                    to = RnDCore3.db.readSingle("SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, 8] + "'", cs);
                }
                else if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "' AND (Finalapproverdecision = 'approved' AND SMEapproverdecision = 'approved')", cs) > 0)
                {
                    status = "ITC APPROVAL";
                    to = RnDCore3.db.readSingle("SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, 7] + "'", cs);
                }
                else if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "' AND (Finalapproverdecision = 'approved')", cs) > 0)
                {
                    status = "SME APPROVAL";
                    to = RnDCore3.db.readSingle("SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, 6] + "'", cs);
                }
                else if (RnDCore3.db.noRow("SELECT Finalapproverdecision, SMEapproverdecision, ITCapproverdecision, QMSASapproverdecision, QMSNADCAPapproverdecision, Doccontrolleapproverdecision FROM DOCREQ WHERE Id = '" + id + "'", cs) > 0)
                {
                    status = "SUPERIOR APPROVAL";
                    to = RnDCore3.db.readSingle("SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, 5] + "'", cs);
                }
                string subject = "NOTIFICATION : Document Request Approval [" + status + "]";
                RnDCore3.db.exe("UPDATE DOCREQ SET Closestatus = '" + status + "' WHERE Id = '" + id +"'", cs);
                //for (int i = 4; i < data.GetLength(1); i++)
                //{
                //    query = "SELECT Email FROM DOCREQUSER WHERE Userid = '" + data[0, i] + "'";
                //    if (RnDCore3.db.noRow(query, cs) > 0)
                //    {
                //        if (!string.IsNullOrEmpty(to))
                //            to += ";";
                //        to += RnDCore3.db.readSingle(query, cs);
                //    }
                //    else if (RnDCore3.db.noRow("SELECT AL_EML FROM EMPL WHERE WinAccount = '" + data[0, i] + "'", cs) > 0)
                //    {
                //        if (!string.IsNullOrEmpty(to))
                //        {
                //            to += ";";
                //        }

                //        to += RnDCore3.db.readSingle(query, cs);
                //    }
                //}
                if(status == "Reject")
                {
                    query = "SELECT Docnumber, Reqtype, Prioritydoc, Closestatus, Docowner, Finalapprovername, Finalapproverdecision, Finalapprovernote, SMEname, SMEapproverdecision,SMEapprovernote, ITCname, ITCapproverdecision, ITCapprovernote, QMSASname, QMSASapproverdecision, QMSASapprovernote, QMSNADCAPname, QMSNADCAPapproverdecision, QMSNADCAPapprovernote, Doccontrollername, Doccontrolleapproverdecision, Doccontrolleapprovernote FROM DOCREQ WHERE Id = '" + id + "'";
                    string[,] susan = RnDCore3.db.read(query, cs);
                    string rejectby = "";
                    string rejectnote = "";
                    if (susan[0, 6] == "reject")
                    {
                        rejectby = susan[0, 5];
                        rejectnote = susan[0, 7];
                    }
                    else if (susan[0, 9] == "reject")
                    {
                        rejectby = susan[0, 8];
                        rejectnote = susan[0, 10];
                    }
                    else if (susan[0, 12] == "reject")
                    {
                        rejectby = susan[0, 11];
                        rejectnote = susan[0, 13];
                    }
                    else if (susan[0, 15] == "reject")
                    {
                        rejectby = susan[0, 14];
                        rejectnote = susan[0, 16];
                    }
                    else if (susan[0, 18] == "reject")
                    {
                        rejectby = susan[0, 17];
                        rejectnote = susan[0, 19];
                    }
                    else if (susan[0, 21] == "reject")
                    {
                        rejectby = susan[0, 20];
                        rejectnote = susan[0, 22];
                    }
                    //"SELECT Email FROM DOCREQUSER WHERE Userid = '" + rejectby + "'"
                    string RejectEmail = RnDCore3.db.readSingle("SELECT Email FROM DOCREQUSER WHERE Userid = '" + rejectby + "'", cs);
                    bodyemail = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><style>" +
                        "body{background-color: White;font-family: Verdana;font-size: xx-small ;}" +
                        "TABLE.nfsMsgTable{border-collapse: separate;border-width: thin;width: 100%;background-color: #cccccc;}" +
                        "TD.nfsMsgLabel{text-align: right;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;background: #eeeeee;}" +
                        "TD.nfsMsgData{text-align: left;font-weight: normal;background: white;font-family: Verdana;font-size: x-small;}" +
                        "TD.nfsMsgHeader{text-align: left;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;	background: #eeeeee;}" +
                        "TD.nfsCCMsgHeader{text-align: left;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;background: #eeeeee;}" +
                        "</style><title>Notification from Document Request Systems</title></head><body><table class='nfsMsgTable'>" +
                        "<tr><td class='nfsCCMsgHeader'></td></tr><tr><td class='nfsMsgHeader'><b>This is an automated message generated from document request system. Please do not reply to this message.</b></td></tr></table>" +
                        "<table class='nfsMsgTable'>";
                    bodyemail += "<tr><td class='nfsMsgLabel'>Document Number</td><td class='nfsMsgData'>" + susan[0, 0] + "</td></tr>" +
                         "<tr><td class='nfsMsgLabel'>Request Type</td><td class='nfsMsgData'>" + susan[0, 1] + "</td></tr>" +
                        "<tr><td class='nfsMsgLabel'>Priority</td><td class='nfsMsgData'>" + susan[0, 2] + "</td></tr>" +
                        "<tr><td class='nfsMsgLabel'>Status</td><td class='nfsMsgData'>" + susan[0, 3] + "</td></tr>" +
                        "<tr><td class='nfsMsgLabel'>Document Owner</td><td class='nfsMsgData'>" + susan[0, 4] + "</td></tr>" +
                        "<tr><td class='nfsMsgLabel'>Rejected By</td><td class='nfsMsgData'>" + rejectby + "</td></tr>" +
                        "<tr><td class='nfsMsgLabel'>Note</td><td class='nfsMsgData'>" + rejectnote + "</td></tr>" +
                        "</table><a href='http://gidbnd02:8011/Home/login/" + id + "'> Click here to access message details. </a><hr><b>Note: Confidential information. If you have experience any error please contact Novi.Susanti@collins.com </b></body></html>";
                    RnDCore3.email.emailSend("donotreply@collins.com", to, RejectEmail, subject, bodyemail);
                }
                else
                {

            
                query = "SELECT Docnumber, Reqtype, Prioritydoc, Closestatus, Docowner, Finalapprovername, Finalapproverdecision, Finalapprovernote, SMEname, SMEapproverdecision,SMEapprovernote, ITCname, ITCapproverdecision, ITCapprovernote, QMSASname, QMSASapproverdecision, QMSASapprovernote, QMSNADCAPname, QMSNADCAPapproverdecision, QMSNADCAPapprovernote, Doccontrollername, Doccontrolleapproverdecision, Doccontrolleapprovernote FROM DOCREQ WHERE Id = '" + id + "'";
                    string[,] susan = RnDCore3.db.read(query, cs);
                    string Approverby = "";           
                    if (susan[0, 3] == "SUPERIOR APPROVAL")
                    {
                        Approverby = susan[0, 5];        
                    }
                    else if (susan[0, 3] == "SME APPROVAL")
                    {
                        Approverby = susan[0, 8];
                    }
                    else if (susan[0, 3] == "ITC APPROVAL")
                    {
                        Approverby = susan[0, 11];
                    }
                    else if (susan[0, 3] == "QMS APPROVAL")
                    {
                        Approverby = susan[0, 14];
                    }
                    else if (susan[0, 3] == "NADCAP APPROVAL")
                    {
                        Approverby = susan[0, 17];
                    }
                    else if (susan[0, 3] == "DOCUMENT CONTROL APPROVAL")
                    {
                        Approverby = susan[0, 20];
                    }


                    bodyemail = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><style>" +
                        "body{background-color: White;font-family: Verdana;font-size: xx-small ;}" +
                        "TABLE.nfsMsgTable{border-collapse: separate;border-width: thin;width: 100%;background-color: #cccccc;}" +
                        "TD.nfsMsgLabel{text-align: right;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;background: #eeeeee;}" +
                        "TD.nfsMsgData{text-align: left;font-weight: normal;background: white;font-family: Verdana;font-size: x-small;}" +
                        "TD.nfsMsgHeader{text-align: left;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;	background: #eeeeee;}" +
                        "TD.nfsCCMsgHeader{text-align: left;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;background: #eeeeee;}" +
                        "</style><title>Notification from Document Request Systems</title></head><body><table class='nfsMsgTable'>" +
                        "<tr><td class='nfsCCMsgHeader'></td></tr><tr><td class='nfsMsgHeader'><b>This is an automated message generated from document request system. Please do not reply to this message.</b></td></tr></table>";
                    bodyemail += "<table class='nfsMsgTable'>" +
                    "<tr><td class='nfsMsgLabel'>Document Number</td><td class='nfsMsgData'>"+susan[0,0]+"</td></tr>" +
                     "<tr><td class='nfsMsgLabel'>Request Type</td><td class='nfsMsgData'>" + susan[0, 1] + "</td></tr>" +
                    "<tr><td class='nfsMsgLabel'>Priority</td><td class='nfsMsgData'>" + susan[0, 2] + "</td></tr>" +
                    "<tr><td class='nfsMsgLabel'>Status</td><td class='nfsMsgData'>" + susan[0, 3] + "</td></tr>" +
                     "<tr><td class='nfsMsgLabel'>Approver Name</td><td class='nfsMsgData'>" + Approverby + "</td></tr>" +
                    "<tr><td class='nfsMsgLabel'>Document Owner</td><td class='nfsMsgData'>" + susan[0, 4] + "</td></tr>" +
                    "</table><a href='http://gidbnd02:8011/Home/login/" + id + "'> Click here to access message details (outside company network). </a><hr><b>Note: Confidential information. If you have experience any error please contact Novi.Susanti@collins.com </b></body></html>";
                     RnDCore3.email.emailSend("donotreply@collins.com", to, "", subject, bodyemail);



            

            // Anda dapat menambahkan action method lain sesuai kebutuhan
        





        //batas CCR Request

    }
}
            
            return temporary;   
        }

        
        // GET: /Home/CCRNew
        public IActionResult CCRNew()
        {
            // Buat instance model sesuai dengan view Anda
            //var model = new EngineeringPortal_CCR(); // Pastikan menggunakan kelas model yang benar

            // Anda dapat menetapkan nilai default untuk model di sini jika diperlukan
            // Contoh:
            // model.CCRNo = "NomorCCRDefault";
            // model.DateReceived = DateTime.Now;
            //EngModel mymodel = new EngModel();
            //ViewBag.CCR = new EngineeringPortal_CCR();
            ViewBag.CCRP1Drawing = new EngineeringPortal_CCRP1Drawing();
            ViewBag.CCRP1Standard = new EngineeringPortal_CCRP1Standard();
            ViewBag.CCRP2GAP = new EngineeringPortal_CCRP2GAP();
            ViewBag.CCRP3GAP = new EngineeringPortal_CCRP3GAP();
            ViewBag.CCRP3Review = new EngineeringPortal_CCRP3Review();
            ViewBag.CCRP3SpesificationImpact = new EngineeringPortal_CCRP3SpesificationImpact();
            ViewBag.CCRP4Verification = new EngineeringPortal_CCRP4Verification();
            ViewBag.CCRP4VerificationFooter = new EngineeringPortal_CCRP4VerificationFooter();
            return View(new EngineeringPortal_CCR());
        }


        public IActionResult SendReminder(int id)
        {
            try
            {
                // Get the document request
                var doc = _db.DOCREQS.Find(id);
                if (doc == null)
                {
                    return NotFound("Document request not found.");
                }

                // Determine the current assign (the next approver)
                string assignUser = null;
                string assignRole = null;

                if (string.IsNullOrEmpty(doc.Finalapproverdecision))
                {
                    assignUser = doc.Finalapprovername;
                    assignRole = "Final Approver";
                }
                else if (string.IsNullOrEmpty(doc.SMEapproverdecision))
                {
                    assignUser = doc.SMEname;
                    assignRole = "SME Approver";
                }
                else if (string.IsNullOrEmpty(doc.ITCapproverdecision))
                {
                    assignUser = doc.ITCname;
                    assignRole = "ITC Approver";
                }
                else if (string.IsNullOrEmpty(doc.QMSASapproverdecision))
                {
                    assignUser = doc.QMSASname;
                    assignRole = "QMS AS Approver";
                }
                else if (string.IsNullOrEmpty(doc.QMSNADCAPapproverdecision))
                {
                    assignUser = doc.QMSNADCAPname;
                    assignRole = "QMS NADCAP Approver";
                }
                else if (string.IsNullOrEmpty(doc.Doccontrolleapproverdecision))
                {
                    assignUser = doc.Doccontrollername;
                    assignRole = "Document Controller Approver";
                }
                else
                {
                    return BadRequest("No pending approver found for this request.");
                }

                // Get email address from DOCREQUSER or EMPL
                string email = null;
                var user = _db.DOCREQUSERs.FirstOrDefault(u => u.Userid == assignUser);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    email = user.Email;
                }
                else
                {
                    // Try to get from EMPL
                    var empl = _db.EMPL.FirstOrDefault(e => e.NO_HRM == assignUser || e.NM_KRY == assignUser);
                    if (empl != null)
                    {
                        var emlProp = empl.GetType().GetProperty("AL_EML");
                        if (emlProp != null)
                        {
                            email = emlProp.GetValue(empl) as string;
                        }
                    }
                }

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Could not find email address for the assigned user.");
                }

                // Compose email
                string subject = $"REMINDER: Document Request Approval Needed [{assignRole}]";
                string body = $@"
        <html>
        <head>
            <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
            <style>
                body{{background-color: White;font-family: Verdana;font-size: xx-small ;}}
                TABLE.nfsMsgTable{{border-collapse: separate;border-width: thin;width: 100%;background-color: #cccccc;}}
                TD.nfsMsgLabel{{text-align: right;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;background: #eeeeee;}}
                TD.nfsMsgData{{text-align: left;font-weight: normal;background: white;font-family: Verdana;font-size: x-small;}}
                TD.nfsMsgHeader{{text-align: left;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;	background: #eeeeee;}}
                TD.nfsCCMsgHeader{{text-align: left;width: 20%;padding-right: 5px;color: Black;font-family: Verdana;font-size: xx-small;font-weight: bold;background: #eeeeee;}}
            </style>
            <title>Reminder from Document Request Systems</title>
        </head>
        <body>
            <table class='nfsMsgTable'>
                <tr><td class='nfsCCMsgHeader'></td></tr>
                <tr><td class='nfsMsgHeader'><b>This is an automated reminder from document request system. Please do not reply to this message.</b></td></tr>
            </table>
            <table class='nfsMsgTable'>
                <tr><td class='nfsMsgLabel'>Document Number</td><td class='nfsMsgData'>{doc.Docnumber}</td></tr>
                <tr><td class='nfsMsgLabel'>Request Type</td><td class='nfsMsgData'>{doc.Reqtype}</td></tr>
                <tr><td class='nfsMsgLabel'>Priority</td><td class='nfsMsgData'>{doc.Prioritydoc}</td></tr>
                <tr><td class='nfsMsgLabel'>Status</td><td class='nfsMsgData'>{doc.Closestatus}</td></tr>
                <tr><td class='nfsMsgLabel'>Approver Name</td><td class='nfsMsgData'>{assignUser}</td></tr>
                <tr><td class='nfsMsgLabel'>Document Owner</td><td class='nfsMsgData'>{doc.Docowner}</td></tr>
            </table>
            <a href='http://gidbnd02:8011/Home/login/{doc.Id}'>Click here to access message details (outside company network).</a>
            <hr>
            <b>Note: Confidential information. If you have experienced any error please contact Novi.Susanti@collins.com</b>
        </body>
        </html>
        ";

                // Send email
                RnDCore3.email.emailSend("donotreply@collins.com", email, "", subject, body);

                TempData["ReminderMessage"] = $"Reminder sent to {assignUser} ({email})";
                return RedirectToAction("Chart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending reminder for document request {Id}", id);
                TempData["ReminderError"] = ex.Message;
                return RedirectToAction("Chart");
            }
        }
        //CCR apps
        //public IActionResult CCRNew(int id = 0)
        //{

        //    if (string.IsNullOrEmpty(RnDCore3.web.cookiesGet(Request, "qmsuser")))
        //    {
        //        return RedirectToAction("login");
        //    }
        //    ViewBag.docown = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER ORDER BY Name", cS);
        //    ViewBag.docitc = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE ITCapprovalaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.docqms = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE AS9100approvalaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.docnadcap = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE NADCAPapprovalaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.docon = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE Doccontrollaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));

        //    if (id <= 0)
        //    {
        //        return View(new Models.DOCREQ());
        //    }
        //    else
        //    {
        //        var data = _db.DOCREQS.Find(id);
        //        return View(data);
        //    }
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult request(int id, [Bind("Id,Reqtype,LinkDoc,Functgroup,FunctgroupOthers,Docnumber,Associateddoc,Doctype,Superseededdoc,Reqinitiatedate,Prioritydoc,AuditNo,Docowner,Docownerdate,Finalapprovername,SMEname,ITCname,QMSASname,QMSNADCAPname,Doccontrollername,Listdoc1,Listdocaffected1,Listdoc2,Listdocaffected2,Listdoc3,Listdocaffected3,Listdoc4,Listdocaffected4,Listdoc5,Listdocaffected5,Listdoc6,Listdocaffected6,Listdoc7,Listdocaffected7,Listdoc8,Listdocaffected8,Listdoc9,Listdocaffected9,Listdoc10,Listdocaffected10,Finalapproverdecision,Finalapproverdate,SMEapproverdecision,SMEdate,ITCapproverdecision,ITCdate,QMSASapproverdecision,QMSASdate,QMSNADCAPapproverdecision,QMSNADCAPdate,Doccontrolleapproverdecision,Doccontrollerdate,documentReason")] DOCREQ dt, IFormFile Fileupload = null)
        //{
        //    ViewBag.docown = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER ORDER BY Name", cS);
        //    ViewBag.docitc = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE ITCapprovalaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.docqms = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE AS9100approvalaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.docnadcap = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE NADCAPapprovalaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.docon = RnDCore3.MVCcore.selectData("SELECT Userid, Name FROM DOCREQUSER WHERE Doccontrollaccess = 'Yes' ORDER BY Name", cS);
        //    ViewBag.adm = cekadmin(cS, RnDCore3.web.cookiesGet(Request, "qmsuser"));
        //    if (ModelState.IsValid)
        //    {
        //        dt.Docowner = RnDCore3.web.cookiesGet(Request, "qmsuser");
        //        dt.Docownerdate = DateTime.Now;
        //        dt.Reqinitiatedate = DateTime.Now;
        //        if (id > 0)
        //        {
        //            if (Fileupload != null)
        //            {
        //                string fileName = Fileupload.FileName;
        //                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetFileName(fileName);
        //                string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", fileName);
        //                if (!string.IsNullOrEmpty(dt.LinkDoc))
        //                {
        //                    string uploadpath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", dt.LinkDoc);
        //                    System.IO.File.Delete(uploadpath2);
        //                }
        //                dt.LinkDoc = fileName;
        //                var stream = new FileStream(uploadpath, FileMode.Create);

        //                Fileupload.CopyToAsync(stream);
        //            }


        //            _db.DOCREQS.Update(dt);
        //            _db.SaveChanges();
        //            sendemail(cS, dt.Id);







        //            return RedirectToAction("List");

        //        }
        //        else
        //        {
        //            if (Fileupload != null)
        //            {
        //                string fileName = Fileupload.FileName;
        //                fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetFileName(fileName);
        //                string uploadpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", fileName);
        //                if (!string.IsNullOrEmpty(dt.LinkDoc))
        //                {
        //                    string uploadpath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\NOVI.SUSANTI", dt.LinkDoc);
        //                    System.IO.File.Delete(uploadpath2);
        //                }
        //                dt.LinkDoc = fileName;
        //                var stream = new FileStream(uploadpath, FileMode.Create);

        //                Fileupload.CopyToAsync(stream);
        //            }
        //            _db.Add(dt);
        //            _db.SaveChanges();
        //            sendemail(cS, dt.Id);



        //            return RedirectToAction("newpage");
        //        }

        //    }
        //    else
        //    {
        //        return View(dt);
        //    }
        //}

    }
}

