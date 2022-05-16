using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Utils
{
    public class MessageBoxHelper
    {
        public string ChooseColumnsMessage(int numberOfColumns)
        {
            return $"Number of columns that needs to be selected: {numberOfColumns}";
        }
    }
}
