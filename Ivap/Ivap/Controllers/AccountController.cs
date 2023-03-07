using IVap.Areas.Master.Repository;
using Ivap.Areas.Master.Models;
using Ivap.CustomAttribute;
using Ivap.Models;
using Ivap.Repository;
using Ivap.Utils;
using Ivap.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Controllers
{
    public class AccountController : IvapBaseController
    {
        #region Login
        [AllowAnonymous]
        [Route("~/", Name = "Defaultloginview")]
        [Route("login", Name = "loginview")]
        public ActionResult Login()
        {
            if (Session["uBo"] != null)
            {
                if (((AppUser)Session["uBo"]).PassChangeDays < 5)
                {
                    return RedirectToAction("PasswordExpiredAlert", "Account");
                }
                AuthorizationRepo aObj = new AuthorizationRepo();
                DataTable dtMenu = new DataTable();
                dtMenu = aObj.GetUserMenu(((AppUser)Session["uBo"]).RoleName, ((AppUser)Session["uBo"]).EID);
                Session["uMenu"] = dtMenu;
                return RedirectToAction("PayrollCompDB", new { controller = "PayrollCompDB", action = "PayrollCompDB", area = "Dashboards" });
                //return RedirectToAction("DummyDashBoard", "Account");

            }
            else
            {
                return View();
            }

        }

        [Route("DummyDashBoard", Name = "DummyDashBoard")]
        public ActionResult DummyDashBoard()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Route("login", Name = "login")]
        public ActionResult Login(LoginVM User)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    AccountRepo ObjURep = new AccountRepo();
                    AppUser ObjU = new AppUser();

                    String Message = "";
                    int PassTry = 0;
                    ObjU = ObjURep.Authenticate(User, ref Message);
                    if (Message == "success")
                    {
                        Session["uBo"] = ObjU;
                        ObjURep.LogLogin(ObjU.UID, System.Web.HttpContext.Current.Session.SessionID);
                        int uid = ((AppUser)Session["uBo"]).UID;
                        AuthorizationRepo aObj = new AuthorizationRepo();
                        DataTable dtMenu = new DataTable();
                        dtMenu = aObj.GetUserMenu(ObjU.RoleName,ObjU.EID);
                        Session["uMenu"] = dtMenu;
                        if (((AppUser)Session["uBo"]).PassChangeDays < 5)
                        {
                            return RedirectToAction("PasswordExpiredAlert", "Account");
                        }

                        return RedirectToAction("PayrollCompDB", new { controller = "PayrollCompDB", action = "PayrollCompDB", area = "Dashboards" });
                        //return RedirectToAction("DummyDashBoard", "Account");
                    }
                    //Invalid Password.
                    if (Message == "Invalid Password")
                    {
                        Message = "Invalid user name or password.";
                        ModelState.AddModelError("", "Invalid user name or password.The account will get locked after 5 consecutive login attempt invalid password.");
                        return View(User);
                    }
                    if (Message == "Your account has been locked")
                    {
                        ModelState.AddModelError("", "Your account has been locked.Please regenerate your password or try after 30 minutes.");
                        return View(User);
                    }
                        if (Message == "PasswordExpired")
                    {
                        ModelState.AddModelError("", "Your password has been expired.Please update your password.");
                        ResetPasswordVM model = new ResetPasswordVM();
                        model.UserID = User.UserName;
                        return View("ResetExpiredPassword", model);
                    }

                    ModelState.AddModelError("", "Invalid user name or password.");
                    return View(User);
                }

                else
                {
                    ModelState.AddModelError("", "Invalid user name or password.");
                    return View(User);
                }
            }
            catch
            {
                throw;
            }
        }

        [Route("LogOut", Name = "LogOut")]
        public ActionResult LogOut()
        {
            //Setting User Log out time in the database
            AccountRepo uRepo = new AccountRepo();
            try
            {
                uRepo.SetLogOut(((AppUser)Session["uBo"]).UID, System.Web.HttpContext.Current.Session.SessionID);
            }
            catch { }
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            return RedirectToAction("Login", "Account");
        }

        #endregion
        #region Password
        [Route("GeneratePassword/{PassLinkKey}", Name = "GeneratePasswordView")]
        public ActionResult GeneratePassword(string PassLinkKey)
        {
            try
            {
                UserVM model = new UserVM();
                model.PassLinkKey = PassLinkKey;
                return View(model);
            }
            catch { throw; }

        }

        [ValidateAntiForgeryToken()]
        [HttpPost]
        [Route("GeneratePassword", Name = "GeneratePassword")]
        public ActionResult GeneratePassword(UserVM model)
        {
            Response ret = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    AccountRepo uRepo = new AccountRepo();
                    ret = uRepo.GeneratePassword(model);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.Message = "Invalid details.";
                }
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [AllowAnonymous]
        [Route("ResetExpiredPassword", Name = "ResetExpiredPassword")]
        public ActionResult ResetExpiredPassword()
        {
            try
            {

                return View();
            }
            catch
            {
                throw;
            }

        }
        
        [Route("PasswordExpiredAlert", Name = "PasswordExpiredAlert")]
        public ActionResult PasswordExpiredAlert()
        {
            try
            {
                return View();
            }
            catch { throw; }

        }
       
        [ValidateAntiForgeryToken()]
        [HttpPost]
        public ActionResult UpdateExpiredPassword(ResetPasswordVM model)
        {
            Response Res = new Response();
            ChangePasswordVM cVm = new ChangePasswordVM();
            try
            {
                //Validating Captcha...
                //var response = Request["g-recaptcha-response"];
                //string secretKey = "6LchbEIUAAAAAOm6bA0GfpSjIXO6HDRv_Zs1ey2_";
                //var client = new WebClient();
                //var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                //var obj = JObject.Parse(result);
                //var status = (bool)obj.SelectToken("success");
                ////Captcha validation failed. Now Return bad response
                //if (!status)
                //{
                //    Res.IsSuccess = false;
                //    Res.Message = "Invalid Captcha.";
                //    return Json(Res, JsonRequestBehavior.AllowGet);
                //}
                if (ModelState.IsValid)
                {
                    cVm.UserName = model.UserID;
                    cVm.Password = model.CurrentPassword;
                    cVm.NewPassword = model.NewPassword;
                    cVm.ConfirmPassword = model.ConfirmPassword;
                    AccountRepo uRepo = new AccountRepo();
                    Res = uRepo.ChangePassword(cVm);
                }
                else
                {
                    Res.IsSuccess = false;
                    Res.Message = "Invalid details";
                }
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Res.IsSuccess = false;
                Res.Message = "Invalid details";
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("ForgetPassword", Name = "ForgetPassword")]
        public ActionResult ForgetPassword(string UserName)
        {
            Response ret = new Response();
            try
            {
                if (UserName.Trim() == "")
                {
                    ret.Message = "Please enter your user name.";
                }
                else
                {
                    AccountRepo uRep = new AccountRepo();
                    string res = uRep.ForgetPassword(UserName);
                    ret.Message = res;
                }
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                throw;
            }

        }

        [Route("Account/ChangePassword", Name = "ChangePasswordView")]
        public ActionResult ChangePassword()
        {
            try
            {
                ChangePasswordVM Model = new ChangePasswordVM();
                Model.UserName = IvapUser.UserID;
                return View();
            }
            catch
            {
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Route("ChangePassword", Name = "ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordVM Model)
        {
            var ret = new Response();
            try
            {

                if (ModelState.IsValid)
                {
                    AccountRepo uRepo = new AccountRepo();
                    var res =  uRepo.ChangePassword(((AppUser)Session["Ubo"]), Model);
                    if(res.IsSuccess==true)
                    return RedirectToAction("ProfileDetail");
                    else
                    {
                        ModelState.AddModelError(string.Empty, res.Message);
                        return View("ChangePassword",Model);
                    }

                }
                else
                {
                    return View("ChangePassword",Model);
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region Profile
        [Route("MyProfile", Name = "MyProfile")]
        public ActionResult ProfileDetail()
        {
            try
            {
                return View();
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetProfile", Name = "GetProfile")]
        public ActionResult GetProfile()
        {
            int uid = ((AppUser)Session["uBo"]).UID;
            UserRepo objUBO = new UserRepo();
            UserModel objModel = new UserModel();
            Response ret = new Response();
            try
            {
                objModel.UID = Convert.ToInt32(uid);
                ret.Data = JsonSerializer.SerializeTable(objUBO.GetUser(objModel));
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.Message = ex.Message;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("UpdateProfile", Name = "UpdateProfile")]
        public ActionResult UpdateProfile(UserProfileVM model)
        {
            var ret = new Response();
            try
            {
                if (ModelState.IsValid)
                {
                    string path = Server.MapPath("~/Docs/Profile/Thumbnail/");
                    string base64String = "";
                    if (model.ProfilePic != null)
                    {
                        if (model.ProfilePic.Length < 30)
                        {
                            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path + model.ProfilePic))
                            {
                                using (MemoryStream m = new MemoryStream())
                                {
                                    image.Save(m, image.RawFormat);
                                    byte[] imageBytes = m.ToArray();
                                    base64String = Convert.ToBase64String(imageBytes);

                                }
                            }
                        }
                        if (model.ProfilePic.Length < 30)
                        {
                            model.ProfilePic = base64String;
                        }
                    }
                    AccountRepo ARepo = new AccountRepo();
                    int res = 1;// uRepo.UpdateProfile(model);
                    if (res == 1)
                    {
                        
                        model.CreatedBy = IvapUser.UID;
                        ret = ARepo.AddUpdateUserProfile(model);
                        ret.IsSuccess = true;
                        ret.Message = "Your profile has been updated successfully!";
                        AppUser ObjAppUser=new AppUser();
                        Session["uBo"]=ObjAppUser.SetProfilePic(IvapUser, base64String);
                        return Json(ret, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Message = "There were some internal error please contaact admin";
                        return Json(ret, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.Message = "Invalid details";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {


                ret.IsSuccess = false;
                ret.Message = "Invalid details";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
        }




        [Route("ProfileImageUpload", Name = "ProfileImageUpload")]
        public ActionResult file(HttpPostedFileBase files)
        {
            string path = "";
            Response ret = new Response();
            if (files != null && files.ContentLength > 0)
                try
                {
                    string fileExt = "";
                    fileExt = Path.GetExtension(files.FileName);
                    if ((fileExt.ToUpper() == ".JPG") || (fileExt.ToUpper() == ".JPEG") || (fileExt.ToUpper() == ".PNG"))
                    {
                        string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                        FileName = FileName.Replace(fileExt, string.Empty);
                        FileName = FileName + fileExt;

                        path = Server.MapPath("~/Docs/Profile/");

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        path = Server.MapPath("~/Docs/Profile/") + FileName;
                        files.SaveAs(path);
                        ret.IsSuccess = true;
                        ret.Data = FileName;
                        ret.Message = "Uploaded";
                        try
                        {
                            Stream strm = files.InputStream;
                            int newWidth;
                            int newHeight;
                            int Width = 40;
                            int Height = 40;
                            using (var image = Image.FromStream(strm))
                            {
                                int originalWidth = image.Width;
                                int originalHeight = image.Height;
                                float percentWidth = (float)Width / (float)originalWidth;
                                float percentHeight = (float)Height / (float)originalHeight;
                                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                                newWidth = (int)(originalWidth * percent);
                                newHeight = (int)(originalHeight * percent);
                                var thumbnailImg = new Bitmap(newWidth, newHeight);
                                var thumbGraph = Graphics.FromImage(thumbnailImg);
                                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                                thumbGraph.DrawImage(image, imageRectangle);
                              var newpath = Server.MapPath("~/Docs/Profile/Thumbnail/");

                              if (!Directory.Exists(newpath))
                                {
                                    Directory.CreateDirectory(newpath);
                                }
                        
                                thumbnailImg.Save(Server.MapPath("~/Docs/Profile/Thumbnail/" + FileName), image.RawFormat);
                            }

                        }
                        catch { }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Data = "File type not supported.";
                        ret.Message = "File type not supported";
                    }
                }
                catch (Exception ex)
                {
                    ret.IsSuccess = false;
                    ret.Data = "Sorry!!!Smoething went wrong.Please try again later.";
                    ret.Message = "Sorry!!!Smoething went wrong.Please try again later.";
                }
            else
            {
                ret.IsSuccess = false;
                ret.Data = "Invalid File.";
                ret.Message = "Invalid File.";
            }
            return Json(ret);
        }
        #endregion

        #region ForgotPassword
        [Route("ForgotPassword", Name = "ForgotPasswordView")]
        public ActionResult ForgotPassword()
        {
            try
            {
                return View();
            }
            catch { throw; }

        }
        #endregion
    }
}