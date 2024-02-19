using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data.SqlClient;
using System.Data;

namespace IntegrationWinService
{
    class GeneralUtils
    {
        public static string SendRequestPostNew(string sUrl, out string sErrormessage,string jsonData)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;

            //uri = new System.Uri(sUrl);
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidation;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(sUrl);

            //-------
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.AllowWriteStreamBuffering = true;

            httpWebRequest.Timeout = 60000;
            httpWebRequest.ReadWriteTimeout = 60000;

            byte[] byte1 = System.Text.Encoding.UTF8.GetBytes(jsonData);
            httpWebRequest.ContentLength = byte1.Length;
            httpWebRequest.Accept = "application/json";

            HttpWebResponse httpResponse;

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                System.IO.Stream responseStream = responseStream = httpResponse.GetResponseStream();

                if (httpResponse.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                else if (httpResponse.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                StreamReader Reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                

                httpResponse.Close();
                responseStream.Close();
                sErrormessage = "";
                return httpResponse.StatusCode.ToString();

            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    //HttpWebResponse httpResponse1 = (HttpWebResponse)response;
                    if (response != null)
                    {
                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data, System.Text.Encoding.UTF8))
                        {
                            
                        }
                        sErrormessage = e.Message;
                        return e.Status.ToString();
                    }

                    sErrormessage = e.Message;
                    return "Response was null inside SendRequestPostNew catch";
                }
            }
        }

        private static bool ServerCertificateValidation(object sender,
          X509Certificate certificate, X509Chain chain,
          SslPolicyErrors sslPolicyErrors)
        {
            HttpWebRequest request = sender as HttpWebRequest;

            if (request != null && request.Address.Host.Equals(
                request.Address.Host, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        public static string CreateJsonString(Object obj)
        {
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-ddTH:mm:ss.fffK",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new CamelCasePropertyNamesContractResolver()

            });

            var jsonObj = JsonConvert.DeserializeObject(jsonData);
            return jsonObj.ToString();
        }
        public static DataTable GetDataTable(string sql, string dtsource, string catalog, out string error)
        {
            try
            {
                error = "";
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = dtsource;
                builder.InitialCatalog = catalog;
                builder.IntegratedSecurity = true;
                builder.ConnectTimeout = 0;

                using (SqlConnection con = new SqlConnection(builder.ConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                return dt;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                error = "Error in GetDataTable. Exception message: " + exp.Message;
                return null;
            }
        }
    }
}
