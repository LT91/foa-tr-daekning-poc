using System;
using System.Collections.Generic;
using System.Text;

namespace TrDaekning.Analyse.Resultater
{
    internal class MedlemsDaekningsResultat
    {
        public int Total { get; }
        public int Daekkede { get; }
        public int IkkeDaekkede { get; }
        public int IkkeDaekkedeMedTRPaaArbejdssted { get; }

        public double AndelDaekkede
        {
            get
            {
                if (Total == 0) return 0;
                return (double)Daekkede / Total;
            }
        }

        public MedlemsDaekningsResultat(int total, int daekkede, int ikkeDaekkede, int ikkeDaekkedeMedTRPaaArbejdssted)
        {
            Total = total;
            Daekkede = daekkede;
            IkkeDaekkede = ikkeDaekkede;
            IkkeDaekkedeMedTRPaaArbejdssted = ikkeDaekkedeMedTRPaaArbejdssted;
        }
    }
}