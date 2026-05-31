using System;
using System.Collections.Generic;
using System.Linq;
using TrDaekning.Analyse.Resultater;
using TrDaekning.Klasser;

namespace TrDaekning.Analyse
{
    internal class AnalyseBeregner
    {
        private List<Medlem> _medlemmer;
        private List<Arbejdssted> _arbejdssteder;
        private List<Overenskomst> _overenskomster;
        private List<Tillidsrepraesentant> _tillidsrepraesentanter;

        public AnalyseBeregner(
            List<Medlem> medlemmer,
            List<Arbejdssted> arbejdssteder,
            List<Overenskomst> overenskomster,
            List<Tillidsrepraesentant> tillidsrepraesentanter)
        {
            _medlemmer = medlemmer ?? throw new ArgumentNullException(nameof(medlemmer));
            _arbejdssteder = arbejdssteder ?? throw new ArgumentNullException(nameof(arbejdssteder));
            _overenskomster = overenskomster ?? throw new ArgumentNullException(nameof(overenskomster));
            _tillidsrepraesentanter = tillidsrepraesentanter ?? throw new ArgumentNullException(nameof(tillidsrepraesentanter));
        }


        // Hjælpefunktioner
        private List<Tillidsrepraesentant> AktiveTR(DateTime dato)
        {
            return _tillidsrepraesentanter.Where(tr => tr.ErAktiv(dato)).ToList();
        }

        private List<Medlem> MedlemmerPaaArbejdssted(int arbejdsstedId)
        {
            return _medlemmer.Where(m => m.ArbejdsstedId == arbejdsstedId).ToList();
        }

        private HashSet<int> RelevanteOverenskomsterPaaArbejdssted(int arbejdsstedId)
        {
            return MedlemmerPaaArbejdssted(arbejdsstedId)
                .Select(m => m.OverenskomstId)
                .ToHashSet();
        }


        private static double BeregnAfstandKm(double breddegrad1, double laengdegrad1, double breddegrad2, double laengdegrad2)
        {
            const double jordRadiusKm = 6371.0;

            // Omregn fra grader til radianer 
            double breddegrad1Rad = GraderTilRadianer(breddegrad1);
            double breddegrad2Rad = GraderTilRadianer(breddegrad2);
            double laengdegrad1Rad = GraderTilRadianer(laengdegrad1);
            double laengdegrad2Rad = GraderTilRadianer(laengdegrad2);

            
            double x = (laengdegrad2Rad - laengdegrad1Rad) * Math.Cos((breddegrad1Rad + breddegrad2Rad) / 2.0);
            double y = (breddegrad2Rad - breddegrad1Rad);

            // Afstand i km
            return Math.Sqrt(x * x + y * y) * jordRadiusKm;
        }

        private static double GraderTilRadianer(double grader)
        {
            return grader * (Math.PI / 180.0);
        }  


        // ## Medlemsniveau
        public bool ErMedlemDaekket(Medlem medlem, DateTime dato)
        {
            return AktiveTR(dato).Any(tr => tr.Daekker(medlem.ArbejdsstedId, medlem.OverenskomstId));
        }

        public bool HarArbejdsstedTR(int arbejdsstedId, DateTime dato)
        {
            return AktiveTR(dato).Any(tr => tr.HarTRPaaArbejdssted(arbejdsstedId));
        }

        public MedlemsDaekningsResultat BeregnMedlemsDaekning(DateTime dato, string? lokalafdelingFilter = null)
        {
            var medlemmer = _medlemmer;
            if (!string.IsNullOrWhiteSpace(lokalafdelingFilter))
                medlemmer = medlemmer.Where(m => m.Lokalafdeling == lokalafdelingFilter).ToList();

            int total = 0;
            int daekkede = 0;
            int ikkeDaekkede = 0;
            int ikkeDaekkedeMedTRPaaArbejdssted = 0;

            foreach (var m in medlemmer)
            {
                total++;

                bool daekket = ErMedlemDaekket(m, dato);
                if (daekket)
                {
                    daekkede++;
                }
                else
                {
                    ikkeDaekkede++;
                    if (HarArbejdsstedTR(m.ArbejdsstedId, dato))
                        ikkeDaekkedeMedTRPaaArbejdssted++;
                }
            }

            return new MedlemsDaekningsResultat(total, daekkede, ikkeDaekkede, ikkeDaekkedeMedTRPaaArbejdssted);
        }


        // ## Arbejdsstedsniveau
        public ArbejdsstedsDaekningsType BeregnArbejdsstedsDaekning(int arbejdsstedId, DateTime dato)
        {
            var relevante = RelevanteOverenskomsterPaaArbejdssted(arbejdsstedId);
            if (relevante.Count == 0)
                return ArbejdsstedsDaekningsType.IngenMedlemmer;

            int daekkede = relevante.Count(overenskomstId =>
                AktiveTR(dato).Any(tr => tr.Daekker(arbejdsstedId, overenskomstId)));

            if (daekkede == 0) return ArbejdsstedsDaekningsType.IkkeDaekket;
            if (daekkede == relevante.Count) return ArbejdsstedsDaekningsType.HeltDaekket;
            return ArbejdsstedsDaekningsType.DelvistDaekket;
        }

        public List<ArbejdsstedsDaekningsResultat> BeregnArbejdsstederDaekning(
            DateTime dato,
            string? lokalafdelingFilter = null,
            string? arbejdsstedTypeFilter = null)
        {
            var arbejdssteder = _arbejdssteder;

            if (!string.IsNullOrWhiteSpace(lokalafdelingFilter))
                arbejdssteder = arbejdssteder.Where(a => a.Lokalafdeling == lokalafdelingFilter).ToList();

            if (!string.IsNullOrWhiteSpace(arbejdsstedTypeFilter))
                arbejdssteder = arbejdssteder.Where(a => a.Type == arbejdsstedTypeFilter).ToList();

            var resultater = new List<ArbejdsstedsDaekningsResultat>();

            foreach (var a in arbejdssteder)
            {
                var daekning = BeregnArbejdsstedsDaekning(a.Id, dato);
                int antalMedlemmer = MedlemmerPaaArbejdssted(a.Id).Count;

                resultater.Add(new ArbejdsstedsDaekningsResultat(a.Id, a.Navn, a.Type, daekning, antalMedlemmer));
            }

            return resultater;
        }

        // ## Valgfællesskaber
        public List<ValgfaellesskabsKandidat> FindValgfaellesskaber(DateTime dato, double radiusKm = 1.0, int minimumMedlemmer = 5)
        {
            var kandidater = new List<ValgfaellesskabsKandidat>();

            for (int i = 0; i < _arbejdssteder.Count; i++)
            {
                var a = _arbejdssteder[i];

                // Fjern helt dækkede arbejdssteder
                if (BeregnArbejdsstedsDaekning(a.Id, dato) == ArbejdsstedsDaekningsType.HeltDaekket)
                    continue;

                // Fjern arbejdssteder uden medlemmer
                var medlemmerA = MedlemmerPaaArbejdssted(a.Id);
                if (medlemmerA.Count == 0) continue;


                for (int j = i + 1; j < _arbejdssteder.Count; j++)
                {
                    var b = _arbejdssteder[j];

                    // Fjern helt dækkede arbejdssteder
                    if (BeregnArbejdsstedsDaekning(b.Id, dato) == ArbejdsstedsDaekningsType.HeltDaekket)
                        continue;

                    // Afstand mellem A og B
                    double dist = BeregnAfstandKm(a.Breddegrad, a.Laengdegrad, b.Breddegrad, b.Laengdegrad);
                    if (dist > radiusKm) continue;

                    //Fjern arbejdssteder uden medlemmer
                    var medlemmerB = MedlemmerPaaArbejdssted(b.Id);
                    if (medlemmerB.Count == 0) continue;

                    // Fælles overenskomster
                    var overenskomstA = medlemmerA.Select(m => m.OverenskomstId).Distinct();
                    var overenskomstB = medlemmerB.Select(m => m.OverenskomstId).Distinct();
                    var faellesOverenskomster = overenskomstA.Intersect(overenskomstB);

                    foreach (int overenskomstId in faellesOverenskomster)
                    {
                        int antalA = medlemmerA.Count(m => m.OverenskomstId == overenskomstId);
                        int antalB = medlemmerB.Count(m => m.OverenskomstId == overenskomstId);

                        // Hvert sted skal være under grænsen
                        if (antalA >= minimumMedlemmer || antalB >= minimumMedlemmer)
                            continue;

                        int samlet = antalA + antalB;

                        if (samlet >= minimumMedlemmer)
                        {
                            var ids = new List<int> { a.Id, b.Id }.ToList();
                            kandidater.Add(new ValgfaellesskabsKandidat( overenskomstId, ids, samlet));
                        }
                    }
                }
            }

            return kandidater;
        }



    }
}