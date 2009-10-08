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
            set { id = value; }
        }

        /// <summary>
        /// The size of the document in bytes.  
        /// </summary>
        public int FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        /// <summary>
        /// The number of pages in the document.  
        /// </summary>
        public int PageCount
        {
            get { return pageCount; }
            set { pageCount = value; }
        }

        /// <summary>
        /// The width of the document.  
        /// </summary>
        public int PageWidth
        {
            get { return pageWidth; }
            set { pageWidth = value; }
        }

        /// <summary>
        /// The height of the document.  
        /// </summary>
        public int PageHeight
        {
            get { return pageHeight; }
            set { pageHeight = value; }
        }

        /// <summary>
        /// The subject of the document.  
        /// </summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        /// <summary>
        /// The title of the document.  
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
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
            Dictionary<string, string> parameters = PostParameters("destroy", webId);

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

            Dictionary<string, string> parameters = PostParameters("show", webId);

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

                    result.Id = webId;
                    node = doc.SelectSingleNode("/document/title");

                    if (node == null)
                    {
                        throw new Vuzit.ClientException("No node data in response", 0);
                    }
                    result.Title = node.InnerText;
                    result.Subject = doc.SelectSingleNode("/document/subject").InnerText;
                    result.PageCount = Convert.ToInt32(doc.SelectSingleNode("/document/page_count").InnerText);
                    result.PageWidth = Convert.ToInt32(doc.SelectSingleNode("/document/width").InnerText);
                    result.PageHeight = Convert.ToInt32(doc.SelectSingleNode("/document/height").InnerText);
                    result.FileSize = Convert.ToInt32(doc.SelectSingleNode("/document/file_size").InnerText);
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
        /// <param name="stream">Document stream. </param>
        /// <param name="fileType">Document file type. </param>
        /// <param name="fileName">Document file name. </param>
        /// <param name="secure">Make the document secure. </param>
        public static Vuzit.Document Upload(Stream stream, string fileType,
                                            string fileName, bool secure)
        {
            Vuzit.Document result = new Vuzit.Document();

            if (fileName == null)
            {
                fileName = "document";
            }

            Dictionary<string, string> parameters = PostParameters("create", null);
            if (fileType != null)
            {
                parameters.Add("file_type", fileType);
            }
            parameters.Add("secure", (secure) ? "1" : "0");

            string url = ParametersToUrl("documents", parameters, null);
            string xml = UploadFile(stream, url, fileName, "upload", null, 
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
            result.Id = node.InnerText;

            return result;
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
        /// <param name="path">Path to the file on disk. </param>
        /// <param name="secure">Make the document public or private. </param>
        /// <param name="fileType">Type of file.</param>
        public static Vuzit.Document Upload(string path, bool secure, string fileType)
        {
            Vuzit.Document result = null;

            if (!File.Exists(path))
            {
                throw new Vuzit.ClientException("Cannot find file at path: " + path, 0);
            }

            FileStream fileStream = new FileStream(path, FileMode.Open,
                                                   FileAccess.Read);

            result = Upload(fileStream, fileType, Path.GetFileName(path), secure);
            fileStream.Close();

            return result;
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
        private static string UploadFile(Stream fileStream, string url, string fileName,
                                         string fileFormName, string contentType,
                                         CookieContainer cookies)
        {
            if(fileFormName == null)
            {
                fileFormName = "file";
            }

            if(contentType == null)
            {
                contentType = "application/octet-stream";
            }

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = WebRequestBuild(url);
            webrequest.UserAgent = Service.UserAgent;
            webrequest.CookieContainer = cookies;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";

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
        #endregion
    }
}
