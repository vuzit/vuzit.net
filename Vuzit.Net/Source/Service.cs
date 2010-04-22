using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Vuzit
{
    /// <summary>
    /// Global Vuzit service class.  
    /// </summary>
    public sealed class Service
    {
        #region Private static properties
        static string serviceUrl = "http://vuzit.com";
        static string publicKey = null;
        static string privateKey = null;
        const string productName = "Vuzit.Net Library 2.1.0";
        static string userAgent = productName;
        #endregion

        #region Public static properties
        /// <summary>
        /// The Vuzit public API key.
        /// </summary>
        /// <example>
        /// <code>
        /// Vuzit.Service.PublicKey = 'YOUR_PUBLIC_API_KEY';
        /// </code>
        /// </example>
        public static string PublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }

        /// <summary>
        /// The Vuzit private API key.  Do NOT share this with anyone!
        /// </summary>
        /// <example>
        /// <code>
        /// Vuzit.Service.PrivateKey = 'YOUR_PRIVATE_API_KEY';
        /// </code>
        /// </example>
        public static string PrivateKey
        {
            get { return privateKey; }
            set { privateKey = value; }
        }

        /// <summary>
        /// The URL of the Vuzit web service.  This only needs to be changed 
        /// if you are running Vuzit Enterprise on your own server.  
        /// The default value is "http://vuzit.com".
        /// </summary>
        /// <example>
        /// <code>
        /// Vuzit.Service.ServiceUrl = "http://vuzit.yourdomain.com";
        /// </code>
        /// </example>
        public static string ServiceUrl
        {
            get { return serviceUrl; }
            set
            {
                string url = value.Trim();
                if (url.EndsWith("/"))
                {
                    throw new ArgumentException("Trailing slashes (/) in service URLs are invalid");
                }
                serviceUrl = url;
            }
        }

        /// <summary>
        /// User agent for the library.  
        /// </summary>
        public static string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value + " (" + productName + ")"; }
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Returns a date/time object as the seconds since the Epoch.  
        /// </summary>
        public static int EpochTime(DateTime date)
        {
            // Account for users being in different time zones.  
            DateTime universalDate = date.ToUniversalTime();

            return (int)(universalDate - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Returns the signature string.  
        /// </summary>
        /// <param name="service">Service name (e.g. 'show').</param>
        /// <param name="id">ID of the document.</param>
        /// <param name="date">Date of the request</param>
        public static string Signature(string service, string id, 
                                       DateTime date, OptionList options)
        {
            StringBuilder result = new StringBuilder();

            if (PublicKey == null)
            {
                throw new ArgumentException("The PublicKey parameter cannot be null");
            }

            if (PrivateKey == null)
            {
                throw new ArgumentException("The PrivateKey parameter cannot be null");
            }

            if (date == DateTime.MinValue)
            {
                date = DateTime.Now;
            }

            if (id == null)
            {
                id = String.Empty;
            }

            result.Append(service).Append(id).Append(PublicKey)
                  .Append(EpochTime(date).ToString());

            if (options != null)
            {
                string[] optionsList = new string[] { "included_pages", 
                                                      "watermark", 
                                                      "query" };
                foreach (string item in optionsList)
                {
                    if (options.Contains(item))
                    {
                        result.Append(options[item]);
                    }
                }
            }

            return CalculateRFC2104HMAC(result.ToString(), PrivateKey);
        }

        /// <summary>
        /// Returns the signature string.  
        /// </summary>
        /// <param name="service">Name of the service: 'show', 'create', or 'destroy'.</param>
        /// <param name="date">Date of the request</param>
        public static string Signature(string service, DateTime date)
        {
            return Signature(service, null, date, null);
        }

        /// <summary>
        /// Returns the signature string.  
        /// </summary>
        /// <param name="service">Name of the service: 'show', 'create', or 'destroy'.</param>
        /// <param name="id">ID of the document.</param>
        public static string Signature(string service, string id)
        {
            return Signature(service, id, DateTime.MinValue, null);
        }

        /// <summary>
        /// Properly encodes a URL to pass to the Vuzit service.  
        /// </summary>
        /// <param name="text">URL text. </param>
        public static string UrlEncode(string text)
        {
            return System.Web.HttpUtility.UrlEncode(System.Web.HttpUtility.UrlPathEncode(text));
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Computes RFC 2104-compliant HMAC signature.
        /// </summary>
        /// <param name="data">The data to be signed.</param>
        /// <param name="key">The signing key.</param>
        private static string CalculateRFC2104HMAC(string text, string key)
        {
            byte[] bData = Encoding.UTF8.GetBytes(text);
            byte[] bKey = Encoding.UTF8.GetBytes(key);

            HMACSHA1 hmac = new HMACSHA1(bKey);
            using (CryptoStream cryptoStream =
                      new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write))
            {
                cryptoStream.Write(bData, 0, bData.Length);
            }
            return Convert.ToBase64String(hmac.Hash);
        }
        #endregion
    }
}
