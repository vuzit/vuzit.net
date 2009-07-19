namespace SignatureTest
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.signatureLabel = new System.Windows.Forms.Label();
            this.signatureValueLabel = new System.Windows.Forms.Label();
            this.publicKeyLabel = new System.Windows.Forms.Label();
            this.privateKeyLabel = new System.Windows.Forms.Label();
            this.publicKeyText = new System.Windows.Forms.TextBox();
            this.privateKeyText = new System.Windows.Forms.TextBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.dateTimeLabel = new System.Windows.Forms.Label();
            this.generateSigButton = new System.Windows.Forms.Button();
            this.methodLabel = new System.Windows.Forms.Label();
            this.documentIdLabel = new System.Windows.Forms.Label();
            this.methodText = new System.Windows.Forms.TextBox();
            this.documentIdText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // signatureLabel
            // 
            this.signatureLabel.AutoSize = true;
            this.signatureLabel.Location = new System.Drawing.Point(31, 190);
            this.signatureLabel.Name = "signatureLabel";
            this.signatureLabel.Size = new System.Drawing.Size(55, 13);
            this.signatureLabel.TabIndex = 0;
            this.signatureLabel.Text = "Signature:";
            // 
            // signatureValueLabel
            // 
            this.signatureValueLabel.AutoSize = true;
            this.signatureValueLabel.Location = new System.Drawing.Point(110, 190);
            this.signatureValueLabel.Name = "signatureValueLabel";
            this.signatureValueLabel.Size = new System.Drawing.Size(10, 13);
            this.signatureValueLabel.TabIndex = 1;
            this.signatureValueLabel.Text = "-";
            // 
            // publicKeyLabel
            // 
            this.publicKeyLabel.AutoSize = true;
            this.publicKeyLabel.Location = new System.Drawing.Point(31, 23);
            this.publicKeyLabel.Name = "publicKeyLabel";
            this.publicKeyLabel.Size = new System.Drawing.Size(59, 13);
            this.publicKeyLabel.TabIndex = 2;
            this.publicKeyLabel.Text = "Public key:";
            // 
            // privateKeyLabel
            // 
            this.privateKeyLabel.AutoSize = true;
            this.privateKeyLabel.Location = new System.Drawing.Point(31, 51);
            this.privateKeyLabel.Name = "privateKeyLabel";
            this.privateKeyLabel.Size = new System.Drawing.Size(64, 13);
            this.privateKeyLabel.TabIndex = 3;
            this.privateKeyLabel.Text = "Private Key:";
            // 
            // publicKeyText
            // 
            this.publicKeyText.Location = new System.Drawing.Point(113, 23);
            this.publicKeyText.Name = "publicKeyText";
            this.publicKeyText.Size = new System.Drawing.Size(227, 20);
            this.publicKeyText.TabIndex = 4;
            this.publicKeyText.Text = "73837690-5f63-11de-8a39-0800200c9a66";
            // 
            // privateKeyText
            // 
            this.privateKeyText.Location = new System.Drawing.Point(113, 49);
            this.privateKeyText.Name = "privateKeyText";
            this.privateKeyText.Size = new System.Drawing.Size(227, 20);
            this.privateKeyText.TabIndex = 5;
            this.privateKeyText.Text = "8958e900-5f63-11de-8a39-0800200c9a66";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Checked = false;
            this.dateTimePicker.Location = new System.Drawing.Point(113, 75);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker.TabIndex = 6;
            this.dateTimePicker.Value = new System.DateTime(2009, 6, 22, 10, 0, 0, 0);
            // 
            // dateTimeLabel
            // 
            this.dateTimeLabel.AutoSize = true;
            this.dateTimeLabel.Location = new System.Drawing.Point(31, 82);
            this.dateTimeLabel.Name = "dateTimeLabel";
            this.dateTimeLabel.Size = new System.Drawing.Size(67, 13);
            this.dateTimeLabel.TabIndex = 7;
            this.dateTimeLabel.Text = "Date / Time:";
            // 
            // generateSigButton
            // 
            this.generateSigButton.Location = new System.Drawing.Point(97, 159);
            this.generateSigButton.Name = "generateSigButton";
            this.generateSigButton.Size = new System.Drawing.Size(120, 23);
            this.generateSigButton.TabIndex = 8;
            this.generateSigButton.Text = "Generate Signature";
            this.generateSigButton.UseVisualStyleBackColor = true;
            this.generateSigButton.Click += new System.EventHandler(this.generateSigButton_Click_1);
            // 
            // methodLabel
            // 
            this.methodLabel.AutoSize = true;
            this.methodLabel.Location = new System.Drawing.Point(31, 109);
            this.methodLabel.Name = "methodLabel";
            this.methodLabel.Size = new System.Drawing.Size(46, 13);
            this.methodLabel.TabIndex = 9;
            this.methodLabel.Text = "Method:";
            // 
            // documentIdLabel
            // 
            this.documentIdLabel.AutoSize = true;
            this.documentIdLabel.Location = new System.Drawing.Point(30, 134);
            this.documentIdLabel.Name = "documentIdLabel";
            this.documentIdLabel.Size = new System.Drawing.Size(73, 13);
            this.documentIdLabel.TabIndex = 10;
            this.documentIdLabel.Text = "Document ID:";
            // 
            // methodText
            // 
            this.methodText.Location = new System.Drawing.Point(113, 104);
            this.methodText.Name = "methodText";
            this.methodText.Size = new System.Drawing.Size(100, 20);
            this.methodText.TabIndex = 11;
            this.methodText.Text = "create";
            // 
            // documentIdText
            // 
            this.documentIdText.Location = new System.Drawing.Point(113, 130);
            this.documentIdText.Name = "documentIdText";
            this.documentIdText.Size = new System.Drawing.Size(100, 20);
            this.documentIdText.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 234);
            this.Controls.Add(this.documentIdText);
            this.Controls.Add(this.methodText);
            this.Controls.Add(this.documentIdLabel);
            this.Controls.Add(this.methodLabel);
            this.Controls.Add(this.generateSigButton);
            this.Controls.Add(this.dateTimeLabel);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.privateKeyText);
            this.Controls.Add(this.publicKeyText);
            this.Controls.Add(this.privateKeyLabel);
            this.Controls.Add(this.publicKeyLabel);
            this.Controls.Add(this.signatureValueLabel);
            this.Controls.Add(this.signatureLabel);
            this.Name = "MainForm";
            this.Text = "Signature Test";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label signatureLabel;
        private System.Windows.Forms.Label signatureValueLabel;
        private System.Windows.Forms.Label publicKeyLabel;
        private System.Windows.Forms.Label privateKeyLabel;
        private System.Windows.Forms.TextBox publicKeyText;
        private System.Windows.Forms.TextBox privateKeyText;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.Label dateTimeLabel;
        private System.Windows.Forms.Button generateSigButton;
        private System.Windows.Forms.Label methodLabel;
        private System.Windows.Forms.Label documentIdLabel;
        private System.Windows.Forms.TextBox methodText;
        private System.Windows.Forms.TextBox documentIdText;
    }
}

