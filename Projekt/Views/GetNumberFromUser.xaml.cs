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

namespace Projekt.Views
{
    /// <summary>
    /// Interaction logic for GetNumberFromUser.xaml
    /// </summary>
    public partial class GetNumberFromUser : Window
    {
        public GetNumberFromUser(string title)
        {
            Title = title;
            InitializeComponent();
        }

        public float result
        {
            get { return float.Parse(TextBox.Text); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(!float.TryParse(TextBox.Text, out _))
            {
                MessageBox.Show("Enter valid number");
            }
            else
            {
                Window.GetWindow(this).DialogResult = true;
                Window.GetWindow(this).Close();
            }
        }
    }
}
