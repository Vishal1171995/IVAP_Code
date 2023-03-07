using Ivap.Areas.FileExplorer.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivap.Areas.FileExplorer.Repository
{
    public class FileExplorerRepo
    {
        public List<FileExplorerModel> GetEntityTreeView(int EID, int FileID, int ParentId, int UID,string RoleName)
        {
            DataSet DsEntity = new DataSet();
            List<FileExplorerModel> finalTree = new List<FileExplorerModel>();
            Response ret = new Response();
            try
            {
                //CustModel.UID = ((User)HttpContext.Current.Session["uBo"]).UID;
                DataTable DtRights = new DataTable();
                DsEntity = GetFileExplorer(EID, FileID, ParentId, UID);
                DsEntity.Tables[0].Columns.Add("IsView");
                DsEntity.Tables[0].Columns.Add("IsCreate");
                if (RoleName != "CLIENT ADMIN")
                {
                    DataView dv = new DataView(DsEntity.Tables[0]);
                    dv.RowFilter = "UserRights like '%{" + UID + ":%'";
                    DtRights = dv.ToTable();
                    if (DtRights.Rows.Count == 0)
                    {
                        DataTable Dt = new DataTable();
                        DataView dv1 = new DataView(DsEntity.Tables[0]);

                        dv1.RowFilter = "UserRights = ''";
                        Dt = dv1.ToTable();
                        if (Dt.Rows.Count > 0)
                            DtRights = Dt;
                        //DtRights = DsEntity.Tables[0];
                    }
                }
                else
                {
                    DtRights = DsEntity.Tables[0];
                }
                for (int i = 0; i < DtRights.Rows.Count; i++)
                {
                    //Customer List
                    FileExplorerModel objEntity = new FileExplorerModel();
                    if (FileID == -1 && ParentId == 0)
                    {
                        objEntity.text = DtRights.Rows[i]["ENTITYNAME"].ToString();
                        objEntity.id = 0;
                        objEntity.hasChildren = Convert.ToBoolean(Convert.ToInt32(DtRights.Rows[i]["hasChildren"]));
                        objEntity.ParentId = 0;
                        objEntity.EID = Convert.ToInt32(DtRights.Rows[i]["EID"]);
                        objEntity.IsAll = true;
                        objEntity.expanded = false;
                        finalTree.Add(objEntity);
                    }
                    else
                    {
                        objEntity.text = DtRights.Rows[i]["FileOriginalName"].ToString();
                        objEntity.id = Convert.ToInt32(DtRights.Rows[i]["FileID"]);
                        objEntity.hasChildren = Convert.ToBoolean(Convert.ToInt32(DtRights.Rows[i]["hasChildren"]));
                        objEntity.ParentId = Convert.ToInt32(DtRights.Rows[i]["PID"]);
                        objEntity.EID = Convert.ToInt32(DtRights.Rows[i]["EID"]);
                        objEntity.IsAll = false;
                        objEntity.IsView = false;
                        string Roles = DtRights.Rows[i]["UserRights"].ToString();
                        string[] arrRoles = Roles.Split(',');
                        for (int j = 0; j < arrRoles.Length; j++)
                        {
                            if (arrRoles[j] != "")
                            {
                                String Str = arrRoles[j].Replace("{", string.Empty).Replace("}", string.Empty);
                                string[] arrRight = Str.Split(':');
                                string Right = arrRight[1].Trim();
                                int UserID = Convert.ToInt32(arrRight[0].Trim());
                                if (UserID == UID)
                                {
                                    if (Right == "2")
                                        objEntity.IsAll = true;
                                    else
                                        objEntity.IsView = true;
                                }
                            }
                            else
                            {
                                objEntity.IsAll = true;
                                objEntity.IsView = true;
                            }

                        }
                        objEntity.expanded = false;
                        finalTree.Add(objEntity);
                    }
                }

                return finalTree;
            }
            catch(Exception Ex)
            {
                throw;
            }
        }
        public DataSet GetFileExplorer(int EID, int FileID, int ParentId, int UID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@ParrentID", ParentId),
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@UID", UID)
                };
                return DataLib.ExecuteDataSet("GetFileExplorer", CommandType.StoredProcedure, p); //GetFileExplorer
            }
            catch { throw; }
        }
        public string GetParrentFolder(int? FileID, string FileType)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@FileType", FileType)
                };
                return DataLib.ExecuteScaler("GetParrentFolder", CommandType.StoredProcedure, p);
            }
            catch { throw; }
        }
        public string GetParrentFolderForRename(int? FileID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                };
                return DataLib.ExecuteScaler("GetParrentFolderForRename", CommandType.StoredProcedure, p);
            }
            catch { throw; }
        }
        public string CreateNewFolder(int? EID, int? FileID, int? ParentID, string FolderName, string SystemGenPath, int UID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@ParrentID", ParentID),
                    new SqlParameter("@OriginalName", FolderName),
                    new SqlParameter("@SystemGenPath", SystemGenPath),
                    new SqlParameter("@UID", UID),
                };
                return DataLib.ExecuteScaler("CreateNewFolder", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }

        public string DeleteFolder(int? FileID, int UID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@UID", UID),
                };
                return DataLib.ExecuteScaler("DeleteFolder", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetAllChildFile(int EID, int FileID,int UID,string RoleName)
        {
            DataSet Ds = new DataSet();
            DataTable DtRights = new DataTable();
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FileID", FileID),
                };
                Ds = DataLib.ExecuteDataSet("GetAllChildFile", CommandType.StoredProcedure, p);
                if (RoleName != "CLIENT ADMIN")
                {
                    DataView dv = new DataView(Ds.Tables[0]);
                    dv.RowFilter = "UserRights like '%{" + UID + ":%'";
                    DtRights = dv.ToTable();
                    if (DtRights.Rows.Count == 0)
                    {
                        DataTable Dt = new DataTable();
                        DataView dv1 = new DataView(Ds.Tables[0]);

                        dv1.RowFilter = "UserRights = ''";
                        Dt = dv1.ToTable();
                        if (Dt.Rows.Count > 0)
                            DtRights = Dt;
                    }
                }
                else
                    DtRights = Ds.Tables[0];

                return DtRights;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public DataSet GetFileMetaData(int EID, int FileID, int FileMetaID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FileMetaID", FileMetaID),
                };
                return DataLib.ExecuteDataSet("GetFileMetaData", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }

        public string AddFile(int EID, int FileID, string OriginalFileName, string TempFileName, string MetaValue, string FileTypeName, string FileExtention, double FileSize, int CreatedBy)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FileOriginalName", OriginalFileName),
                    new SqlParameter("@FileSystemGeneratedName", TempFileName),
                    new SqlParameter("@FileTypeName", FileTypeName),
                    new SqlParameter("@MetaValue", MetaValue),
                    new SqlParameter("@FileExtention", FileExtention),
                    new SqlParameter("@FileSize", FileSize),
                    new SqlParameter("@CreatedBy", CreatedBy)
                };
                return DataLib.ExecuteScaler("AddFile", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetFileInfo(int FileID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                };
                return DataLib.ExecuteDataSet("GetFileInfo", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetMetaDetaFileForSearch(int EID, int FileID, string text)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@text", text),
                };
                return DataLib.ExecuteDataSet("GetMetaDetaFileForSearch", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }
        public string RenmaeFile(int FileID, string OldName, string NewName, string NewPath, string Filetype)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@OldName", OldName),
                    new SqlParameter("@NewName", NewName),
                    new SqlParameter("@FileSystemGeneratedName", NewPath),
                    new SqlParameter("@Filetype", Filetype),
                };
                return DataLib.ExecuteScaler("RenmaeFile", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetDocFileExplorer(int CompID, int SiteID, int ActID, int ActivityID, int Year, string Month, int UID)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompID",CompID ),
                    new SqlParameter("@SiteID",SiteID ),
                    new SqlParameter("@ActID",ActID ),
                    new SqlParameter("@ActivityID",ActivityID ),
                    new SqlParameter("@Year",Year ),
                    new SqlParameter("@Month",Month ),
                    new SqlParameter("@UID",UID ),
                };

                return DataLib.ExecuteDataTable("GetDocFileExplorer", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex) { throw; }
        }

        public string SendMailEXl(string RootParentFolder, int FileID)
        {
            StringBuilder mailbody = new StringBuilder("");
            string mailTo = "";
            string CC = "";
            string BCC = "";
            mailbody.Append("<body><header style=\"width:996px; margin:auto;\"><div style=\"font-family:Arial, Helvetica, sans-serif; color:#444; font-size:18px;font-weight:bold;\"> ");
            mailbody.Append(" Dear User,       </div></header><section style=\"width:996px; margin:auto;\"><div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;\"><p>The following activity has been initiated on MyndAct portal file explore. The summary of the same is as below:  ");
            mailbody.Append("<br>User have uploaded a document. <br>Request you to take the necessary action, if any, so that the compliance task can be completed within stipulated time period<br>");
            mailbody.Append(" Click here to login to MyndAct http://myndact.com/?AspxAutoDetectCookieSupport=1  </p></div> ");
            mailbody.Append(" </section> <footer style=\"width:996px; margin:auto;\"> <div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;padding:5px 0px;\">Thanks & Regards <br> MyndAct Team</div> ");
            mailbody.Append(" </footer></body>");

            DataTable dt = new DataTable();
            DataTable dtEmails = GetFileEmails(FileID);
            if (dtEmails.Rows.Count > 0)
            {
                mailTo = dtEmails.Rows[0]["To"].ToString();
                CC = dtEmails.Rows[0]["CC"].ToString();
                BCC = dtEmails.Rows[0]["BCC"].ToString();
            }
            if (mailTo != "")
            {
                //    mailTo = "pallavi.bhardwaj@myndsol.com";
                //CommanUtills.SendMail(mailTo, "Document Upload Alert", mailbody.ToString(), CC, "", BCC);
            }
            return "";
        }

        public DataTable GetFileEmails(int FileID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                };
                return DataLib.ExecuteDataTable("GetFileEmails", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }

        public DataTable RootParent(int FileID)
        {

            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                };
                return DataLib.ExecuteDataTable("GetRootParent", CommandType.StoredProcedure, p);

            }
            catch
            {
                throw;
            }

        }
        public DataSet GetAcessRoleForFileExplorer(int EID,int FileID)
        {
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FileID", FileID),
                };
                return DataLib.ExecuteDataSet("GetAcessRoleForFileExplorer", CommandType.StoredProcedure, p);
            }
            catch
            {
                throw;
            }
        }
        public Response SetUserRights(int FileID,int EID,string UserRights)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@UserRights", UserRights),
                };
                string result = DataLib.ExecuteScaler("SetUserRights", CommandType.StoredProcedure, p);
                res.IsSuccess = true;
                res.Message = "Role Set sucessfully";
                return res;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public DataTable GetFileExplorerByFileID(int FileID)
        {

            try
            {
                SqlParameter[] p =
                {
                    new SqlParameter("@FileID", FileID),
                };
                return DataLib.ExecuteDataTable("GetFileExplorerByFileID", CommandType.StoredProcedure, p);

            }
            catch
            {
                throw;
            }

        }
        public bool IsAuthorized(int FileID, int UID)
        {
            DataTable Dt = new DataTable();
            bool IsValid = true;
            try
            {
                Dt = GetFileExplorerByFileID(Convert.ToInt32(FileID));
                if (Dt.Rows.Count > 0)
                {
                    string Roles = Dt.Rows[0]["UserRights"].ToString();
                    if (Roles != "")
                    {
                        string[] arrRoles = Roles.Split(',');
                        for (int j = 0; j < arrRoles.Length; j++)
                        {
                            String Str = arrRoles[j].Replace("{", string.Empty).Replace("}", string.Empty);
                            string[] arrRight = Str.Split(':');
                            string Right = arrRight[1].Trim();
                            int UserID = Convert.ToInt32(arrRight[0].Trim());
                            if (UserID == UID)
                            {
                                if (Right == "2")
                                {
                                    IsValid = true;
                                    break;
                                }
                                else
                                {
                                    IsValid = false;
                                }
                            }
                        }
                    }
                }
                return IsValid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}