using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tenduke.Client.DefaultBrowserSample
{
    /// <summary>
    /// Interaction logic for AuthenticatingMessageWindow.xaml
    /// </summary>
    public partial class AuthenticatingMessageWindow : Window
    {
        public AuthenticatingMessageWindow()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
