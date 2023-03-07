using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


//Namespace Models.Utility

namespace Ivap.Utils
{
    public class DataLib
    {


        public enum Connection
        {
            ConnectionString
        }
        public static string GetConnectionString()
        {
            string retValue = "";
            retValue = ConfigurationManager.AppSettings["ConnectionString"];
            return retValue;
        }

        internal static DataTable ExecuteDataTable(string v, CommandType storedProcedure)
        {
            throw new NotImplementedException();
        }

        public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType, SqlParameter[] sqlParms)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    //Prepare the Command

                    using (SqlCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.SelectCommand = cmd;
                            adapter.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch (SqlException Ex)
            {
                throw;
            }
            catch (Exception Ex)
            {

                throw new Exception("DataLib.ExecuteDataSet" + Ex.Message);
            }
        }
        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, SqlParameter[] sqlParms)
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    //Prepare the Command
                    using (SqlCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.SelectCommand = cmd;
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (SqlException Ex)
            {
                throw;
            }
            catch (Exception Ex)
            {

                throw;
            }
        }

        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, SqlParameter[] sqlParms,SqlConnection con,SqlTransaction trans)
        {
            try
            {
                DataTable dt = new DataTable();
                
                    //Prepare the Command
                    using (SqlCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            cmd.Transaction = trans;
                            adapter.SelectCommand = cmd;
                            adapter.Fill(dt);
                        }
                    }
                return dt;
            }
            catch (SqlException Ex)
            {
                throw;
            }
            catch (Exception Ex)
            {

                throw;
            }
        }
        public static string ExecuteScaler(string cmdText, CommandType cmdType, SqlParameter[] sqlParms)
        {

            try
            {
                string retString = string.Empty;
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        retString = Convert.ToString(cmd.ExecuteScalar());
                    }
                }
                return retString;
            }
            catch (SqlException Ex)
            {
                throw;
            }
            catch (Exception Ex)
            {

                throw;
            }

        }


        public static string ExecuteScaler(string cmdText, CommandType cmdType, SqlParameter[] sqlParms,SqlConnection con,SqlTransaction trans)
        {

            try
            {
                string retString = string.Empty;
                using (SqlCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                {
                    cmd.Transaction = trans;
                    retString = Convert.ToString(cmd.ExecuteScalar());
                   
                }
                return retString;
            }
            catch (SqlException Ex)
            {
                throw;
            }
            catch (Exception Ex)
            {

                throw;
            }

        }
        public static int ExecuteNonQuery(string cmdText, CommandType cmdType, SqlParameter[] sqlParms)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        int val = cmd.ExecuteNonQuery();
                        return val;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw;
            }
            catch (Exception exx)
            {
                throw new Exception("DataLib.ExecuteNonQuery", exx);
            }
        }

        public static int ExecuteNonQuery(string cmdText, CommandType cmdType, SqlParameter[] sqlParms, SqlConnection conn, SqlTransaction trans)
        {
            try
            {

                using (SqlCommand cmd = PrepareCommand(conn, cmdType, cmdText, sqlParms))
                {
                    cmd.Transaction = trans;
                    int val = cmd.ExecuteNonQuery();
                    return val;
                }
            }
            catch (SqlException ex)
            {
                throw;
            }
            catch (Exception exx)
            {
                throw new Exception("DataLib.ExecuteNonQuery", exx);
            }
        }

        public static SqlCommand PrepareCommand(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms, SqlTransaction trans = null)
        {
            if (!(conn.State == ConnectionState.Open))
            {
                conn.Open();
            }
            try
            {

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.Parameters.Clear();
                cmd.CommandType = cmdType;
                if (cmdParms != null)
                {
                    foreach (SqlParameter parm in cmdParms)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }
                return cmd;
            }
            catch (SqlException ex)
            {
                throw;
            }
            catch (Exception exx)
            {
                throw;
            }
        }

        public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType, SqlParameter[] sqlParms, SqlConnection con, SqlTransaction trans)
        {
            DataSet ds = new DataSet();
            try
            {
                //Prepare the Command

                using (SqlCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        cmd.Transaction = trans;
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                    }
                }
                return ds;
            }
            catch (SqlException Ex)
            {
                throw;
            }
            catch (Exception Ex)
            {

                throw new Exception("DataLib.ExecuteDataSet" + Ex.Message);
            }
        }


    }
}

