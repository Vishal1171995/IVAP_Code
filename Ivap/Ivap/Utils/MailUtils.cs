
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace Ivap.Utils
{
    public static class MailUtils
    {
        static string UserName;
        static string Password;
        static string Host;
        static string port;


        static MailUtils()
        {
            try
            {
                GetMailSetup();
            }
            catch 
            { throw; }
        }
        

        public static void  GetMailSetup()
        {
            try
            {
                DataTable dt = DataLib.ExecuteDataTable("GetMailSetup", CommandType.StoredProcedure,null);
                if (dt.Rows.Count > 0)
                {
                    UserName = dt.Rows[0]["UserName"].ToString();
                    Password = dt.Rows[0]["Password"].ToString();
                    port = dt.Rows[0]["Port"].ToString();
                    Host = dt.Rows[0]["Host"].ToString();

                }
            }
            catch { } 
        }


        public static string SendMail(string ToMail, string Subject, string MailBody, string CC = "", string Attachments = "", string BCC = "")
        {
            string ret = "fail";
            Array arrToArray = default(Array);
            Array arrCC = default(Array);
            Array arrBCC = default(Array);
            Array arrAtch = default(Array);
            char[] splitter = { ',' };
            //ToMail = "ajeet.kumar@myndsol.com";
            //CC = "amit.nehra@myndsol.com";
            //When multiple recepient seperated by ';'

            try
            {
                arrToArray = ToMail.Split(splitter);
                arrCC = CC.Split(splitter);
                arrBCC = BCC.Split(splitter);
                arrAtch = Attachments.Split(splitter);
                MailMessage message = new MailMessage();
                message.From = new MailAddress(UserName);
                message.Subject = Subject;
                message.Body = MailBody;
                message.IsBodyHtml = true;
                //message.SubjectEncoding = System.Text.Encoding.UTF8
                //message.BodyEncoding = System.Text.Encoding.UTF8
                //Adding To Mail 
                try
                {
                    foreach (string s in arrToArray)
                    {
                        if (!string.IsNullOrEmpty(s.Trim()))
                        {
                            message.To.Add(new MailAddress(s));
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                //Adding CC Mail 
                try
                {
                    foreach (string s in arrCC)
                    {
                        if (!string.IsNullOrEmpty(s.Trim()))
                        {
                            message.CC.Add(new MailAddress(s));
                        }
                    }

                }
                catch (Exception ex)
                {
                }

                //Adding BCC Mail 
                try
                {
                    foreach (string s in arrBCC)
                    {
                        if (!string.IsNullOrEmpty(s.Trim()))
                        {
                            message.Bcc.Add(new MailAddress(s));
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                //Adding BCC Mail 
                try
                {
                    foreach (string s in arrAtch)
                    {
                        if (!string.IsNullOrEmpty(s.Trim()))
                        {
                            message.Attachments.Add(new Attachment(s));
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                //Adding SMTP details
                NetworkCredential nCred = new NetworkCredential();
                nCred.UserName = UserName;
                nCred.Password = Password;

                SmtpClient client = new SmtpClient();
                client.Host = Host;
                //client.EnableSsl = true;
                client.Port = 25;
                client.UseDefaultCredentials = true;
                client.Credentials = nCred;
                client.Send(message);
                ret = "Mail Sent";
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message;
            }

            return ret;
        }
    }
}


