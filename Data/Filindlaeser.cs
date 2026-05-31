using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TrDaekning.Data
{
    internal static class FilIndlaeser
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static List<T> LaesListe<T>(string sti)
        {
            if (!File.Exists(sti))
                throw new FileNotFoundException($"JSON-fil blev ikke fundet: {sti}");

            string json = File.ReadAllText(sti);

            var data = JsonSerializer.Deserialize<List<T>>(json, Options);
            if (data == null)
                throw new InvalidOperationException($"Kunne ikke deserialisere filen: {sti}");

            return data;
        }
    }
}