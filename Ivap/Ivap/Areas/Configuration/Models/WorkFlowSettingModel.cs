using Ivap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Configuration.Models
{
    public class PMSWorkFlowSettingItem 
    {
        public int USER_ROLE { get; set; }
        public bool ISCheck { get; set; }
        public WFORDERING ORDERING { get; set; }
        public string CompleteCssClass { set; get; }
        public int UserCount { get; set; }

        public string Icon_Name { set; get; }
 

    }

    public class WFSettingModel : BaseModel
    {
        public int FILE_ID { get; set; }
        public  PMSWorkFlowSettingItem WorkflowSettingMaker { get; set; }
        public PMSWorkFlowSettingItem WorkflowSettingChecker { get; set; }
        public PMSWorkFlowSettingItem WorkflowSettingClientAdmin { get; set; }
        public PMSWorkFlowSettingItem WorkflowSettingMyndMaker { get; set; }
        public PMSWorkFlowSettingItem WorkflowSettingMyndChcker { get; set; }
        public SelectList FileList { get; set; }

    }
    public enum WFORDERING
    {
        Maker = 1,
        Checker = 2,
        ClientAdmin = 3,
        MyndMaker = 4,
        MyndChcker = 5

    }
}