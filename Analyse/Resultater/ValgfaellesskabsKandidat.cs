using System;
using System.Collections.Generic;
using System.Text;

namespace TrDaekning.Analyse.Resultater
{
    internal class ValgfaellesskabsKandidat
    {
        public int OverenskomstId { get; }
        public List<int> ArbejdsstedIds { get; }
        public int SamletAntalMedlemmer { get; }

        public ValgfaellesskabsKandidat(int overenskomstId, List<int> arbejdsstedIds, int samletAntalMedlemmer)
        {
            OverenskomstId = overenskomstId;
            ArbejdsstedIds = arbejdsstedIds;
            SamletAntalMedlemmer = samletAntalMedlemmer;
        }
    }
}