#region Copyright
//
// Nini Configuration Project.
// Copyright (C) 2006 Brent R. Matzelle.  All rights reserved.
//
// This software is published under the terms of the MIT X11 license, a copy of 
// which has been included with this distribution in the LICENSE.txt file.
// 
// Original code written by: R. LOPES (GriffonRL)
// Article: http://thecodeproject.com/csharp/command_line.asp
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VuzitCL
{
    /// <summary>
    /// Class for parsing command-line arguments.  
    /// </summary>
    public class ArgvParser
    {
        #region Private variables
        Dictionary<string, string> parameters;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Base constructor for a single argument.  
        /// </summary>
        public ArgvParser(string args)
        {
            Regex Extractor = new Regex(@"(['""][^""]+['""])\s*|([^\s]+)\s*",
                                        RegexOptions.Compiled);
            MatchCollection matches;
            string[] parts;
            
            // Get matches (first string ignored because 
            // Environment.CommandLine starts with program filename)
            matches = Extractor.Matches (args);
            parts = new string[matches.Count - 1];

            for (int i = 1; i < matches.Count; i++)
            {
                parts[i-1] = matches[i].Value.Trim ();
            }
        }
        
        /// <summary>
        /// Constructor that takes in a standard set of arguments.  
        /// </summary>
        public ArgvParser (string[] args)
        {
            Extract (args);
        }
        #endregion
        
        #region Public properties
        /// <summary>
        /// Returns the parser option or null if it does not exist.  
        /// </summary>
        public string this [string param]
        {
            get
            {
                if (parameters.ContainsKey(param))
                {
                    return parameters[param];
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the data for an argument.  
        /// </summary>
        public string GetArg(string argument1, string argument2)
        {
            string result = null;

            if (this[argument1] != null)
            {
                result = this[argument1];
            }

            if (result == null && this[argument2] != null)
            {
                result = this[argument2];
            }

            return result;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Extract command line parameters and values stored in a string array
        /// </summary>
        private void Extract(string[] args)
        {
            parameters = new Dictionary<string, string>();
            Regex splitter = new Regex (@"^([/-]|--){1}(?<name>\w+)([:=])?(?<value>.+)?$",
                                        RegexOptions.Compiled);
            char[] trimChars = {'"','\''};
            string parameter = null;
            Match part;

            // Valid parameters forms: {-,/,--}param{ , = ,:}((",')value(",'))
            // Examples: -param1 value1 --param2 /param3:"Test-:-work" 
            // /param4 = happy -param5 '-- = nice = --'
            foreach(string arg in args)
            {
                part = splitter.Match(arg);
                if (!part.Success) {
                    // Found a value (for the last parameter found (space separator))
                    if (parameter != null) {
                        parameters[parameter] = arg.Trim (trimChars);
                    }
                } else {
                    // Matched a name, optionally with inline value
                    parameter = part.Groups["name"].Value;
                    parameters.Add (parameter, 
                                    part.Groups["value"].Value.Trim (trimChars));
                }
            }
        }
        #endregion
    }
}
