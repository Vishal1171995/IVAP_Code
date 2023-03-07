
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
    public class GlobalLocationRepo
    {
        public Response AddUpdateGlobalLocation(GlobalLocationModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@LID",model.TID),
                new SqlParameter("@LOC_CODE", model.LOC_CODE),
                new SqlParameter("@LOC_NAME", model.LOC_NAME),
                 new SqlParameter("@STATE_ID", model.STATE_ID),
                new SqlParameter("@ISMETRO", model.ISMETRO),
                //new SqlParameter("@STATE_ID", model.State_Id),
                //new SqlParameter("@ISMETRO", model.Is_Metro),
                 new SqlParameter("@UID", model.CreatedBy),
                  new SqlParameter("@ISACTIVE", model.IsActive),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateGlobalLocation", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    res.Message = "Global Location created successfully.";
                    res.IsSuccess = true;
                    return res;
                }

                if (result == 0)
                {
                    res.Message = "Global Location updated successfully.";
                    res.IsSuccess = false;
                    return res;
                }


                if (result == -1)
                {
                    res.Message = "Location Name must be unique.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == -2)
                {
                    res.Message = "Location Code must be unique.";
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

        public DataTable GetGlobalLocation(GlobalLocationModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@LocID", objModel.TID),
                   // new SqlParameter("@CompID", objModel.Company_Id),
                    new SqlParameter("@StateID", objModel.STATE_ID),
                    new SqlParameter("@LocName", objModel.LOC_NAME),
                    //new SqlParameter("@IsAct", objModel.IsActive),
                };
                dt = DataLib.ExecuteDataTable("GetGlobalLocation", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet GetGridGlobalLocation(GridGLocation GModel)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@FromNumber", GModel.from),
                    new SqlParameter("@ToNumber", GModel.To),
                    new SqlParameter("@SQLSortString", GModel.SortingStr),
                    new SqlParameter("@SQLFilterString", GModel.FilterStr),
                };

                return DataLib.ExecuteDataSet("GetGridGlobalLocation", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetGlobalLoationHis(GlobalLocationModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@LID", objModel.TID),
                };
                dt = DataLib.ExecuteDataTable("GetGlobalLocationHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
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
                            if (sorting[i].field == "Status") sorting[i].field = "Site.IsAct";
                            if (i == 0)
                            {
                                sortingStr = sorting[i].field + " " + sorting[i].dir;
                            }
                            else
                            {
                                sortingStr += " , " + sorting[i].field + " " + sorting[i].dir;
                            }
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
                        if (filter.filters[i].field == "STATUS" && filter.filters[i].value.Trim().ToUpper() == "ACTIVE")
                        {
                            filter.filters[i].field = "Site.IsAct";
                            filter.filters[i].value = " =1";
                            filters += filter.filters[i].field + filter.filters[i].value;
                            break;
                        }
                        else if (filter.filters[i].field == "STATUS" && filter.filters[i].value.Trim().ToUpper() == "INACTIVE")
                        {
                            filter.filters[i].field = "Site.IsAct";
                            filter.filters[i].value = " =0";
                            filters += filter.filters[i].field + filter.filters[i].value;
                        }

                        if (filter.filters[i].@operator == AppConstants.GDFilter.Equal)
                        {
                            condition = " = UPPER('" + filter.filters[i].value.Trim() + "') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.NoEqual)
                        {
                            condition = " != UPPER('" + filter.filters[i].value.Trim() + "') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.StartWith)
                        {
                            condition = " Like UPPER('" + filter.filters[i].value.Trim() + "%') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.contains)
                        {
                            condition = " Like UPPER('%" + filter.filters[i].value.Trim() + "%') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.Doesnotcontain)
                        {
                            condition = " Not Like UPPER('%" + filter.filters[i].value.Trim() + "%') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.EndsWith)
                        {
                            condition = " Like UPPER('%" + filter.filters[i].value.Trim() + "') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.Gte)
                        {
                            condition = " >= UPPER('" + filter.filters[i].value.Trim() + "') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.Gt)
                        {
                            condition = " > UPPER('" + filter.filters[i].value.Trim() + "') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.Lte)
                        {
                            condition = " <= UPPER('" + filter.filters[i].value.Trim() + "') ";
                        }
                        if (filter.filters[i].@operator == AppConstants.GDFilter.Lt)
                        {
                            condition = "< UPPER('" + filter.filters[i].value.Trim() + "') ";
                        }
                        filters += "UPPER(" + filter.filters[i].field + ")" + condition;
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