using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class PAYRollWorkFlowSettingItem
    {
        public int USER_ROLE { get; set; }
        public bool ISCheck { get; set; }
        public PAYRoll_WFORDERING ORDERING { get; set; }
        public string CompleteCssClass { set; get; }
        public int UserCount { get; set; }

        public string Icon_Name { set; get; }
 

    }

    public class PAYRollWFSettingModel : BaseModel
    {
        public int FILE_ID { get; set; }
        public PAYRollWorkFlowSettingItem WorkflowSettingMaker { get; set; }
        public PAYRollWorkFlowSettingItem WorkflowSettingChecker { get; set; }
        public PAYRollWorkFlowSettingItem WorkflowSettingClientAdmin { get; set; }
        public PAYRollWorkFlowSettingItem WorkflowSettingMyndMaker { get; set; }
        public PAYRollWorkFlowSettingItem WorkflowSettingMyndChcker { get; set; }
        public SelectList FileList { get; set; }

    }
    public enum PAYRoll_WFORDERING
    {
        MyndMaker = 1,
        MyndChcker = 2,
        Maker = 3,
        Checker = 4,
        ClientAdmin = 5,

    }
}