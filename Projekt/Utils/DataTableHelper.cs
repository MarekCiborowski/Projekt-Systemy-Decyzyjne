using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Utils
{
    public class DataTableHelper
    {
        private DataTable _dataTable;
        private Dictionary<string,int> columnNamesIndexes = new();

        private int getHighestColumnIndex => columnNamesIndexes.Values.Max();

        public DataTable GetDataTable()
        {
            return _dataTable;
        }

        public DataTable GetDataTableFromCsvData(List<List<KeyValuePair<string,object>>> csvData)
        {
            DataTable data = new();
            columnNamesIndexes = new();

            // dodanie definicji kolumn
            int index = 0;

            foreach(var element in csvData.First())
            {
                data.Columns.Add(element.Key);
                columnNamesIndexes.Add(element.Key, index++);
            }

            foreach(var record in csvData)
            {
                List<object> valuesList = new List<object>();

                record.ForEach(r => valuesList.Add(r.Value));
                _ = data.Rows.Add(valuesList.ToArray());
            }

            _dataTable = data;
            return data;
        }

        public DataTable UpdateDataTable(int rowIndex, int columnIndex, string newValue)
        {
            _dataTable.Rows[rowIndex][columnIndex] = newValue;

            return _dataTable;
        }

        public List<string> GetColumnsNames()
        {
            var columnsNames = new List<string>();
            foreach(var element in _dataTable.Columns)
            {
                columnsNames.Add(element.ToString());
            }

            return columnsNames;
        }

        public DataTable AddNewTextToNumberColumn(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)){
                return _dataTable;
            }

            var newColumnName = $"{columnName}-ToNumber";
            var newColumnIndex = getHighestColumnIndex + 1;
            columnNamesIndexes.Add(newColumnName, newColumnIndex);

            var dict = new Dictionary<string, int>();
            var columnIndex = columnNamesIndexes[columnName];

            var textValues = _dataTable.AsEnumerable().Select(r => r.Field<string>(columnName)).ToList();
            _dataTable.Columns.Add(newColumnName);

            int numberValue = 1;
            int rowIndex = 0;
            foreach (var textValue in textValues)
            {
                if (!dict.ContainsKey(textValue))
                {
                    dict.Add(textValue, numberValue++);
                }

                _dataTable.Rows[rowIndex++][newColumnIndex] = dict[textValue];
            }

            return _dataTable;
        }

        public DataTable DiscretizeColumn(string columnName, int numberOfBins)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return _dataTable;
            }

            var columnIndex = columnNamesIndexes[columnName];
            if (!float.TryParse(_dataTable.Rows[0][columnIndex].ToString(), out _))
            {
                throw new ArgumentException();
            }

            var newColumnName = $"{columnName}-Discretized";
            var newColumnIndex = getHighestColumnIndex + 1;
            columnNamesIndexes.Add(newColumnName, newColumnIndex);

            var floatValues = _dataTable.AsEnumerable()
                .Select(r => r.Field<string>(columnName))
                .Select(r => float.Parse(r))
                .ToList();

            _dataTable.Columns.Add(newColumnName);

            var minValue = floatValues.Min();
            var maxValue = floatValues.Max();
            var width = (maxValue - minValue) / numberOfBins;

            int rowIndex = 0;
            foreach (var floatValue in floatValues)
            {
                int newValue = floatValue == maxValue
                    ? numberOfBins // maksymalna wartość
                    : (int)((floatValue - minValue) / width) + 1;

                _dataTable.Rows[rowIndex++][newColumnIndex] = newValue;
            }

            return _dataTable;
        }
    }
}
