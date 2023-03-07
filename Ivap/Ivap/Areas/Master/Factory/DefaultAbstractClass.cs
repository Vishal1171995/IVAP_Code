using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.Master.Repository;
using Ivap.Areas.Master.ViewModel;
using Ivap.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Factory
{
    abstract class DefaultAbstractClass
    {
        protected SqlTransaction trans;
        protected SqlConnection conn;
        protected DataTable objdtComponent;
        protected DataTable objdtDefaultFile;
        protected Response res;
        protected int CreatedBy;
        protected int EID;
        private FileSetupModel objFileSetUpmodel;
        private FileSetupRepo objFileRepo;
        private WFSettingModel objPayrollInpputWFModel;
        private WorkFlowSettingRepo objWFRepo;
        private PAYRollWFSettingModel objPMSOutPutWModel;
        private int File_ID = 0;
        private EntityRepo objRepo;
        private int ClientAdmin =0;

        public virtual void CreateTransaction()
        {
            conn = new SqlConnection(DataLib.GetConnectionString());
            conn.Open();
            trans = conn.BeginTransaction();
        }

        public virtual Response EntityCreation(EntityOnBoardVM objModel)
        {
            objRepo = new EntityRepo();
            res = objRepo.CreateEntity(objModel, conn: conn, trans: trans);
            if (res.IsSuccess)
                EID = Convert.ToInt32(res.Data);
            CreatedBy = objModel.EntityModel.CreatedBy;
            return res;
        }
        public virtual void GetComponentDetail()
        {

            objdtComponent = new DataTable();
            SqlParameter[] PNextApprovernew = new SqlParameter[]{
                    };
            objdtComponent = DataLib.ExecuteDataTable("GetTemplateComponent", CommandType.StoredProcedure, PNextApprovernew);

        }
        public virtual void SetCommanProperties(int Entity_ID, int Created_By)
        {
            EID = Entity_ID;
            CreatedBy = Created_By;
        }
        public virtual void CreateHRDEntityComponent()
        {
            if (objdtComponent.Rows.Count > 0 && res.IsSuccess)
            {
                DataView dvHRDMAST = new DataView(objdtComponent);
                dvHRDMAST.RowFilter = "COMPONENT_FILE_TYPE='HRDMAST'";
                EntityComponentModel objEntityModel = new EntityComponentModel();
                objEntityModel.CreatedBy = CreatedBy;
                objEntityModel.EID = EID;
                objEntityModel.File_Type = "HRDMAST";
                //Comma seperated gloabal component ID
                objEntityModel.Globle_Component_ID = dvHRDMAST.ToTable().AsEnumerable().Select(x => x.Field<int>("TID").ToString()).ToList();
                EntityComponentRepo EComRepo = new EntityComponentRepo();
                res = EComRepo.AddUpdateEntityComponent(objEntityModel, Con: conn, Trans: trans);
            }
        }

        public virtual void CreatePAYEntityComponent()
        {
            if (objdtComponent.Rows.Count > 0 && res.IsSuccess)
            {
                DataView dvHRDMAST = new DataView(objdtComponent);
                dvHRDMAST.RowFilter = "COMPONENT_FILE_TYPE='PAYMAST'";
                EntityComponentModel objEntityModel = new EntityComponentModel();
                objEntityModel.CreatedBy = CreatedBy;
                objEntityModel.EID = EID;
                objEntityModel.File_Type = "PAYMAST";
                //Comma seperated gloabal component ID
                objEntityModel.Globle_Component_ID = dvHRDMAST.ToTable().AsEnumerable().Select(x => x.Field<int>("TID").ToString()).ToList();
                EntityComponentRepo EComRepo = new EntityComponentRepo();
                res = EComRepo.AddUpdateEntityComponent(model: objEntityModel, Con: conn, Trans: trans);
            }
        }

        public virtual void PublishData()
        {

            EntityComponentRepo objEntityComponentRepo = new EntityComponentRepo();
            res = objEntityComponentRepo.setPublishHRDMaster(EID: EID, trans: trans, con: conn);
            if (res.IsSuccess)
                res = objEntityComponentRepo.setPublishPayMaster(EID: EID, tran: trans, con: conn);
        }
        public virtual void FileCreation()
        {
            if (res.IsSuccess)
            {
                GetDefaultFileDetail();
                foreach (DataRow objDR in objdtDefaultFile.Rows)
                {
                    objFileSetUpmodel = new FileSetupModel();
                    objFileSetUpmodel.CATEGORY = Convert.ToString(objDR["File_Category"]);
                    objFileSetUpmodel.EID = EID;
                    objFileSetUpmodel.FILE_TYPE = Convert.ToString(objDR["File_Type"]);
                    objFileSetUpmodel.FILE_NAME = Convert.ToString(objDR["File_Name"]);
                    objFileSetUpmodel.FILE_DESC = Convert.ToString(objDR["File_Description"]);
                    objFileSetUpmodel.IsActive = true;
                    objFileSetUpmodel.CreatedBy = CreatedBy;
                    objFileSetUpmodel.Transpose = false;
                    objFileSetUpmodel.PayRollInputFile = objDR["PayRollInputFile"] == DBNull.Value ? 0 : Convert.ToInt32(GetPayrollSourceInputFile(Convert.ToString(objDR["PayRollInputFile"])));
                    objFileRepo = new FileSetupRepo();
                    res = objFileRepo.SetUpAddUpdateFileType(objFileSetUpmodel, trans: trans, conn: conn);
                    //workflow creation for two kinds of file one is payroll input file and second one is PMS output file type start here 
                    //for Payroll Input File Creation work flow will not run so file id not getting out there
                    if (res.IsSuccess)
                    {
                        File_ID = Convert.ToInt32(res.Data);
                    }
                        if (objFileSetUpmodel.FILE_TYPE == "Payroll Input File")
                    {
                        PayrollInputWorkFlowCreation();
                    }
                    else if (objFileSetUpmodel.FILE_TYPE == "PMS Output File")
                    {
                        PMSOutPutWorkFlowCreation();
                    }
                    //workflow creation for two kinds of file one is payroll input file and second one is PMS output file type end here 
                    //Now Create Component for each file 
                    ComponentCreation(Convert.ToInt32(objDR["TID"]));
                }
            }
        }

        private void ClientAdminUser()
        {
            SqlParameter[] PNextApprovernew = new SqlParameter[]{
                 new SqlParameter("@Entity_ID",EID),
                    };
            ClientAdmin= Convert.ToInt32(DataLib.ExecuteScaler("GetClientAdmin", CommandType.StoredProcedure, PNextApprovernew, con: conn, trans: trans));
        }
        public virtual void DataAccessRights() {
            ClientAdminUser();
            DataTable objAllFile = new DataTable();
            SqlParameter[] PNextApprovernew = new SqlParameter[]{
                 new SqlParameter("@Entity_ID",EID),
                    };
            objAllFile= DataLib.ExecuteDataTable("GetAllFiles", CommandType.StoredProcedure, PNextApprovernew, con: conn, trans: trans);
            if (objAllFile.Rows.Count > 0) {
                DataAccessControlRepo objAccessRepo = new DataAccessControlRepo();
              res=  objAccessRepo.setAddUpdateDataAccess(AccessCheck: string.Join(",", objAllFile.AsEnumerable().Select(x => x.Field<int>("TID")).ToArray()), ActionName: "IVAP_MST_FILETYPE", UID: ClientAdmin, CreatedBy: CreatedBy, con: conn, tran: trans);
            }
        }
        private int GetPayrollSourceInputFile(string PayrollInputFile) {
            
            SqlParameter[] PNextApprovernew = new SqlParameter[]{
                 new SqlParameter("@File_Name",PayrollInputFile),
                 new SqlParameter("@Entity_ID",EID),
                    };
            return Convert.ToInt32(DataLib.ExecuteScaler("GetInputFileID", CommandType.StoredProcedure, PNextApprovernew,con:conn,trans:trans));
        }
        private void PayrollInputWorkFlowCreation()
        {
            if (res.IsSuccess)
            {
                File_ID = Convert.ToInt32(res.Data);
                objPayrollInpputWFModel = new WFSettingModel();
                objWFRepo = new WorkFlowSettingRepo();
                //Now set file ID to set workflow for payroll input file type
                objPayrollInpputWFModel.FILE_ID = File_ID;
                objPayrollInpputWFModel.EID = EID;
                objWFRepo.GetWorkFlowSetting(objPayrollInpputWFModel,con:conn,tran:trans);
                objPayrollInpputWFModel.WorkflowSettingChecker.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingClientAdmin.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingMaker.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingMyndChcker.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingMyndMaker.ISCheck = true;

                res = objWFRepo.AddUpdateWorkFlowSetting(Model: objPayrollInpputWFModel, Con: conn, Trans: trans);
            }
        }
        private void PMSOutPutWorkFlowCreation()
        {
            if (res.IsSuccess)
            {
                File_ID = Convert.ToInt32(res.Data);
                objPMSOutPutWModel = new PAYRollWFSettingModel();
                //Now set file ID to set workflow for pms output file type
                objPMSOutPutWModel.FILE_ID = File_ID;
                objPMSOutPutWModel.EID = EID;
                objWFRepo.GetPayRollWorkFlowSetting(objPMSOutPutWModel);

                objPayrollInpputWFModel.WorkflowSettingChecker.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingClientAdmin.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingMaker.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingMyndChcker.ISCheck = true;
                objPayrollInpputWFModel.WorkflowSettingMyndMaker.ISCheck = true;

                res = objWFRepo.PayRollAddUpdateWorkFlowSetting(Model: objPMSOutPutWModel, Con: conn, Trans: trans);

            }
        }

        private void ComponentCreation(int TemplateID)
        {
            if (res.IsSuccess)
            {
                if (File_ID != 0)
                {
                    objFileRepo = new FileSetupRepo();
                    objFileSetUpmodel = new FileSetupModel();
                    objFileSetUpmodel.FileID = File_ID;
                    objFileSetUpmodel.CreatedBy = CreatedBy;
                    objFileSetUpmodel.EID = EID;
                    objFileSetUpmodel.ENTITY_Component_ID = GetFileComponent(TemplateID);
                    if (objFileSetUpmodel.ENTITY_Component_ID.Count() > 0)
                    {
                        res = objFileRepo.SetAddUpdateFileCompDetail(model: objFileSetUpmodel, con: conn, trans: trans);
                    }
                }
            }
        }


        private List<string> GetFileComponent(int TemplateID)
        {
            return objRepo.GetComponentForFile(TemplateID: TemplateID, Entity_ID: EID).AsEnumerable().Select(x => x.Field<int>("TID").ToString()).ToList();
        }

        private void GetDefaultFileDetail()
        {
            objdtDefaultFile = objRepo.GetDefaultFileDetail();
        }
        public virtual void CloseTransaction()
        {
            if (res.IsSuccess)
            {
                trans.Commit();
            }
            else if (res.IsSuccess == false && Convert.ToString(res.Message).Contains("component must be unique"))
            {
                trans.Commit();
                res.IsSuccess = true;
                res.Message = "Entity Sucessfully Created";
            }
            else
            {
                trans.Rollback();
            }
            conn.Dispose();
        }

        public Response Run(EntityOnBoardVM objModel)
        {
            try
            {
                CreateTransaction();
                EntityCreation(objModel);
                GetComponentDetail();
                SetCommanProperties(Entity_ID: EID, Created_By: CreatedBy);
                CreateHRDEntityComponent();
                CreatePAYEntityComponent();
                PublishData();
                FileCreation();
                DataAccessRights();
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Message = "error in Default setup configuration please try again";
            }
            finally
            {
                CloseTransaction();
            }
            return res;
        }
    }
}