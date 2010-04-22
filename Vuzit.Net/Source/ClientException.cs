using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace Vuzit
{
    /// <summary>
    /// Vuzit library exception handler class.  
    /// </summary>
    public class ClientException : System.Exception
    {
        #region Public properties
        /// <summary>
        /// The error message.  
        /// </summary>
        public override string Message
        {
            get { return message; }
        }

        /// <summary>
        /// The Vuzit error code.  
        /// </summary>
        public int Code
        {
            get { return code; }
        }
        #endregion

        #region Private variables
        string message = null;
        int code = -1;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new exception from a WebException instance.  
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex">The WebException returned. </param>
        public ClientException(string message, WebException ex)
        {
            HttpWebResponse response = (HttpWebResponse)ex.Response;

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(Base.ReadHttpResponse(response));

                XmlNode node = doc.SelectSingleNode("/err/code");
                if (node != null)
                {
                    string code = doc.SelectSingleNode("/err/code").InnerText;
                    string msg = doc.SelectSingleNode("/err/msg").InnerText;

                    this.code = Convert.ToInt32(code);
                    this.message = msg;
                }
            }
            catch
            {
                // There was invalid XML so default to the base error
                Stream errorStream = ex.Response.GetResponseStream();
                StreamReader errorReader = new StreamReader(errorStream);
                string responseText = errorReader.ReadToEnd();
                errorStream.Close();

                this.message = responseText;
                this.code = 0;
            }
        }

        /// <summary>
        /// Creates a new exception instance.  
        /// </summary>
        /// <param name="message">Error message. </param>
        public ClientException(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// Creates a new exception instance.  
        /// </summary>
        /// <param name="message">Error message. </param>
        /// <param name="code">Error code. </param>
        public ClientException(string message, int code)
        {
            this.message = message;
            this.code = code;
        }

        /// <summary>
        /// Creates a new exception instance.  
        /// </summary>
        /// <param name="message">Error message. </param>
        /// <param name="code">Error code. </param>
        public ClientException(string message, string code)
        {
            this.message = message;
            this.code = Convert.ToInt32(code);
        }
        #endregion
    }
}
