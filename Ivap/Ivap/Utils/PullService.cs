using Ivap.Areas.Secondary_Transportation.Repository;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Ivap.Utils
{
    public class PullService
    {

        public static void Start()
        {
            DataTable dt = new DataTable();
            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("dtMobile",OracleDbType.RefCursor,ParameterDirection.Output),
                };
                dt = OracleDataLib.ExecuteDataTable("GETMOBILENUMBER", CommandType.StoredProcedure, parameters);
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    PullDataFromApi(dt.Rows[0]["MOBILE_NUMBER"].ToString());
                }
            }
            catch
            {
                throw;
            }
        }



        public static string PullDataFromApi(string MobileNumber)
        {

            string soapResult="";
            //SMSUtils ObjSMS = new SMSUtils("9278646536", "Schedulet is Running...");
            //ObjSMS.SendSMS();
            try
            {
                var _url = "https://172.30.192.161/lmwservice/service.asmx";

                //172.30.191.161

                XmlDocument soapEnvelopeXml = CreateSoapEnvelope(MobileNumber);
                HttpWebRequest webRequest = CreateWebRequest(_url);
                //webRequest.Proxy =  new WebProxy("172.30.196.214", 8002);
                try
                {
                    InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                    // begin async call to web request.

                    IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                    // suspend this thread until call is complete. You might want to
                    // do something usefull here like update your UI.
                    asyncResult.AsyncWaitHandle.WaitOne();

                    // get the response from the completed web request.

                    using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                    {
                        using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                        {
                            soapResult = rd.ReadToEnd();
                        }
                    }
                }
                catch
                { }
                //soapResult = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><GetLocationMultipleResult xmlns=\"http://LMWService/\"><LocationData><Msisdn /><Lat /><Lon /><Datetime>2018-05-21T12:48:19.389+05:30</Datetime><Errcode>0201</Errcode><Errdesc>MSISDN not Active in System for Tracking</Errdesc></LocationData><LocationData><Msisdn>9650146854</Msisdn><Lat>28.49959</Lat><Lon>77.07944</Lon><Datetime>2018-05-21T12:44:02.261+05:30</Datetime><Errcode /><Errdesc /></LocationData></GetLocationMultipleResult></soap:Body></soap:Envelope>";

                XElement xmlTree = XElement.Parse(soapResult);
                //IEnumerable<XElement> Element = xmlTree.Element("soap/GetLocationMultipleResult");http://LMWService
                IEnumerable<XElement> LocationMultipleResult = xmlTree.Descendants(XNamespace.Get("http://LMWService/") + "LocationData");

                var XmlResult = "";
                string MobileNo = "";
                string Lat = "";
                string Long = "";
                string Address = "";
                string Stime = "";
                //<Msisdn>9278656536</Msisdn><Lat>1</Lat><Lon>2</Lon><Datetime>Date1</Datetime>
                foreach(var Item in LocationMultipleResult)
                {
                    try
                    {
                        var LSTMSIDN = Item.Descendants(XNamespace.Get("http://LMWService/") + "Msisdn").ToList();
                        MobileNo = LSTMSIDN[0].Value;
                        var LSTLAT = Item.Descendants(XNamespace.Get("http://LMWService/") + "Lat").ToList();
                        Lat = LSTLAT[0].Value;

                        var LSTLON = Item.Descendants(XNamespace.Get("http://LMWService/") + "Lon").ToList();
                        Long = LSTLON[0].Value;

                        var LSTDateTime = Item.Descendants(XNamespace.Get("http://LMWService/") + "Datetime").ToList();
                        Stime = LSTDateTime[0].Value;
                        if (Lat != "" && Long != "" && MobileNo != "" && Stime != "")
                        {
                            GPSDataRepo GPSRepo = new GPSDataRepo();
                            Response Res = new Response();
                            Stime = Stime.Replace("T", " ");
                            Stime = Stime.Replace("+5:30", string.Empty).Trim().TrimEnd('0');
                            //string[] arrdatetime = datetime.Split(' ');
                            DateTime dtNew = Convert.ToDateTime(Stime);
                            Res = GPSRepo.AddGPSDataItemPOC(MobileNo, Lat, Long, dtNew, Address);
                        }
                    }
                    catch
                    { }
                    
                }
                return soapResult;
            }
            catch
            {
                return soapResult;
            }
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            webRequest.KeepAlive = false;

            //string UserName = "ideawh";
            //string Password = "idea@wh";
            //string Credential = UserName + ":" + Password;
            //string Base64 = Base64Encode(Credential);
            //String authInfo = "Basic " + Base64;
            //webRequest.Headers.Add("Authorization", authInfo);

            //webRequest.ContentLength = 870;
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(string MobileNumber)
        {
            XmlDocument soapEnvelop = new XmlDocument();
            string XML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Header><UserCredentials xmlns=\"http://LMWService/\"><userName>Mynd</userName><password>mynd98#</password></UserCredentials></soap:Header><soap:Body><LocMultLocation xmlns=\"http://LMWService/\"><MSISDNRequest><Msisdn>"+MobileNumber+"</Msisdn></MSISDNRequest></LocMultLocation></soap:Body></soap:Envelope>";
            soapEnvelop.LoadXml(@XML);
            return soapEnvelop;
        }


        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}