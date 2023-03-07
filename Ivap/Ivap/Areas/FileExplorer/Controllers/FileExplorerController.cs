using Ivap.ActionFilters;
using Ivap.Areas.FileExplorer.Models;
using Ivap.Areas.FileExplorer.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.FileExplorer.Controllers
{
    [CustomAuthrizationActionFilter(Order = 1)]
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("FileExplorer")]
    public class FileExplorerController : IvapBaseController
    {
        [ViewAction]
        [Route("~/FileExplorer", Name = "FileExplorer")]
        public ActionResult FileExplorer()
        {
            return View();
        }
        [ViewAction]
        [HttpGet]
        [Route("GetEntityTreeView", Name = "GetEntityTreeView")]
        public ActionResult GetEntityTreeView(int? EID, int? FileID, int? ParentId, bool? hasChildren)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            List<FileExplorerModel> finalTree = new List<FileExplorerModel>();
            try
            {
                int UID = IvapUser.UID;
                EID = IvapUser.EID;
                FileID = FileID == null ? -1 : FileID;
                finalTree = objRepo.GetEntityTreeView(EID.HasValue ? EID.Value : 0, FileID.HasValue ? FileID.Value : 0, ParentId.HasValue ? ParentId.Value : 0, UID,IvapUser.RoleName);
                return Json(finalTree, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetAllChildFile", Name = "GetAllChildFile")]
        public ActionResult GetAllChildFile(int EID, int FileID)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            DataTable Dt = new DataTable();
            try
            {
                Dt = objRepo.GetAllChildFile(EID, FileID,IvapUser.UID,IvapUser.RoleName);
                ret.Data = JsonSerializer.SerializeTable(Dt);
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetFileMetaData", Name = "GetFileMetaData")]
        public ActionResult GetFileMetaData(int EID, int FileID, int FileMetaID)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            DataSet Ds = new DataSet();
            try
            {
                Ds = objRepo.GetFileMetaData(EID, FileID, FileMetaID);
                ret.Data = JsonSerializer.SerializeTable(Ds.Tables[0]);
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }
        [ADDUpdateAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateNewFolder", Name = "CreateNewFolder")]
        public ActionResult CreateNewFolder(int? EID, int? FileID, int? ParentID, string SelectedText)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            string directoryPath;
            DataTable Dt = new DataTable();
            try
            {
                Dt = objRepo.GetFileExplorerByFileID(Convert.ToInt32(FileID));
                bool IsValid = true;
                IsValid = objRepo.IsAuthorized(Convert.ToInt32(FileID), IvapUser.UID);
                if (!IsValid)
                {
                    ret.IsSuccess = false;
                    ret.Data = "-3";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                string Path = objRepo.GetParrentFolder(FileID, "Folder");
                directoryPath = Server.MapPath(string.Format("~/{0}/", "Docs/FileExplorer/" + EID + "/" + Path + "/" + SelectedText + ""));
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    int UID = IvapUser.UID;
                    ret.Data = objRepo.CreateNewFolder(EID, FileID, ParentID, SelectedText, SelectedText, UID);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.Data = "-2";
                }
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }

        [ADDUpdateAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteFolder", Name = "DeleteFolder")]
        public ActionResult DeleteFolder(int? EID, int? FileID, int? ParentID, string FileType)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            string directoryPath;
            try
            {
                bool IsValid = true;
                IsValid = objRepo.IsAuthorized(Convert.ToInt32(FileID), IvapUser.UID);
                if (!IsValid)
                {
                    ret.IsSuccess = false;
                    ret.Data = "-3";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }

                string Path = objRepo.GetParrentFolder(FileID, FileType);
                if (FileType == "Folder")
                {
                    directoryPath = Server.MapPath(string.Format("~/{0}/", "Docs/FileExplorer/" + EID + "/" + Path + ""));
                    if (Directory.Exists(directoryPath))
                    {
                        int UID = IvapUser.UID;
                        ret.Data = objRepo.DeleteFolder(FileID, UID);
                        if (ret.Data != "-1")
                        {
                            Directory.Delete(directoryPath, true);
                            ret.IsSuccess = true;
                        }
                        else
                        {
                            ret.IsSuccess = false;
                        }
                    }
                    else
                    {
                        ret.Data = "-2";
                        ret.IsSuccess = false;
                    }
                }
                else
                {
                    directoryPath = Server.MapPath("~/Docs/FileExplorer/" + EID + "/" + Path + "");
                    //directoryPath = Path;
                    if ((System.IO.File.Exists(directoryPath)))
                    {
                        int UID = IvapUser.UID;
                        ret.Data = objRepo.DeleteFolder(FileID, UID);
                        if (ret.Data != "-1")
                        {
                            System.IO.File.Delete(directoryPath);
                            ret.IsSuccess = true;
                        }
                        else
                        {
                            ret.IsSuccess = false;
                        }
                    }
                    else
                    {
                        ret.Data = "-2";
                        ret.IsSuccess = false;
                    }
                }


                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }
        [ADDUpdateAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("AddFile", Name = "AddFile")]
        public ActionResult AddFile(int EID, int FileID, string OriginalFileName, string TempFileName, string MetaValue, string FileTypeName)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            string OldPath, NewPath;
            string result = "";
            try
            {
                bool IsValid = true;
                IsValid = objRepo.IsAuthorized(Convert.ToInt32(FileID), IvapUser.UID);
                if (!IsValid)
                {
                    ret.IsSuccess = false;
                    ret.Data = "-3";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                int UID = IvapUser.UID;
                string PPath = objRepo.GetParrentFolder(FileID, "Folder");
                OldPath = Server.MapPath("~/Docs/FileExplorerTemp");
                NewPath = Server.MapPath(string.Format("~/{0}/", "Docs/FileExplorer/" + EID + "/" + PPath + ""));
                string FileExt = "";
                double FileSize = 0;
                string FileSystemGeneratedName = "";
                if (!Directory.Exists(NewPath))
                {
                    Directory.CreateDirectory(NewPath);
                }
                foreach (var f in Directory.GetFiles(OldPath, TempFileName))
                {
                    var fi = new FileInfo(f);
                    FileExt = fi.Extension;
                    FileSize = fi.Length;
                    fi.MoveTo(Path.Combine(NewPath, fi.Name));
                    FileSystemGeneratedName = fi.FullName;
                }
                DataTable dtParentDet = objRepo.RootParent(FileID);
                int RootParentFolderID = Convert.ToInt32(dtParentDet.Rows[0]["FileID"].ToString());
                string RootParentFolder = dtParentDet.Rows[0]["RootParent"].ToString();
                //string result = objRepo.AddFile(CustID, FileID, OriginalFileName.Split('.')[0], TempFileName, MetaValue, FileTypeName, FileExt, FileSize, UID);
                if (FileExt != "")
                {
                    result = objRepo.AddFile(EID, FileID, OriginalFileName.Split('.')[0], TempFileName, MetaValue, FileTypeName, FileExt, FileSize, UID);
                    ret.IsSuccess = true;
                    ret.Data = result;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.Data = "-7";
                }

                //int companyid = ((User)Session["uBo"]).Company;
                //if ((RootParentFolder == "Input From EXL" || RootParentFolder == "Output From Mynd") && CustomerID == "464")
                //{
                try
                {
                    objRepo.SendMailEXl(RootParentFolder, RootParentFolderID);
                }
                catch { }

                //}
                //string FileSystemGeneratedName = PPath + "\\" + TempFileName;
                ret.IsSuccess = true;
                ret.Data = result;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [Route("UploadFileMetaData", Name = "UploadFileMetaData")]
        [HttpPost]
        public ActionResult UploadFileMetaData(HttpPostedFileBase fileToUpload)
        {
            string path = "";
            Response ret = new Response();
            //FileExtUtils fileext = new FileExtUtils();
            if (fileToUpload != null && fileToUpload.ContentLength > 0)
                try
                {
                    if (fileToUpload.ContentLength <= 31457280) //20MB {1048576 *20}
                    {
                        string fileExt = "";
                        string orignalFileName = fileToUpload.FileName;
                        fileExt = Path.GetExtension(fileToUpload.FileName);
                        //if ((fileExt.ToLower() != ".exe") && (fileExt.ToLower() != ".vb") && (fileExt.ToLower() != ".js") && (fileExt.ToLower() != ".cs") && (fileExt.ToLower() != ".html") && (fileExt.ToLower() != ".htm") && (fileExt.ToLower() != ".php"))
                        if (FileExtUtils.checkFileExt(fileExt.ToLower()))
                        {
                            string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                            FileName = FileName.Replace(fileExt, string.Empty);
                            FileName = FileName + fileExt;
                            path = Server.MapPath("~/Docs/FileExplorerTemp/") + FileName;
                            fileToUpload.SaveAs(path);
                            ret.Data = FileName + "$" + orignalFileName;
                            ret.IsSuccess = true;
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
                    ret.Data = "Sorry!!!Something went wrong.Please try again later.";
                    ret.Message = "Sorry!!!Something went wrong.Please try again later.";
                }
            else
            {
                ret.IsSuccess = false;
                ret.Data = "Invalid File.";
                ret.Message = "Invalid File.";
            }
            return Json(ret);
        }
        [ViewAction]
        [HttpGet]
        [Route("CreateZipAndDownload", Name = "CreateZipAndDownload")]
        public ActionResult CreateZipAndDownload(int EID, int FileID)
        {
            FileExplorerRepo objRepo = new FileExplorerRepo();
            Response ret = new Response();
            try
            {
                string FolderPath = objRepo.GetParrentFolder(FileID, "");
                FolderPath = Server.MapPath("~/Docs/FileExplorer/" + EID + "/" + FolderPath + "");

                string SourceFolderPath = System.IO.Path.Combine(FolderPath);

                byte[] fileBytes = System.IO.File.ReadAllBytes(FolderPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FolderPath);
            }
            catch { throw; }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetFileInfo", Name = "GetFileInfo")]
        public ActionResult GetFileInfo(int FileID)
        {
            FileExplorerRepo objRepo = new FileExplorerRepo();
            DataSet Ds = new DataSet();
            Response ret = new Response();
            try
            {
                Ds = objRepo.GetFileInfo(FileID);
                ret.IsSuccess = true;
                ret.Data = JsonSerializer.SerializeTable(Ds.Tables[0]);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }
        [ViewAction]
        [HttpGet]
        [Route("GetMetaDetaFileForSearch", Name = "GetMetaDetaFileForSearch")]
        public ActionResult GetMetaDetaFileForSearch(int EID, int FileID, string text)
        {
            FileExplorerRepo objRepo = new FileExplorerRepo();
            DataSet Ds = new DataSet();
            Response ret = new Response();
            try
            {
                ret.IsSuccess = true;
                Ds = objRepo.GetMetaDetaFileForSearch(EID, FileID, text);
                if (Ds.Tables[0].Rows.Count > 0)
                    ret.Data = "1";
                else
                    ret.Data = "0";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }
        [ADDUpdateAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("RenmaeFile", Name = "RenmaeFile")]
        public ActionResult RenmaeFile(int EID, int FileID, string OldName, string NewName, string FileType)
        {
            FileExplorerRepo objRepo = new FileExplorerRepo();
            DataSet Ds = new DataSet();
            string OlddirectoryPath = "";
            string NewdirectoryPath = "";
            Response ret = new Response();
            try
            {
                bool IsValid = true;
                IsValid = objRepo.IsAuthorized(Convert.ToInt32(FileID), IvapUser.UID);
                if (!IsValid)
                {
                    ret.IsSuccess = false;
                    ret.Data = "-4";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
                string Path = objRepo.GetParrentFolderForRename(FileID);
                OlddirectoryPath = Server.MapPath("~/Docs/FileExplorer/" + EID + "/" + Path + "/" + OldName + "");
                NewdirectoryPath = Server.MapPath("~/Docs/FileExplorer/" + EID + "/" + Path + "/" + NewName + "");

                //Server.MapPath("~/Docs/FileExplorer/" + CustID + "/" + Path + "");
                if (FileType == "Folder")
                {
                    if (Directory.Exists(OlddirectoryPath))
                    {
                        //Directory.CreateDirectory(directoryPath);
                        if (!Directory.Exists(NewdirectoryPath))
                        {
                            Directory.Move(OlddirectoryPath, NewdirectoryPath);
                            int UID = IvapUser.UID;
                            ret.Data = objRepo.RenmaeFile(FileID, OldName, NewName, NewName, FileType);
                            ret.IsSuccess = true;
                        }
                        else
                        {
                            ret.Data = "-3";
                            ret.IsSuccess = false;
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.Data = "-2";
                    }
                }
                else
                {
                    ret.Data = objRepo.RenmaeFile(FileID, OldName, NewName, NewdirectoryPath, FileType);
                    ret.IsSuccess = true;
                }
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetAcessRoleForFileExplorer", Name = "GetAcessRoleForFileExplorer")]
        public ActionResult GetAcessRoleForFileExplorer(int FileID)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            DataSet Ds = new DataSet();
            try
            {
                Ds = objRepo.GetAcessRoleForFileExplorer(IvapUser.EID, FileID);
                ret.Data = JsonSerializer.SerializeObject(Ds);
                ret.IsSuccess = true;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }
        [ValidateAntiForgeryToken]
        [ADDUpdateAction]
        [HttpPost]
        [Route("SetUserRights", Name = "SetUserRights")]
        public ActionResult SetUserRights(int FileID,string UserRights)
        {
            Response ret = new Response();
            FileExplorerRepo objRepo = new FileExplorerRepo();
            try
            {
                ret = objRepo.SetUserRights(FileID, IvapUser.EID, UserRights);
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch { throw; }
        }
    }
}