using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt
{
    /// <summary>qq
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<dynamic> csvData;

        public MainWindow()
        {
            this.DataContext = csvData;
            InitializeComponent();
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                DetectColumnCountChanges = true,
                BadDataFound = null,
                WhiteSpaceChars = new char[] { ' ', '\t' },
                TrimOptions = TrimOptions.Trim,
            };

            using (var reader = new StreamReader("D:\\Studia mgr\\Obliczenia naukowe\\4\\cities.csv"))
            using (var csv = new CsvReader(reader, csvConfiguration))
            {
                csvData = csv
                    .GetRecords<dynamic>()
                    .Select(r => new List<KeyValuePair<string, object>>(r))
                    .ToList();

                IEnumerable<KeyValuePair<string, object>> xd = csvData.First();
                var list = new List<object>() { xd.First().Value, 65 };

                StringBuilder message = new StringBuilder();

                //foreach (var xd in csvData)
                //{
                //    IEnumerable<KeyValuePair<string, object>> xa = xd;
                //    var x = xa.ToList();
                //    foreach (var d in x)
                //    {
                //        message.Append($"{d.Key} - {d.Value}\n");
                //    }

                //    MessageBox.Show(message.ToString(), "Yee boi", MessageBoxButton.OK);

                //}



                //csv.Read();
                //csv.ReadHeader();
                //while (csv.Read())
                //{
                //    var record = csv.GetRecord<dynamic>();
                //    var x = record.GetType();
                //    foreach (var v in x as IDictionary<string, object>)
                //    {
                //        string key = v.Key;
                //        object value = v.Value;
                //    }
                //    // Do something with the record.
                //}

                dataGrid.ItemsSource = new List<object> { 1 };
            }

        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
