using System;
using System.Collections;
using System.Text;

namespace Vuzit
{
    /// <summary>
    /// Class for handling optional parameters for methods.  These options 
    /// can then be directly applied to the web service parameterse.  
    /// </summary>
    public class OptionList
    {
        #region Private variables
        private Hashtable list = new Hashtable();
        #endregion

        #region Public properties
        /// <summary>
        /// Returns the number of items in the list.  
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// Returns a list of the keys in the list.  
        /// </summary>
        public ICollection Keys
        {
            get { return list.Keys; }
        }

        /// <summary>
        /// Returns the item in the list at that key.  
        /// </summary>
        public string this[string key]
        {
            get { return (string)list[key]; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds an option then returns itself so that you can chain add items
        /// to the list.  
        /// </summary>
        public OptionList Add(string key, string value)
        {
            if (value != null && value.Length > 0)
            {
                list.Add(key, value);
            }

            return this;
        }

        /// <summary>
        /// Adds an option then returns itself so that you can chain add items
        /// to the list.  
        /// </summary>
        public OptionList Add(string key, int value)
        {
            return Add(key, value.ToString());
        }

        /// <summary>
        /// Adds an option then returns itself so that you can chain add items
        /// to the list.  
        /// </summary>
        public OptionList Add(string key, bool value)
        {
            return Add(key, (value) ? "1" : "0");
        }

        /// <summary>
        /// Returns true if the list contains an item by that key name.  
        /// </summary>
        public bool Contains(string key)
        {
            return list.Contains(key);
        }
        #endregion
    }
}
