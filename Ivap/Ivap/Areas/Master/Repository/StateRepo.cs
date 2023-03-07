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
    public class StateRepo
    {
        public DataTable GetState()
        {
            try
            {
                DataTable dt = DataLib.ExecuteDataTable("GetStateList", CommandType.StoredProcedure, null);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public Response AddUpdateState(StateModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@SID",model.StateId),
                new SqlParameter("@COUNTRY",model.Country_Name),
                new SqlParameter("@STATE_CODE", model.State_Code),
                new SqlParameter("@STATE_NAME", model.State_Name),
                 new SqlParameter("@UID", model.CreatedBy),
                  new SqlParameter("@ISACTIVE", model.IsActive),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateState", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    res.Message = "State created successfully.";
                    res.IsSuccess = true;
                    return res;
                }

                if (result == 0)
                {
                    res.Message = "State updated successfully.";
                    res.IsSuccess = false;
                    return res;
                }


                if (result == -1)
                {
                    res.Message = "State Name must be unique.";
                    res.IsSuccess = true;
                    return res;
                }

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetState(StateModel sm)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@StateID", sm.StateId),
                    new SqlParameter("@StateName", sm.State_Name),
                };
                dt = DataLib.ExecuteDataTable("GetState", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetStateHis(StateModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@SID", objModel.StateId),
                };
                dt = DataLib.ExecuteDataTable("GetStateHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}