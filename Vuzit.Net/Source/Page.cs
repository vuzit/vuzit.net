using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Xml.XPath;

namespace Vuzit
{
    /// <summary>
    /// Class for load analytic page data.
    /// </summary>
    public sealed class Page : Vuzit.Base
    {
        #region Private static variables
        private int pageNumber = -1;
        private string pageText = null;
        #endregion

        #region Public properties
        /// <summary>
        /// The page number.  
        /// </summary>
        public int Number
        {
            get { return pageNumber; }
        }

        /// <summary>
        /// The page text.  
        /// </summary>
        public string Text
        {
            get { return pageText; }
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Returns all pages.  
        /// </summary>
        public static Vuzit.Page[] FindAll(string webId)
        {
            return FindAll(webId, new OptionList());
        }

        /// <summary>
        /// Finds all pages matching the given criteria.  
        /// </summary>
        public static Vuzit.Page[] FindAll(string webId, OptionList options)
        {
            if (webId == null)
            {
                throw new Vuzit.ClientException("webId cannot be null");
            }

            List<Vuzit.Page> result = new List<Vuzit.Page>();

            OptionList parameters = PostParameters(options, "index", webId);

            string url = ParametersToUrl("documents/" + webId + "/pages.xml", parameters);
            HttpWebRequest request = WebRequestBuild(url);
            request.UserAgent = Service.UserAgent;
            request.Method = "GET";

            // Catch 403, etc forbidden errors
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response == null)
                    {
                        throw new Vuzit.ClientException("No XML response from server", 0);
                    }

                    XmlDocument doc = new XmlDocument();
                    try
                    {
                        doc.LoadXml(ReadHttpResponse(response));
                    }
                    catch (ClientException ex)
                    {
                        throw new Vuzit.ClientException("Incorrect XML response: " + ex.Message, 0);
                    }

                    XmlNode node = doc.SelectSingleNode("/err/code");
                    if (node != null)
                    {
                        string msg = doc.SelectSingleNode("/err/msg").InnerText;
                        throw new Vuzit.ClientException("Web service error: " + msg, node.InnerText);
                    }

                    XmlNodeList list = doc.SelectNodes("/pages/page");

                    foreach (XmlNode childNode in list)
                    {
                        result.Add(NodeToPage(childNode));
                    }
                }
            }
            catch (Vuzit.ClientException ex)
            {
                throw ex;  // Rethrow because I want to see this exception
            }
            catch (WebException ex)
            {
                throw new Vuzit.ClientException("HTTP response error", ex);
            }

            return result.ToArray();
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Converts an XML node to a Vuzit Page.  
        /// </summary>
        private static Vuzit.Page NodeToPage(XmlNode rootNode)
        {
            Vuzit.Page result = new Vuzit.Page();
            XmlNode node = rootNode.SelectSingleNode("text");

            if (node == null)
            {
                throw new Vuzit.ClientException("No page text in response", 0);
            }
            result.pageNumber = NodeValueInt(rootNode, "number");
            result.pageText = NodeValue(rootNode, "text");

            return result;
        }
        #endregion
    }
}
