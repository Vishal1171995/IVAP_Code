
using Ivap.Areas.Master.CustomValidation;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Areas.Master.Repository;


namespace Ivap.Areas.Configuration.Models
{
    public class GlobalComponentModel : BaseModel
    {
        public int COMPONENTID { get; set; }
        [Required(ErrorMessage = "Component File Type required.")]
        public string COMPONENT_FILE_TYPE { get; set; }
        [Required(ErrorMessage = "Component Type required.")]
        public string COMPONENT_TYPE { get; set; }

        [Required(ErrorMessage = "Component Sub Type required.")]
        public string COMPONENT_SUB_TYPE { get; set; }
        [Required(ErrorMessage = "Component Name required")]
        // [RegularExpression("^[A-Za-z0-9\\d@_.]{2,100}$", ErrorMessage = "Only alphanumeric allowed.")]
        [RegularExpression("^[A-Za-z0-9\\d_]{2,100}$", ErrorMessage = "Only alphanumeric allowed.")]
        // [RegularExpression(@"[a-zA-Z-_][a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric allowed")]
        public string COMPONENT_NAME { get; set; }

        [Required(ErrorMessage = "Component Datatype required.")]
        public string COMPONENT_DATATYPE { get; set; }

        [Required(ErrorMessage = "Component Display Name required.")]
        [RegularExpression("^[a-zA-Z0-9\\d' '@_]{2,200}$", ErrorMessage = "Only alphanumeric allowed.")]
        public string COMPONENT_DISPLAY_NAME { get; set; }

        [Required(ErrorMessage = "Component Description required.")]
        public string COMPONENT_DESCRIPTION { get; set; }
        //[Required(ErrorMessage = "Min Length required.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Min Length required")]
        [CompTypeValidation]
        public int? MIN_LENGTH { get; set; }

       // [Required(ErrorMessage = "Max Length required.")]
        //[Range(2, int.MaxValue, ErrorMessage = "Max Length Greater then Min Length")]
        [CompTypeMaxValidation]
        public int? MAX_LENGTH { get; set; }
        public bool  MANDATORY { get; set; }
        public string Expression { get; set; }
        public string Extra_Validation { get; set; }
        // [Required(ErrorMessage = "Component FieldName required.")]
        [CompTypeTableNameValidation]
        public string Component_FieldName { get; set; }
       // [Required(ErrorMessage = "Component TableName required.")]
       [CompTypeColumnNameValidation]
        public string Component_TableName { get; set; }
        public SelectList Component_File_TypeList { get; set; }
        public SelectList COMPONENT_DATATYPEList { get; set; }
        public SelectList Component_TableNameList { get; set; }
        public SelectList Validation_List { get; set; }

    }



    public class GlobalComponentVM : BaseModel
    {
        public int COMPONENTID { get; set; }
        [Required(ErrorMessage = "Component File Type required.")]
        public string COMPONENT_FILE_TYPE { get; set; }
        [Required(ErrorMessage = "Component Type required.")]
        public string COMPONENT_TYPE { get; set; }

        [Required(ErrorMessage = "Component Sub Type required.")]
        public string COMPONENT_SUB_TYPE { get; set; }
        [Required(ErrorMessage = "Component Name required")]
        public string COMPONENT_NAME { get; set; }
        [Required(ErrorMessage = "Component Datatype required.")]
        public string COMPONENT_DATATYPE { get; set; }

        [Required(ErrorMessage = "Component Display Name required.")]
        public string COMPONENT_DISPLAY_NAME { get; set; }

        [Required(ErrorMessage = "Component Description required.")]
        public string COMPONENT_DESCRIPTION { get; set; }
        [Required(ErrorMessage = "Min Length required.")]
        public string MIN_LENGTH { get; set; }

        [Required(ErrorMessage = "Max Length required.")]
        public string MAX_LENGTH { get; set; }
        public bool MANDATORY { get; set; }
    public string Expression { get; set; }
    public string Extra_Validation { get; set; }
    public string Component_FieldName { get; set; }
    public string Component_TableName { get; set; }
}

    public class GlobalComponentGrid
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}