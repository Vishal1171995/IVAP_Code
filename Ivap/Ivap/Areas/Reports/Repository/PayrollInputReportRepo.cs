using Ivap.Areas.Reports.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivap.Areas.Reports.Repository
{
    public class PayrollInputReportRepo
    {
        public DataTable GetPayDates(int Entity_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", Entity_ID),
                };
                dt = DataLib.ExecuteDataTable("GetPayDates", CommandType.StoredProcedure, parameters);
                return dt;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }
        public DataTable GetClosingMonth(int Entity_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@Entity_Code", Entity_ID),
                };
                dt = DataLib.ExecuteDataTable("GetClosingMonth", CommandType.StoredProcedure, parameters);
                return dt;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }

        public DataTable GetInputFileReportData(PayrollInputFileReportModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", objModel.EID),
                    new SqlParameter("@PayDate", objModel.SelectedPayDate),
                };
                dt = DataLib.ExecuteDataTable("GetPayrollInputReport_New", CommandType.StoredProcedure, parameters);
                return dt;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }

        public DataTable GetSchemaOfInputFile(int EID, int FILE_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FILE_ID",FILE_ID),

                };
                dt = DataLib.ExecuteDataTable("GET_FILE_SETUP_UPLOAD", CommandType.StoredProcedure, parameters);
                return dt;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }

        public DataTable GetSchemaOfInputReportFile(int EID, int FILE_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FILE_ID",FILE_ID),

                };
                dt = DataLib.ExecuteDataTable("GET_FILE_SETUP_Input_Report", CommandType.StoredProcedure, parameters);
                return dt;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }


        public DataTable GetInputFileReportDownload(PayrollInputFileReportModel objModel)
        {
            DataTable dtSchema = GetSchemaOfInputReportFile(objModel.EID, objModel.File_ID);
            string EMP_CODE = "EMP_CODE";
            string PAYDATE = "PAYDATE";

            //Now Replace Master valued with TIDs
            bool MasterComponentAvailable = false;
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("select ");
            foreach (DataRow drSchema in dtSchema.Rows)
            {
                if (Convert.ToString(drSchema["COMPONENT_NAME"]).ToString().ToUpper() == "PAYDATE")
                {
                    PAYDATE = Convert.ToString(drSchema["COMPONENT_DISPLAY_NAME"]);
                }
                if (Convert.ToString(drSchema["COMPONENT_NAME"]).ToString().ToUpper() == "EMP_CODE")
                {
                    EMP_CODE = Convert.ToString(drSchema["COMPONENT_DISPLAY_NAME"]);
                }
                if (Convert.ToString(drSchema["spl_field_type"]) == "" || Convert.ToString(drSchema["spl_field_type"]) == "LOOKUP VALUED")
                {
                    if (drSchema["COMPONENT_DATATYPE"].ToString().ToUpper() == "MASTER")
                    {
                        sqlQuery.Append(drSchema["COMPONENT_TABLE_NAME"]).Append(".").Append(drSchema["COMPONENT_COLUMN_NAME"])
                                        .Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                    }
                    else if (drSchema["COMPONENT_DATATYPE"].ToString().ToUpper() == "DATETIME")
                    {
                        sqlQuery.Append(" convert(varchar,ivap_temp_his_" + objModel.EID + "." + drSchema["COMPONENT_NAME"] + ", 103)").Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                    }
                    else
                    {
                        sqlQuery.Append(" ivap_temp_his_" + objModel.EID).Append(".").Append(drSchema["COMPONENT_NAME"]).Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                    }
                }
                else if (Convert.ToString(drSchema["spl_field_type"]) == "DEFAULT VALUED")
                {
                    sqlQuery.Append(Convert.ToString(drSchema["Spl_Field_Value"]))
                                       .Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                }

            }
            string StrMainQry = sqlQuery.ToString().Trim().TrimEnd(',');
            StrMainQry = StrMainQry + " from ivap_temp_his_" + objModel.EID;
            //Just for testing + " where FILE_ID=" + objModel.SubFile_ID;

            StringBuilder SbMasterDataJoin = new StringBuilder();
            DataView dvMaster = new DataView(dtSchema);
            dvMaster.RowFilter = "COMPONENT_DATATYPE='MASTER'";
            DataTable dtMasterComponent = dvMaster.ToTable();
            StringBuilder SbTableName = new StringBuilder();

            for (int i = 0; i < dtMasterComponent.Rows.Count; i++)
            {
                MasterComponentAvailable = true;
                SbTableName.Append(" left join ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"])
                    .Append(" on ivap_temp_his_" + objModel.EID).Append(".").Append(dtMasterComponent.Rows[i]["COMPONENT_NAME"])
                    .Append(" = ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append(".TID");
            }
            // we will check access rights for master data for specific user and entity
            DataSet dsAllMaster = new DataSet();
            if (MasterComponentAvailable == true)
            {
                StrMainQry += SbTableName.ToString();
            }
            StrMainQry = StrMainQry + " where convert(varchar,ivap_temp_his_" + objModel.EID + ".PAYDATE,103)='" + objModel.PayDate.ToString("dd/MM/yyyy") + "' and ivap_temp_his_" + objModel.EID + ".FILE_ID=" + objModel.SubFile_ID + " and ivap_temp_his_" + objModel.EID + ".CURR_STATUS='APPROVED'";
            DataTable dtData = DataLib.ExecuteDataTable(StrMainQry, CommandType.Text, null);
            if (dtSchema.Rows.Count > 0)
            {
                bool isTranspose = false;
                isTranspose = Convert.ToInt32(dtSchema.Rows[0]["Transpose"]) == 1 ? true : false;
                if (isTranspose)
                {

                    return TrasnposeData(dtData, TransposeScheema(objModel.File_ID), EMP_CODE: EMP_CODE, PAYDATE: PAYDATE, dtInputScheema: dtSchema);
                }
            }
            return dtData;
        }


        public DataTable TrasnposeData(DataTable dt, DataTable dtTransposeScheema, string EMP_CODE, string PAYDATE, DataTable dtInputScheema)
        {
            DataTable dtOutPut = new DataTable();
            // Now itration over transpose scheema and get value from subfile data Payroll Input Processing
            foreach (DataRow drScheema in dtTransposeScheema.Rows)
            {
                if (Convert.ToString(drScheema["Field_Type"]).ToUpper() == "TRANSPOSEVALUE")
                {
                    dtOutPut.Columns.Add(Convert.ToString(drScheema["Display_Name"]), typeof(decimal));
                }
                else
                {
                    dtOutPut.Columns.Add(Convert.ToString(drScheema["Display_Name"]), typeof(string));
                }
            }
            if (dtTransposeScheema.Rows.Count == 0)
            {
                try
                {
                    dtOutPut.Columns.Add(EMP_CODE, typeof(string));
                    dtOutPut.Columns.Add(PAYDATE, typeof(string));
                    dtOutPut.Columns.Add("COMPONENT_NAME", typeof(string));
                    dtOutPut.Columns.Add("COMPONENT_VALUE", typeof(string));
                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (!(dc.ColumnName.ToString().ToUpper().Trim() == EMP_CODE.ToString().ToUpper() || dc.ColumnName.ToString().ToUpper().Trim() == PAYDATE.ToString().ToUpper()))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])))
                                {
                                    DataRow drOutPut = dtOutPut.NewRow();
                                    drOutPut[EMP_CODE] = dr[EMP_CODE];
                                    drOutPut[PAYDATE] = dr[PAYDATE];
                                    drOutPut["COMPONENT_NAME"] = dc.ColumnName;
                                    drOutPut["COMPONENT_VALUE"] = dr[dc.ColumnName];
                                    dtOutPut.Rows.Add(drOutPut);
                                }
                            }
                        }
                    }

                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                {

                }
                return dtOutPut;
            }
            else
            {
                try
                {
                    foreach (DataRow drScheema in dtInputScheema.Rows)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (Convert.ToString(dc.ColumnName).Trim().ToUpper() == Convert.ToString(drScheema["Component_Display_Name"]).Trim().ToUpper())
                            {
                                dc.ColumnName = Convert.ToString(drScheema["Component_Name"]);
                                break;
                            }
                        }
                    }
                    foreach (DataRow dr in dt.Rows)
                    {

                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (IsSkipColumnName(dc: dc, dt: dtTransposeScheema, Filter_Text: "COMPONENT"))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])))
                                {
                                    DataRow drOutPut = dtOutPut.NewRow();
                                    foreach (DataRow drTranspose in dtTransposeScheema.Rows)
                                    {
                                        if (Convert.ToString(drTranspose["Field_Type"]) == "COMPONENT")
                                        {
                                            drOutPut[Convert.ToString(drTranspose["DISPLAY_NAME"])] = dr[Convert.ToString(drTranspose["COMPONENT_NAME"])];
                                        }
                                        else if (Convert.ToString(drTranspose["Field_Type"]) == "DEFAULTVALUE")
                                        {
                                            drOutPut[Convert.ToString(drTranspose["DISPLAY_NAME"])] = Convert.ToString(drTranspose["DEFAULT_VALUE"]);
                                        }
                                        else if (Convert.ToString(drTranspose["Field_Type"]) == "TRANSPOSEFIELD")
                                        {
                                            drOutPut[Convert.ToString(drTranspose["DISPLAY_NAME"])] = (from r in dtInputScheema.AsEnumerable() where r.Field<string>("COMPONENT_NAME") == Convert.ToString(dc.ColumnName) select r.Field<string>("COMPONENT_DISPLAY_NAME")).First<string>();
                                        }
                                        else if (Convert.ToString(drTranspose["Field_Type"]) == "TRANSPOSEVALUE")
                                        {
                                            drOutPut[Convert.ToString(drTranspose["DISPLAY_NAME"])] = Convert.ToString(dr[dc.ColumnName]);
                                        }
                                    }
                                    dtOutPut.Rows.Add(drOutPut);
                                }
                            }
                        }
                    }

                    return dtOutPut;

                }
                catch
                {
                    throw;
                }
            }
        }

        //public DataTable InsertRow(DataTable dt,DataTable dtTranspose) {
        //    try
        //    {
        //        DataRow drOutPut = dt.NewRow();

        //        return dt;
        //    }
        //    catch(Exception ex) {
        //        throw;
        //    }
        //}
        private bool IsSkipColumnName(DataColumn dc, DataTable dt, string Filter_Text)
        {
            bool res = false;
            try
            {
                if (Filter_Text.ToString() != "")
                {
                    DataView dv = new DataView(dt);
                    dv.RowFilter = "Field_Type='" + Filter_Text + "'";
                    DataTable dtFilterdRow = dv.ToTable();
                    foreach (DataRow dr in dtFilterdRow.Rows)
                    {
                        if (dc.ColumnName.ToString().Trim().ToUpper() == Convert.ToString(dr["COMPONENT_NAME"]).ToUpper())
                        {
                            return res;
                        }
                    }
                    if (dtFilterdRow.Rows.Count > 0)
                    {
                        res = true;
                        return res;
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {

                throw;
            }
            return res;
        }

        private DataTable TransposeScheema(int File_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@File_ID",File_ID),

                };
                dt = DataLib.ExecuteDataTable("GetTransposeScheema", CommandType.StoredProcedure, parameters);
                return dt;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }

        }
    }
}