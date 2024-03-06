using System.Windows.Forms;
using Tenduke.Client.EntApi.Authz;

namespace Tenduke.Client.WinFormsSample.NetFramework
{
    /// <summary>
    /// Form for displaying details of an <see cref="AuthorizationDecision"/>.
    /// </summary>
    public partial class AuthorizationDecisionDetailsForm : Form
    {
        /// <summary>
        /// The <see cref="AuthorizationDecision"/> to show.
        /// </summary>
        public AuthorizationDecision AuthorizationDecision { get; set; }

        public AuthorizationDecisionDetailsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the form is shown.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void AuthorizationDecisionDetailsForm_Shown(object sender, System.EventArgs e)
        {
            if (AuthorizationDecision == null)
            {
                textBoxDetails.Text = "";
            }
            else
            {
                textBoxDetails.Text = AuthorizationDecision.ToString();
            }
        }
    }
}
