using Ivap.Areas.InputProcessing.Validator.OtherValidation;
using Ivap.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Validator
{
    public  class BasicValidationBase: BaseModel
    {
        public string Value { set; get; }

        public string IsRequired { set; get; }
        public string DataType { set; get; }

        public string DisplayName { set; get; }

        public string ColumnName { set; get; }

        public string DateFormat { set; get; }

        public decimal MinLength { set; get; }

        public decimal MaxLength { set; get; }

        public DataSet dsMasterData { set; get; }

        public int UID { set; get; }

        public string Table_Name { set; get; }

        public string Reg_Exp { set; get; }

        public string Extra_ValidationType { set; get; }

        public  virtual ValidationResponse Validate(BasicValidationBase ObjVal)
        {
            ValidationResponse Res = new ValidationResponse();
            return Res;
        }

        public ValidationResponse ExecuteBasicValidation(BasicValidationBase ObjValP)
        {
            try
            {
                ValidationResponse Res = new ValidationResponse();
                Res.IsSuccess = true;
                Res.ValidationErrorType = "SUCCESS";
                BasicValidationBase ObjVal = new RequiredValidation();
                // First Run Required Validation 
                Res = ObjVal.Validate(ObjValP);
                if (Res.IsSuccess == false)
                    return Res;

                ObjVal = new DataFormatValidation();
                Res = ObjVal.Validate(ObjValP);
                if (Res.IsSuccess == false)
                    return Res;
                if (ObjValP.DataType == "MASTER")
                {
                    ObjVal = new MasterValuedValidation();
                    Res = ObjVal.Validate(ObjValP);
                    if (Res.IsSuccess == false)
                        return Res;

                    ObjVal = new MasterValuedAccessVoilation();
                    Res = ObjVal.Validate(ObjValP);
                    if (Res.IsSuccess == false)
                        return Res;
                }

                var ObjExtraValidator = ExtraValidationsFactory.Create(ObjValP.Extra_ValidationType);
                Res = ObjExtraValidator.Validate(ObjValP);
                if (Res.IsSuccess == false)
                    return Res;



                return Res;

            }
            catch
            {
                throw;
            }
        }

    }

    
}