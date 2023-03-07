using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ivap.Areas.Master.Models;
using System.Dynamic;
using Ivap.Utils;
using Ivap.Areas.Secondary_Transportation.Models;
using Ivap.Areas.Secondary_Transportation.Repository;
using RestSharp;
using System.Xml;
using System.IO;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace Ivap.Utils
{
    public class SMSUtils
    {

        [Required(ErrorMessage = "Please provide Mobile No.")]
        [RegularExpression("",ErrorMessage ="Invalid Mobile No.")]
        public  static string MobileNo { get; set; }

        [Required(ErrorMessage = "Please provide Content.")]
        public static string MessageBody { get; set; }


        public SMSUtils(string MobiNo, string MesgBody)
        {
            MobileNo = MobiNo;
            MessageBody = MesgBody;

        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string  SendSMS()
        {
            var _url = "https://172.30.80.16:63002/";

            // creating the xml document containing the receiver detail and body of the message.
            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(@"<?xml version=""1.0"" encoding=""UTF-8""?><message><sms type=""mt""><destination><address><number type=""international"">91" + MobileNo + @"</number></address></destination><source><address><number type=""abbreviated"">51515</number></address></source><pid>0</pid><rsr type=""success_failure""/><ud type=""text"" encoding=""default"">" + MessageBody + @"</ud><param name=""unique_id"" value=""71110071413371900520""/><param name=""developer_content_id"" value=""IBM""/><param name=""mo_keyword"" value=""text""/><param name=""content_description"" value=""itcms""/><param name=""content_type"" value=""text""/></sms></message>");

            // creating a web Request

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
            webRequest.ContentType = "text/xml;";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.KeepAlive = false;
                  //credentials for web request
            string UserName = "ideawh";
            string Password = "idea@wh";
            string Credential = UserName + ":" + Password;
            string Base64 = Base64Encode(Credential);
            String authInfo = "Basic " + Base64;
            webRequest.Headers.Add("Authorization", authInfo);
           
            
           // Inserting Soap envelop into Web request
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelop.Save(stream);
            }

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                //Console.Write(soapResult);
            }

            return soapResult;

        }

    }

   

}