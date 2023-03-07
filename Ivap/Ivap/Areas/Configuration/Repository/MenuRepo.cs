using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Configuration.Repository
{
    public class MenuRepo
    {
        public object SqlDataLib { get; private set; }
        public Response UpdateMenu(MenuModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@EID",model.EID),
                    new SqlParameter("@MenuName",model.MenuName),
                    new SqlParameter("@Roles",model.Roles),
                    new SqlParameter("@CreatedBy",model.CreatedBy),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("UpdateMenu", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                opration = Response.operation.Update;
                res = res.GetResponse(opration, "Menu", result, "Menu Name");
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetMenu(MenuModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", objModel.TID),
                    new SqlParameter("@MenuName", objModel.MenuName),
                    new SqlParameter("@EID", objModel.EID),
                };
                dt = DataLib.ExecuteDataTable("GetMenu", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response ActiveInactiveMenu(MenuModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@IsAct",model.IsActive),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ChangeMenuStatus", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Status Change Sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response SetDisplayOrder_UP(MenuModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@Name",model.MenuName),
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@EID",model.EID),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetDisplayOrder_UP", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Status Change Sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response SetDisplayOrder_down(MenuModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@Name",model.MenuName),
                    new SqlParameter("@PID",model.PID),
                    new SqlParameter("@EID",model.EID),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetDisplayOrder_down", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Status Change Sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response GetRoles(int RoleID)
        {
            Response ret = new Response();
            RoleRepo objRepo = new RoleRepo();
            RoleModel objModel = new RoleModel();
            DataTable Dt = new DataTable();
            try
            {
                objModel.RoleID = RoleID;
                objModel.IsActive = true;
                Dt = objRepo.GetRoles(objModel);
                Dt.Columns.Add("IsView");
                Dt.Columns.Add("IsCreate");
                Dt.Columns.Add("IsDelete");
                Dt.Columns.Add("IsEdit");
                foreach (DataRow dr in Dt.Rows)
                {
                    dr["IsView"] = false;
                    dr["IsCreate"] = false;
                    dr["IsDelete"] = false;
                    dr["IsEdit"] = false;
                }
                ret.Data = JsonSerializer.SerializeTable(Dt);
                ret.IsSuccess = true;
                return ret;
            }
            catch (Exception ex)
            {
                ret.Message = ex.Message;
                ret.IsSuccess = false;
                return ret;
            }
        }
    }
}