using System;
using System.Collections.Generic;

namespace TrDaekning.Klasser
{
    internal class Tillidsrepraesentant
    {
        public int MedlemId { get; set; }
        public DateTime FraDato { get; set; }
        public DateTime? TilDato { get; set; }

        public List<int> DaekkedeArbejdssteder { get; set; } = new();
        public List<int> DaekkedeOverenskomster { get; set; } = new();

        // Constructor til JSON-deserialisering
        public Tillidsrepraesentant() { }

        // Constructor til manuel oprettelse/validering
        public Tillidsrepraesentant(
            int medlemId,
            DateTime fraDato,
            DateTime? tilDato,
            List<int> daekkedeArbejdssteder,
            List<int> daekkedeOverenskomster)
        {
            if (medlemId <= 0) throw new ArgumentException("MedlemId skal være positiv.");

            if (daekkedeArbejdssteder == null || daekkedeArbejdssteder.Count == 0)
                throw new ArgumentException("TR skal dække mindst ét arbejdssted.");

            if (daekkedeOverenskomster == null || daekkedeOverenskomster.Count == 0)
                throw new ArgumentException("TR skal dække mindst én overenskomst.");

            if (tilDato.HasValue && tilDato.Value < fraDato)
                throw new ArgumentException("TilDato kan ikke være før FraDato.");

            MedlemId = medlemId;
            FraDato = fraDato;
            TilDato = tilDato;

            DaekkedeArbejdssteder = daekkedeArbejdssteder;
            DaekkedeOverenskomster = daekkedeOverenskomster;
        }

        public bool ErAktiv(DateTime dato)
        {
            return dato >= FraDato && (!TilDato.HasValue || dato <= TilDato.Value);
        }

        public bool Daekker(int arbejdsstedId, int overenskomstId)
        {
            return DaekkedeArbejdssteder.Contains(arbejdsstedId)
                && DaekkedeOverenskomster.Contains(overenskomstId);
        }

        public bool HarTRPaaArbejdssted(int arbejdsstedId)
        {
            return DaekkedeArbejdssteder.Contains(arbejdsstedId);
        }
    }
}