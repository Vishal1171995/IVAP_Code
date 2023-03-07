using Ivap.Areas.Master.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Repository
{
    public class CurrencyRepo
    {
        public Response AddUpdateCurrency(CurrencyModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@CID",model.CID),
                new SqlParameter("@CURRENCY_CODE", model.CURRENCY_CODE),
                new SqlParameter("@CURRENCY_NAME", model.CURRENCY_NAME),
                 new SqlParameter("@UID", model.CreatedBy),
                  new SqlParameter("@ISACTIVE", model.IsActive),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateCurrency", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.CID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Currency", result, "Currency name");
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetCurrency(CurrencyModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@CurrencyID", objModel.CID),
                    new SqlParameter("@CurrencyCode",objModel.CURRENCY_CODE ),
                    new SqlParameter("@CurrencyName", objModel.CURRENCY_NAME),
                    new SqlParameter("@ISAct", objModel.IsActive),

                };
                dt = DataLib.ExecuteDataTable("GetCurrency", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetCurrencyHis(CurrencyModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@CID", objModel.CID),
                };
                dt = DataLib.ExecuteDataTable("GetCurrencyHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}