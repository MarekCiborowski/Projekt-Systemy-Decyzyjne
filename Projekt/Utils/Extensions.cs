using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Utils
{
    public static class Extensions
    {
        public static float StdDev(this IEnumerable<float> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                float avg = values.Average();
                float sum = values.Sum(d => (d - avg) * (d - avg));
                ret = Math.Sqrt(sum / count);
            }
            return (float)ret;
        }
    }
}
