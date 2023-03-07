
using Ivap.Areas.Master.Repository;
using Ivap.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Areas.Master.Models
{
    public class UserModel:BaseModel 
    {
        public int UID { set; get; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[A-Za-z0-9.\\d@_.]{5,25}$", ErrorMessage = "Invalid UserID.")]
        public string USERID { set; get; }

        //[Required(ErrorMessage = "Please enter user name.")]
        //[StringLength(100, ErrorMessage = "User Name can be min 4 and  max 100 characters long.", MinimumLength = 4)]
        //public string UserName { set; get; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "Value can be min 4 and  max 100 characters long.", MinimumLength = 4)]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "Value can be min 4 and  max 100 characters long.", MinimumLength = 4)]
        public string LastName { set; get; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        //[Validation_User_Mobile_Email]
        public string Email { set; get; }
        [Required(ErrorMessage = "Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Role don't exist.")]
        public int Role { set; get; }
        public int TransactionID { set; get; }
        public string PassToken { set; get; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^(\\+?\\d{1,4}[\\s-])?(?!0+\\s+,?$)\\d{10}\\s*,?$", ErrorMessage ="Invalid Mobile No")]
        //[Validation_User_Mobile_Email]
        public string MobileNo { set; get; }
        public SelectList EntityList { get; set; }
        public SelectList RoleList
        {
            get;
            set;
        }

        public SelectList CircleList
        {
            get;
            set;
        }

        public string UserType { set; get; }
        public string ProfilePic { set; get; }

        ///

        public string UID_Text { set; get; }
        public string EID_TEXT { get; set; }
        public string USERID_Text { set; get; }

        public string FirstName_Text { set; get; }
        public string LastName_Text { set; get; }

        public string Email_Text { set; get; }
        public string Circle_Text { set; get; }
        public string Role_Text { set; get; }
        public string TransactionID_Text { set; get; }
        public string PassToken_Text { set; get; }
        public string MobileNo_Text { set; get; }
        public string UserType_Text { set; get; }
        public string ProfilePic_Text { set; get; }

        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_USER", "ViewUser");

            this.UID_Text = ObjMetaRepo.GetDisPlayName("TID");
            this.EID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.USERID_Text = ObjMetaRepo.GetDisPlayName("USERID");
            this.FirstName_Text = ObjMetaRepo.GetDisPlayName("USER_FIRSTNAME");
            this.LastName_Text = ObjMetaRepo.GetDisPlayName("USER_LASTNAME");
            this.Email_Text = ObjMetaRepo.GetDisPlayName("USER_EMAIL");
            this.Role_Text = ObjMetaRepo.GetDisPlayName("USER_ROLE");
            this.MobileNo_Text = ObjMetaRepo.GetDisPlayName("USER_MOBILENO");
           // this.PassToken_Text = ObjMetaRepo.GetDisPlayName("USER_ROLE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }


    public class UserModelVM : BaseModel
    {
        public int UID { set; get; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[A-Za-z0-9\\d@_.]{5,25}$", ErrorMessage = "Invalid UserID.")]
        public string UserID { set; get; }

        //[Required(ErrorMessage = "Please enter user name.")]
        //[StringLength(100, ErrorMessage = "User Name can be min 4 and  max 50 characters long.", MinimumLength = 4)]
        // public string UserName { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "Value can be min 4 and  max 100 characters long.", MinimumLength = 4)]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "Value Can be min 4 and  max 100 characters long.", MinimumLength = 4)]
        public string LastName { set; get; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { set; get; }

        //[Required(ErrorMessage = "Required")]
        public string Circle { set; get; }

        [Required(ErrorMessage = "Required")]
        public string Role { set; get; }

        [RegularExpression("^(\\+?\\d{1,4}[\\s-])?(?!0+\\s+,?$)\\d{10}\\s*,?$", ErrorMessage = "Invalid Mobile No")]
        public string MobileNo { set; get; }

        public string UID_Text { set; get; }
        public string EID_TEXT { get; set; }
        public string USERID_Text { set; get; }

        public string FirstName_Text { set; get; }
        public string LastName_Text { set; get; }

        public string Email_Text { set; get; }
        public string Circle_Text { set; get; }
        public string Role_Text { set; get; }
        public string TransactionID_Text { set; get; }
        public string PassToken_Text { set; get; }
        public string MobileNo_Text { set; get; }
        public string UserType_Text { set; get; }
        public string ProfilePic_Text { set; get; }

        public string ISACTIVE_TEXT { get; set; }
        public string Screen_Name { set; get; }
        public void SetDisplayName()
        {
            MasterMetaRepo ObjMetaRepo = new MasterMetaRepo(this.EID, "IVAP_MST_USER", "ViewUser");

            this.UID_Text = ObjMetaRepo.GetDisPlayName("TID");
            this.EID_TEXT = ObjMetaRepo.GetDisPlayName("ENTITY_ID");
            this.USERID_Text = ObjMetaRepo.GetDisPlayName("USERID");
            this.FirstName_Text = ObjMetaRepo.GetDisPlayName("USER_FIRSTNAME");
            this.LastName_Text = ObjMetaRepo.GetDisPlayName("USER_LASTNAME");
            this.Email_Text = ObjMetaRepo.GetDisPlayName("USER_EMAIL");
            this.Role_Text = ObjMetaRepo.GetDisPlayName("USER_ROLE");
            this.MobileNo_Text = ObjMetaRepo.GetDisPlayName("USER_MOBILENO");
            // this.PassToken_Text = ObjMetaRepo.GetDisPlayName("USER_ROLE");
            this.Screen_Name = ObjMetaRepo.Screen_Name;
        }

    }
    public class UserProfileVM : BaseModel
    {
        public int UID { set; get; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "First Name can be min 4 and  max 50 characters long.", MinimumLength = 3)]
        public string FirstName { set; get; }

        
        public string LastName { set; get; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { set; get; }


        [RegularExpression("^(\\+?\\d{1,4}[\\s-])?(?!0+\\s+,?$)\\d{10}\\s*,?$", ErrorMessage = "Invalid Mobile No")]
        public string MobileNo { set; get; }
        public string ProfilePic { set; get; }

    }
    public class GridUser
    {
        public int from { get; set; }

        public int Circle { get; set; }
        public int To { get; set; }
        public string SortingStr { get; set; }
        public string FilterStr { get; set; }

    }
}