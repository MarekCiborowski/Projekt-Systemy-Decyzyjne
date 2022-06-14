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
        private const float MinGroupedRecordsPercent = 0.3f;

        public static List<ClassificationIntersections> Classify(ColumnValues[] columnValues, int[] distinctClassValues)
        {
            var minNumberOfIntersections = Math.Ceiling(2 * Math.Sqrt(columnValues[0].ClassificationModels.Count()) - 2);
            var minGroupedRecordsForClass = new Dictionary<int, int>();

            for (int i = 0; i < distinctClassValues.Count(); i++)
            {
                minGroupedRecordsForClass[distinctClassValues[i]] = (int)Math.Ceiling(columnValues[0].ClassificationModels
                    .Where(c => c.ClassValue == distinctClassValues[i]).Count() * MinGroupedRecordsPercent);
            }

            var maxStatisticalError = (int)Math.Ceiling(columnValues[0].ClassificationModels.Count() * StatisticalErrorMaxPercent / (distinctClassValues.Count() - 1));

            var classificationIntersectionsList = new List<ClassificationIntersections>();

            for (int columnIndex = 0; columnIndex < columnValues.Count(); columnIndex++)
            {
                var currentColumnIntersections = new ClassificationIntersections
                {
                    ColumnName = columnValues[columnIndex].ColumnName
                };

                var classAndNumberOfRecordsDictionary = new Dictionary<int, int>();
                ResetClassAndNumberOfRecordsDictionary(classAndNumberOfRecordsDictionary, distinctClassValues);
                var sortedColumnValues = columnValues[columnIndex].ClassificationModels.OrderBy(c => c.ColumnValue).ToArray();

                IterateOverRecords(true, sortedColumnValues, classAndNumberOfRecordsDictionary, currentColumnIntersections,
                    distinctClassValues, minGroupedRecordsForClass, maxStatisticalError);
                IterateOverRecords(false, sortedColumnValues, classAndNumberOfRecordsDictionary, currentColumnIntersections,
                    distinctClassValues, minGroupedRecordsForClass, maxStatisticalError);

                classificationIntersectionsList.Add(currentColumnIntersections);
            }

            return classificationIntersectionsList;
        }

        public static int GetNumberOfRecordsOfDifferentClass(int classValue, Dictionary<int, int> classAndNumberOfRecordsDictionary)
        {
            var result = 0;
            foreach (var key in classAndNumberOfRecordsDictionary.Keys.Where(k => k != classValue))
            {
                result += classAndNumberOfRecordsDictionary[key];
            }

            return result;
        }

        public static void ResetClassAndNumberOfRecordsDictionary(Dictionary<int, int> classAndNumberOfRecordsDictionary, int[] distinctClassValues)
        {
            foreach (var classValue in distinctClassValues)
            {
                classAndNumberOfRecordsDictionary[classValue] = 0;
            }
        }

        public static void IterateOverRecords(bool isAscending,
            ClassificationModel[] sortedColumnValues,
            Dictionary<int, int> classAndNumberOfRecordsDictionary,
            ClassificationIntersections currentColumnIntersections,
            int[] distinctClassValues,
            Dictionary<int,int> minGroupedRecordsForClass,
            int maxStatisticalError
            )
        {
            ClassificationModel previousValue = null;

            int startingColumnIndex = isAscending
                ? 0
                : sortedColumnValues.Length - 1; 

            // idąc od lewej
            for (int sortedColumnValuesIndex = startingColumnIndex;
                (isAscending) ? sortedColumnValuesIndex < sortedColumnValues.Length : sortedColumnValuesIndex >= 0;)
            {
                var classificationModel = sortedColumnValues[sortedColumnValuesIndex];

                classAndNumberOfRecordsDictionary[classificationModel.ClassValue]
                    = classAndNumberOfRecordsDictionary[classificationModel.ClassValue] + 1;

                var classWithHighestNumberOfRecords = GetClassWithHighestNumberOfRecords(classAndNumberOfRecordsDictionary);
                
                var currentNumberOfClassRecords = classAndNumberOfRecordsDictionary[classWithHighestNumberOfRecords];
                var currentStatisticalErrorForClass
                    = GetNumberOfRecordsOfDifferentClass(classWithHighestNumberOfRecords, classAndNumberOfRecordsDictionary);

                // reset w przypadku nie osiągnięcia minimalnego zagęszczenia
                // i przekroczenia minimalnego błędu
                if (currentNumberOfClassRecords < minGroupedRecordsForClass[classWithHighestNumberOfRecords]
                    && currentStatisticalErrorForClass >= maxStatisticalError)
                {
                    ResetClassAndNumberOfRecordsDictionary(classAndNumberOfRecordsDictionary, distinctClassValues);
                }

                //dodanie nowego punktu przecięcia i reset
                else if (currentStatisticalErrorForClass == maxStatisticalError
                    && currentNumberOfClassRecords >= minGroupedRecordsForClass[classWithHighestNumberOfRecords])
                {
                    currentColumnIntersections.IntersectionPoints
                        .Add((previousValue.ColumnValue + classificationModel.ColumnValue) / 2);
                    ResetClassAndNumberOfRecordsDictionary(classAndNumberOfRecordsDictionary, distinctClassValues);
                }

                previousValue = classificationModel;
                if (isAscending)
                {
                    sortedColumnValuesIndex++;
                }
                else
                {
                    sortedColumnValuesIndex--;
                }
            }
        }

        private static int GetClassWithHighestNumberOfRecords(Dictionary<int,int> dict)
        {
            return dict.First(d => d.Value == dict.Values.Max()).Key;
        }
    }
}
