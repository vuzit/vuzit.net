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
        #region Public static methods
        /// <summary>
        /// Loads a web response into a string.  
        /// </summary>
        public static string ReadHttpResponse(HttpWebResponse response)
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
        #endregion

        #region Protected static methods
        /// <summary>
        /// Changes an array (hash table) of parameters to a url.  
        /// </summary>
        protected static string ParametersToUrl(string resource, 
                                                OptionList parameters, 
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
        protected static OptionList PostParameters(OptionList options, string method, string id)
        {
            options.Add("method", method);
            options.Add("key", Service.PublicKey);
            DateTime date = DateTime.Now;
            string signature = Service.Signature(method, id, date);
            options.Add("signature", signature);
            options.Add("timestamp", Service.EpochTime(date).ToString());

            return options;
        }

        /// <summary>
        /// Uploads a file by HTTP POST.  Code adapted from this project:
        /// http://www.codeproject.com/KB/cs/uploadfileex.aspx
        /// </summary>
        /// <param name="stream">Stream of the document. </param>
        /// <param name="url">URL of the request. </param>
        /// <param name="fileName">Name of the file. </param>
        /// <param name="fileFormName">Form file name. </param>
        /// <param name="contentType">File content type. </param>
        /// <param name="cookies">Cookies for request. </param>
        protected static string UploadFile(Stream fileStream, string url, string fileName,
                                           string fileFormName, string contentType,
                                           CookieContainer cookies)
        {
            if (fileFormName == null)
            {
                fileFormName = "file";
            }

            if (contentType == null)
            {
                contentType = "application/octet-stream";
            }

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = WebRequestBuild(url);
            webrequest.UserAgent = Service.UserAgent;
            webrequest.CookieContainer = cookies;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";

            // If the stream is over 3 megabytes in size
            if (fileStream.Length > (3 * 1048576))
            {
                // Set the timeout to 5 minutes
                webrequest.Timeout = (5 * 60 * 1000);
            }

            // Build up the post message header
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            sb.Append(fileName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contentType);
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself
            // NOTE: This was hacked to fix a bug found by someone in the 
            //       comments because it broke Tomcat.  Evidently it breaks
            //       Rails as well.  
            byte[] boundaryBytes =
                   Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            long length = postHeaderBytes.Length + fileStream.Length +
                                                   boundaryBytes.Length;
            webrequest.ContentLength = length;

            Stream requestStream = webrequest.GetRequestStream();

            // Write out our post header
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

            // Write out the file contents
            byte[] buffer = new Byte[checked((uint)Math.Min(4096,
                                     (int)fileStream.Length))];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }

            // Write out the trailing boundary
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

            WebResponse response = null;
            try
            {
                response = webrequest.GetResponse();
            }
            catch (WebException ex)
            {
                throw new Vuzit.ClientException("HTTP response error", ex);
            }

            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string result = sr.ReadToEnd();

            // Close all resources
            requestStream.Close();
            s.Close();

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
        #endregion
    }
}
