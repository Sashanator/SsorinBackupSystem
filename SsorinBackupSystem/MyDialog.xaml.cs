using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LabWork_4__REWORKED_
{
    /// <summary>
    /// Логика взаимодействия для MyDialog.xaml
    /// </summary>
    public partial class MyDialog : Window
    {
        public MyDialog()
        {
            InitializeComponent();
        }

        public string ResponseText {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void ClickOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
