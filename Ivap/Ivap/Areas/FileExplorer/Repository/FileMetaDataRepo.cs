using Ivap.Areas.FileExplorer.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.FileExplorer.Repository
{
    public class FileMetaDataRepo
    {
        public DataTable GetMetaDataList(FileMetaDataModel model)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@FileMetaID",model.FileMetaID ),
                    new SqlParameter("@EID",model.EID ),


                };

                return DataLib.ExecuteDataTable("[GetMetaDataList]", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }

        }
        public Response CreateUpdateMetaData(FileMetaDataModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@FileMetaID", model.FileMetaID ),
                    new SqlParameter("@EID", model.EID),
                    new SqlParameter("@Description", model.Description),
                    new SqlParameter("@MetaData", model.MetaData),
                    new SqlParameter("@FileTypeName", model.FileTypeName),
                   new SqlParameter("@CreatedBy", model.CreatedBy),

                };
                int result= Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateMetaData", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.FileMetaID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "FileMetaData", result, "FileMetadata");
                return res;
            }
            catch
            {
                throw;
            }
        }
    }
}