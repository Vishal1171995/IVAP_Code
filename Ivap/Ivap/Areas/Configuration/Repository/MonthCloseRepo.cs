using Ivap.Areas.Configuration.Models;
using Ivap.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Configuration.Repository
{
    public class MonthCloseRepo
    {
        public DataTable GetMonthClose(MonthCloseModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", objModel.EID),
                    new SqlParameter("@TID", objModel.TID),
                };
                dt = DataLib.ExecuteDataTable("GetMonthClose", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response SetStatus(MonthCloseModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@EID",model.EID),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@UID",model.CreatedBy),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetStatus_ForMonthClose", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Month " + model.Status + " successfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response SetDefaultCurrentMonth(MonthCloseModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID",model.EID),
                    new SqlParameter("@UID",model.CreatedBy),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetDefaultCurrentMonth", CommandType.StoredProcedure, parameters));
                if(result > 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Current month sucessfully set as default month";
                }
                else if (result == -1)
                {
                    res.IsSuccess = true;
                    res.Message = "Current month already exists.";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response SetDefaultMonth(MonthCloseModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@EID",model.EID),
                    new SqlParameter("@UID",model.CreatedBy),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetDefaultMonth", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Default month set sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public KendoGridUtils GetCommandButtonForGrid(string RouteName)
        {
            try
            {
                KendoGridUtils objKGrid = new KendoGridUtils();
                int CommandCount = 0;
                List<CommandButton> LstCommand = new List<CommandButton>();
                
                if (AuthorizationRepo.IsValidAction(RouteName, "UpdateAction"))
                {
                    CommandButton ObjEditButton = new CommandButton();
                    ObjEditButton.name = "Edit";
                    ObjEditButton.text = "";
                    ObjEditButton.click = "EditHandler";
                    ObjEditButton.iconClass = "kIcon kIconEdit ";
                    ObjEditButton.title = "Edit";
                    LstCommand.Add(ObjEditButton);
                    CommandCount++;

                    CommandButton ObjEditButton1 = new CommandButton();
                    ObjEditButton1.name = "DefaultMonth";
                    ObjEditButton1.text = "";
                    ObjEditButton1.click = "Set_Default_Month";
                    ObjEditButton1.iconClass = "kIcon k-config";
                    ObjEditButton1.title = "Set Default Month";
                    LstCommand.Add(ObjEditButton1);
                    CommandCount++;
                }
                if (AuthorizationRepo.IsValidAction(RouteName, "ViewAction"))
                {
                    CommandButton ObjViewButton = new CommandButton();
                    ObjViewButton.name = "View";
                    ObjViewButton.text = "";
                    ObjViewButton.click = "ViewHandler";
                    ObjViewButton.iconClass = "kIcon kIconView";
                    ObjViewButton.title = "View";
                    LstCommand.Add(ObjViewButton);
                    CommandCount++;
                }
                if (CommandCount > 0)
                {
                    Command ObjCommand = new Command();
                    ObjCommand.title = "Action";
                    ObjCommand.width = 40;
                    ObjCommand.command = LstCommand;
                    objKGrid.Command = ObjCommand;
                    if (CommandCount > 1)
                        ObjCommand.width = 100;
                }
                return objKGrid;
            }
            catch
            {
                throw;
            }

        }
        public DataTable GetMonthCloseHistory(MonthCloseModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", objModel.TID),
                };
                dt = DataLib.ExecuteDataTable("GetMonthCloseHistory", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}