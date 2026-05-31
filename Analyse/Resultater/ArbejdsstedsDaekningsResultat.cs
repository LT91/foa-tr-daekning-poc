using System;
using System.Collections.Generic;
using System.Text;

namespace TrDaekning.Analyse.Resultater
{
    internal class ArbejdsstedsDaekningsResultat
    {
        public int ArbejdsstedId { get; }
        public string ArbejdsstedNavn { get; }
        public string ArbejdsstedType { get; }
        public ArbejdsstedsDaekningsType Daekning { get; }
        public int AntalMedlemmer { get; }

        public ArbejdsstedsDaekningsResultat(
            int arbejdsstedId,
            string arbejdsstedNavn,
            string arbejdsstedType,
            ArbejdsstedsDaekningsType daekning,
            int antalMedlemmer)
        {
            ArbejdsstedId = arbejdsstedId;
            ArbejdsstedNavn = arbejdsstedNavn;
            ArbejdsstedType = arbejdsstedType;
            Daekning = daekning;
            AntalMedlemmer = antalMedlemmer;
        }
    }
}