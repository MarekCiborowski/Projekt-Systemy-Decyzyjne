using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Utils
{
    public static class ClassificationHelper
    {
        private const float StatisticalErrorMaxPercent = .1f;
        private const float MinGroupedRecordsPercent = 0.5f;

        public static List<ClassificationIntersections> Classify(ClassificationModel[] classificationModels, int[] distinctClassValues)
        {
            var minNumberOfIntersections = Math.Ceiling(2 * Math.Sqrt(classificationModels.Count()) - 2);
            var minGroupedRecordsForClass = new int[distinctClassValues.Count()];

            for (int i = 0; i < distinctClassValues.Count(); i++)
            {
                minGroupedRecordsForClass[i] = (int)Math.Ceiling(classificationModels
                    .Where(c => c.ClassValue == distinctClassValues[i]).Count() * MinGroupedRecordsPercent);
            }

            var maxStatisticalError = classificationModels.Count() * StatisticalErrorMaxPercent / (distinctClassValues.Count() - 1);

            for (int classValue = 0; classValue < distinctClassValues.Count(); classValue++)
            {
                var 
            }

            return null;
        }

    }
}
