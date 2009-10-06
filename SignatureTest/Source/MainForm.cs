using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace SignatureTest
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Main method for the form.  
        /// </summary>
        public MainForm(string[] arguments)
        {
            // else handle whatever.  
            InitializeComponent();
        }

        private static string URLEncode(string text)
        {
            return HttpUtility.UrlEncode(HttpUtility.UrlPathEncode(text));
        }

        /// <summary>
        /// Handles the on load event for the main form.  
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles clicking the generate signature button.  
        /// </summary>
        private void generateSigButton_Click_1(object sender, EventArgs e)
        {
            Vuzit.Service.PublicKey = publicKeyText.Text;
            Vuzit.Service.PrivateKey = privateKeyText.Text;

            // NOTE: The dateTimePicker is currently hard-coded for 10:00 AM.  
            string sig = Vuzit.Service.Signature(methodText.Text, 
                                                 documentIdText.Text, 
                                                 dateTimePicker.Value);

            signatureValueLabel.Text = sig;
        }
    }
}
