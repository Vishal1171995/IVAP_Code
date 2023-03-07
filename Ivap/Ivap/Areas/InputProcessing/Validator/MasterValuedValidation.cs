using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator
{
    public class MasterValuedValidation: BasicValidationBase
    {
        public override ValidationResponse Validate(BasicValidationBase ObjVal)
        {

            ValidationResponse Res = new ValidationResponse();
            //Skip validation is Data is not supplyed. Required validation will be take care by required validator.
            if(ObjVal.Value.Trim()=="")
            {
                Res.IsSuccess = true;
                return Res;
            }
            Res.IsSuccess = false;
            Res.ValidationErrorType = "Master Valued Validation";
            Res.Message = "Invalid " + ObjVal.DisplayName;
            DataView dv = new DataView(ObjVal.dsMasterData.Tables[0]);
            dv.RowFilter = "Master_Name='" + ObjVal.Table_Name + "' AND Value='"+ ObjVal.Value+"'";
            DataTable dt = dv.ToTable();
            if (dt.Rows.Count > 0)
                Res.IsSuccess = true;
            return Res;
        }
    }
}