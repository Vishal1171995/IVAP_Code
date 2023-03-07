using Ivap.Areas.CAPA.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Ivap.Areas.CAPA.Repository
{
    public class CapaRepo
    {
        public Response AddUpdateCapaForEach(CapaModel Model)
        {
            Response res = new Response();
            Response res1 = new Response();
            SqlTransaction trans = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(DataLib.GetConnectionString());
                conn.Open();
                trans = conn.BeginTransaction();

                int CapaID = AddUpdateCapa(Model);
                if (CapaID < 0)
                {
                    res.Message = "Failed! Please try again.";
                    res.IsSuccess = false;
                    return res;

                }
                if (Model.CorrectiveAction_Detail != null)
                {

                    var Correctivecollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CorrectiveDetailModel>>(Model.CorrectiveAction_Detail);
                    var CorrectiveRemarkcollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CorrectiveRemarklModel>>(Model.CapaConversationCorrective);
                    if (Correctivecollection != null)
                    {
                        foreach (CorrectiveDetailModel objCorrective in Correctivecollection)
                        {
                            res = AddUpdateCapaCorrective(Capa_Update: 0, Capa_ID: CapaID, Corrective_Action: objCorrective.Corrective_Action, Corrective_Text: objCorrective.CorrectiveAction_Text, Corrective_Owner: objCorrective.CorrectiveAction_Owner, UID: Model.CreatedBy, Email: objCorrective.Corrective_Email);
                          //  Task LogTask = Task.Factory.StartNew(() => MailUtils.SendMailAsync(objCorrective.Corrective_Email, "Entity creation alert!!", "mailbody.ToString()", "", "", ""));
                            foreach (CorrectiveRemarklModel objRemarkCorrective in CorrectiveRemarkcollection)
                            {
                                if (objCorrective.CID == objRemarkCorrective.Corrective_CID)
                                {

                                    res1 = AddUpdateConversastion(Con_TID: 0, Capa_ID: CapaID, ITEM_ID: Convert.ToInt32(res.Data.ToString()), ITEM_NAME: objRemarkCorrective.Corrective_Item_Name, REMARK: objRemarkCorrective.Remark, ATTACHMENT: objRemarkCorrective.originalFileName, SYSTEM_ATTACHMENT: objRemarkCorrective.TempFileName, STATUS: objRemarkCorrective.Status, CLOSURE_DATE: objRemarkCorrective.Closure_Date, UID: Model.CreatedBy, EID: Model.EID);
                                      
                                    continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }


                            if (res.IsSuccess == false)
                            {
                                break;
                            }
                        }
                    }
                }
                if (Model.PreventiveAction_Detail != null)
                {
                    var Preventivecollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PreventiveDetailModel>>(Model.PreventiveAction_Detail);
                    var Preventiveremarkcollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PreventiveRemarklModel>>(Model.CapaConversationPreventive);
                    if (Preventivecollection != null)
                    {
                        foreach (PreventiveDetailModel objPreventive in Preventivecollection)
                        {

                            res = AddUpdateCapaPreventive(Capa_Update: 0, Capa_ID: CapaID, Preventive_Action: objPreventive.Preventive_Action, Preventive_Text: objPreventive.PreventiveAction_Text, Preventive_Owner: objPreventive.PreventiveAction_Owner, UID: Model.CreatedBy, Email: objPreventive.Preventive_Email);
                           // Task LogTask = Task.Factory.StartNew(() => MailUtils.SendMailAsync(objPreventive.Preventive_Email, "Entity creation alert!!", "mailbody.ToString()", "", "", ""));
                            foreach (PreventiveRemarklModel objRemarkPreventive in Preventiveremarkcollection)
                            {
                                if (objPreventive.PID == objRemarkPreventive.Preventive_CID)
                                {
                                    res1 = AddUpdateConversastion(Con_TID: 0, Capa_ID: CapaID, ITEM_ID: Convert.ToInt32(res.Data.ToString()), ITEM_NAME: objRemarkPreventive.Preventive_Item_Name, REMARK: objRemarkPreventive.Remark, ATTACHMENT: objRemarkPreventive.originalFileName, SYSTEM_ATTACHMENT: objRemarkPreventive.TempFileName, STATUS: objRemarkPreventive.Status, CLOSURE_DATE: objRemarkPreventive.Closure_Date, UID: Model.CreatedBy, EID: Model.EID);
                                    continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            if (res.IsSuccess == false)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                trans.Rollback();
                conn.Dispose();
            }
            return res;
        }
        public int AddUpdateCapa(CapaModel model)
        {
            Response res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@TID",model.TID),
                 new SqlParameter("@ENTITY_ID",model.EID),
                new SqlParameter("@UID",model.CreatedBy),
                new SqlParameter("@ISSUE", model.Issue),
                new SqlParameter("@ISSUE_DESCRIPTION", model.Issue_Description),
                new SqlParameter("@CUSTOMER_IMPACT", model.Customer_Impact),
                new SqlParameter("@SEQUENCE_OF_EVENT", model.Sequence_of_events),
                 new SqlParameter("@COMMUNICATION_PROCESS", model.Communication_process),
                  new SqlParameter("@ROOT_CAUSE", model.Root_Cause),
                  new SqlParameter("@CATEGORY", model.Category),
                new SqlParameter("@FINANCE_TYPE", model.Finance_Type),
                new SqlParameter("@FINANCE_AMOUNT", model.Finance_Amount),
                 new SqlParameter("@IMPACT_VALUE", model.Impact_Value),
                  new SqlParameter("@STAGE", model.Stage),
                   new SqlParameter("@INCIDENT_DATE", model.Incident_date),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ADDUPDATECAPA", CommandType.StoredProcedure, parameters));

                if (result > 0)
                {
                    res.Message = " created successfully.";
                    res.IsSuccess = true;
                    return result;
                }

                if (result == 0)
                {
                    res.Message = "CAPA Updated successfully.";
                    res.IsSuccess = true;
                    return result;
                }



                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response UpdateCapaForEach(CapaModel Model)
        {
            Response res = new Response();
            int CapaID = Convert.ToInt32(Model.TID);
            int CapaIDs = AddUpdateCapa(Model);
            if (CapaID < 0)
            {
                res.Message = "Failed! Please try again.";
                res.IsSuccess = false;
                return res;
            }
            if (Model.CorrectiveAction_Detail != null)
            {

                var Correctivecollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CorrectiveDetailModel_Update>>(Model.CorrectiveAction_Detail);
                if (Correctivecollection != null)
                {
                    foreach (CorrectiveDetailModel_Update objCorrective in Correctivecollection)
                    {
                        res = AddUpdateCapaCorrective(Capa_Update: objCorrective.TID, Capa_ID: CapaID, Corrective_Action: objCorrective.Corrective_Action, Corrective_Text: objCorrective.Action_Text, Corrective_Owner: objCorrective.Action_Owner, UID: Model.CreatedBy, Email: objCorrective.Owner_Email);
                        if (res.IsSuccess == false)
                        {
                            break;
                        }
                    }
                }
            }
            if (Model.PreventiveAction_Detail != null)
            {

                var Preventivecollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PreventiveDetailModel_Update>>(Model.PreventiveAction_Detail);
                if (Preventivecollection != null)
                {
                    foreach (PreventiveDetailModel_Update objPreventive in Preventivecollection)
                    {
                        res = AddUpdateCapaPreventive(Capa_Update: objPreventive.TID, Capa_ID: CapaID, Preventive_Action: objPreventive.Preventive_Action, Preventive_Text: objPreventive.Action_Text, Preventive_Owner: objPreventive.Action_Owner, UID: Model.CreatedBy, Email: objPreventive.Owner_Email);
                        if (res.IsSuccess == false)
                        {
                            break;
                        }
                    }
                }
            }
            return res;
        }
        public Response AddUpdateConversastion(int Con_TID, int Capa_ID, int ITEM_ID, string ITEM_NAME, string REMARK, string ATTACHMENT, string SYSTEM_ATTACHMENT, string STATUS, string CLOSURE_DATE, int UID, int EID)
        {
            moveFile(SYSTEM_ATTACHMENT, EID);
            Response res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@TID",Con_TID),
                new SqlParameter("@CAPAID",Capa_ID),
                new SqlParameter("@ITEM_ID",ITEM_ID),
                new SqlParameter("@ITEM_NAME",ITEM_NAME),
                new SqlParameter("@REMARK",REMARK),
                new SqlParameter("@ATTACHMENT",ATTACHMENT),
                 new SqlParameter("@SYSTEM_ATTACHMENT",SYSTEM_ATTACHMENT),
                 new SqlParameter("@STATUS",STATUS),
                new SqlParameter("@CLOSURE_DATE",CLOSURE_DATE),
                new SqlParameter("@UID",UID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ADDUPDATECAPA_CONVERSASTION", CommandType.StoredProcedure, parameters));

                if (result > 0)
                {
                    res.Message = "Conversation created successfully.";
                    res.IsSuccess = true;
                    res.Data = result.ToString();
                    return res;
                }

                if (result == 0)
                {
                    res.Message = "Conversation Updated successfully.";
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
        private bool moveFile(string fileName, int Entity_ID)
        {
            bool result = false;
            try
            {
                if (fileName != "")
                {
                   
                    string sourceFile = HttpContext.Current.Server.MapPath("~/Docs/TEMP/" + fileName);
                    string targetFile = HttpContext.Current.Server.MapPath("~/Docs/Entity_" + Entity_ID + "/CAPA/") + fileName;
                    string fileExist = HttpContext.Current.Server.MapPath("~/Docs/Entity_" + Entity_ID + "/CAPA/") + fileName;

                    if (File.Exists(fileExist))
                    {
                        result = true;
                        return result;
                    }
                    bool exists = Directory.Exists(HttpContext.Current.Server.MapPath("~/Docs/Entity_" + Entity_ID + "/CAPA/"));
                    if (!exists)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Docs/Entity_" + Entity_ID + "/CAPA/"));
                    }
                    File.Move(sourceFile, targetFile);
                    result = true;
                }
                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public Response AddUpdateCapaCorrective(int Capa_Update, int Capa_ID, string Corrective_Action, string Corrective_Text, string Corrective_Owner, int UID, string Email)
        {
            Response res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@CAPA_UPDATE",Capa_Update),
                new SqlParameter("@CAPA_ID",Capa_ID),
                new SqlParameter("@CORRECTIVE_ACTION",Corrective_Action),
                new SqlParameter("@CORRECTIVEACTION_TEXT",Corrective_Text),
                new SqlParameter("@CORRECTIVEACTION_OWNER",Corrective_Owner),
                 new SqlParameter("@Owner_Email",Email),
                new SqlParameter("@UID",UID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ADDUPDATECORRECTIVE", CommandType.StoredProcedure, parameters));

                if (result > 0)
                {
                    res.Message = "CAPA created successfully.";
                    res.IsSuccess = true;
                    res.Data = result.ToString();
                    return res;
                }

                if (result == 0)
                {
                    res.Message = "CAPA Updated successfully.";
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
        public Response AddUpdateCapaPreventive(int Capa_Update, int Capa_ID, string Preventive_Action, string Preventive_Text, string Preventive_Owner, int UID, string Email)
        {
            Response res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@CAPA_UPDATE",Capa_Update),
                new SqlParameter("@CAPA_ID",Capa_ID),
                new SqlParameter("@PREVENTIVE_ACTION",Preventive_Action),
                new SqlParameter("@PREVENTIVEACTION_TEXT",Preventive_Text),
                new SqlParameter("@PREVENTIVEACTION_OWNER",Preventive_Owner),
                  new SqlParameter("@Owner_Email",Email),
                new SqlParameter("@UID",UID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ADDUPDATEPREVENTIVE", CommandType.StoredProcedure, parameters));

                if (result > 0)
                {
                    res.Message = "CAPA created successfully.";
                    res.IsSuccess = true;
                    res.Data = result.ToString();
                    return res;
                }

                if (result == 0)
                {
                    res.Message = "CAPA Updated successfully.";
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
        public Response DeleteCAPAItemDetails(int Item_ID, string Type)
        {
            Response res = new Response();

            try
            {
                if (Type == "Corrective")
                {
                    SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TID",Item_ID)
                };

                    DataLib.ExecuteNonQuery("DeleteCorrective", CommandType.StoredProcedure, parameters);
                    res.IsSuccess = true;
                    res.Message = "Item sucessfully deleted";
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TID",Item_ID)
                };

                    DataLib.ExecuteNonQuery("DeletePreventive", CommandType.StoredProcedure, parameters);
                    res.IsSuccess = true;
                    res.Message = "Item sucessfully deleted";
                }

                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}