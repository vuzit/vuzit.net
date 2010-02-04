using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VuzitCL
{
    /// <summary>
    /// Main class of the command-line application.  
    /// </summary>
    class Program
    {
        #region Main execution functions
        /// <summary>
        /// Executes the document request.  It's in this function so I can 
        /// trap errors in the Main function.  
        /// </summary>
        static void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsageGeneral();
                return;
            }

            switch (args[0])
            {
                case "upload":
                    UploadCommand(args);
                    break;
                case "delete":
                    DeleteCommand(args);
                    break;
                case "load":
                    LoadCommand(args);
                    break;
                case "help":
                    HelpCommand(args);
                    break;
                case "search":
                    SearchCommand(args);
                    break;
                default:
                    Console.WriteLine("Incorrect option: " + args[0]);
                    break;
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
            catch (Vuzit.ClientException ex)
            {
                Console.WriteLine("WEB ERROR: [{0}] {1}", ex.Code, ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }
        #endregion

        #region Sub-command functions
        /// <summary>
        /// Executes the delete sub-command.  
        /// </summary>
        static void DeleteCommand(string[] args)
        {
            string id = LastOption(args);
            string[] options = CleanArgs(args);
            ArgvParser parser = new ArgvParser(options);

            if (!GlobalParametersLoad(parser))
            {
                return;
            }

            Vuzit.Document.Destroy(id);
            Console.WriteLine("DELETED: {0}", id);
        }

        /// <summary>
        /// Executes the help sub-command.  
        /// </summary>
        static void HelpCommand(string[] args)
        {
            switch (LastOption(args))
            {
                case "upload":
                    PrintUsageUpload();
                    break;
                case "load":
                    PrintUsageLoad();
                    break;
                case "delete":
                    PrintUsageDelete();
                    break;
                case "search":
                    PrintUsageSearch();
                    break;
                default:
                    Console.WriteLine("Unknown option: " + LastOption(args));
                    break;
            }
        }

        /// <summary>
        /// Executes the load command.  
        /// </summary>
        static void LoadCommand(string[] args)
        {
            string id = LastOption(args);
            string[] options = CleanArgs(args);
            ArgvParser parser = new ArgvParser(options);

            if (!GlobalParametersLoad(parser))
            {
                return;
            }
            
            Vuzit.Document document = Vuzit.Document.Find(id);
            Console.WriteLine("LOADED: {0}", document.Id);
            Console.WriteLine("title: {0}", document.Title);
            Console.WriteLine("subject: {0}", document.Subject);
            Console.WriteLine("pages: {0}", document.PageCount);
            Console.WriteLine("width: {0}", document.PageWidth);
            Console.WriteLine("height: {0}", document.PageHeight);
            Console.WriteLine("size: {0}", document.FileSize);
            Console.WriteLine("status: {0}", document.Status);

            Console.WriteLine("Download URL: {0}", Vuzit.Document.DownloadUrl(id, "pdf"));
        }

        /// <summary>
        /// Executes the search sub-command.  
        /// </summary>
        static void SearchCommand(string[] args)
        {
            string id = LastOption(args);
            string[] options = CleanArgs(args);
            ArgvParser parser = new ArgvParser(options);

            if (!GlobalParametersLoad(parser))
            {
                return;
            }

            Vuzit.OptionList list = new Vuzit.OptionList();
            if (parser.GetArg("q", "query") != null)
            {
                list.Add("query", parser.GetArg("q", "query"));
            }

            Vuzit.Document[] documents = Vuzit.Document.FindAll(list);

            int i = 1;
            Console.WriteLine("{0} documents found", documents.Length);
            Console.WriteLine("");

            foreach (Vuzit.Document document in documents)
            {
                Console.WriteLine("LOADED [{0}]: {1}", i, document.Id);
                Console.WriteLine("title: {0}", document.Title);
                Console.WriteLine("subject: {0}", document.Subject);
                Console.WriteLine("pages: {0}", document.PageCount);
                Console.WriteLine("width: {0}", document.PageWidth);
                Console.WriteLine("height: {0}", document.PageHeight);
                Console.WriteLine("size: {0}", document.FileSize);
                Console.WriteLine("status: {0}", document.Status);
                Console.WriteLine("excerpt: {0}", document.Excerpt);

                Console.WriteLine("Download URL: {0}", 
                               Vuzit.Document.DownloadUrl(document.Id, "pdf"));
                i++;
            }
        }

        /// <summary>
        /// Executes the upload sub-command.  
        /// </summary>
        static void UploadCommand(string[] args)
        {
            string path = LastOption(args);
            string[] options = CleanArgs(args);
            ArgvParser parser = new ArgvParser(options);

            if (!GlobalParametersLoad(parser))
            {
                return;
            }

            Vuzit.OptionList list = new Vuzit.OptionList();

            if (parser[""] != null || parser["secure"] != null)
            {
                list.Add("secure", true);
            }
            else
            {
                list.Add("secure", false);
            }

            if (parser["p"] != null || parser["download-pdf"] != null)
            {
                list.Add("download_pdf", true);
            }

            if (parser["d"] != null || parser["download-document"] != null)
            {
                list.Add("download_document", true);
            }

            Vuzit.Document document = Vuzit.Document.Upload(path, list);
            Console.WriteLine("UPLOADED: {0}", document.Id);
        }
        #endregion

        #region Utility functions
        /// <summary>
        /// Removes the head and trailing arguments so that the parser doesn't get confused.  
        /// </summary>
        static string[] CleanArgs(string[] args)
        {
            string[] result = new string[args.Length - 2];

            int item = 0;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (i != 0 && i != args.Length - 1)
                {
                    result[item] = args[i];
                    item += 1;
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the global parameters.  If they were not correctly added then it fails.  
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        static bool GlobalParametersLoad(ArgvParser parser)
        {
            if (parser["k"] == null && parser["keys"] == null)
            {
                Console.WriteLine("Must provide the --key parameter");
                return false;
            }

            string[] keys = ((parser["k"] != null) ? parser["k"] : parser["keys"]).Split(',');

            if (keys.Length != 2)
            {
                Console.WriteLine("ERROR: Please provide both --key parameters (PUBLIC,PRIVATE)");
                Console.WriteLine("");
                PrintUsageGeneral();
            }
            Vuzit.Service.PublicKey = keys[0];
            Vuzit.Service.PrivateKey = keys[1];
            Vuzit.Service.UserAgent = "VuzitCL .NET 1.1.0";

            if (parser["u"] != null || parser["service-url"] != null)
            {
                string url = (parser["u"] != null) ? parser["u"] : parser["service-url"];
                Vuzit.Service.ServiceUrl = url;
            }

            return true;
        }

        /// <summary>
        /// Returns the last command option from a set of arguments.  
        /// </summary>
        static string LastOption(string[] args)
        {
            return args[args.Length - 1];
        }
        #endregion

        #region Print usage methods
        /// <summary>
        /// Prints the delete sub-command usage options.  
        /// </summary>
        static void PrintUsageDelete()
        {
            Console.WriteLine("delete: Delete a document");
            Console.WriteLine("usage: delete [OPTIONS] WEB_ID");
            Console.WriteLine("");
            Console.WriteLine("Valid options:");
            Console.WriteLine("  none");
            Console.WriteLine("");
            PrintUsageGlobal();
        }

        /// <summary>
        /// Prints the usage of the application.  
        /// </summary>
        static void PrintUsageGeneral()
        {
            Console.WriteLine("VuzitCL - Vuzit Command Line");
            Console.WriteLine("Usage: vuzitcl -k PUBLIC_KEY,PRIVATE_KEY [OPTIONS]");
            Console.WriteLine("");
            Console.WriteLine("Type 'vuzitcl help <subcommand>' for help on a specific subcommand.");
            Console.WriteLine("");
            Console.WriteLine("Available sub-commands:");
            Console.WriteLine("");
            Console.WriteLine("  delete");
            Console.WriteLine("  load");
            Console.WriteLine("  search");
            Console.WriteLine("  upload");
            Console.WriteLine("  help");
        }

        /// <summary>
        /// Prints the global usage.  
        /// </summary>
        static void PrintUsageGlobal()
        {
            Console.WriteLine("Global Options:");
            Console.WriteLine("  -k, --keys=PUB_KEY,PRIV_KEY    Developer API keys - REQUIRED");
            Console.WriteLine("  -u, --service-url=URL          Sets the service URL (e.g. http://domain.com)");
        }

        /// <summary>
        /// Prints the load sub-command usage options.  
        /// </summary>
        static void PrintUsageLoad()
        {
            Console.WriteLine("load: Loads a document");
            Console.WriteLine("usage: load [OPTIONS] WEB_ID");
            Console.WriteLine("");
            Console.WriteLine("Valid options:");
            Console.WriteLine("  none");
            Console.WriteLine("");
            PrintUsageGlobal();
        }

        /// <summary>
        /// Prints the search sub-command usage options.  
        /// </summary>
        static void PrintUsageSearch()
        {
            Console.WriteLine("search: Upload a file to Vuzit.");
            Console.WriteLine("usage: search [OPTIONS]");
            Console.WriteLine("");
            Console.WriteLine("Valid options:");
            Console.WriteLine("  -q, --query         Query keywords");
            Console.WriteLine("  -l, --limit         Limits the results to a number");
            Console.WriteLine("  -o, --offset        Offsets the results at this number");
            Console.WriteLine("");
            PrintUsageGlobal();
        }

        /// <summary>
        /// Prints the upload sub-command usage options.  
        /// </summary>
        static void PrintUsageUpload()
        {
            Console.WriteLine("upload: Upload a file to Vuzit.");
            Console.WriteLine("usage: upload [OPTIONS] PATH");
            Console.WriteLine("");
            Console.WriteLine("Valid options:");
            Console.WriteLine("  -s, --secure                   Make the document secure (not public)");
            Console.WriteLine("  -p, --download-pdf             Make the PDF downloadable");
            Console.WriteLine("  -d, --download-document        Make the original document downloadable");
            Console.WriteLine("");
            PrintUsageGlobal();
        }
        #endregion
    }
}
