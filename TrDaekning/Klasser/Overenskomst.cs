using System;

namespace TrDaekning.Klasser
{
    internal class Overenskomst
    {
        public int Id { get; set; }
        public string Navn { get; set; } = "";

        // Constructor til JSON-deserialisering
        public Overenskomst() { }

        // Constructor til manuel oprettelse/validering
        public Overenskomst(int id, string navn)
        {
            if (id <= 0) throw new ArgumentException("Id skal være positiv.");
            if (string.IsNullOrWhiteSpace(navn)) throw new ArgumentException("Navn må ikke være tomt.");

            Id = id;
            Navn = navn;
        }
    }
}