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
        public DataTable GetDataTableFromCsvData(List<List<KeyValuePair<string,object>>> csvData)
        {
            DataTable data = new();

            // dodanie definicji kolumn
            csvData.First()
                .ForEach(c => data.Columns.Add(c.Key));

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
    }
}
