using Ivap.Areas.Master.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Ivap.Utils;
using Ivap.Areas.Master.ViewModel;
using Ivap.Areas.Master.Repository;

namespace IVap.Areas.Master.Repository
{
    public class UserRepo
    {
        public object SqlDataLib { get; private set; }

        public Response AddUpdateUser(UserModel model)
        {
            SqlConnection con=null;
            SqlTransaction trans= null;
            try
            {

                con = new SqlConnection(DataLib.GetConnectionString());
                if (con.State == ConnectionState.Closed)
                { con.Open(); }
                
                 Response res = new Response();
                //Generation Random Number for Encryption
                string GenerateLink = RandomUtils.CreateRandomString();
                trans = con.BeginTransaction();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    
                    new SqlParameter("@UID",model.UID),
                    new SqlParameter("@ENITYTID",model.EID),
                    new SqlParameter("@USERID",model.USERID),
                    new SqlParameter("@USER_FIRSTNAME",model.FirstName),
                    new SqlParameter("@USER_LASTNAME",model.LastName),
                    new SqlParameter("@USER_EMAIL",model.Email),
                    new SqlParameter("@USER_ROLE",model.Role),
                    new SqlParameter("@USER_MOBILENO", model.MobileNo),
                    new SqlParameter("@PassToken",GenerateLink),
                    new SqlParameter("@CreatedBy",model.CreatedBy ),
                    new SqlParameter("@ISACTIVE",model.IsActive ),
                   };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateUser_1", CommandType.StoredProcedure, parameters,con,trans));
                if(result==-100)
                {
                    Response RoleDuplicateError = new Response();
                    RoleDuplicateError.Message = "User Creation Failed: Only one user is allowed on this role in a circle";
                    RoleDuplicateError.IsSuccess = false;
                    return RoleDuplicateError;
                }
                if (result > 0)
                {
                    if (model.UID == 0)
                    {
                        //string host = "http://" + HttpContext.Current.Request.Url.Host;
                        string host= "http://" + HttpContext.Current.Request.Url.Authority;
                        string urlkey = HttpContext.Current.Server.UrlEncode(GenerateLink);

                        StringBuilder mailbody = new StringBuilder("");
                        mailbody.Append(" <body><header style=\"width:996px; margin:auto;\"><div style=\"font-family:Arial, Helvetica, sans-serif; color:#444; font-size:18px;font-weight:bold;\"> ");
                        mailbody.Append(" Dear " + model.FirstName + ",       </div></header><section style=\"width:996px; margin:auto;\"><div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;\"><p>Thank You for registering with ");
                        mailbody.Append("<span style=\"color:#ff6600;\"> ITCMS</span> portal. Please <a style=\"color:#ff6600;text-decoration:none;\" href=\"" + host + "/GeneratePassword/" + urlkey + "\"> click here </a> for account activation . </p></div> ");
                        mailbody.Append(" </section> <footer style=\"width:996px; margin:auto;\"> <div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;padding:5px 0px;\">Thanks,<br> ITCMS Team</div> ");
                        mailbody.Append(" </footer></body>");
                        string EmailStatus =MailUtils.SendMail(model.Email, "Greetings from ITCMS", mailbody.ToString(), "", "", "");
                        EmailStatus = "Mail Sent";
                        if (EmailStatus == "Mail Sent")
                        {
                            trans.Commit();
                            res = res.GetResponse(Response.operation.ADD, "User", result, "UserID");
                        }
                        else { trans.Rollback(); }
                    }
                    else
                    {
                        trans.Commit();
                        res = res.GetResponse(Response.operation.Update, "User", result, "UserID");
                    }
                }
                else if (result == -1)
                {
                   
                    string operationname = "User";
                    string duplicatefields = "UserID";
                    res.IsSuccess = false;
                    if (operationname.Contains(","))
                        res.Message = "Creation failed. Combination of " + duplicatefields + " must be unique.";
                    else
                        res.Message = "Creation failed. " + duplicatefields + " must be unique.";
                }
                else if (result == -2) {
                    res.IsSuccess = false;
                    res.Message = "One vendor User may work only one circle kindly select another circle to procced the vendor user";
                }else if (result == 0)
                {
                        trans.Commit();
                        res = res.GetResponse(Response.operation.Update, "User", result, "UserID");
                }
                return res;


            }
            catch(Exception Ex)
            {
                trans.Rollback();
                throw;
            }
        }
        public DataTable GetUser(UserModel model)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UID",model.UID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@USERID",model.USERID),
                    new SqlParameter("@USER_ROLE",model.Role),
                };

                return DataLib.ExecuteDataTable("GetUser", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetRoles()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DataLib.ExecuteDataTable("GetRolesForUser", CommandType.StoredProcedure,null);
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetActivateUser(string UID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("p_UID",UID),
                };
                return DataLib.ExecuteDataTable("[GetUserForActivation]", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }       
        public DataTable GetUserHistory(UserModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UID",model.UID ),
                };
                return DataLib.ExecuteDataTable("GetUserHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public Response ChangeStatus(string UID, string Status)
        {
            Response ret = new Response();

            SqlParameter[] paramsoldcheck = new SqlParameter[]
            {  new SqlParameter("p_UID", UID) ,
                new SqlParameter("p_Status", Status=="1"? 0:1),

            };
            string reschk = DataLib.ExecuteScaler("ChangeStatus", CommandType.StoredProcedure, paramsoldcheck);
            if (reschk == "1")
            { ret.IsSuccess = true; ret.Message = "Success"; }
            else { ret.IsSuccess = false; ret.Message = "Failed"; }


            return ret;
        }      
       
        public string DownLoadUser(UserModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DataLib.ExecuteDataTable("GetUser", CommandType.StoredProcedure, null);
                return CSVUtills.DataTableToCSV(dt, ",");
            }
            catch { throw; }
        }

        public string UploadUserDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
        {
            try
            {
                RoleRepo RoleR = new RoleRepo();
                RoleModel RoleM = new RoleModel();
                EntityRepo EntityR = new EntityRepo();
                EntityModel EntityM = new EntityModel();
                //string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + filename;  
                //Convert csv file into DataTable
                Response ret = new Response();
                DataTable dt = null;
                try
                {
                    dt = ExcellUtils.ExlToDataTable(FilePath, "xlsx");
                }
                catch (Exception Ex)
                {
                    return Ex.Message;
                }

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_USER", "ViewUser"))
                {
                    return "Invalid File Format";
                }
                DataColumnCollection columns = dt.Columns;
                if (!columns.Contains("Response"))
                {
                    dt.Columns.Add("Response");
                }

                if (!columns.Contains("Message"))
                {
                    dt.Columns.Add("Message");
                }
                UserModel UserModel = new UserModel();
                UserModelVM UserVM= new UserModelVM();
                UserModel.EID = EID;
                UserModel.SetDisplayName();
                UserVM.EID = EID;
                UserVM.SetDisplayName();
                string strerr = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        //UserVM = new UserModelVM();
                        string UID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        UserVM.UID = Convert.ToInt32(UID);
                        UserVM.UserID = Convert.ToString(dt.Rows[i][UserModel.USERID_Text]).Trim();
                        UserVM.FirstName = Convert.ToString(dt.Rows[i][UserModel.FirstName_Text]).Trim();
                        UserVM.LastName = Convert.ToString(dt.Rows[i][UserModel.LastName_Text]).Trim();
                        UserVM.Email = Convert.ToString(dt.Rows[i][UserModel.Email_Text]).Trim();
                        UserVM.Role = Convert.ToString(dt.Rows[i][UserModel.Role_Text]).ToUpper();
                        UserVM.MobileNo = Convert.ToString(dt.Rows[i][UserModel.MobileNo_Text]).Trim();
                        UserVM.EID = EID;
                        //VtypeVM.IsActive = Convert.ToBoolean(dt.Rows[i]["Status"].ToString().Replace("\"", "") == "1" ? true : false);
                        UserVM.CreatedBy = CreatedBy;

                        var results = new List<ValidationResult>();
                        var vc = new ValidationContext(UserVM, null, null);
                        var isValid = Validator.TryValidateObject(UserVM, vc, results, true);
                        var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                        //strerr = string.Join(" ", errors);
                        strerr = MasterMetaRepo.GenerateError(UserVM, results);
                        if (isValid)
                        {
                            RoleM.RoleName = UserVM.Role;
                            //UserModel = new UserModel();
                            var Roleid = RoleR.GetRoles(RoleM);
                            DataView dvRoles = new DataView(Roleid);
                            dvRoles.RowFilter = "ROLENAME='" + UserVM.Role + "'";
                            DataTable dtRoles = dvRoles.ToTable();
                            if (dtRoles.Rows.Count > 0)
                            {
                                UserModel.Role = Convert.ToInt32(dtRoles.Rows[0]["TID"].ToString());
                            }
                            else
                            {
                                UserModel.Role = -1;
                            }
                            UserModel.UID = UserVM.UID;
                            UserModel.USERID = UserVM.UserID;
                            UserModel.FirstName = UserVM.FirstName;
                            UserModel.LastName = UserVM.LastName;
                            UserModel.Email = UserVM.Email;
                            UserModel.Role = UserModel.Role;
                            UserModel.MobileNo = UserVM.MobileNo;
                            UserModel.EID = UserVM.EID;
                            UserModel.UserType = null;
                            UserModel.CreatedBy = UserVM.CreatedBy;

                            results = new List<ValidationResult>();
                            vc = new ValidationContext(UserModel, null, null);
                            isValid = Validator.TryValidateObject(UserModel, vc, results, true);
                            errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            //strerr = string.Join(" ", errors);
                            strerr = MasterMetaRepo.GenerateError(UserModel, results);
                            if (isValid)
                            {
                                ret = AddUpdateUser(UserModel);

                                if (ret.IsSuccess == true)
                                {
                                    SuccessCount += 1;
                                    dt.Rows[i]["Message"] = ret.Message;
                                    dt.Rows[i]["Response"] = "Success";
                                }
                                else
                                {
                                    FailCount += 1;
                                    if (ret.Data == "-1")
                                    {
                                        dt.Rows[i]["Message"] = "Duplicate";
                                    }
                                    else
                                    {
                                        dt.Rows[i]["Message"] = ret.Message;
                                    }
                                    dt.Rows[i]["Response"] = "Failed";
                                }
                            }
                            else
                            {
                                FailCount += 1;
                                dt.Rows[i]["Response"] = strerr;
                            }
                        }
                        else
                        {
                            FailCount += 1;
                            dt.Rows[i]["Response"] = "Failed";
                            dt.Rows[i]["Message"] = strerr;
                            //Message
                        }
                    }
                    catch(Exception Ex)
                    {
                        FailCount += 1;
                        dt.Rows[i]["Response"] = "Failed";
                        dt.Rows[i]["Message"] = "Invalid Data Format";
                        continue;
                    }
                }
                //Converting dt into csv FIle
                FilePath = ExcellUtils.DataTableToExcel(dt);
                return FilePath;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public Response UserSignUp(CreateUserVM model, SqlConnection conn, SqlTransaction Trans)
        {

            try
            {
                Response res = new Response();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID",model.UserID),
                    new SqlParameter("@EID",model.EID),
                    new SqlParameter("@USER_FirstName",model.FirstName),
                    new SqlParameter("@USER_LastName",model.LastName),
                    new SqlParameter("@USER_Email",model.Email),
                    new SqlParameter("@USER_ROLE",model.Role),
                    new SqlParameter("@Password", model.Password),
                    new SqlParameter("@USER_MobileNo",model.MobileNo),
                    new SqlParameter("@CreatedBy",model.Created_By)
                   };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("UserSignUp", CommandType.StoredProcedure, parameters, conn, Trans));
                res.IsSuccess = true;
                res.Data = result.ToString();
                return res;
            }
            catch
            {
                throw;
            }
        }


    }
}