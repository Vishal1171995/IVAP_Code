
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class ComponentModel : BaseModel
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
        [Range(1, int.MaxValue, ErrorMessage = "Width required")]
        public int? MIN_LENGTH { get; set; }

        [Required(ErrorMessage = "Max Length required.")]
        [Range(2, int.MaxValue, ErrorMessage = "Width required")]
        public int? MAX_LENGTH { get; set; }
        public bool  MANDATORY { get; set; }

    }


    public class ComponentVM : BaseModel
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
    }

    public class ComponentBank
    {
        public int from { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}