using Ivap.Areas.Configuration.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Ivap.Areas.Configuration.Repository
{
    public class ComponentRepo
    {
        public Response AddUpdateComponent(ComponentModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ComponentID",model.COMPONENTID),
                    new SqlParameter("@COMPONENT_FILE_TYPE",model.COMPONENT_FILE_TYPE),
                    new SqlParameter("@COMPONENT_TYPE",model.COMPONENT_TYPE),
                    new SqlParameter("@COMPONENT_SUB_TYPE",model.COMPONENT_SUB_TYPE),
                    new SqlParameter("@COMPONENT_NAME",model.COMPONENT_NAME),
                    new SqlParameter("@COMPONENT_DATATYPE",model.COMPONENT_DATATYPE),
                    new SqlParameter("@COMPONENT_DISPLAY_NAME",model.COMPONENT_DISPLAY_NAME),
                    new SqlParameter("@COMPONENT_DESCRIPTION",model.COMPONENT_DESCRIPTION),
                    new SqlParameter("@MIN_LENGTH",model.MIN_LENGTH),
                     new SqlParameter("@MAX_LENGTH",model.MAX_LENGTH),
                    new SqlParameter("@MANDATORY",model.MANDATORY),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACT",model.IsActive),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateComponent", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.COMPONENTID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Component", result, "Component Name");
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetComponent(ComponentModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ComponentID",model.COMPONENTID),
                    new SqlParameter("@CompFileType",model.COMPONENT_FILE_TYPE),
                    new SqlParameter("@ISAct",model.IsActive)
                };

                return DataLib.ExecuteDataTable("GetComponent", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }



        public DataTable GetComponentHistory(ComponentModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@COMPONENT_ID",model.COMPONENTID ),
                };
                return DataLib.ExecuteDataTable("GetComponentHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
       

       
    }
}