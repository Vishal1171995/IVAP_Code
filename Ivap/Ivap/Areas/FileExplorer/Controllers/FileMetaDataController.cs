using Ivap.ActionFilters;
using Ivap.Areas.FileExplorer.Models;
using Ivap.Areas.FileExplorer.Repository;
using Ivap.Controllers;
using Ivap.CustomAttribute;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.FileExplorer.Controllers
{
    [CustomAuthrizationActionFilter(Order = 1)]
    [CustomAuthActionFilter(Order = 0)]
    [RouteArea("FileExplorer")]
    public class FileMetaDataController : IvapBaseController
    {
        [ViewAction]
        [Route("FileMetaData", Name = "FileMetaData")]
        public ActionResult FileMetaData()
        {
            FileMetaDataModel Model = new FileMetaDataModel();
            try
            {
                return View(Model);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ADDUpdateAction]
        [Route("CreateFileMetaData", Name = "CreateFileMetaData")]
        public ActionResult CreateFileMetaData(FileMetaDataModel model)
        {
            FileMetaDataRepo repMetadata = new FileMetaDataRepo();
            Response res = new Response();
            try
            {
                model.CreatedBy = IvapUser.UID;
                model.EID = IvapUser.EID;
                res = repMetadata.CreateUpdateMetaData(model);
                return Json(res);
            }
            catch
            {
                //res = res.GetResponse("Company", "", -2);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [ViewAction]
        [HttpGet]
        [Route("GetMetaData", Name = "GetMetaData")]
        public ActionResult GetMetaData(int MetaDataID)
        {
            FileMetaDataRepo repMetadata = new FileMetaDataRepo();
            FileMetaDataModel Model = new FileMetaDataModel();
            Model.FileMetaID = MetaDataID;
            Model.EID = IvapUser.EID;
            Response res = new Response();
            try
            {
                DataTable dt = repMetadata.GetMetaDataList(Model);
                res.IsSuccess = true;
                res.Data = JsonSerializer.SerializeTable(dt);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }
    }
}