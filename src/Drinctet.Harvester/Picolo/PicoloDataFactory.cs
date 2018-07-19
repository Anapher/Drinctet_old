using System.Collections.Generic;
using Csv;

namespace Drinctet.Harvester.Picolo
{
    public class PicoloDataFactory
    {
        private readonly string _source;

        public PicoloDataFactory(string source)
        {
            _source = source;
        }

        public IEnumerable<string[]> FetchLines()
        {
            foreach (var csvLine in CsvReader.ReadFromText(_source, new CsvOptions{HeaderMode = HeaderMode.HeaderAbsent}))
            {
                var arr = new string[csvLine.ColumnCount];
                for (var i = 0; i < arr.Length; i++)
                    arr[i] = csvLine[i];

                yield return arr;
            }
        }
    }
}