using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Utils
{
    public class ClassificationModel
    {
        public InitialValue[] InitialValues { get; set; }

        public int ClassValue { get; set; }

        public int[] BinaryValues { get; set; }

        public ClassificationModel(int numberOfColumns, int classValue)
        {
            InitialValues = new InitialValue[numberOfColumns];
            ClassValue = classValue;
        }
    }
}
