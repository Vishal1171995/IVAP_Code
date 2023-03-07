using Ivap.Areas.Master.CustomValidation;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class FileComponentModel:BaseModel
    {
        public int FILE_ID { set; get; }
        public int COMPONENTID { get; set; }
        
        public string COMPONENT_FILE_TYPE { get; set; }
       
        public int MIN_VALUE { get; set; }

       
        public int MAX_VALUE { get; set; }
       
        public string COMPONENT_NAME { get; set; }

        //[Required(ErrorMessage = "Required")]
        public string COMPONENT_DATATYPE { get; set; }

        [Required(ErrorMessage = "Required")]
        public string COMPONENT_DISPLAY_NAME { get; set; }

        [Required(ErrorMessage = "Required")]
        public string COMPONENT_DESCRIPTION { get; set; }

        //[CompTypeValidation]
        [Required(ErrorMessage = "Required")]
        public int? MIN_LENGTH { get; set; }
        //[CompTypeMaxValidation]
        [Required(ErrorMessage = "Required")]
        public int? MAX_LENGTH { get; set; }
        public bool MANDATORY { get; set; }
        
        public string GL_Code { get; set; }
       
        public string PMS_CODE { get; set; }

        //[CompTypeTableNameValidation]

        [Required(ErrorMessage = "Required")]
        public string Component_FieldName { get; set; }

        public string Component_FieldNameShow { get; set; }

        //[CompTypeColumnNameValidation]
        public string Expression { get; set; }
        public string Extra_Validation { get; set; }
        public SelectList Validation_List { get; set; }
       
        public string Component_TableName { get; set; }
        public SelectList Component_File_TypeList { get; set; }
        public SelectList COMPONENT_DATATYPEList { get; set; }
        public SelectList Component_TableNameList { get; set; }
    }
}