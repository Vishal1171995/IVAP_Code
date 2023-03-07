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
    public class RoleRepo
    {
        public object SqlDataLib { get; private set; }
        public Response AddUpdateRole(RoleModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@RoleID", model.RoleID),
                //new SqlParameter("@EntityID", model.EntityID),
                new SqlParameter("@RoleName", model.RoleName),
                new SqlParameter("@RoleType", model.RoleType),
                new SqlParameter("@IsAct", model.IsActive),
                new SqlParameter("@UID", model.CreatedBy),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateRole", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.RoleID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Role", result, "Role name");
                return res;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public DataSet GetGridRoles(GridRoles gRoles)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FromNumber", gRoles.from),
                    new SqlParameter("@ToNumber", gRoles.To),
                    new SqlParameter("@SQLSortString", gRoles.SortingStr),
                    new SqlParameter("@SQLFilterString", gRoles.FilterStr)
                };
                ds = DataLib.ExecuteDataSet("GetGridRoles", CommandType.StoredProcedure, parameters);
                return ds;
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetRoles(RoleModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@RoleID", objModel.RoleID),
                    //new SqlParameter("@EntityID", objModel.EntityID),
                    new SqlParameter("@ROLENAME", objModel.RoleName),
                    new SqlParameter("@IsAct", objModel.IsActive),
                };
                dt = DataLib.ExecuteDataTable("GetRoles", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public DataTable GetUserRoleHis(RoleModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ROLEID", objModel.RoleID),
                };
                dt = DataLib.ExecuteDataTable("GetUserRoleHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        #region Grid Sort And Filter
        public string sortGrid(List<SortDescription> sorting)
        {
            string sortingStr = "";
            try
            {
                if (sorting != null)
                {
                    if (sorting.Count != 0)
                    {
                        for (int i = 0; i < sorting.Count; i++)
                        {
                            if (sorting[i].field == "Status") sorting[i].field = "IsAct";
                            sortingStr += ", " + sorting[i].field + " " + sorting[i].dir;
                        }
                    }
                }
                return sortingStr;
            }
            catch
            {
                throw;
            }
        }

        public string FilterGrid(FilterContainer filter)
        {
            string filters = "";
            string logic;
            string condition = "";
            try
            {

                int c = 1;
                if (filter != null)
                {
                    for (int i = 0; i < filter.filters.Count; i++)
                    {
                        logic = filter.logic;

                        //filter.filters[i].field
                        if (filter.filters[i].field == "Status") filter.filters[i].field = "IsAct";

                        if (filter.filters[i].@operator == "eq")
                        {
                            condition = " = '" + filter.filters[i].value + "' ";
                        }
                        if (filter.filters[i].@operator == "neq")
                        {
                            condition = " != '" + filter.filters[i].value + "' ";
                        }
                        if (filter.filters[i].@operator == "startswith")
                        {
                            condition = " Like '" + filter.filters[i].value + "%' ";
                        }
                        if (filter.filters[i].@operator == "contains")
                        {
                            condition = " Like '%" + filter.filters[i].value + "%' ";
                        }
                        if (filter.filters[i].@operator == "doesnotcontains")
                        {
                            condition = " Not Like '%" + filter.filters[i].value + "%' ";
                        }
                        if (filter.filters[i].@operator == "endswith")
                        {
                            condition = " Like '%" + filter.filters[i].value + "' ";
                        }
                        if (filter.filters[i].@operator == "gte")
                        {
                            condition = " >= '" + filter.filters[i].value + "' ";
                        }
                        if (filter.filters[i].@operator == "gt")
                        {
                            condition = " > '" + filter.filters[i].value + "' ";
                        }
                        if (filter.filters[i].@operator == "lte")
                        {
                            condition = " <= '" + filter.filters[i].value + "' ";
                        }
                        if (filter.filters[i].@operator == "lt")
                        {
                            condition = "< '" + filter.filters[i].value + "' ";
                        }
                        filters += filter.filters[i].field + condition;
                        if (filter.filters.Count > c)
                        {
                            filters += logic;
                            filters += " ";
                        }
                        c++;
                    }
                }
                return filters;
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}