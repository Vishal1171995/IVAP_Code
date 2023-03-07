using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Configuration.Repository
{
    public class DataAccessControlRepo
    {
        public DataTable CopyRightUser(int UID,int EID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                   new SqlParameter("@UID", UID),
                    new SqlParameter("@ENTITY_ID", EID),
                };
                dt = DataLib.ExecuteDataTable("GETUSERCOPYRIGHT", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetAccessControl(string ActionName,int EID,int UID)
        {
            DataTable dt = new DataTable();
            try
            {
                    SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ACTION_NAME", ActionName),
                    new SqlParameter("@EID", EID),
                     new SqlParameter("@UID", UID),

                };
                dt = DataLib.ExecuteDataTable("GETDATAACCESSCONTROL", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataTable GetMenuAccess(int EID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                   
                    new SqlParameter("@EID",EID),
                };
                dt = DataLib.ExecuteDataTable("GETUSERMENUACCESS_1", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response setAddUpdateDataAccess(string AccessCheck, string ActionName, int UID, int CreatedBy, SqlConnection con, SqlTransaction tran)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                     new SqlParameter("@DAID","0"),
                    new SqlParameter("@UID",UID),
                    new SqlParameter("@TABLE_NAME",ActionName),
                    new SqlParameter("@TIDS",AccessCheck),
                  new SqlParameter("@ISACTIVE","1"),
                   new SqlParameter("@CREATED_BY",CreatedBy),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ADDUPDATEDATAACCESS", CommandType.StoredProcedure, parameters,con:con,trans:tran));

                if (result > 0)
                {
                    Res.Message = "DataAccessControl created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (result == 0)
                {
                    Res.Message = "DataAccessControl updated successfully.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -1)
                {
                    Res.Message = "Failed!!! DataAccessControl must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }

                return Res;
            }
            catch {
                Res.IsSuccess = false;
                Res.Message = "Error in Data Access control for files";
                return Res;
            }
        }
        public Response AddUpdateDataAccess(string AccessCheck, string ActionName, int UID,int CreatedBy)
        {
            Response Res = new Response();
            SqlConnection con = null;
            SqlTransaction tran = null;
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                con.BeginTransaction();
               Res= setAddUpdateDataAccess(AccessCheck: AccessCheck, ActionName: ActionName, UID: UID, CreatedBy: CreatedBy, con:con, tran:tran);
                if (Res.IsSuccess)
                {
                    tran.Commit();
                }
                else {
                    tran.Rollback();
                }
                con.Dispose();
                return Res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response CopyToAnotherUser(int COPYID, string UID,int EID)
        {
            DataTable dt = new DataTable();
            Response res = new Response();
            try
            {

                string[] User = UID.Split(',');
                dt = GetDataAccess(COPYID);
                for (var j = 0; j < User.Length; j++)
                {
                    if (User[j] != "")
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                res = AddUpdateDataAccess(dt.Rows[i][3].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt32(User[j]), EID);
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception ex) { throw; }
        }

        public Response CopyRightSubmit(int COPYID, int UID)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    
                  new SqlParameter("@UID",UID),
                   new SqlParameter("@COPYUSER",COPYID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("COPYRIGHTTOANOTHERUSER", CommandType.StoredProcedure, parameters));

                if (result > 0)
                {
                    Res.Message = "DataAccessControl created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                return Res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetDataAccess(int UID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@UID", UID),
                    
                };
                dt = DataLib.ExecuteDataTable("GETDATAACCESSUSER", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}