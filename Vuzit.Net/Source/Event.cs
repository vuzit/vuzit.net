using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Xml.XPath;

namespace Vuzit
{
    /// <summary>
    /// Class for load analytic event data.
    /// </summary>
    public sealed class Event : Vuzit.Base
    {
        #region Private static variables
        private string id = null;
        private string eventType = null;
        private string remoteHost = null;
        private string referer = null;
        private string userAgent = null;
        private string custom = null;
        private DateTime requestedAt = DateTime.MinValue;
        private int page = -1;
        private int duration = -1;
        #endregion

        #region Public properties
        /// <summary>
        /// The document ID.  
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// The type of event.  
        /// </summary>
        public string EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// The IP address of the user that loaded the page.
        /// </summary>
        public string RemoteHost
        {
            get { return remoteHost; }
        }

        /// <summary>
        /// The referring URL that loaded the document. 
        /// </summary>
        public string Referer
        {
            get { return referer; }
        }

        /// <summary>
        /// The user agent (browser) that viewed hte page.  
        /// </summary>
        public string UserAgent
        {
            get { return userAgent; }
        }

        /// <summary>
        /// The custom string associated with the document.
        /// </summary>
        public string Custom
        {
            get { return custom; }
        }

        /// <summary>
        /// The time of the event.
        /// </summary>
        public DateTime RequestedAt
        {
            get { return requestedAt; }
        }

        /// <summary>
        /// The page number being viewed.  
        /// </summary>
        public int Page
        {
            get { return page; }
        }

        /// <summary>
        /// The duration that a page was viewed.  
        /// </summary>
        public int Duration
        {
            get { return duration; }
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Returns all events.  
        /// </summary>
        public static Vuzit.Event[] FindAll(string webId)
        {
            return FindAll(webId, new OptionList());
        }

        /// <summary>
        /// Finds all documents matching the given criteria.  
        /// </summary>
        public static Vuzit.Event[] FindAll(string webId, OptionList options)
        {
            if (webId == null)
            {
                throw new Vuzit.ClientException("webId cannot be null");
            }

            List<Vuzit.Event> result = new List<Vuzit.Event>();

            OptionList parameters = PostParameters(options, "index", null);
            parameters.Add("web_id", webId);

            string url = ParametersToUrl("events.xml", parameters);
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

                    XmlNodeList list = doc.SelectNodes("/events/event");

                    foreach (XmlNode childNode in list)
                    {
                        result.Add(NodeToEvent(childNode));
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
        /// Converts an XML node to a Vuzit event.  
        /// </summary>
        private static Vuzit.Event NodeToEvent(XmlNode rootNode)
        {
            Vuzit.Event result = new Vuzit.Event();
            XmlNode node = rootNode.SelectSingleNode("web_id");

            if (node == null)
            {
                throw new Vuzit.ClientException("No web_id in response", 0);
            }
            result.id = node.InnerText;
            result.eventType = NodeValue(rootNode, "event");
            result.remoteHost = NodeValue(rootNode, "remote_host");
            result.referer = NodeValue(rootNode, "referer");
            result.userAgent = NodeValue(rootNode, "user_agent");
            result.custom = NodeValue(rootNode, "custom");
            result.requestedAt = UnixTimeStampToDateTime(
                                   NodeValueDouble(rootNode, "requested_at")
                                 );
            result.page = NodeValueInt(rootNode, "page");
            result.duration = NodeValueInt(rootNode, "duration");

            return result;
        }
        #endregion
    }
}
