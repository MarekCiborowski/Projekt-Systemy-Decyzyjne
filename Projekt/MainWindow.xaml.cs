using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Projekt.Models;
using Projekt.Utils;
using Projekt.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        // pierwsza lista - lista wszystkich rekordów
        // druga lista - lista wartości(kolumn) danego rekordu
        // para klucz wartość - połączenie nazwy kolumny z wartością pojedynczego rekordu
        private List<List<KeyValuePair<string, object>>> csvData;
        private DataTableHelper dataTableHelper = new DataTableHelper();

        public MainWindow()
        {
            this.DataContext = csvData;
            InitializeComponent();
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new();
            fileDialog.Filter = "CSV files(*.csv; )|*.csv;" + "|All files (*.*)|*.*";
            fileDialog.CheckFileExists = true;
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    string path = new Uri(fileDialog.FileName).AbsolutePath.Replace("%20", " ");
                    FilePathTextBox.Text = path;

                }
                catch (Exception exc)
                {
                    _ = MessageBox.Show(exc.Message);
                }
            }

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                DetectColumnCountChanges = true,
                BadDataFound = null,
                WhiteSpaceChars = new char[] { ' ', '\t' },
                TrimOptions = TrimOptions.Trim,
            };

            using (StreamReader reader = new StreamReader(FilePathTextBox.Text))
            using (CsvReader csv = new CsvReader(reader, csvConfiguration))
            {
                csvData = csv
                    .GetRecords<dynamic>()
                    .Select(r => new List<KeyValuePair<string, object>>(r))
                    .ToList();

                dataGrid.ItemsSource = dataTableHelper.GetDataTableFromCsvData(csvData).DefaultView;
            }
        }


        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int rowIndex = ((DataGrid)sender).ItemContainerGenerator.IndexFromContainer(e.Row);
            int colIndex = e.Column.DisplayIndex;
            var newValue = ((TextBox)e.EditingElement).Text;
            dataGrid.ItemsSource = dataTableHelper.UpdateDataTable(rowIndex, colIndex, newValue).DefaultView;
        }

        private void ChangeTextToNumber_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice oneColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string selectedColumn = string.Empty;
            if(oneColumnChoice.ShowDialog() == true)
            {
                selectedColumn = oneColumnChoice.result;
            }

            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = dataTableHelper.AddNewTextToNumberColumn(selectedColumn).DefaultView;
        }

        private void Discretize_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice oneColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string selectedColumn = string.Empty;
            if (oneColumnChoice.ShowDialog() == true)
            {
                selectedColumn = oneColumnChoice.result;
            }

            GetNumberFromUser getNumberFromUser = new GetNumberFromUser("Enter number of bins");
            int numberOfBins = 0;
            if (getNumberFromUser.ShowDialog() == true)
            {
                numberOfBins = (int)getNumberFromUser.result;
            }

            dataGrid.ItemsSource = null;
            try
            {
                dataGrid.ItemsSource = dataTableHelper.DiscretizeColumn(selectedColumn, numberOfBins).DefaultView;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Cannot discretize string value");
                dataGrid.ItemsSource = dataTableHelper.GetDataTable().DefaultView;
            }
        }

        private void Standardize_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice oneColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string selectedColumn = string.Empty;
            if (oneColumnChoice.ShowDialog() == true)
            {
                selectedColumn = oneColumnChoice.result;
            }

            dataGrid.ItemsSource = null;
            try
            {
                dataGrid.ItemsSource = dataTableHelper.StandardizeColumn(selectedColumn).DefaultView;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Cannot standardize string value");
                dataGrid.ItemsSource = dataTableHelper.GetDataTable().DefaultView;
            }
        }


        private void ScaleRange_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice oneColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string selectedColumn = string.Empty;
            if (oneColumnChoice.ShowDialog() == true)
            {
                selectedColumn = oneColumnChoice.result;
            }

            GetNumberFromUser getNewMinimumFromUser = new GetNumberFromUser("Enter new minimal value");
            float minValue = 0;
            if (getNewMinimumFromUser.ShowDialog() == true)
            {
                minValue = getNewMinimumFromUser.result;
            }

            GetNumberFromUser getNewMaximumFromUser = new GetNumberFromUser("Enter new maximum value");
            float maxValue = 0;
            if (getNewMaximumFromUser.ShowDialog() == true)
            {
                maxValue = getNewMaximumFromUser.result;
            }

            dataGrid.ItemsSource = null;
            try
            {
                dataGrid.ItemsSource = dataTableHelper.ScaleRangeOfColumn(selectedColumn, minValue, maxValue).DefaultView;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Cannot change range of string value");
                dataGrid.ItemsSource = dataTableHelper.GetDataTable().DefaultView;
            }
        }
    }
} 
