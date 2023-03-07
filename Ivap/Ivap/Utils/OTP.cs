using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Utils
{
    public class OTPUtils
    {
        [RegularExpression(@"^[0-9]{6,6}$", ErrorMessage = "Invalid OTP")]
        [Required(ErrorMessage = "OTP Required.")]
        public int OTP { set; get; }

        public string  HashedOTP { set; get; }

        [Required]

        public string TRANSACTION_ITEM { set; get; }

        [Required]
        public string TRANSACTION_ID { set; get; }
        [Required]
        public string CREATED_FOR { set; get; }

        [Required]
        [RegularExpression(@"^([0]|\+91[\-\s]?)?[789]\d{9}$", ErrorMessage = "Entered Mobile No is not valid.")]
        public string MobileNo { set; get; }

        public string Email { set; get; }

        public int  ConsumeOTP { set; get; }

        public Response GenerateOTP()
        {
            try
            {
                Response Res = new Response();
                //Generating Random number of six degits
                OTP = GenerateRandomOTP(6);
                 HashedOTP = HashingLib.GenerateSHA512String(OTP.ToString());
                string MessageToBeSent = ("You have initiated " + CREATED_FOR + " at Idea LAS. OTP IS " + OTP + ". DON'T SHARE IT WITH ANY ONE.").ToUpper();
                string MessageToBeSaved = ("You have initiated " + CREATED_FOR + " at Idea LAS. OTP IS " + HashedOTP + ". DON'T SHARE IT WITH ANY ONE.").ToUpper();

                //Now Sending OTP TO mobile number
                var results = new List<ValidationResult>();
                var vc = new ValidationContext(this, null, null);
                bool isValid = Validator.TryValidateObject(this, vc, results, true);
                var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                if (isValid == false)
                {
                    Res.IsSuccess = false;
                    Res.Message = errors[0];
                    return Res;
                }
                //Save Data Into Our DataBase

                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("P_OTP",OracleDbType.Varchar2, HashedOTP,ParameterDirection.Input),
                new SqlParameter("P_CREATEDFOR_TRANSACTION_ITEM",OracleDbType.Varchar2,TRANSACTION_ITEM,ParameterDirection.Input),
                new SqlParameter("P_CREATEDFOR_TRANSACTION_ID",OracleDbType.Varchar2,TRANSACTION_ID,ParameterDirection.Input),
                new SqlParameter("P_CREATED_FOR",OracleDbType.Varchar2,CREATED_FOR,ParameterDirection.Input),
                new SqlParameter("P_OTPMessage",OracleDbType.Varchar2,MessageToBeSaved,ParameterDirection.Input),
                new SqlParameter("P_Email",OracleDbType.Varchar2,Email,ParameterDirection.Input),
                new SqlParameter("P_MobileNumber",OracleDbType.Varchar2,MobileNo,ParameterDirection.Input),
                new SqlParameter("Result",OracleDbType.Int32,ParameterDirection.Output),
                };
                int result = Convert.ToInt32(OracleDataLib.ExecuteScaler("GenerateOTP", CommandType.StoredProcedure, parameters));
                //Now Send Messgae To Mobile Number
                //string Baseurl = "http://121.241.247.144:7501/failsafe/HttpLink?aid=633208&pin=intech@1" + "&mnumber=" + MobileNo + "&message=" + MessageToBeSent;
                //var client = new RestClient(Baseurl);
                //var request = new RestRequest(Method.GET);
                //IRestResponse response = client.Execute(request);
                //var content = response.Content; // raw content as string
                SMSUtils ObjSMS = new SMSUtils(MobileNo, MessageToBeSent);
                ObjSMS.SendSMS();
                //Send Email To Distributor 

                if (Email.Trim() != "")
                {
                    string EmailBody = "";
                    string EmailSubject = "";
                    EmailSubject = "OTP for " + CREATED_FOR;
                    //MailUtils.SendMail(Email, EmailSubject, EmailBody);
                }
                //Mask Mobile number
                string MaskedMobileNumber = CommanUtills.MaskString(MobileNo, 2, 4);
                Res.IsSuccess = true;
                Res.Message = "OTP has been sent on your registered mobile number " + MaskedMobileNumber +".";
                return Res;
            }
            catch
            {
                throw;
            }
        }


        public Response ValidateOTP()
        {
            try
            {
                Response Res = new Response();

                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("P_OTP", HashedOTP,ParameterDirection.Input),
                new SqlParameter("P_USEDFOR_TRANSACTION_ITEM",OracleDbType.Varchar2,TRANSACTION_ITEM,ParameterDirection.Input),
                new SqlParameter("P_USEDFOR_TRANSACTION_ID",OracleDbType.Varchar2,TRANSACTION_ID,ParameterDirection.Input),
                new SqlParameter("P_CREATED_FOR",OracleDbType.Varchar2,CREATED_FOR,ParameterDirection.Input),
                new SqlParameter("P_Consume",OracleDbType.Int32,ConsumeOTP,ParameterDirection.Input),
                new SqlParameter("Result",OracleDbType.Int32,ParameterDirection.Output),
                };
                int result = Convert.ToInt32(OracleDataLib.ExecuteScaler("ValidateOTP", CommandType.StoredProcedure, parameters));
                if(result==1)
                {
                    Res.IsSuccess = true;
                    Res.Message = "OTP Validated successfully.";
                }
                else
                {
                    Res.IsSuccess = false;
                    Res.Message = "Invalid OTP.";
                }
                
                return Res;
            }
            catch
            {
                throw;
            }
        }


        private int GenerateRandomOTP(int iOTPLength)
        {
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };  

            string sOTP = String.Empty;
            string sTempChars = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < iOTPLength; i++)
            {
                int p = rand.Next(0, saAllowedCharacters.Length);
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return Convert.ToInt32(sOTP);

        }  
    }
}