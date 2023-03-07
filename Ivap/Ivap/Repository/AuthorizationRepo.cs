using Ivap.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Repository
{
    public class AuthorizationRepo
    {
        public DataTable GetUserMenu(string Roles,int EID)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlParameter[] P = new SqlParameter[] {
                    new SqlParameter("@Roles", Roles),
                    new SqlParameter("@Entity_ID", EID)
                };
                dt = DataLib.ExecuteDataTable("GetUserMenu_EID", CommandType.StoredProcedure, P);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public static bool IsValidAction(string RouteName, string ActionType)
        {
            try
            {
                bool IsValid = false;
                AppUser uBo = new AppUser();
                uBo = (AppUser)HttpContext.Current.Session["uBo"];
                DataTable dt = new DataTable();
                dt = (DataTable)HttpContext.Current.Session["uMenu"];
                DataView dv = new DataView(dt);

                dv.RowFilter = "Route='" + RouteName + "' AND ROLES like '%{" + uBo.RoleName + ":%'";
                DataTable dtMenu = dv.ToTable();
                //no any menu exists for this user hence it is not authrized
                if (dtMenu.Rows.Count == 0)
                    return false;
                if (ActionType == "PageView")
                    return true;
                string Roles = dtMenu.Rows[0]["ROLES"].ToString();
                string[] arrRoles = Roles.Split(',');
                for (int i = 0; i < arrRoles.Length; i++)
                {
                    String Str = arrRoles[i].Replace("{", string.Empty).Replace("}", string.Empty);
                    string[] arrRight = Str.Split(':');
                    string Right = arrRight[1].Trim();
                    string Role = arrRight[0].Trim().ToUpper();

                    if (uBo.RoleName.Trim().ToUpper() == Role)
                    {
                        if (ActionType.Trim() == "CreateAction" || ActionType.Trim() == "UpdateAction")
                        {
                            if (Right == "2" || Right == "3")
                            {
                                IsValid = true;
                                break;
                            }
                        }
                        else if (ActionType.Trim() == "ViewAction")
                        {
                            if (Right == "1" || Right == "3")
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }
                }
                return IsValid;

            }
            catch { throw; }

        }

        public static bool IsAuthorized(string ControllerName, string ActionType)
        {
            try
            {
                bool IsValid = false;
                AppUser uBo = new AppUser();
                uBo = (AppUser)HttpContext.Current.Session["uBo"];
                DataTable dt = new DataTable();
                dt = (DataTable)HttpContext.Current.Session["uMenu"];
                DataView dv = new DataView(dt);

                dv.RowFilter = "Controller='" + ControllerName + "' AND ROLES like '%{" + uBo.RoleName + ":%'";
                DataTable dtMenu = dv.ToTable();
                //no any menu exists for this user hence it is not authrized
                if (dtMenu.Rows.Count == 0)
                    return false;
                
                string Roles = dtMenu.Rows[0]["ROLES"].ToString();
                string[] arrRoles = Roles.Split(',');
                for (int i = 0; i < arrRoles.Length; i++)
                {
                    String Str = arrRoles[i].Replace("{", string.Empty).Replace("}", string.Empty);
                    string[] arrRight = Str.Split(':');
                    string Right = arrRight[1].Trim();
                    string Role = arrRight[0].Trim().ToUpper();

                    if (uBo.RoleName.Trim().ToUpper() == Role)
                    {
                        if (ActionType.Trim() == "CreateAction" || ActionType.Trim() == "UpdateAction")
                        {
                            if (Right == "2" || Right == "3")
                            {
                                IsValid = true;
                                break;
                            }
                        }
                        else if (ActionType.Trim() == "ViewAction")
                        {
                            if (Right == "1" || Right == "3")
                            {
                                IsValid = true;
                                break;
                            }
                        }
                    }
                }
                return IsValid;

            }
            catch { throw; }

        }

    }
}