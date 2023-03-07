using Ivap.Areas.Master.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Repository
{
    public class MinWageRepo
    {
        public object SqlDataLib { get; private set; }
        public Response AddUpdateMinWage(MinWageModel model)
        {
            Response res = new Response();
            try
            {
                //if (model.EFF_DT_FROM != null)
                //{
                //    string[] arrFromDate = model.EFF_DT_FROM.Split('/');
                //    model.EFF_DT_FROM = arrFromDate[2] + "-" + arrFromDate[1] + "-" + arrFromDate[0];
                //}
                //if (model.EFF_DATE_TO != null)
                //{
                //    string[] arrToDate = model.EFF_DATE_TO.Split('/');
                //    model.EFF_DATE_TO = arrToDate[2] + "-" + arrToDate[1] + "-" + arrToDate[0];
                //}
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@MinWageID",model.MinWageID),
                    new SqlParameter("@STATE_ID",model.STATE_ID),
                    //new SqlParameter("@LOCATION_ID",model.LOCATION_ID),
                    new SqlParameter("@CATEGORY",model.CATEGORY),
                    new SqlParameter("@MIN_WAGE",model.MIN_WAGE),
                    new SqlParameter("@EFF_DT_FROM",model.EFF_DT_FROM),
                    new SqlParameter("@EFF_DATE_TO",model.EFF_DATE_TO),
                    new SqlParameter("@ISACTIVE",model.IsActive),
                    new SqlParameter("@UID",model.CreatedBy),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateMinWage", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.MinWageID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "MinWage", result, "Min Wage");
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetMinWage(MinWageModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@MinWageID", objModel.MinWageID),
                    new SqlParameter("@STATE_ID", objModel.STATE_ID),
                    //new SqlParameter("@LOCATION_ID", objModel.LOCATION_ID),
                    new SqlParameter("@IsAct", objModel.IsActive),
                };
                dt = DataLib.ExecuteDataTable("GetMinWage", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetMinWageHistory(MinWageModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@MinWageID", objModel.MinWageID),
                };
                dt = DataLib.ExecuteDataTable("GetMinWageHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}