using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
namespace ESTutorial.Helpers
{
    public static class FileReader<T>
    {
        public static IEnumerable<T> GetFile(string fileName)
        {
            TextReader reader = new StreamReader($"{fileName}");
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csvReader.GetRecords<T>();

            return records;
        }
    }
}
