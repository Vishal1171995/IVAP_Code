using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ivap.Utils
{
    public static  class JsonSerializer
    {
        public static string SerializeTable(DataTable dt)
        {
            try
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                string jsonData = "";
                serializerSettings.Converters.Add(new DataTableConverter());
                serializerSettings.MaxDepth = Int32.MaxValue;
                jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings);
                return jsonData;
            }
            catch
            {
                throw;
            }
        }

        public static string SerializeObject(Response Obj)
        {
            try
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                string jsonData = "";
                //serializerSettings.Converters.Add(typeof(Response));
                jsonData = JsonConvert.SerializeObject(Obj, Newtonsoft.Json.Formatting.None, serializerSettings);
                return jsonData;
            }
            catch
            {
                throw;
            }
        }

        public static string SerializeObject(Object Obj)
        {
            try
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                string jsonData = "";
                //serializerSettings.Converters.Add(typeof(Response));
                jsonData = JsonConvert.SerializeObject(Obj, Newtonsoft.Json.Formatting.None, serializerSettings);
                return jsonData;
            }
            catch
            {
                throw;
            }
        }

    }
}