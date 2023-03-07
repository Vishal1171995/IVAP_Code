using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Models
{
    public class AppUser
    {
        public int UID { private set; get; }

        public int EID { private set; get; }
        public string UserID { private set; get; }

        public string FirstName { private set; get; }

        public string LastName { private set; get; }

        public string Email { private set; get; }


        public int Role { private set; get; }

        public string RoleName { private set; get; }

        public int PassChangeDays { private set; get; }

        public string MobileNo { private set; get; }


        public string ProfilePic { private set; get; }

        public DateTime Lastlogintime { private set; get; }

        public DateTime PayDate { private set; get; }

        public AppUser SetAppUser(DataSet dsU)
        {
            AppUser objU = new AppUser();
            objU.UID = Convert.ToInt32(dsU.Tables[0].Rows[0]["UID"]);
            objU.EID = Convert.ToInt32(dsU.Tables[0].Rows[0]["Entity_ID"]);
            objU.Role = Convert.ToInt32(dsU.Tables[0].Rows[0]["User_Role"]);
            objU.UserID = Convert.ToString(dsU.Tables[0].Rows[0]["UserID"]);
            objU.Email = Convert.ToString(dsU.Tables[0].Rows[0]["User_Email"]);
            objU.FirstName = Convert.ToString(dsU.Tables[0].Rows[0]["User_FirstName"]);
            objU.LastName = Convert.ToString(dsU.Tables[0].Rows[0]["User_LastName"]);
            objU.RoleName = Convert.ToString(dsU.Tables[0].Rows[0]["RoleName"]);
            int PassChangeBefore = Convert.ToInt32(dsU.Tables[0].Rows[0]["PassChangeDayCount"]);
            objU.PassChangeDays = 60 - PassChangeBefore;
            objU.ProfilePic = Convert.ToString(dsU.Tables[0].Rows[0]["PROFILEPIC"]);
            objU.MobileNo = Convert.ToString(dsU.Tables[0].Rows[0]["USER_MOBILENO"]);
            if (dsU.Tables[1].Rows.Count > 0)
            {
                int Month = Convert.ToInt32(dsU.Tables[1].Rows[0]["Month"].ToString());
                int Year = Convert.ToInt32(dsU.Tables[1].Rows[0]["Year"].ToString());
                //int Month = 11;
                //int Year = 2018;
                var LastDays = DateTime.DaysInMonth(Year, Month);

                DateTime CurrentMonth = new DateTime(Year, Month, LastDays);
                objU.PayDate = CurrentMonth;
            }
            else
            {
                objU.PayDate = DateTime.Today.AddDays(0 - DateTime.Today.AddDays(-30).Day);
            }
            //objU.Lastlogintime = Convert.ToDateTime(dt.Rows[0]["LastLogin"]);
            //objU.PayDate = PayDate.AddDays(0 - DateTime.Today.AddDays(-30).Day);
            return objU;
        }

        public AppUser SetProfilePic(AppUser ObjProfile,string ProfileImageBase64)
        {
            AppUser objU = new AppUser();
            objU = ObjProfile;
            objU.ProfilePic = ProfileImageBase64;
            return objU;
        }
    }


}