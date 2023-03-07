using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Text;
using Ivap.ViewModel;
using System.ComponentModel.DataAnnotations;
using Ivap.Models;
using Ivap.Areas.Master.Models;
using IVap.Areas.Master.Repository;

namespace Ivap.Repository
{
    public class AccountRepo
    {

        #region Account
        public DataTable GetUserFromPassKey(UserVM ObjU)
        {
            try
            {
                DataTable dtU = new DataTable();
                SqlParameter[] parameters = new SqlParameter[] 
                {
                    new SqlParameter("p_RandomKey",ObjU.PassLinkKey)
                };

                dtU = DataLib.ExecuteDataTable("GetUserFromPassKey", CommandType.StoredProcedure, parameters);
                return dtU;
            }
            catch
            {
                throw;
            }
        }        
        public Response GeneratePassword(UserVM Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                DataTable dt = new DataTable();
                //Getting Details of user from Database based on Key
                dt = GetUserFromPassKey(Model);
                if (dt.Rows.Count != 1 )
                {
                    Res.Message = "Invalid Request.";
                    return Res;
                }


                string KeyType = Convert.ToString(dt.Rows[0]["KEYTYPE"]);
                //Hashing Password
                Model.Password = HashingLib.GenerateSHA512String(Model.Password);
                Model.UID = Convert.ToInt32(dt.Rows[0]["UID"]);
                //Handling case of new user
                SqlParameter[] P = null;
                if (KeyType.Trim().ToUpper() == "NEW USER")
                {
                    //ActivateUser
                    P = new SqlParameter[] {
                        new SqlParameter("P_Uid", Model.UID),
                         new SqlParameter("p_Password", Model.Password), 
                         new SqlParameter("p_RandomKey", Model.PassLinkKey)
                    };
                    string ActRes = DataLib.ExecuteScaler("ActivateUser", CommandType.StoredProcedure, P);
                    if (ActRes != "1")
                    {
                        Res.Message = "Invalid request or invalid activation link.";
                        return Res;
                    }
                    Res.Message = "Your password generated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                P = new SqlParameter[] 
                {
                    new SqlParameter("P_UID",Model.UID),
                    new SqlParameter("P_Password", Model.Password),
                };
                string ChangePwdRes = DataLib.ExecuteScaler("RegeneratePassword", CommandType.StoredProcedure, P);
                if (ChangePwdRes=="0")
                {
                    //"Your New Password cannot be last three previous passwords."
                    Res.Message = "New Password should not be same as of your last three passwords.";
                    return Res;
                }
                if (ChangePwdRes == "1")
                {
                    Res.Message = "Your password generated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                Res.Message = "Sorry!!! Something went wrong. Please try again later.";
                return Res;
            }
            catch
            {
                throw;
            }
        }
        //public string  GetLinkStatus(string passlink)
        //{
        //    SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("p_Randomkey",OracleDbType.Varchar2,passlink,ParameterDirection.Input),
        //        new SqlParameter("Result",OracleDbType.Int32 ,ParameterDirection.Output)
        //    };
        //    string res = DataLib.ExecuteScaler("GetLinkStatusbyPasslink", CommandType.StoredProcedure, parameters);

        //    return res;
        //}
        public void LogLogin(int UID,string SessionID)
        {

            try
            {
                string IPAddress = CommanUtills.GetIPAddress();
                string MackAddress="";
                SqlParameter[] P = new SqlParameter[]
                {
                    new SqlParameter("@Uid", UID),
                    new SqlParameter("@Ipaddress", IPAddress),
                    new SqlParameter("@Mackaddress",MackAddress ),
                    new SqlParameter("@SessionID",SessionID ),
                };
                DataLib.ExecuteScaler("LogUserLogin", CommandType.StoredProcedure, P);
            }
            catch { }
        }

        public Response CheckLoggedInUser(int UID,string SessionID)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;

                SqlParameter[] P = new SqlParameter[]
                {
                    new SqlParameter("P_UID", UID)
                };
                DataTable dt = DataLib.ExecuteDataTable("CHECKLOGGEDINUSER", CommandType.StoredProcedure, P);
                string LoggedSessionid = dt.Rows[0]["session_ID"].ToString();
                if(LoggedSessionid.Trim().ToUpper()==SessionID.Trim().ToUpper())
                {
                    Res.IsSuccess = true;
                    Res.Data = "Valid user";
                }
                else
                {
                    Res.IsSuccess = false;
                    Res.Data = "In-Valid user";
                }
                return Res;
            }

            catch
            {
                throw new Exception();
            }
        }


        public void SetLogOut(int UID,string SessionID)
        {

            try
            {
                SqlParameter[] P = new SqlParameter[] { new SqlParameter("@UID", UID) };
                DataLib.ExecuteNonQuery("SetLogOut", CommandType.StoredProcedure, P);
            }
            catch { }
        }
        public AppUser Authenticate(LoginVM Model, ref string Message)
        {

            AppUser objU = new AppUser();
            Message = "Invalid user name or password.";
            try
            {
                //Hash User Password
                DataSet dsU = new DataSet();

                SqlParameter[] parameters = new SqlParameter[]
                {                   
                    new SqlParameter("@Userid",Model.UserName.Trim()),
                    new SqlParameter("@DomainName",Model.EntityCode)
                };

                dsU = DataLib.ExecuteDataSet("AuthenticateUser_EID", CommandType.StoredProcedure, parameters);
                if (dsU.Tables[0].Rows.Count != 1)
                    return objU;

                Model.Password = HashingLib.GenerateSHA512String(Model.Password.Trim());
                string CurrectPassword = Convert.ToString(dsU.Tables[0].Rows[0]["Password"]).Trim();
                int InvalidPassTry = Convert.ToInt32(dsU.Tables[0].Rows[0]["PasswordTry"]);
                if (CurrectPassword != Model.Password)
                {
                    //PassTry = InvalidPassTry;
                    Message = "Invalid Password";
                    
                    if (InvalidPassTry >= 5)
                    {
                        Message = "Your account has been locked";                     
                    }
                    return objU;
                }              
                if (InvalidPassTry>=5)
                {
                    //Message = "Your account has been locked.Please regenerate password again using forget password.";
                    Message = "Your account has been locked";
                    return objU;
                }

                int PassChangeBefore = Convert.ToInt32(dsU.Tables[0].Rows[0]["PassChangeDayCount"]);

                if (PassChangeBefore >= 60)
                {
                    Message = "PasswordExpired";
                    return objU;
                }
                string IsAuth = "0";
                IsAuth = Convert.ToString(dsU.Tables[0].Rows[0]["IsAuth"]);
                switch (IsAuth)
                {
                    //1 means valid user. Load all the details into user objects
                    case "1":
                        {
                            objU = objU.SetAppUser(dsU);
                            
                            Message = "success";
                            return objU;
                        }
                    //0 Means User has been created but not activated
                    case "0":
                        {
                            Message = "Your account has not been activated yet.Please activate it.";
                            return objU;
                        }
                    default:
                        {
                            Message = "Invalid user name or password.";
                            return objU;
                        }
                }
            }
            catch(Exception ex)
            {
                Message = "Invalid user name or password.";
                return objU;
            }
        }
        public Response  UpdateProfile(UpdateProfileVM Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                Res.Message = "Sorry!!! Unable to update pfofile.";

                SqlParameter[] P = new SqlParameter[]
                {
                    new SqlParameter("P_UID",Model.UID),
                    new SqlParameter("P_USER_FIRSTNAME", Model.FirstName),
                    new SqlParameter("P_USER_LASTNAME",Model.LastName ),
                    new SqlParameter("p_USER_EMAIL",Model.Email ),
                    new SqlParameter("p_USER_MOBILENO",Model.MobileNo),
                    new SqlParameter("p_ProfilePic",Model.ProfilePic)
                };
                string res=DataLib.ExecuteScaler("UpdateProfile", CommandType.StoredProcedure, P);
                if(res=="1")
                {
                    Res.IsSuccess = true;
                    Res.Message = "Profile Updated successfully.";
                }
                else
                {
                    Res.IsSuccess = false;
                    Res.Message = "Unable to update profile.";
                }
                return Res;
            }

            catch
            {
                throw new Exception();
            }
        }
        public Response AddUpdateUserProfile(UserProfileVM model)
        {
            Response res = new Response();
            try
            {
                int result = 0;
                SqlParameter[] parameters = new SqlParameter[]{
                 new SqlParameter("@p_UID",model.UID),
                    new SqlParameter("@p_USER_FirstName",model.FirstName),
                    new SqlParameter("@p_USER_LastName",model.LastName),
                    new SqlParameter("@p_USER_Email",model.Email),
                    new SqlParameter("@p_USER_MobileNo",model.MobileNo),
                     new SqlParameter("@p_USER_PROFILEPIC",model.ProfilePic),
                    new SqlParameter("@p_CreatedBy",model.CreatedBy )
                };

                result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateUserProfile", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.UID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Profile", result, "UID");
                return res;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public Response  ChangePassword(ChangePasswordVM Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                Res.Message = "Sorry. Unable to process your request. Please try again later";
                var dt = new DataTable();

                //Now Get User Details from The databased based on supplyed userid

                UserRepo URepo = new UserRepo();
                UserModel UModel = new UserModel();
                UModel.USERID = Model.UserName;
                dt = URepo.GetUser(UModel);

                if(dt.Rows.Count==0)
                {
                    Res.Message = "Invalid User.";
                    return Res;
                }
                
                //Now Check Current Password
                string CurrentPassword = dt.Rows[0]["PASSWORD"].ToString();
                Model.Password = HashingLib.GenerateSHA512String(Model.Password);
                Model.NewPassword = HashingLib.GenerateSHA512String(Model.NewPassword);

                //Hashing Current Password
                if (CurrentPassword != Model.Password)
                {
                    Res.Message = "Invalid Current Password.";
                    return Res;
                }

                //Now Change User Password accordingly

                int UID = Convert.ToInt32(dt.Rows[0]["UID"].ToString());

                SqlParameter[] P = new SqlParameter[]
                {
                    new SqlParameter("P_UID", UID),
                    new SqlParameter("P_PASSWORD", Model.NewPassword),
                };
                string ProcRes = DataLib.ExecuteScaler("ChangePassword", CommandType.StoredProcedure, P);
                //Now Checking Procedure Response
                if(ProcRes=="0")
                {
                    Res.Message = "You can not use your last three password.";
                    return Res;
                }
                if (ProcRes == "1")
                {
                    Res.Message = "Congratulations. Your password has changed successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }

                return Res;

            }
            catch
            {
                throw;
            }
        }

        public Response ChangePassword(AppUser UDetails,ChangePasswordVM Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                Res.Message = "Sorry!!! Unable to process your request. Please try again later";
                var dt = new DataTable();

                //Now Get User Details from The databased based on supplyed userid

                UserRepo URepo = new UserRepo();
                UserModel UModel = new UserModel();
                UModel.UID = UDetails.UID;
                dt = URepo.GetUser(UModel);

                if (dt.Rows.Count == 0)
                {
                    Res.Message = "Sorry!!! Your are not a valid user.";
                    return Res;
                }

                //Now Check Current Password
                string CurrentPassword = dt.Rows[0]["PASSWORD"].ToString();
                Model.Password = HashingLib.GenerateSHA512String(Model.Password);
                Model.NewPassword = HashingLib.GenerateSHA512String(Model.NewPassword);

                //Hashing Current Password
                if (CurrentPassword != Model.Password)
                {
                    Res.Message = "Sorry!!! Your Current password is wrong.";
                    return Res;
                }

                //Now Change User Password accordingly

                int UID = Convert.ToInt32(dt.Rows[0]["UID"].ToString());

                SqlParameter[] P = new SqlParameter[]
                {
                    new SqlParameter("P_UID", UID),
                    new SqlParameter("P_PASSWORD", Model.NewPassword),
                };
                string ProcRes = DataLib.ExecuteScaler("ChangePassword", CommandType.StoredProcedure, P);
                //Now Checking Procedure Response
                if (ProcRes == "0")
                {
                    Res.Message = "Sorry!!! You can not use your last three password.";
                    return Res;
                }
                if (ProcRes == "1")
                {
                    Res.Message = "Congratulations!!! Your password has changed successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }

                return Res;

            }
            catch
            {
                throw;
            }
        }

        public string ForgetPassword(string UID)
        {
            string ret = "";
            try
            {

                DataTable dt = new DataTable();
                UserModel uBo = new UserModel();
                UserRepo urepo = new UserRepo();
                int result;
                if (int.TryParse(UID, out result))
                {
                    uBo.UID = Convert.ToInt32(UID);
                }
                else { uBo.USERID = UID; }
               
                dt = urepo.GetUser(uBo);
                if (dt.Rows.Count == 1)
                {
                    if (dt.Rows[0]["IsAUTH"].ToString() != "1")
                    {
                        ret = "If your email/username address exists in our database, you will receive a password recovery link at your email address in a few minutes.";
                        return ret;
                    }
                    uBo.UID = Convert.ToInt32(dt.Rows[0]["UID"].ToString());
                    uBo.FirstName = dt.Rows[0]["USER_FIRSTNAME"].ToString();
                    uBo.LastName = dt.Rows[0]["USER_LASTNAME"].ToString();
                    uBo.Email = dt.Rows[0]["USER_EMAIL"].ToString();
                    uBo.PassToken = RandomUtils.CreateRandomString();
                    //   uBo.Passkey = AesEncUtil.EncryptText(uBo.User_Name, uBo.skey + uBo.User_Name);
                    // uBo.Passkey = uBo.Passkey.Replace("/", "").Replace("\\", "");
                    SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("P_UID", uBo.UID),
                            new SqlParameter("P_RANDOMKEY",uBo.PassToken),
                        };
                    int res = Convert.ToInt32(DataLib.ExecuteScaler("ResetPassLink", CommandType.StoredProcedure, parameters));
                    if (res > 0)
                    {
                        string host ="https://"+ HttpContext.Current.Request.Url.Host;
                        StringBuilder mailbody = new StringBuilder("");
                        mailbody.Append(" <body><header style=\"width:996px; margin:auto;\"><div style=\"font-family:Arial, Helvetica, sans-serif; color:#444; font-size:18px;font-weight:bold;\"> ");
                        mailbody.Append(" Dear " + uBo.FirstName + " " + uBo.LastName + ",       </div></header><section style=\"width:996px; margin:auto;\"><div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;\"><p>Your request to change password is received ");
                        mailbody.Append("<span style=\"color:#ff6600;\">on IVAP</span> portal. Please <a style=\"color:#ff6600;text-decoration:none;\" href=\"" + host + "/GeneratePassword/" + uBo.PassToken + "\"> click here </a> to change your password. </p></div> ");
                        mailbody.Append(" </section> <footer style=\"width:996px; margin:auto;\"> <div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;padding:5px 0px;\">Thanks!<br> Team IVAP</div> ");
                        mailbody.Append(" </footer></body>");
                        MailUtils.SendMail(uBo.Email, "Password reset from IVAP", mailbody.ToString(), "", "", "");
                    }

                    return "If your email/username address exists in our database, you will receive a password recovery link at your email address in a few minutes.";
                    //In Case of new User Send him Activation Email To his Email
                    // return ret;
                }
                else
                {
                    ret = "If your email/username address exists in our database, you will receive a password recovery link at your email address in a few minutes.";
                    return ret;
                }

            }
            catch { throw; }
        }
       
        #endregion
    }
}
