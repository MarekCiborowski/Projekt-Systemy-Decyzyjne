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

        private void MaximumValues_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice oneColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string selectedColumn = string.Empty;
            if (oneColumnChoice.ShowDialog() == true)
            {
                selectedColumn = oneColumnChoice.result;
            }

            GetNumberFromUser getNumberFromUser = new GetNumberFromUser("Enter percentage value");
            int percentageValue = 0;
            if (getNumberFromUser.ShowDialog() == true)
            {
                percentageValue = (int)getNumberFromUser.result;
            }

            dataGrid.ItemsSource = null;
            try
            {
                dataGrid.ItemsSource = dataTableHelper.MaximumValues(selectedColumn, percentageValue).DefaultView;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Cannot get maximum of string value column");
                dataGrid.ItemsSource = dataTableHelper.GetDataTable().DefaultView;
            }
        }

        private void MinimalValues_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice oneColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string selectedColumn = string.Empty;
            if (oneColumnChoice.ShowDialog() == true)
            {
                selectedColumn = oneColumnChoice.result;
            }

            GetNumberFromUser getNumberFromUser = new GetNumberFromUser("Enter percentage value");
            int percentageValue = 0;
            if (getNumberFromUser.ShowDialog() == true)
            {
                percentageValue = (int)getNumberFromUser.result;
            }

            dataGrid.ItemsSource = null;
            try
            {
                dataGrid.ItemsSource = dataTableHelper.MinimalValues(selectedColumn, percentageValue).DefaultView;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Cannot get minimum of string value column");
                dataGrid.ItemsSource = dataTableHelper.GetDataTable().DefaultView;
            }
        }

        private void ExtractTwoDimensionChart_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice firstColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string firstColumn = string.Empty;
            if (firstColumnChoice.ShowDialog() == true)
            {
                firstColumn = firstColumnChoice.result;
            }

            OneColumnChoice secondColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string secondColumn = string.Empty;
            if (secondColumnChoice.ShowDialog() == true)
            {
                secondColumn = secondColumnChoice.result;
            }

            var firstColumnValues = dataTableHelper.GetAllValuesFromColumn(firstColumn);
            var secondColumnValues = dataTableHelper.GetAllValuesFromColumn(secondColumn);

            var textToWrite = new List<string>();

            textToWrite.Add($"{firstColumnValues.Count}");

            textToWrite.Add($"{firstColumn}");
            firstColumnValues.ForEach(f => textToWrite.Add(f.ToString()));

            textToWrite.Add($"{secondColumn}");
            secondColumnValues.ForEach(s => textToWrite.Add(s.ToString()));

            File.WriteAllLines($"TwoDimensionData_{firstColumn}_{secondColumn}.txt", textToWrite);
        }

        private void ExtractThreeDimensionChart_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice firstColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string firstColumn = string.Empty;
            if (firstColumnChoice.ShowDialog() == true)
            {
                firstColumn = firstColumnChoice.result;
            }

            OneColumnChoice secondColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string secondColumn = string.Empty;
            if (secondColumnChoice.ShowDialog() == true)
            {
                secondColumn = secondColumnChoice.result;
            }

            OneColumnChoice thirdColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string thirdColumn = string.Empty;
            if (thirdColumnChoice.ShowDialog() == true)
            {
                thirdColumn = thirdColumnChoice.result;
            }

            var firstColumnValues = dataTableHelper.GetAllValuesFromColumn(firstColumn);
            var secondColumnValues = dataTableHelper.GetAllValuesFromColumn(secondColumn);
            var thirdColumnValues = dataTableHelper.GetAllValuesFromColumn(thirdColumn);

            var textToWrite = new List<string>();

            textToWrite.Add($"{firstColumnValues.Count}");

            textToWrite.Add($"{firstColumn}");
            firstColumnValues.ForEach(f => textToWrite.Add(f.ToString()));

            textToWrite.Add($"{secondColumn}");
            secondColumnValues.ForEach(s => textToWrite.Add(s.ToString()));

            textToWrite.Add($"{thirdColumn}");
            thirdColumnValues.ForEach(t => textToWrite.Add(t.ToString()));

            File.WriteAllLines($"ThreeDimensionData_{firstColumn}_{secondColumn}_{thirdColumn}.txt", textToWrite);
        }

        private class Range
        {
            public float MinValue { get; set; }

            public float MaxValue { get; set; }

            public int Count { get; set; }
        }

        private void ExtractHistogramContinuous_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice firstColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string firstColumn = string.Empty;
            if (firstColumnChoice.ShowDialog() == true)
            {
                firstColumn = firstColumnChoice.result;
            }

            GetNumberFromUser getNumberFromUser = new GetNumberFromUser("Enter number of ranges");
            int numberOfRanges = 0;
            if (getNumberFromUser.ShowDialog() == true)
            {
                numberOfRanges = (int)getNumberFromUser.result;
            }

            var floatValues = dataTableHelper.GetAllValuesFromColumn(firstColumn);

            var minValue = floatValues.Min();
            var maxValue = floatValues.Max();
            var width = (maxValue - minValue) / numberOfRanges;

            var maxValueInRange = minValue + width;
            var minValueInRange = minValue;
            List<Range> ranges = new List<Range>();

            // przedział domknięty z lewej tylko dla pierwszego zakresu
            while(minValueInRange < maxValue)
            {
                var count = minValueInRange == minValue
                    ? floatValues.Count(f => f >= minValueInRange && f <= maxValueInRange)
                    : floatValues.Count(f => f > minValueInRange && f <= maxValueInRange);

                ranges.Add(new Range
                {
                    MinValue = minValueInRange,
                    MaxValue = maxValueInRange,
                    Count = count
                });

                minValueInRange += width;
                maxValueInRange += width;
            }

            var textToWrite = new List<string>();

            textToWrite.Add($"{firstColumn}");
            textToWrite.Add($"{ranges.Count}");

            ranges.ForEach(r => textToWrite.Add($"{string.Format("{0:0.00}",r.MinValue)}-{string.Format("{0:0.00}", r.MaxValue)} {r.Count}"));

            File.WriteAllLines($"HistogramContinuousValues_{firstColumn}_{numberOfRanges}_ranges.txt", textToWrite);
        }

        private class DiscreteHistogramValue
        {
            public float Value { get; set; }

            public int Count { get; set; }
        }

        private void ExtractHistogramDiscrete_Click(object sender, RoutedEventArgs e)
        {
            OneColumnChoice firstColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
            string firstColumn = string.Empty;
            if (firstColumnChoice.ShowDialog() == true)
            {
                firstColumn = firstColumnChoice.result;
            }

            var floatValues = dataTableHelper.GetAllValuesFromColumn(firstColumn);
            var distinctFloatValues = floatValues.Distinct();
            List<DiscreteHistogramValue> discreteValues = new List<DiscreteHistogramValue>();

            foreach(var distinctValue in distinctFloatValues)
            {
                discreteValues.Add(new DiscreteHistogramValue
                {
                    Value = distinctValue,
                    Count = floatValues.Count(f => f == distinctValue)
                });
            }

            var textToWrite = new List<string>();

            textToWrite.Add($"{firstColumn}");
            textToWrite.Add($"{discreteValues.Count}");

            discreteValues.ForEach(d => textToWrite.Add($"{d.Value} {d.Count}"));

            File.WriteAllLines($"HistogramDiscreteValues_{firstColumn}.txt", textToWrite);
        }
    }
} 
