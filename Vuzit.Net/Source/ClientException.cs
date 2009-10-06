using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

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
            string response = null;
            Stream errorStream = ex.Response.GetResponseStream();
            StreamReader errorReader = new StreamReader(errorStream);
            response = errorReader.ReadToEnd();
            errorStream.Close();

            // TODO: Have it parse out the error code and message
            //       from the "/err/code" and "/err/msg" nodes

            this.message = message + ", Response: " + response;
            this.code = 0;
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
