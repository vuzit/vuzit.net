using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Vuzit
{
    /// <summary>
    /// Base web client class.  
    /// </summary>
    public abstract class Base
    {
        /// <summary>
        /// Changes an array (hash table) of parameters to a url.  
        /// </summary>
        protected static string ParametersToUrl(string resource, 
                                                Dictionary<string, string> parameters, 
                                                string webId)
        {
            StringBuilder result = new StringBuilder();

            result.Append(Service.ServiceUrl).Append("/").Append(resource);
            if (webId != null)
            {
                result.Append("/").Append(webId);
            }
            result.Append(".xml?");

            foreach (string key in parameters.Keys)
            {
                string valueText = parameters[key];
                // Do not add keys with null or empty values
                if (valueText != null && valueText.Length > 0)
                {
                    result.Append(key).Append("=");
                    result.Append(Service.UrlEncode(valueText));
                    result.Append("&");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns the default HTTP post parameters array.  
        /// </summary>
        protected static Dictionary<string, string> PostParameters(string method, string id)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add("method", method);
            result.Add("key", Service.PublicKey);
            DateTime date = DateTime.Now;
            string signature = Service.Signature(method, id, date);
            result.Add("signature", signature);
            result.Add("timestamp", Service.EpochTime(date).ToString());

            return result;
        }

        /// <summary>
        /// Loads a web response into a string.  
        /// </summary>
        protected static string ReadHttpResponse(HttpWebResponse response)
        {
            string result = String.Empty;

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a web request.  
        /// </summary>
        protected static HttpWebRequest WebRequestBuild(string url)
        {
            HttpWebRequest result = null;

            result = (HttpWebRequest)System.Net.WebRequest.Create(url);

            if (url.StartsWith("https://"))
            {
                // Always validate the server
                ServicePointManager.ServerCertificateValidationCallback +=
                            delegate(object sender, X509Certificate certificate,
                                                    X509Chain chain,
                                                    SslPolicyErrors sslPolicyErrors)
                            {
                                return true; // Always accept
                            };
            }

            return result;
        }
    }
}
