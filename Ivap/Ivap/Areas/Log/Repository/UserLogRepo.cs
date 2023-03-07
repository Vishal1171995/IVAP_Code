using Ivap.Areas.Log.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Log.Repository
{
    public class UserLogRepo
    {

        public DataTable GetUserLog(int UID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{

                    new SqlParameter("@UID", UID),
                };
                dt = DataLib.ExecuteDataTable("GetUserLog", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataSet GetUserLog_New(GridUserLogClass GModel)
        {
            DataSet dt = new DataSet();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                     new SqlParameter("@FromNumber", GModel.from),
                    new SqlParameter("@ToNumber", GModel.To),
                    new SqlParameter("@SQLSortString", GModel.SortingStr),
                    new SqlParameter("@SQLFilterString", GModel.FilterStr),
                    new SqlParameter("@EID", GModel.EID),
                };
                dt = DataLib.ExecuteDataSet("GetUserLog_New", CommandType.StoredProcedure, parameters);
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
                            //if (sorting[i].field == "Status") sorting[i].field = "Site.IsAct";
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
                        //if (filter.filters[i].field == "STATUS" && filter.filters[i].value.Trim().ToUpper() == "ACTIVE")
                        //{
                        //    filter.filters[i].field = "Site.IsAct";
                        //    filter.filters[i].value = " =1";
                        //    filters += filter.filters[i].field + filter.filters[i].value;
                        //    break;
                        //}
                        //else if (filter.filters[i].field == "STATUS" && filter.filters[i].value.Trim().ToUpper() == "INACTIVE")
                        //{
                        //    filter.filters[i].field = "Site.IsAct";
                        //    filter.filters[i].value = " =0";
                        //    filters += filter.filters[i].field + filter.filters[i].value;
                        //}

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
        #endregion Grid Sort And Filter

    }
}