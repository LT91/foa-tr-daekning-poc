using System;

namespace TrDaekning.Klasser
{
    internal class Medlem
    {
        public int Id { get; set; }
        public string Navn { get; set; } = "";
        public string Lokalafdeling { get; set; } = "";

        public int ArbejdsstedId { get; set; }
        public int OverenskomstId { get; set; }

        // Constructor til JSON-deserialisering
        public Medlem() { }

        // Constructor til manuel oprettelse/validering
        public Medlem(int id, string navn, string lokalafdeling, int arbejdsstedId, int overenskomstId)
        {
            if (id <= 0) throw new ArgumentException("Id skal være positiv.");
            if (string.IsNullOrWhiteSpace(navn)) throw new ArgumentException("Navn må ikke være tomt.");
            if (string.IsNullOrWhiteSpace(lokalafdeling)) throw new ArgumentException("Lokalafdeling må ikke være tom.");

            Id = id;
            Navn = navn;
            Lokalafdeling = lokalafdeling;
            ArbejdsstedId = arbejdsstedId;
            OverenskomstId = overenskomstId;
        }
    }
}