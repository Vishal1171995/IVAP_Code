using Ivap.Models;

namespace Ivap.Areas.MOM.Models
{
    public class BaseMinutesDetails : BaseModel
    {
        public int MID { get; set; }

        public int? ITEM_ID { get; set; }
        public string MINUTES { get; set; }
        public string RESPONSIBILITY { get; set; }
        public string E_C_D { get; set; }

        public string A_C_D { get; set; }

        public string MINUTES_STATUS { get; set; }

        public string closedRemarks { get; set; }

        public string systemFileName { get; set; }

        public string originalFileName { get; set; }


        //public bool MoveFile(int MOMID, string fileName)
        //{
        //    throw new NotImplementedException();
        //}
    }
}