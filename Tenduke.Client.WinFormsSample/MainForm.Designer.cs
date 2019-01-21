namespace Tenduke.Client.WinFormsSample
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labelWelcome = new System.Windows.Forms.Label();
            this.labelComputerIdentifier = new System.Windows.Forms.Label();
            this.textBoxComputerId = new System.Windows.Forms.TextBox();
            this.groupBoxAuthorizationDecisions = new System.Windows.Forms.GroupBox();
            this.buttonReleaseLicense = new System.Windows.Forms.Button();
            this.buttonShowData = new System.Windows.Forms.Button();
            this.listViewAuthorizationDecisions = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGranted = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonRequestAuthorizationDecision = new System.Windows.Forms.Button();
            this.comboBoxConsumeMode = new System.Windows.Forms.ComboBox();
            this.textBoxAuthorizedItemName = new System.Windows.Forms.TextBox();
            this.labelAuthorizedItemName = new System.Windows.Forms.Label();
            this.groupBoxAuthorizationDecisions.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelWelcome
            // 
            this.labelWelcome.AutoSize = true;
            this.labelWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWelcome.Location = new System.Drawing.Point(12, 9);
            this.labelWelcome.Name = "labelWelcome";
            this.labelWelcome.Size = new System.Drawing.Size(126, 13);
            this.labelWelcome.TabIndex = 0;
            this.labelWelcome.Text = "Welcome anonymous";
            // 
            // labelComputerIdentifier
            // 
            this.labelComputerIdentifier.AutoSize = true;
            this.labelComputerIdentifier.Location = new System.Drawing.Point(12, 40);
            this.labelComputerIdentifier.Name = "labelComputerIdentifier";
            this.labelComputerIdentifier.Size = new System.Drawing.Size(94, 13);
            this.labelComputerIdentifier.TabIndex = 1;
            this.labelComputerIdentifier.Text = "Computer identifier";
            // 
            // textBoxComputerId
            // 
            this.textBoxComputerId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComputerId.Location = new System.Drawing.Point(112, 37);
            this.textBoxComputerId.Name = "textBoxComputerId";
            this.textBoxComputerId.Size = new System.Drawing.Size(573, 20);
            this.textBoxComputerId.TabIndex = 2;
            // 
            // groupBoxAuthorizationDecisions
            // 
            this.groupBoxAuthorizationDecisions.Controls.Add(this.buttonReleaseLicense);
            this.groupBoxAuthorizationDecisions.Controls.Add(this.buttonShowData);
            this.groupBoxAuthorizationDecisions.Controls.Add(this.listViewAuthorizationDecisions);
            this.groupBoxAuthorizationDecisions.Controls.Add(this.buttonRequestAuthorizationDecision);
            this.groupBoxAuthorizationDecisions.Controls.Add(this.comboBoxConsumeMode);
            this.groupBoxAuthorizationDecisions.Controls.Add(this.textBoxAuthorizedItemName);
            this.groupBoxAuthorizationDecisions.Controls.Add(this.labelAuthorizedItemName);
            this.groupBoxAuthorizationDecisions.Location = new System.Drawing.Point(12, 63);
            this.groupBoxAuthorizationDecisions.Name = "groupBoxAuthorizationDecisions";
            this.groupBoxAuthorizationDecisions.Size = new System.Drawing.Size(673, 463);
            this.groupBoxAuthorizationDecisions.TabIndex = 3;
            this.groupBoxAuthorizationDecisions.TabStop = false;
            this.groupBoxAuthorizationDecisions.Text = "Authorization decisions";
            // 
            // buttonReleaseLicense
            // 
            this.buttonReleaseLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReleaseLicense.Location = new System.Drawing.Point(576, 434);
            this.buttonReleaseLicense.Name = "buttonReleaseLicense";
            this.buttonReleaseLicense.Size = new System.Drawing.Size(90, 23);
            this.buttonReleaseLicense.TabIndex = 6;
            this.buttonReleaseLicense.Text = "Release license";
            this.buttonReleaseLicense.UseVisualStyleBackColor = true;
            this.buttonReleaseLicense.Click += new System.EventHandler(this.buttonReleaseLicense_Click);
            // 
            // buttonShowData
            // 
            this.buttonShowData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonShowData.Location = new System.Drawing.Point(495, 434);
            this.buttonShowData.Name = "buttonShowData";
            this.buttonShowData.Size = new System.Drawing.Size(75, 23);
            this.buttonShowData.TabIndex = 5;
            this.buttonShowData.Text = "Show data";
            this.buttonShowData.UseVisualStyleBackColor = true;
            this.buttonShowData.Click += new System.EventHandler(this.buttonShowData_Click);
            // 
            // listViewAuthorizationDecisions
            // 
            this.listViewAuthorizationDecisions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewAuthorizationDecisions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderGranted,
            this.columnHeaderData});
            this.listViewAuthorizationDecisions.FullRowSelect = true;
            this.listViewAuthorizationDecisions.GridLines = true;
            this.listViewAuthorizationDecisions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewAuthorizationDecisions.Location = new System.Drawing.Point(6, 45);
            this.listViewAuthorizationDecisions.MultiSelect = false;
            this.listViewAuthorizationDecisions.Name = "listViewAuthorizationDecisions";
            this.listViewAuthorizationDecisions.Size = new System.Drawing.Size(661, 383);
            this.listViewAuthorizationDecisions.TabIndex = 4;
            this.listViewAuthorizationDecisions.UseCompatibleStateImageBehavior = false;
            this.listViewAuthorizationDecisions.View = System.Windows.Forms.View.Details;
            this.listViewAuthorizationDecisions.SelectedIndexChanged += new System.EventHandler(this.listViewAuthorizationDecisions_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 200;
            // 
            // columnHeaderGranted
            // 
            this.columnHeaderGranted.Text = "Granted";
            this.columnHeaderGranted.Width = 50;
            // 
            // columnHeaderData
            // 
            this.columnHeaderData.Text = "Data";
            this.columnHeaderData.Width = 411;
            // 
            // buttonRequestAuthorizationDecision
            // 
            this.buttonRequestAuthorizationDecision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRequestAuthorizationDecision.Location = new System.Drawing.Point(592, 17);
            this.buttonRequestAuthorizationDecision.Name = "buttonRequestAuthorizationDecision";
            this.buttonRequestAuthorizationDecision.Size = new System.Drawing.Size(75, 23);
            this.buttonRequestAuthorizationDecision.TabIndex = 3;
            this.buttonRequestAuthorizationDecision.Text = "Authorize";
            this.buttonRequestAuthorizationDecision.UseVisualStyleBackColor = true;
            this.buttonRequestAuthorizationDecision.Click += new System.EventHandler(this.buttonRequestAuthorizationDecision_Click);
            // 
            // comboBoxConsumeMode
            // 
            this.comboBoxConsumeMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxConsumeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxConsumeMode.FormattingEnabled = true;
            this.comboBoxConsumeMode.Items.AddRange(new object[] {
            "check",
            "consume"});
            this.comboBoxConsumeMode.Location = new System.Drawing.Point(465, 18);
            this.comboBoxConsumeMode.Name = "comboBoxConsumeMode";
            this.comboBoxConsumeMode.Size = new System.Drawing.Size(121, 21);
            this.comboBoxConsumeMode.TabIndex = 2;
            // 
            // textBoxAuthorizedItemName
            // 
            this.textBoxAuthorizedItemName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAuthorizedItemName.Location = new System.Drawing.Point(47, 19);
            this.textBoxAuthorizedItemName.Name = "textBoxAuthorizedItemName";
            this.textBoxAuthorizedItemName.Size = new System.Drawing.Size(412, 20);
            this.textBoxAuthorizedItemName.TabIndex = 1;
            // 
            // labelAuthorizedItemName
            // 
            this.labelAuthorizedItemName.AutoSize = true;
            this.labelAuthorizedItemName.Location = new System.Drawing.Point(6, 22);
            this.labelAuthorizedItemName.Name = "labelAuthorizedItemName";
            this.labelAuthorizedItemName.Size = new System.Drawing.Size(35, 13);
            this.labelAuthorizedItemName.TabIndex = 0;
            this.labelAuthorizedItemName.Text = "Name";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 538);
            this.Controls.Add(this.groupBoxAuthorizationDecisions);
            this.Controls.Add(this.textBoxComputerId);
            this.Controls.Add(this.labelComputerIdentifier);
            this.Controls.Add(this.labelWelcome);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "10Duke Client Sample";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.groupBoxAuthorizationDecisions.ResumeLayout(false);
            this.groupBoxAuthorizationDecisions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWelcome;
        private System.Windows.Forms.Label labelComputerIdentifier;
        private System.Windows.Forms.TextBox textBoxComputerId;
        private System.Windows.Forms.GroupBox groupBoxAuthorizationDecisions;
        private System.Windows.Forms.Label labelAuthorizedItemName;
        private System.Windows.Forms.TextBox textBoxAuthorizedItemName;
        private System.Windows.Forms.ComboBox comboBoxConsumeMode;
        private System.Windows.Forms.Button buttonRequestAuthorizationDecision;
        private System.Windows.Forms.ListView listViewAuthorizationDecisions;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderGranted;
        private System.Windows.Forms.ColumnHeader columnHeaderData;
        private System.Windows.Forms.Button buttonShowData;
        private System.Windows.Forms.Button buttonReleaseLicense;
    }
}

