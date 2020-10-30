namespace Tenduke.Client.WinForms
{
    partial class WebBrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebBrowserForm));
            this.panelWebBrowserContainer = new System.Windows.Forms.Panel();
            this.chromiumWebBrowser = new CefSharp.WinForms.ChromiumWebBrowser();
            this.panelWebBrowserContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWebBrowserContainer
            // 
            this.panelWebBrowserContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelWebBrowserContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelWebBrowserContainer.Controls.Add(this.chromiumWebBrowser);
            this.panelWebBrowserContainer.Location = new System.Drawing.Point(0, 0);
            this.panelWebBrowserContainer.Margin = new System.Windows.Forms.Padding(0);
            this.panelWebBrowserContainer.Name = "panelWebBrowserContainer";
            this.panelWebBrowserContainer.Size = new System.Drawing.Size(784, 582);
            this.panelWebBrowserContainer.TabIndex = 0;
            // 
            // chromiumWebBrowser
            // 
            this.chromiumWebBrowser.ActivateBrowserOnCreation = false;
            this.chromiumWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chromiumWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.chromiumWebBrowser.Name = "chromiumWebBrowser";
            this.chromiumWebBrowser.Size = new System.Drawing.Size(784, 582);
            this.chromiumWebBrowser.TabIndex = 0;
            // 
            // WebBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 581);
            this.Controls.Add(this.panelWebBrowserContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WebBrowserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sign on";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebBrowserForm_FormClosing);
            this.Shown += new System.EventHandler(this.WebBrowserForm_Shown);
            this.panelWebBrowserContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelWebBrowserContainer;
        private CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser;
    }
}