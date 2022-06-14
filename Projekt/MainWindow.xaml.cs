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
using System.Windows.Markup;
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
            FrameworkElement.LanguageProperty.OverrideMetadata(
                   typeof(FrameworkElement),
                   new FrameworkPropertyMetadata(XmlLanguage.GetLanguage("de-DE")));
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
            if (oneColumnChoice.ShowDialog() == true)
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
            firstColumnValues.ForEach(f => textToWrite.Add(string.Format("{0:0.00}", f).Replace(',', '.')));

            textToWrite.Add($"{secondColumn}");
            secondColumnValues.ForEach(s => textToWrite.Add(string.Format("{0:0.00}", s).Replace(',', '.')));

            File.WriteAllLines($"TwoDimensionData.txt", textToWrite);

            MessageBox.Show("File generated");
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
            firstColumnValues.ForEach(f => textToWrite.Add(string.Format("{0:0.00}", f).Replace(',', '.')));

            textToWrite.Add($"{secondColumn}");
            secondColumnValues.ForEach(s => textToWrite.Add(string.Format("{0:0.00}", s).Replace(',', '.')));

            textToWrite.Add($"{thirdColumn}");
            thirdColumnValues.ForEach(t => textToWrite.Add(string.Format("{0:0.00}", t).Replace(',', '.')));

            File.WriteAllLines($"ThreeDimensionData.txt", textToWrite);

            MessageBox.Show("File generated");
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
            while (minValueInRange < maxValue)
            {
                var count = minValueInRange == minValue || maxValueInRange == maxValue
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

            ranges.ForEach(r => textToWrite.Add($"{string.Format("{0:0.00}", r.MinValue).Replace(',', '.')}" +
                $"-{string.Format("{0:0.00}", r.MaxValue).Replace(',', '.')} {r.Count}"));

            File.WriteAllLines($"HistogramContinuousValues.txt", textToWrite);

            MessageBox.Show("File generated");
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

            foreach (var distinctValue in distinctFloatValues)
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

            discreteValues.ForEach(d => textToWrite.Add($"{string.Format("{0:0.00}", d.Value).Replace(',', '.')} {d.Count}"));

            File.WriteAllLines($"HistogramDiscreteValues.txt", textToWrite);

            MessageBox.Show("File generated");
        }

        private void Classification_Click(object sender, RoutedEventArgs e)
        {
            GetNumberFromUser getNumberOfColumns = new GetNumberFromUser("Enter number of columns for classification");

            int numberOfColumns = 0;
            if (getNumberOfColumns.ShowDialog() == true)
            {
                numberOfColumns = (int)getNumberOfColumns.result;
            }

            string[] columnNames = new string[numberOfColumns];
            for (int i = 0; i < numberOfColumns; i++)
            {
                OneColumnChoice columnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());
                string columnName = string.Empty;
                if (columnChoice.ShowDialog() == true)
                {
                    columnName = columnChoice.result;
                }

                columnNames[i] = columnName;
            }

            MessageBox.Show("Pick column containing class (must have int values)");
            OneColumnChoice classColumnChoice = new OneColumnChoice(dataTableHelper.GetColumnsNames());

            string classColumn = string.Empty;
            if (classColumnChoice.ShowDialog() == true)
            {
                classColumn = classColumnChoice.result;
            }

            var classValues = dataTableHelper.GetAllValuesFromColumn(classColumn);
            var numberOfRecords = classValues.Count;
            var columnValuesArray = new ColumnValues[numberOfColumns];

            for (int columnIndex = 0; columnIndex < numberOfColumns; columnIndex++)
            {
                var columnValues = dataTableHelper.GetAllValuesFromColumn(columnNames[columnIndex]);
                var columnValuesElementInArray = new ColumnValues
                {
                    ClassificationModels = new ClassificationModel[numberOfRecords],
                    ColumnName = columnNames[columnIndex]
                };

                for (int recordIndex = 0; recordIndex < numberOfRecords; recordIndex++)
                {
                    columnValuesElementInArray.ClassificationModels[recordIndex] = new ClassificationModel
                    {
                        ColumnValue = columnValues[recordIndex],
                        ClassValue = (int)classValues[recordIndex]
                    };
                }

                columnValuesArray[columnIndex] = columnValuesElementInArray;
            }

            var intClassValues = classValues.Select(c => (int)c).ToArray();
            var classificationIntersections = ClassificationHelper.Classify(columnValuesArray, intClassValues.Distinct().ToArray());
            
            foreach(var classificationIntersection in classificationIntersections)
            {
                classificationIntersection.IntersectionPoints 
                    = classificationIntersection.IntersectionPoints.Distinct().OrderBy(i => i).ToList();
            }

            var numberOfHyperPlanes = 1;
            var multiplier = 1;
            foreach (var classificationIntersection in classificationIntersections)
            {
                numberOfHyperPlanes += classificationIntersection.IntersectionPoints.Count * multiplier;
                if (classificationIntersection.IntersectionPoints.Count != 0)
                {
                    multiplier = numberOfHyperPlanes; 
                }
            }

            var binaryVectorLength = numberOfHyperPlanes == 2
                ? 1
                : (int)Math.Ceiling(Math.Sqrt(numberOfHyperPlanes));

            var classificationResults = new ClassificationResult[numberOfRecords];
            for(int i = 0; i< numberOfRecords; i++)
            {
                classificationResults[i] = new ClassificationResult();
            }

            for(int recordIndex = 0; recordIndex < numberOfRecords; recordIndex++)
            {
                var classValue = intClassValues[recordIndex];
                classificationResults[recordIndex].ClassValue = classValue;

                var nonBinaryCoordinates = 0;
                foreach(var columnValue in columnValuesArray)
                {
                    var coordinatesMultiplier = nonBinaryCoordinates + 1;
                    var classificationIntersection = classificationIntersections.First(c => c.ColumnName == columnValue.ColumnName);
                    var floatValueFromColumn = columnValue.ClassificationModels[recordIndex].ColumnValue;

                    for(int intersectionIndex = 0; intersectionIndex < classificationIntersection.IntersectionPoints.Count;
                        intersectionIndex++)
                    {
                        if(floatValueFromColumn >= classificationIntersection.IntersectionPoints[intersectionIndex])
                        {
                            nonBinaryCoordinates += 1*coordinatesMultiplier;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                var binaryCoordinates = Convert.ToString(nonBinaryCoordinates, 2);
                var binaryBuilder = new StringBuilder();

                for(int i = 0; i < binaryVectorLength - binaryCoordinates.Length; i++)
                {
                    binaryBuilder.Append("0");
                }
                binaryBuilder.Append(binaryCoordinates);

                classificationResults[recordIndex].BinaryCoordinates = string.Join(' ', binaryBuilder.ToString().ToArray());
            }

            var classificationTextToWrite = new List<string>();
            classificationTextToWrite.Add(intClassValues.Distinct().Count().ToString());
            classificationTextToWrite.Add(binaryVectorLength.ToString());
            foreach(var classificationResult in classificationResults)
            {
                classificationTextToWrite.Add($"{classificationResult.ClassValue};{classificationResult.BinaryCoordinates}");
            }

            File.WriteAllLines("ClassificationResult.txt", classificationTextToWrite);
            MessageBox.Show("Classification file saved");

            if (numberOfColumns == 2)
            {
                var firstColumnValues = columnValuesArray[0].ClassificationModels;
                var secondColumnValues = columnValuesArray[1].ClassificationModels;

                var textToWrite = new List<string>();

                textToWrite.Add($"{firstColumnValues.Count()}");

                textToWrite.Add($"{columnValuesArray[0].ColumnName}");
                foreach(var firstColumnValue in firstColumnValues)
                {
                    textToWrite.Add(string.Format("{0:0.00}", firstColumnValue.ColumnValue).Replace(',','.'));
                }

                textToWrite.Add($"{columnValuesArray[1].ColumnName}");
                foreach (var secondColumnValue in secondColumnValues)
                {
                    textToWrite.Add(string.Format("{0:0.00}", secondColumnValue.ColumnValue).Replace(',', '.'));
                }

                textToWrite.Add("Number of classes");
                textToWrite.Add(intClassValues.Distinct().Count().ToString());

                textToWrite.Add("Classes");
                foreach (var secondColumnValue in secondColumnValues)
                {
                    textToWrite.Add(secondColumnValue.ClassValue.ToString());
                }

                textToWrite.Add("First Column Intersections");
                foreach (var firstColumnIntersection in classificationIntersections[0].IntersectionPoints)
                {
                    textToWrite.Add(string.Format("{0:0.00}", firstColumnIntersection).Replace(',', '.'));
                }

                textToWrite.Add("Second Column Intersections");
                foreach (var secondColumnIntersection in classificationIntersections[1].IntersectionPoints)
                {
                    textToWrite.Add(string.Format("{0:0.00}", secondColumnIntersection).Replace(',', '.'));
                }

                File.WriteAllLines($"ClassificationDataForChart.txt", textToWrite);

                MessageBox.Show("File for classification chart saved");
            }


        }
    }
}
