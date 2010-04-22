using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Vuzit
{
    /// <summary>
    /// Class for uploading, loading, and deleting documents using the Vuzit 
    /// Web Service API.
    /// </summary>
    public sealed class Document : Vuzit.Base
    {
        #region Private static variables
        private string id = null;
        private int fileSize = -1;
        private int pageCount = -1;
        private int pageWidth = -1;
        private int pageHeight = -1;
        private int status = -1;
        private string subject = null;
        private string title = null;
        private string excerpt = null;
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
        /// A short excerpt from a document.  
        /// </summary>
        public string Excerpt
        {
            get { return excerpt; }
        }

        /// <summary>
        /// The size of the document in bytes.  
        /// </summary>
        public int FileSize
        {
            get { return fileSize; }
        }

        /// <summary>
        /// The number of pages in the document.  
        /// </summary>
        public int PageCount
        {
            get { return pageCount; }
        }

        /// <summary>
        /// The width of the document.  
        /// </summary>
        public int PageWidth
        {
            get { return pageWidth; }
        }

        /// <summary>
        /// The height of the document.  
        /// </summary>
        public int PageHeight
        {
            get { return pageHeight; }
        }

        /// <summary>
        /// The status of the document.  
        /// </summary>
        public int Status
        {
            get { return status; }
        }

        /// <summary>
        /// The subject of the document.  
        /// </summary>
        public string Subject
        {
            get { return subject; }
        }

        /// <summary>
        /// The title of the document.  
        /// </summary>
        public string Title
        {
            get { return title; }
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Deletes a document by the ID.  Returns true if it succeeded.  It 
        /// throws a Vuzit_Exception on failure.
        /// </summary>
        public static void Destroy(string webId)
        {
            if (webId == null)
            {
                throw new Vuzit.ClientException("webId cannot be null");
            }

            OptionList parameters = PostParameters(new OptionList(), "destroy", webId);

            string url = ParametersToUrl("documents/" + webId + ".xml", parameters);
            HttpWebRequest request = WebRequestBuild(url);
            request.UserAgent = Service.UserAgent;
            request.Method = "DELETE";

            // Catch 403, etc forbidden errors
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Vuzit.ClientException("HTTP error", response.StatusCode.ToString());
                    }
                }
            }
            catch(Vuzit.ClientException ex)
            {
                throw ex;  // Rethrow because I want to see this exception
            }
            catch(WebException ex)
            {
                throw new Vuzit.ClientException("HTTP response error", ex);
            }
        }

        /// <summary>
        /// Returns a download URL.  
        /// </summary>
        public static string DownloadUrl(string webId, string fileExtension)
        {
            if (webId == null)
            {
                throw new Vuzit.ClientException("webId cannot be null");
            }

            OptionList parameters = PostParameters(new OptionList(), "show", webId);
            return ParametersToUrl("documents/" + webId + "." + fileExtension, parameters);
        }

        /// <summary>
        /// Returns the documents but without any options.  
        /// </summary>
        public static Vuzit.Document Find(string webId)
        {
            return Find(webId, new OptionList());
        }

        /// <summary>
        /// Finds a document by the ID.  It throws a Vuzit.Exception on failure. 
        /// </summary>
        public static Vuzit.Document Find(string webId, OptionList options)
        {
            if (webId == null)
            {
                throw new Vuzit.ClientException("webId cannot be null");
            }
            Vuzit.Document result = null;

            OptionList parameters = PostParameters(options, "show", webId);

            string url = ParametersToUrl("documents/" + webId + ".xml", parameters);
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

                    XmlNode docNode = doc.SelectSingleNode("document");
                    if (docNode == null)
                    {
                        throw new Vuzit.ClientException("No document found by that ID", 0);
                    }

                    result = NodeToDocument(docNode);
                    result.id = webId;
                }
            }
            catch(Vuzit.ClientException ex)
            {
                throw ex;  // Rethrow because I want to see this exception
            }
            catch (WebException ex)
            {
                throw new Vuzit.ClientException("HTTP response error", ex);
            }

            return result;
        }

        /// <summary>
        /// Returns a list of all documents in the account.  
        /// </summary>
        public static Vuzit.Document[] FindAll()
        {
            return FindAll(new OptionList());
        }

        /// <summary>
        /// Finds all documents matching the given criteria.  
        /// </summary>
        public static Vuzit.Document[] FindAll(OptionList options)
        {
            List<Vuzit.Document> result = new List<Vuzit.Document>();

            OptionList parameters = PostParameters(options, "index", null);

            string url = ParametersToUrl("documents.xml", parameters);
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

                    XmlNodeList list = doc.SelectNodes("/documents/document");

                    foreach (XmlNode childNode in list)
                    {
                        result.Add(NodeToDocument(childNode));
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

        /// <summary>
        /// Upload a document via a Stream.  
        /// </summary>
        public static Vuzit.Document Upload(Stream stream, OptionList options)
        {
            if (stream == null)
            {
                throw new Vuzit.ClientException("stream cannot be null");
            }

            Vuzit.Document result = new Vuzit.Document();

            if (!options.Contains("file_name"))
            {
                options.Add("file_name", "document");
            }

            OptionList parameters = PostParameters(options, "create", null);

            string url = ParametersToUrl("documents.xml", parameters);
            string xml = UploadFile(stream, url, options["file_name"], "upload", null,
                                    new CookieContainer());

            // Load the document section.  
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch
            {
                throw new Vuzit.ClientException("Incorrect XML response: " + xml, 0);
            }

            XmlNode node = doc.SelectSingleNode("/document/web_id");

            if (node == null)
            {
                throw new Vuzit.ClientException("No node data in response", 0);
            }
            result.id = node.InnerText;

            return result;
        }

        /// <summary>
        /// Uploads a file to Vuzit. It throws a <Vuzit.Exception> on failure.
        /// </summary>
        public static Vuzit.Document Upload(string path, OptionList options)
        {
            Vuzit.Document result = null;

            if (!File.Exists(path))
            {
                throw new Vuzit.ClientException("Cannot find file at path: " + path, 0);
            }

            FileStream fileStream = new FileStream(path, FileMode.Open,
                                                   FileAccess.Read);

            if(!options.Contains("file_name"))
            {
                options.Add("file_name", Path.GetFileName(path));
            }
            result = Upload(fileStream, options);
            fileStream.Close();

            return result;
        }

        /// <summary>
        /// Uploads a document with all of the default settings: secure 
        /// on and no content type. 
        /// </summary>
        public static Vuzit.Document Upload(string path)
        {
            return Upload(path, new OptionList());
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Converts an XML node to a Vuzit document.  
        /// </summary>
        private static Vuzit.Document NodeToDocument(XmlNode rootNode)
        {
            Vuzit.Document result = new Vuzit.Document();
            XmlNode node = rootNode.SelectSingleNode("web_id");

            if (node == null)
            {
                throw new Vuzit.ClientException("No web_id in response", 0);
            }
            result.id = node.InnerText;
            result.title = NodeValue(rootNode, "title");
            result.subject = NodeValue(rootNode, "subject");
            result.pageCount = NodeValueInt(rootNode, "page_count");
            result.pageWidth = NodeValueInt(rootNode, "width");
            result.pageHeight = NodeValueInt(rootNode, "height");
            result.fileSize = NodeValueInt(rootNode, "file_size");
            result.status = NodeValueInt(rootNode, "status");
            result.excerpt = NodeValue(rootNode, "excerpt");

            return result;
        }
        #endregion
    }
}
