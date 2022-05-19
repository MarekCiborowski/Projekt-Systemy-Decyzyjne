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

namespace Projekt
{
    /// <summary>
    /// Interaction logic for ChangeTextToNumber.xaml
    /// </summary>
    public partial class OneColumnChoice : Window
    {
        private List<string> columnNames;
        public string result
        {
            get { return listBox.SelectedItem.ToString(); }
        }

        public OneColumnChoice(List<string> columnNames)
        {
            this.columnNames = columnNames;
            
            
            InitializeComponent();
            foreach (var columnName in columnNames)
            {
                listBox.Items.Add(columnName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }
    }
}
