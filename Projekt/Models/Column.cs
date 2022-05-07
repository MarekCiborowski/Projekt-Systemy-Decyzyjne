using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class Column
    {
        public Type ColumnType { get; set; }

        public string ColumnName { get; set; }

        public List<string> ColumnValues { get; set; }
    }
}
