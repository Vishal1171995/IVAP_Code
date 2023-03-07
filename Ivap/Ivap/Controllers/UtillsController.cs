using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Utils;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Ivap.Repository;
using Ivap.Areas.Master.Models;
using Ivap.ActionFilters;
using System.Web.Hosting;
using Ivap.Areas.Master.Repository;

namespace Ivap.Controllers
{
    [CustomAuthActionFilter]
    public class UtillsController : IvapBaseController
    {
        // GET: Utills
        [Route("FilesUpload", Name = "FilesUpload")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult FilesUpload(HttpPostedFileBase files, string Folder)
        {
            string path = "";
            Response ret = new Response();
            if (files != null && files.ContentLength > 0)
                try
                {
                    if (files.ContentLength <= 20971520) //20MB {1048576 *20}
                    {
                        string fileExt = "";
                        fileExt = Path.GetExtension(files.FileName);
                        if (fileExt.ToLower() == ".csv")
                        {
                            string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                            FileName = FileName.Replace(fileExt, string.Empty);
                            FileName = FileName + fileExt;
                            path = Server.MapPath("~/Docs/" + Folder + "/") + FileName;
                            files.SaveAs(path);
                            ret.IsSuccess = true;
                            ret.Data = "{\"original\":\"" + files.FileName + "\",\"new\":\"" + FileName + "\"}";
                            ret.Message = "Uploaded";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.Data = "File type not supported.";
                            ret.Message = "File type not supported";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Data = "File size must be less than 20mb.";
                        ret.Message = "File size must be less than 20mb.";
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

        [Route("MTRFilesUpload", Name = "MTRFilesUpload")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult MTRFilesUpload(HttpPostedFileBase files, string Folder)
        {
            string path = "";
            Response ret = new Response();
            if (files != null && files.ContentLength > 0)
                try
                {
                    if (files.ContentLength <= 20971520) //20MB {1048576 *20}
                    {
                        string fileExt = "";
                        fileExt = Path.GetExtension(files.FileName);
                        if (fileExt.ToUpper() == ".XLS" || fileExt.ToUpper() == ".XLSX")
                        {
                            string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                            FileName = FileName.Replace(fileExt, string.Empty);
                            FileName = FileName + fileExt;
                            path = Server.MapPath("~/Docs/" + Folder + "/") + FileName;
                            files.SaveAs(path);
                            ret.IsSuccess = true;
                            ret.Data = "{\"original\":\"" + files.FileName + "\",\"new\":\"" + FileName + "\"}";
                            ret.Message = "Uploaded";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.Data = "File type not supported.";
                            ret.Message = "File type not supported";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Data = "File size must be less than 20mb.";
                        ret.Message = "File size must be less than 20mb.";
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

        [HttpGet]
        [Route("DownLoadResultFile", Name = "DownLoadResultFile")]
        public ActionResult DownLoadResultFile(string FileName)
        {
            Response ret = new Response();
            try
            {
                FileName = FileName.Replace("/", "").Replace("..","").Replace("\\","");
                
                FileName = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ResultFile.csv");
            }
            catch
            {
                throw;
            }
        }

        [Route("RemoveFiles", Name = "RemoveFiles")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/Doc/Temp/"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }


        [Route("FilesUploadForOther", Name = "FilesUploadForOther")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult FilesUploadForOther(HttpPostedFileBase files, string Folder)
        {
            string path = "";
            Response ret = new Response();
            if (files != null && files.ContentLength > 0)
                try
                {
                    if (files.ContentLength <= 20971520) //20MB {1048576 *20}
                    {
                        string fileExt = "";
                        fileExt = Path.GetExtension(files.FileName);
                        switch (Folder)
                        {
                            case "DL":
                                if (fileExt.ToUpper() != ".pdf".ToUpper() && fileExt.ToUpper() != ".png".ToUpper() && fileExt.ToUpper() != ".jpeg".ToUpper() && fileExt.ToUpper() != ".jpg".ToUpper())
                                {
                                    ret.IsSuccess = false;
                                    ret.Data = "File Type must be only pdf,jpeg,png,jpg";
                                    ret.Message = "File Type must be only pdf,jpeg,png,jpg";
                                    return Json(ret);
                                }
                                break;
                            case "Vehicles":
                                if (fileExt.ToUpper() != ".pdf".ToUpper() && fileExt.ToUpper() != ".png".ToUpper() && fileExt.ToUpper() != ".jpeg".ToUpper() && fileExt.ToUpper() != ".jpg".ToUpper())
                                {
                                    ret.IsSuccess = false;
                                    ret.Data = "File Type must be only pdf,jpeg,png,jpg";
                                    ret.Message = "File Type must be only pdf,jpeg,png,jpg";
                                    return Json(ret);
                                }
                                break;
                        }
                        if (FileExtUtils.checkFileExt(fileExt.ToLower()))
                        {

                            string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                            FileName = FileName.Replace(fileExt, string.Empty);
                            FileName = FileName + fileExt;
                            bool exists = System.IO.Directory.Exists(Server.MapPath("~/Docs/" + Folder + "/"));
                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath("~/Docs/" + Folder + "/"));
                            path = Server.MapPath("~/Docs/" + Folder + "/") + FileName;
                            files.SaveAs(path);
                            ret.IsSuccess = true;
                            ret.Data = "{\"original\":\"" + files.FileName + "\",\"new\":\"" + FileName + "\"}";
                            ret.Message = "Uploaded";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.Data = "File type not supported";
                            ret.Message = "File type not supported";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Data = "File size must be less than 20mb.";
                        ret.Message = "File size must be less than 20mb.";
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            else
            {
                ret.IsSuccess = false;
                ret.Data = "Invalid File.";
                ret.Message = "Invalid File.";
            }
            return Json(ret);
        }

        [Route("MultiFilesUpload", Name = "MultiFilesUpload")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult MultiFilesUpload(IEnumerable<HttpPostedFileBase> files, string Folder)
        {
            string path = "";
            Response ret = new Response();
            if (files != null)
            {
                try
                {
                    foreach (var file in files)
                    {
                        if (file.ContentLength <= 20971520) //20MB {1048576 *20}
                        {
                            string fileExt = "";
                            fileExt = Path.GetExtension(file.FileName);
                            if (FileExtUtils.checkFileExt(fileExt.ToLower()))
                            {

                                string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                                FileName = FileName.Replace(fileExt, string.Empty);
                                FileName = FileName + fileExt;
                                bool exists = System.IO.Directory.Exists(Server.MapPath("~/Docs/" + Folder + "/"));
                                if (!exists)
                                    System.IO.Directory.CreateDirectory(Server.MapPath("~/Docs/" + Folder + "/"));
                                path = Server.MapPath("~/Docs/" + Folder + "/") + FileName;
                                file.SaveAs(path);
                                ret.IsSuccess = true;
                                ret.Data = "{\"original\":\"" + file.FileName + "\",\"new\":\"" + FileName + "\"}";
                                ret.Message = "Uploaded";
                            }
                            else
                            {
                                ret.IsSuccess = false;
                                ret.Data = "File type not supported";
                                ret.Message = "File type not supported";
                            }

                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                ret.IsSuccess = false;
                ret.Data = "Invalid File.";
                ret.Message = "Invalid File.";
            }
            return Json(ret);
        }

        [Route("RemoveMultiFilesUpload", Name = "RemoveMultiFilesUpload")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RemoveMultiFilesUpload(string[] fileNames, string Folder, string System_FileName)
        {
            string path = "";
            Response ret = new Response();
            if (fileNames != null)
            {
                try
                {
                    foreach (var fullName in fileNames)
                    {
                        var fileName = Path.GetFileName(fullName);
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/Docs/" + Folder + "/" + System_FileName));
                        path = Server.MapPath("~/Docs/" + Folder + "/") + System_FileName;
                        System.IO.File.Delete(path);
                        ret.IsSuccess = true;
                        ret.Data = "{\"original\":\"" + fileName + "\",\"new\":\"" + System_FileName + "\"}";
                        ret.Message = "Deleted";
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                ret.IsSuccess = true;
                ret.Data = "Invalid File.";
                ret.Message = "Invalid File.";
            }
            return Json(ret);
        }



        [Route("ExcelFilesUpload", Name = "ExcelFilesUpload")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ExcelFilesUpload(HttpPostedFileBase files, string Folder)
        {
            string path = "";
            Response ret = new Response();
            if (files != null && files.ContentLength > 0)
                try
                {
                    if (files.ContentLength <= 20971520) //20MB {1048576 *20}
                    {
                        string fileExt = "";
                        fileExt = Path.GetExtension(files.FileName);
                        if (fileExt.ToUpper() == ".XLS" || fileExt.ToUpper() == ".XLSX")
                        {
                            string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                            FileName = FileName.Replace(fileExt, string.Empty);
                            FileName = FileName + fileExt;
                            path = Server.MapPath("~/Docs/" + Folder + "/") + FileName;
                            files.SaveAs(path);
                            ret.IsSuccess = true;
                            ret.Data = "{\"original\":\"" + files.FileName + "\",\"new\":\"" + FileName + "\"}";
                            ret.Message = "Uploaded";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.Data = "File type not supported.";
                            ret.Message = "File type not supported";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Data = "File size must be less than 20mb.";
                        ret.Message = "File size must be less than 20mb.";
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

        [HttpGet]
        [Route("DownLoadResultFileForExcel", Name = "DownLoadResultFileForExcel")]
        public ActionResult DownLoadResultFileForExcel(string FileName)
        {
            Response ret = new Response();
            try
            {
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");

                FileName = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FileName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ResultFile.xlsx");
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        [Route("DownLoadSampleForExcel", Name = "DownLoadSampleForExcel")]
        public ActionResult DownLoadSampleForExcel(string TableName, string ActionName,string SampleName)
        {
            DataSet ds = new DataSet();
            try
            {
                DataTable Dt = new DataTable();
                Dt = ExcellUtils.GetDisplayName(IvapUser.EID, TableName, ActionName);
                string FileName = ExcellUtils.DataTableToExcel(Dt);
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, SampleName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}