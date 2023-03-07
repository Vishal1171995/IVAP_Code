using Ivap.ActionFilters;
using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.InputProcessing.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Controllers
{
    [CustomAuthActionFilter(Order = 0)]
    [CustomAuthrizationActionFilter(Order = 1)]
    [RouteArea("Configuration", AreaPrefix = "")]
    [RoutePrefix("Configuration")]
    public class FileSetupController : IvapBaseController
    {
        // GET: Configuration/FileSetup
        [ViewAction]
        [Route("FileSetupList", Name = "FileSetupList")]
        public ActionResult FileSetupList()
        {

            FileSetupModel CModel = new FileSetupModel();
            FileSetupRepo FileRepo = new FileSetupRepo();
            GlobalComponentModel GModel = new GlobalComponentModel();
            DataTable Dt = new DataTable();
            EntityComponentRepo objRepo = new EntityComponentRepo();
            bool IsPublished = false;
            try
            {
                Dt = objRepo.CheckPublistStatus(IvapUser.EID).Tables[0];
                if (Dt.Rows.Count > 0)
                    IsPublished = true;
                ViewBag.IsPublished = IsPublished;
                CModel.FILE_CATEGORY = DropdownUtils.ToSelectList(FileRepo.GetCategory(CModel), "CATEGORY", "CATEGORY");
                CModel.PayRollInputFileList= DropdownUtils.ToSelectList(FileRepo.PayRollInputFile(IvapUser.EID), "TID", "FILE_NAME");
                return View(CModel);
            }
            catch { throw; }

        }

        [ViewAction]
        [Route("TransposeFileComponent", Name = "TransposeFileComponent")]
        public ActionResult TransposeFileComponent(int FileID)
        {

            TransposeModel CModel = new TransposeModel();
            FileSetupRepo FileRepo = new FileSetupRepo();
            DataTable Dt = new DataTable();
            try
            {
                UploadInputRepo ObjR = new UploadInputRepo();
                DataTable dtSchema = ObjR.GetSchemaOfInputFile(IvapUser.EID, FileID);

                //Dt = FileRepo.GetTranspose(FileID);
                DataTable dtcomponent = dtSchema.DefaultView.ToTable(false, "COMPONENT_NAME");
                Response res = new Response();
                res.Data = JsonSerializer.SerializeTable(dtcomponent);
                res.IsSuccess = true;
                //return Json(res, JsonRequestBehavior.AllowGet);
                return PartialView("TransposeView", CModel);
            }
            catch { throw; }

        }


        [ViewAction]
        [Route("TransposeFile", Name = "TransposeFile")]
        public ActionResult TransposeFile(int FileID)
        {

            TransposeModel CModel = new TransposeModel();
            FileSetupRepo FileRepo = new FileSetupRepo();
            DataTable Dt = new DataTable();
            try
            {
                UploadInputRepo ObjR = new UploadInputRepo();
                DataTable dtSchema = FileRepo.GetTransposeComponent(IvapUser.EID,FileID);
                DataTable dtcomponent = dtSchema.DefaultView.ToTable(false, "COMPONENT_NAME", "COMPONENT_DISPLAY_NAME");
                Response res = new Response();
                res.Data = JsonSerializer.SerializeTable(dtcomponent);
                res.IsSuccess = true;
                 return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch { throw; }

        }

        [ViewAction]
        [Route("GetTransposeFile", Name = "GetTransposeFile")]
        public ActionResult GetTransposeFile(int FileID)
        {

            FileSetupModel CModel = new FileSetupModel();
            FileSetupRepo FileRepo = new FileSetupRepo();
          
            try
            {
                Response Res = new Response();
                DataTable dt = FileRepo.GetTranspose(FileID);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);


            }
            catch { throw; }

        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateFileType", Name = "AddUpdateFileType")]
        public ActionResult AddUpdateFileType(FileSetupModel Model)
        {

            FileSetupRepo objrepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                if (Model.FILE_TYPE == "PMS Input File")
                {
                    if (Model.PayRollInputFile == 0)
                    {
                        res.Message = "Please Select SourcePayRoll Input File.";
                        res.IsSuccess = true;
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                res = objrepo.AddUpdateFileType(Model);

                if (res.Data != null)
                {
                    objrepo.SetWorkFlowForFileSetup(Model.FILE_TYPE, Convert.ToInt32(res.Data),IvapUser.EID);

                }

                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                throw;
            }
        }



        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateTranspose", Name = "AddUpdateTranspose")]
        public ActionResult AddUpdateTranspose(TransposeModel Model)
        {
            FileSetupRepo objrepo = new FileSetupRepo();
            Response res = new Response();
            DataTable dt = new DataTable();
            try
            {

                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                
                res = objrepo.AddUpdateTranspose(Model);
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                throw;
            }
        }



        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("UpdateFileComponent", Name = "UpdateFileComponent")]
        public ActionResult UpdateFileComponent(FileComponentModel Model)
        {
            FileSetupRepo objrepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                if (Model.COMPONENT_DATATYPE == "MASTER")
                {
                    res = objrepo.UpdateFileComponentMASTER(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (Model.MIN_VALUE < Model.MIN_LENGTH)
                    {

                        res.IsSuccess = false;
                        res.Message = "Did not Update.....Min Length value excceded";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                    if (Model.MAX_VALUE < Model.MAX_LENGTH)
                    {
                        res.IsSuccess = false;
                        res.Message = "Did not Update......MAX Length value excceded";
                        return Json(res, JsonRequestBehavior.AllowGet);

                    }
                    res = objrepo.UpdateFile_Component(Model);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }



            }
            catch
            {
                throw;
            }
        }



        [ViewAction]
        [HttpGet]
        [Route("GetFileType", Name = "GetFileType")]
        public ActionResult GetFileType(int FileID)
        {
            FileSetupRepo objUBO = new FileSetupRepo();
            FileSetupModel objModel = new FileSetupModel();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                objModel.FileID = FileID;
                objModel.EID = IvapUser.EID;
                objModel.CreatedBy = IvapUser.UID;
                DataTable dt = objUBO.GetFileType(objModel);
                res = objUBO.GetCommandButtonForGrid_For_FileSetup("FileSetupList");
                res.Data = JsonSerializer.SerializeTable(dt);
                //return Json(res, JsonRequestBehavior.AllowGet);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ViewAction]
        [Route("GetEntityComponentsList", Name = "GetEntityComponentsList")]
        public ActionResult GetEntityComponentsList(string Search_Text, string EntityCompID, int FileID)
        {
            string Globle_Component_ID = string.Join(",", EntityCompID);
            FileSetupRepo objRepo = new FileSetupRepo();
            try
            {
                Response Res = new Response();
                DataTable dt = objRepo.SearchFileComponent(IvapUser.EID, Search_Text, EntityCompID, FileID);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetTransposeByID", Name = "GetTransposeByID")]
        public ActionResult GetTransposeByID(int TID)
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            TransposeModel objModel = new TransposeModel();
            KendoGridUtils res = new KendoGridUtils();
            try
            {
                //objModel.TID = TID;
                DataTable dt = objRepo.GetTransposeByID(TID);
                res.Data = JsonSerializer.SerializeTable(dt);
                var UserGrid = Json(res, JsonRequestBehavior.AllowGet);
                UserGrid.MaxJsonLength = int.MaxValue;
                return UserGrid;

            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateFileCompDetail", Name = "AddUpdateFileCompDetail")]
        public ActionResult AddUpdateFileCompDetail(List<string> EntityCompID, int FileID)
        {
            FileSetupModel model = new FileSetupModel();
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                model.CreatedBy = IvapUser.UID;
                model.EID = IvapUser.EID;
                model.ENTITY_Component_ID = EntityCompID;
                model.FileID = FileID;
                res = objRepo.AddUpdateFileCompDetail(model);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }



        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("ResetFileComponent", Name = "ResetFileComponent")]
        public ActionResult ResetFileComponent(int FileID)
        {
            FileSetupModel model = new FileSetupModel();
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                res = objRepo.ResetFileComponent(FileID);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }




        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("DeleteFileType", Name = "DeleteFileType")]
        public ActionResult DeleteFileType(int FileID)
        {
            FileSetupModel model = new FileSetupModel();
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                model.FileID = FileID;
                model.EID = IvapUser.EID;
                res = objRepo.DeleteFileType(model);
            }
            catch
            {
                res.IsSuccess = false;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [ViewAction]
        [Route("GetFileCompDtl", Name = "GetFileCompDtl")]
        public ActionResult GetFileCompDtl(int FileID)
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            FileSetupModel objModel = new FileSetupModel();
            Response Res = new Response();
            try
            {
                objModel.FileID = FileID;
                objModel.EID = IvapUser.EID;
                DataTable dt = objRepo.GetFileCompDtl(objModel);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [Route("GetFileComponent", Name = "GetFileComponent")]
        public ActionResult GetFileComponent(int TID)
        {

            FileSetupRepo objRepo = new FileSetupRepo();
            FileSetupModel objModel = new FileSetupModel();
            FileComponentModel RtModelComponent = new FileComponentModel();
            Response Res = new Response();
            try
            {
              
                RtModelComponent = objRepo.GetFileComponentByTID(TID);
                RtModelComponent.Validation_List = DropdownUtils.ToSelectList(objRepo.GetValidation(RtModelComponent), "VALIDATION_FIELD", "VALIDATION_FIELD");
                return PartialView("FileComponent", RtModelComponent);
            }
            catch
            {
                throw;
            }
        }




        [ViewAction]
        [Route("GetFileComponentHis", Name = "GetFileComponentHis")]
        public ActionResult GetFileComponentHis(int FileID)
        {

            FileSetupRepo objRepo = new FileSetupRepo();
            FileSetupModel objModel = new FileSetupModel();
            FileComponentModel RtModel = new FileComponentModel();
            Response Res = new Response();
            try
            {

                DataTable dt = objRepo.GetFileComponentHis(FileID);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [Route("GetFileComponentView", Name = "GetFileComponentView")]
        public ActionResult GetFileComponentView(int FileID)
        {

            FileSetupRepo objRepo = new FileSetupRepo();
            FileSetupModel objModel = new FileSetupModel();
            FileComponentModel RtModel = new FileComponentModel();
            Response Res = new Response();
            try
            {

                DataTable dt = objRepo.GetFileComponent(FileID);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }



        [ViewAction]
        [Route("GetFileHis", Name = "GetFileHis")]
        public ActionResult GetFileHis(int FileID)
        {

            FileSetupRepo objRepo = new FileSetupRepo();
            FileSetupModel objModel = new FileSetupModel();
            FileComponentModel RtModel = new FileComponentModel();
            Response Res = new Response();
            try
            {

                DataTable dt = objRepo.GetFileHis(FileID);
                Res.Data = JsonSerializer.SerializeTable(dt);
                Res.IsSuccess = true;
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("DeleteFileCompDtl", Name = "DeleteFileCompDtl")]
        public ActionResult DeleteFileCompDtl(string FileCompDtlIDs)
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                res = objRepo.DeleteFileCompDtl(IvapUser.EID, FileCompDtlIDs);
            }
            catch
            {
                res.IsSuccess = false;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //SetOrderTranspose_Down
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetDisplayOrderTranspose_UP", Name = "SetDisplayOrderTranspose_UP")]
        public ActionResult SetDisplayOrderTranspose_UP(int TID, int FileID)
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            DataTable dt = new DataTable();
            try
            {
                res = objRepo.SetDisplayOrderTranspose_UP(TID, FileID, IvapUser.EID);
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
                //return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetOrderTranspose_Down", Name = "SetOrderTranspose_Down")]
        public ActionResult SetOrderTranspose_Down(int TID, int FileID)
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            DataTable dt = new DataTable();
            try
            {
                res = objRepo.SetOrderTranspose_Down(TID, FileID, IvapUser.EID);

                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
                //return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetOrderFileCompDtl_UP", Name = "SetOrderFileCompDtl_UP")]
        public ActionResult SetOrderFileCompDtl_UP(int TID, int FileID)
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                res = objRepo.SetOrderFileCompDtl_UP(TID, FileID, IvapUser.EID);
                return Json(res);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetOrderFileCompDtl_Down", Name = "SetOrderFileCompDtl_Down")]
        public ActionResult SetOrderFileCompDtl_Down(int TID, int FileID)
        {
            FileSetupRepo objRepo = new FileSetupRepo();
            Response res = new Response();
            try
            {
                res = objRepo.SetOrderFileCompDtl_Down(TID, FileID, IvapUser.EID);
                return Json(res);
            }
            catch
            {
                throw;
            }
        }

        [ADDUpdateAction]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        [Route("SetDisplayOrder_FileCompDtl", Name = "SetDisplayOrder_FileCompDtl")]
        public ActionResult SetDisplayOrder_FileCompDtl(List<FileDtlModel> Model)
        {
            Response res = new Response();
            FileSetupRepo objRepo = new FileSetupRepo();
            try
            {
                if (ModelState.IsValid)
                {
                    res = objRepo.SetDisplayOrder_FileCompDtl(Model, IvapUser.EID);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("FileSetupSample", Name = "FileSetupSample")]
        public ActionResult FileSetupSample(int FileID, string File_Name)
        {
            FileSetupRepo objFileSetupRepo = new FileSetupRepo();
            DataTable dt = new DataTable();
            try
            {
                int EID = IvapUser.EID;
                dt = objFileSetupRepo.FileSetupSampleDownload(EID, FileID);
                string FileName = objFileSetupRepo.DataTableToExcel_FileSample(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, File_Name + ".xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [ViewAction]
        [HttpGet]
        [Route("DownloadFileComponent", Name = "DownloadFileComponent")]
        public ActionResult DownloadFileComponent(int FileID, string File_Name)
        {
           
            FileSetupRepo objFileSetupRepo = new FileSetupRepo();
            DataTable dt = new DataTable();
            try
            {
                dt = objFileSetupRepo.DownLoadFileComponent(FileID);
                string FileName = ExcellUtils.DataTableToExcel(dt);
                FileName = FileName.Replace("/", "").Replace("..", "").Replace("\\", "");
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, File_Name + ".xlsx");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("getComponentTableNameFile", Name = "getComponentTableNameFile")]
        public ActionResult getComponentTableNameFile(string TableName)
        {
            Response res = new Response();
            GlobalComponentRepo CRepo = new GlobalComponentRepo();
            try
            {
                DataTable dt = CRepo.GetComponentTableName(TableName);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ViewAction]
        [Route("PmsSpecialComponent", Name = "PmsSpecialComponent")]
        public ActionResult PmsSpecialComponent()
        {

            TransposeModel CModel = new TransposeModel();
            SpecialComponentModel Smodel = new SpecialComponentModel();
            FileSetupRepo FileRepo = new FileSetupRepo();
            DataTable Dt = new DataTable();
            try
            {
                
                return PartialView("SpecialComponent",Smodel);
            }
            catch { throw; }

        }

        [ViewAction]
        [HttpGet]
        [Route("GetSpecialComponentName", Name = "GetSpecialComponentName")]
        public ActionResult GetSpecialComponentName(string searchStr)
        {
            FileSetupRepo objFileSetupRepo = new FileSetupRepo();
            Response res = new Response();
            DataTable dt = new DataTable();
            try
            {
                int EID = IvapUser.EID;
                dt = objFileSetupRepo.GetSpecialComponentName(EID,searchStr);
                res.IsSuccess = true;
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }


        [ADDUpdateAction]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("AddUpdateSpecialComponent", Name = "AddUpdateSpecialComponent")]
        public ActionResult AddUpdateSpecialComponent(SpecialComponentModel Model)
        {
            FileSetupRepo objrepo = new FileSetupRepo();
            //SpecialComponentModel objModel = new SpecialComponentModel();
            Response res = new Response();
            DataTable dt = new DataTable();
            try
            {
                Model.CreatedBy = IvapUser.UID;
                Model.EID = IvapUser.EID;
                res = objrepo.AddUpdateSpecialComponent(Model);
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                throw;
            }
        }

        [ViewAction]
        [Route("GetSpecialFileComponentTid", Name = "GetSpecialFileComponentTid")]
        public ActionResult GetSpecialFileComponentTid(int TID)
        {

            FileSetupRepo objRepo = new FileSetupRepo();
            SpecialComponentModel objModel = new SpecialComponentModel();
            //FileComponentModel RtModelComponent = new FileComponentModel();
            Response Res = new Response();
            try
            {

                objModel = objRepo.GetSpecialFileComponentTid(TID);
                return PartialView("SpecialComponent", objModel);
            }
            catch
            {
                throw;
            }
        }

    }
}
