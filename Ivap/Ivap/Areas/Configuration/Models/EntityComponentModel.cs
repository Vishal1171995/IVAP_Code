using Ivap.Areas.Configuration.CustomValidation;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class EntityComponentModel: BaseModel
    {
        public int EntityCOMPID { get; set; }
        public int? ENTITYID { get; set; }
        public List<string> Globle_Component_ID { get; set; }
        public string COMPONENT_FILE_TYPE { get; set; }

        [Required(ErrorMessage = "Component Display Name required.")]
        [RegularExpression("^[a-zA-Z0-9\\d' '@_]{2,200}$", ErrorMessage = "Only alphanumeric allowed.")]
        public string COMPONENT_DISPLAY_NAME { get; set; }
         
        [Required(ErrorMessage = "Component Datatype required.")]
        public string COMPONENT_DATATYPE { get; set; }
       
        [EntityCompTypeValidation]
        public int? MIN_LENGTH { get; set; }

       
        [EntityCompTypeMaxValidation]
        public int? MAX_LENGTH { get; set; }
      
        [EntityCompTypeTableNameValidation]
        public string Component_FieldName { get; set; }
        // [Required(ErrorMessage = "Component TableName required.")]
        [EntityCompTypeColumnNameValidation]
        public string Component_TableName { get; set; }
        public bool MANDATORY { get; set; }
        public string File_Type { set; get; }

        public string GL_Code { set; get; }

        public string PMS_Code { set; get; }
        public string Expression { get; set; }
        public string Extra_Validation { get; set; }

        public SelectList COMPONENT_DATATYPEList { get; set; }
        public SelectList Component_TableNameList { get; set; }
        public SelectList Validation_List { get; set; }
    }
    

    public class EntityComponentVM : BaseModel
    {
        public int EntityCOMPID { get; set; }

        [Required(ErrorMessage = "Component Datatype required.")]
        public string COMPONENT_DATATYPE { get; set; }

        [Required(ErrorMessage = "Component Display Name required.")]
        public string COMPONENT_DISPLAY_NAME { get; set; }
        [Required(ErrorMessage = "Min Length required.")]
        public string MIN_LENGTH { get; set; }

        [Required(ErrorMessage = "Max Length required.")]
        public string MAX_LENGTH { get; set; }
        public bool MANDATORY { get; set; }
 
        [Required(ErrorMessage = "Component File Type required.")]
        public string COMPONENT_FILE_TYPE { get; set; }
        [Required(ErrorMessage = "Component Type required.")]
        public string COMPONENT_TYPE { get; set; }

        [Required(ErrorMessage = "Component Sub Type required.")]
        public string COMPONENT_SUB_TYPE { get; set; }
        [Required(ErrorMessage = "Component Name required")]
        public string COMPONENT_NAME { get; set; }
 

        [Required(ErrorMessage = "Component Description required.")]
        public string COMPONENT_DESCRIPTION { get; set; }
       
    }

    public class EntityComponentGrid
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}