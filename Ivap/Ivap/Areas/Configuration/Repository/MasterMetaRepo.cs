using Ivap.Areas.Master.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Ivap.Areas.Configuration.Models;

namespace Ivap.Areas.Configuration.Repository
{
    public class MasterMetaRepo
    {
        public DataTable GetMasterMeta(MasterMetaModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ENTITYID",model.ENTITY_ID),
                    new SqlParameter("@SCREENNAME",model.SCREEN_NAME),
                };

                return DataLib.ExecuteDataTable("GetMataMaster", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetDropdownLabel(MasterMetaModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ENTITYID",model.ENTITY_ID),
                };
                return DataLib.ExecuteDataTable("MASTERLABEL", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }


        public int CheckDublicateMasterMeta(List<MasterMetaModel> model)
        {
            Response res = new Response();
            try
            {
                int result = 0;
                foreach (var Items in model)
                {
                    SqlParameter[] parameters = new SqlParameter[]{
                     new SqlParameter("@ENTITYID",Items.ENTITY_ID),
                    new SqlParameter("@SCREENNAME",Items.SCREEN_NAME),
                    new SqlParameter("@DISPLAY_NAME",Items.DISPLAY_NAME),
                    new SqlParameter("@DISPLAY_ORDER",Items.DISPLAY_ORDER),
                     new SqlParameter("@TID",Items.TID),
                };
                    result = Convert.ToInt32(DataLib.ExecuteScaler("CheckMataMasterCount", CommandType.StoredProcedure, parameters));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response UpdateMasterMeta(List<MasterMetaModel> model)
        {
            int CountDisplay = 0;
            int CountOrder = 0;
            int count = 0;
            Response res = new Response();
            try
            {
                
                CountDisplay = model.GroupBy(p => p.DISPLAY_NAME).Where(x => x.Count() > 1).Count();
                CountOrder = model.GroupBy(p => p.DISPLAY_ORDER).Where(x => x.Count() > 1).Count();
                count = CheckDublicateMasterMeta(model);
                if (CountDisplay > 0)
                {
                    res.Message = "Creation failed.Display Name must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                if (CountOrder > 0)
                {
                    res.Message = "Creation failed. Display Order must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                if (count == 3)
                {
                    res.Message = "Creation failed.Display Name must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                if (count == 2)
                {
                    res.Message = "Creation failed. Display Order must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                 

                foreach (var Items in model) {
                    SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",Items.TID),
                    new SqlParameter("@DISPLAY_NAME",Items.DISPLAY_NAME),
                    new SqlParameter("@DISPLAY_ORDER",Items.DISPLAY_ORDER),
                };
                    int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateChangeLabel", CommandType.StoredProcedure, parameters));
                    Response.operation opration = Response.operation.Update;
                    res = res.GetResponse(opration, "MasterMeta", result, "");
                }
             
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


       

    }
}