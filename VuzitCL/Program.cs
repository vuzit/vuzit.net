using System;
using System.Collections.Generic;
using System.Text;

namespace VuzitCL
{
    /// <summary>
    /// Main class of the command-line application.  
    /// </summary>
    class Program
    {
        /// <summary>
        /// Executes the document request.  It's in this function so I can 
        /// trap errors in the Main function.  
        /// </summary>
        static void Execute(string[] args)
        {
            ArgvParser parser = new ArgvParser(args);

            if (parser["k"] == null && parser["keys"] == null)
            {
                Console.WriteLine("Must provide parameters");
                PrintUsage();
            }

            string[] keys = ((parser["k"] != null) ? parser["k"] : parser["keys"]).Split(',');

            if (keys.Length != 2)
            {
                Console.WriteLine("ERROR: Please provide both --key parameters (PUBLIC,PRIVATE)");
                Console.WriteLine("");
                PrintUsage();
            }
            Vuzit.Service.PublicKey = keys[0];
            Vuzit.Service.PrivateKey = keys[1];
            Vuzit.Service.UserAgent = "VuzitCL .NET 1.0.0";
            if (parser["s"] != null || parser["service"] != null)
            {
               string url = (parser["s"] != null) ? parser["s"] : parser["service-url"];
               Vuzit.Service.ServiceUrl = url;
            }

            if (parser["h"] != null || parser["help"] != null)
            {
                PrintUsage();
            }

            if (parser["d"] != null || parser["delete"] != null)
            {
                string id = (parser["d"] != null) ? parser["d"] : parser["delete"];
                Vuzit.Document.Destroy(id);
                Console.WriteLine("DELETED: {0}", id);
            }

            if (parser["l"] != null || parser["load"] != null)
            {
                string id = (parser["l"] != null) ? parser["l"] : parser["load"];
                Vuzit.Document document = Vuzit.Document.FindById(id);
                Console.WriteLine("LOADED: {0}", document.Id);
                Console.WriteLine("title: {0}", document.Title);
                Console.WriteLine("subject: {0}", document.Subject);
                Console.WriteLine("pages: {0}", document.PageCount);
                Console.WriteLine("width: {0}", document.PageWidth);
                Console.WriteLine("height: {0}", document.PageHeight);
                Console.WriteLine("size: {0}", document.FileSize);
            }

            if (parser["u"] != null || parser["upload"] != null)
            {
                string path = (parser["u"] != null) ? parser["u"] : parser["upload"];
                // TODO: Add the -p secure support to this. 
                Vuzit.Document document = Vuzit.Document.Upload(path, true);
                Console.WriteLine("UPLOADED: {0}", document.Id);
            }
        }

        /// <summary>
        /// Main application.  
        /// </summary>
        /// <param name="args">Command-line arguments. </param>
        static void Main(string[] args)
        {
            try
            {
                Execute(args);
            }
            catch (Vuzit.Exception ex)
            {
                Console.WriteLine("Web service error [{0}]: {1}", ex.Code, ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Prints the usage of the application.  
        /// </summary>
        static void PrintUsage()
        {
            Console.WriteLine("VuzitCL - Vuzit Command Line");
            Console.WriteLine("Usage: vuzitcl -k PUBLIC_KEY,PRIVATE_KEY [OPTIONS]");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  -k, --keys=PUB_KEY,PRIV_KEY    Developer API keys - REQUIRED");
            Console.WriteLine("  -u, --upload=PATH              File to upload");
            Console.WriteLine("  -p, --public                   Make uploaded file public");
            Console.WriteLine("  -l, --load=ID                  Loads the document data");
            Console.WriteLine("  -d, --delete=ID                Deletes a document");
            Console.WriteLine("  -s, --service-url=URL          Sets the service URL (e.g. http://domain.com)");
            Console.WriteLine("  -v, --verbose                  Prints more messages");
            Console.WriteLine("  -h, --help                     Show this message");
        }
    }
}
