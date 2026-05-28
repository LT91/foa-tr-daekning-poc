using System;
using System.Collections.Generic;
using System.Text;

namespace TrDaekning.Analyse.Resultater
{
    internal class ValgfaellesskabsKandidat
    {
       // public string Noegle { get; }
        public int OverenskomstId { get; }
        public List<int> ArbejdsstedIds { get; }
        public int SamletAntalMedlemmer { get; }

        public ValgfaellesskabsKandidat(//string noegle, 
            int overenskomstId, List<int> arbejdsstedIds, int samletAntalMedlemmer)
        {
            //Noegle = noegle;
            OverenskomstId = overenskomstId;
            ArbejdsstedIds = arbejdsstedIds;
            SamletAntalMedlemmer = samletAntalMedlemmer;
        }
    }
}