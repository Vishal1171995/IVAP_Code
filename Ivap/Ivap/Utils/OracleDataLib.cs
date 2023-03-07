using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;

namespace Ivap.Utils
{
    public class OracleDataLib
    {


        public static string ConStr=ConfigurationManager.AppSettings["ConnectionString"];
        

        public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType, OracleParameter[] sqlParms)
        {
            DataSet ds = new DataSet();
            try
            {
                using (OracleConnection con = new OracleConnection(ConStr))
                {
                    //Prepare the Command

                    using (OracleCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.SelectCommand = cmd;
                            adapter.Fill(ds);
                        }
                    }
                }
                return ds;
            }
            catch 
            {
                throw;
            }
        }
        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, OracleParameter[] sqlParms)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection con = new OracleConnection(ConStr))
                {
                    //Prepare the Command
                    using (OracleCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.SelectCommand = cmd;
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception Ex)
            {

                throw;
            }
        }

        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, OracleParameter[] sqlParms, OracleConnection con, OracleTransaction trans)
        {
            try
            {
                DataTable dt = new DataTable();

                //Prepare the Command
                using (OracleCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        cmd.Transaction = trans;
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                    }
                }
                return dt;
            }
            catch (Exception Ex)
            {

                throw;
            }
        }


        public static string ExecuteScaler(string cmdText, CommandType cmdType, OracleParameter[] sqlParms, string outParameter)
        {

            try
            {
                object retString;
                using (OracleConnection con = new OracleConnection(ConStr))
                {
                    using (OracleCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        retString = Convert.ToString(cmd.ExecuteScalar());
                        retString = cmd.Parameters[outParameter].Value.ToString();
                    }
                }
                return retString.ToString();
            }
            catch (Exception Ex)
            {

                throw;
            }

        }


        public static string ExecuteScaler(string cmdText, CommandType cmdType, OracleParameter[] sqlParms)
        {

            try
            {
                string retString = string.Empty;
                using (OracleConnection con = new OracleConnection(ConStr))
                {
                    using (OracleCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        cmd.ExecuteScalar();
                        retString = cmd.Parameters["Result"].Value.ToString();
                    }
                }
                return retString;
            }
            catch (Exception Ex)
            {

                throw;
            }

        }


        public static string ExecuteScaler(string cmdText, CommandType cmdType, OracleParameter[] sqlParms, OracleConnection con, OracleTransaction trans)
        {

            try
            {
                string retString = string.Empty;
                using (OracleCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                {
                    cmd.Transaction = trans;
                    cmd.ExecuteScalar();
                    retString = cmd.Parameters["Result"].Value.ToString();
                    return retString;
                }
                return retString;
            }
            catch (Exception Ex)
            {

                throw;
            }

        }
        public static int ExecuteNonQuery(string cmdText, CommandType cmdType, OracleParameter[] sqlParms)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(ConStr))
                {
                    using (OracleCommand cmd = PrepareCommand(con, cmdType, cmdText, sqlParms))
                    {
                        int val = cmd.ExecuteNonQuery();
                        return val;
                    }
                }
            }
            catch (Exception exx)
            {
                throw;
            }
        }
        public static OracleCommand PrepareCommand(OracleConnection conn, CommandType cmdType, string cmdText, OracleParameter[] cmdParms, OracleTransaction trans = null)
        {
            if (!(conn.State == ConnectionState.Open))
            {
                conn.Open();
            }
            try
            {

                OracleCommand cmd = new OracleCommand() ;
                cmd.BindByName = true;
                cmd.Connection = conn;
                cmd.CommandText = cmdText;
                cmd.Parameters.Clear();
                cmd.CommandType = cmdType;
                if (cmdParms != null)
                {
                    foreach (OracleParameter parm in cmdParms)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }
                return cmd;
            }
            catch (Exception exx)
            {
                throw;
            }
        }

    }
}