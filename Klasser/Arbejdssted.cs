using System;
using System.Collections.Generic;

namespace TrDaekning.Klasser
{
    internal class Arbejdssted
    {
        public int Id { get; set; }
        public string Navn { get; set; } = "";
        public string Adresse { get; set; } = "";
        public string Type { get; set; } = "";
        public string Lokalafdeling { get; set; } = "";

        // Til valgfællesskaber 
        public double Breddegrad { get; set; }
        public double Laengdegrad { get; set; }

        // Constructor Til JSON-deserialisering
        public Arbejdssted() { }

        // Constructor til manuel oprettelse/validering
        public Arbejdssted(
            int id,
            string navn,
            string adresse,
            string type,
            string lokalafdeling,
            double breddegrad,
            double laengdegrad)
        {
            if (id <= 0) throw new ArgumentException("Id skal være positiv.");
            if (string.IsNullOrWhiteSpace(navn)) throw new ArgumentException("Navn må ikke være tomt.");
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Type må ikke være tom.");
            if (string.IsNullOrWhiteSpace(lokalafdeling)) throw new ArgumentException("Lokalafdeling må ikke være tom.");

            adresse ??= "";

            Id = id;
            Navn = navn;
            Adresse = adresse;
            Type = type;
            Lokalafdeling = lokalafdeling;
            Breddegrad = breddegrad;
            Laengdegrad = laengdegrad;
        }
    }
}