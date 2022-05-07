using Microsoft.Win32;
using Projekt.Models;
using System;
using System.Collections.Generic;
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
        private List<Property> properties;

        public MainWindow()
        {
            InitializeComponent();
            properties = new List<Property>();
        }

        private void LoadTxtFileButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            List<string> readText;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                readText = File.ReadAllLines(openFileDialog.FileName).ToList();
            }
            else
            {
                throw new Exception("Could not open file dialog");
            }

            bool headerWasRead = false;
            foreach (string line in readText)
            {
                if (line[0] == '#')
                {
                    continue;
                }
                else if (!headerWasRead)
                {
                    string[] headers = line.Split(' ');
                    properties.AddRange(headers.Select(h => new Property(h)));

                    headerWasRead = true;
                }
                else
                {
                    string[] values = line.Split(' ');
                    for(int i = 0; i < properties.Count; i++)
                    {
                        properties[i].Values.Add(values[i]);
                    }
                }
            }

            var x = 0;
                
        }
    }
}
