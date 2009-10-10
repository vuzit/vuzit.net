using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Vuzit
{
    /// <summary>
    /// Class for uploading, loading, and deleting documents using the Vuzit 
    /// Web Service API: http://vuzit.com/developer/documents_api.
    /// </summary>
    public sealed class Document : Vuzit.Base
    {
        #region Private static variables
        private string id = null;
        private int fileSize = -1;
        private int pageCount = -1;
        private int pageWidth = -1;
        private int pageHeight = -1;
        private string subject = null;
        private string title = null;
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
        /// <param name="documentId">ID of the document to destroy. </param>
        public static void Destroy(string webId)
        {
            OptionList parameters = PostParameters(new OptionList(), "destroy", webId);

            string url = ParametersToUrl("documents", parameters, webId);
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
        /// Finds a document by the ID.  It throws a Vuzit.Exception on failure. 
        /// </summary>
        /// <param name="documentId">Id of the document. </param>
        public static Vuzit.Document FindById(string webId)
        {
            Vuzit.Document result = new Vuzit.Document();

            OptionList parameters = PostParameters(new OptionList(), "show", webId);

            string url = ParametersToUrl("documents", parameters, webId);
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

                    result.id = webId;
                    node = doc.SelectSingleNode("/document/title");

                    if (node == null)
                    {
                        throw new Vuzit.ClientException("No node data in response", 0);
                    }
                    result.title = node.InnerText;
                    result.subject = doc.SelectSingleNode("/document/subject").InnerText;
                    result.pageCount = Convert.ToInt32(doc.SelectSingleNode("/document/page_count").InnerText);
                    result.pageWidth = Convert.ToInt32(doc.SelectSingleNode("/document/width").InnerText);
                    result.pageHeight = Convert.ToInt32(doc.SelectSingleNode("/document/height").InnerText);
                    result.fileSize = Convert.ToInt32(doc.SelectSingleNode("/document/file_size").InnerText);
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
        /// Upload a document via a Stream.  
        /// </summary>
        public static Vuzit.Document Upload(Stream stream, OptionList options)
        {
            Vuzit.Document result = new Vuzit.Document();

            if (!options.Contains("file_name"))
            {
                options.Add("file_name", "document");
            }

            OptionList parameters = PostParameters(options, "create", null);

            string url = ParametersToUrl("documents", parameters, null);
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
        /// Upload a document via a Stream.  
        /// </summary>
        public static Vuzit.Document Upload(Stream stream, string fileType,
                                            string fileName, bool secure)
        {
            return Upload(stream, new OptionList()
                                       .Add("file_name", fileName)
                                       .Add("file_type", fileType)
                                       .Add("secure", secure));
        }

        /// <summary>
        /// Upload a file but sets the default document name (default: document.{fileType}).  
        /// </summary>
        public static Vuzit.Document Upload(Stream stream, string fileType, bool secure)
        {
            return Upload(stream, fileType, null, secure);
        }

        /// <summary>
        /// Upload a file but sets the default document name 
        /// (default: document.{fileType}) and security (default: true).  
        /// </summary>
        public static Vuzit.Document Upload(Stream stream, string fileType)
        {
            return Upload(stream, fileType, null, true);
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
        /// Uploads a file to Vuzit. It throws a <Vuzit.Exception> on failure.
        /// </summary>
        public static Vuzit.Document Upload(string path, bool secure, string fileType)
        {
            return Upload(path, new OptionList()
                                     .Add("file_type", fileType)
                                     .Add("secure", secure));
        }

        /// <summary>
        /// Uploads a document with no content type.  
        /// </summary>
        public static Vuzit.Document Upload(string path, bool secure)
        {
            return Upload(path, secure, null);
        }

        /// <summary>
        /// Uploads a document with all of the default settings: secure 
        /// on and no content type. 
        /// </summary>
        public static Vuzit.Document Upload(string path)
        {
            return Upload(path, true, null);
        }
        #endregion

        #region Private static methods
        #endregion
    }
}
