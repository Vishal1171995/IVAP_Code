using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Utils
{
    public class Response
    {
        public bool IsSuccess { set; get; }
        public string  Data { set; get; }

        public string Message { set; get; }


        public enum operation {
            ADD,
            Update

        }
        public Response GetResponse(operation Opration, string operationname,int result,string duplicatefields)
        {

            Response objRes = new Response();
            switch (Opration)
            {
                case operation.ADD:
                    {
                        objRes.Data = result.ToString();
                        if (result > 0)
                        {
                            objRes.IsSuccess = true;
                            objRes.Message = operationname + " created. Your transaction-id is " + result;
                            
                        }
                        else if (result == -1)
                        {
                            objRes.IsSuccess = false;
                            if (operationname.Contains(","))
                                objRes.Message = "Creation failed. Combination of " + duplicatefields + " must be unique.";
                            else
                                objRes.Message = "Creation failed. " + duplicatefields + " must be unique.";
                        }
                        break;
                    }
                case operation.Update:
                    {

                        if (result == 0)
                        {
                            objRes.IsSuccess = true;
                            objRes.Message = operationname + " updated successfully.";
                            objRes.Data = operationname.ToString();
                        }
                        else if (result == -1)
                        {
                            objRes.IsSuccess = false;
                            if (operationname.Contains(","))
                                objRes.Message = "Creation failed. Combination of " + duplicatefields + " must be unique.";
                            else
                                objRes.Message = "Creation failed. " + duplicatefields + " must be unique.";
                            objRes.Data = operationname.ToString();
                        }
                        break;
                    }
            }
            return objRes;
        }

       
    }
}