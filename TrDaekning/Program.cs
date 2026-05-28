using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrDaekning.Analyse;
using TrDaekning.Analyse.Resultater;
using TrDaekning.Data;
using TrDaekning.Klasser;

namespace TrDaekning
{
    internal class Program
    {
        // JSON indlæsning 
        private static (
            List<Medlem> medlemmer,
            List<Arbejdssted> arbejdssteder,
            List<Overenskomst> overenskomster,
            List<Tillidsrepraesentant> tillidsrepraesentanter
        ) IndlaesTestdataFraJson()
        {
            string basePath = AppContext.BaseDirectory;
            string dataDir = Path.Combine(basePath, "Data");

            var arbejdssteder = FilIndlaeser.LaesListe<Arbejdssted>(Path.Combine(dataDir, "arbejdssteder.json"));
            var medlemmer = FilIndlaeser.LaesListe<Medlem>(Path.Combine(dataDir, "medlemmer.json"));
            var overenskomster = FilIndlaeser.LaesListe<Overenskomst>(Path.Combine(dataDir, "overenskomster.json"));
            var tillidsrepraesentanter = FilIndlaeser.LaesListe<Tillidsrepraesentant>(Path.Combine(dataDir, "tillidsrepraesentanter.json"));

            return (medlemmer, arbejdssteder, overenskomster, tillidsrepraesentanter);
        }

        // Validering af JSON data
        static void ValidateData(
List<Medlem> medlemmer,
List<Arbejdssted> arbejdssteder,
List<Overenskomst> overenskomster,
List<Tillidsrepraesentant> tillidsrepraesentanter)
        {
            if (arbejdssteder.Any(a => a.Id <= 0)) throw new Exception("Ugyldigt ArbejdsstedId (<=0).");
            if (medlemmer.Any(m => m.Id <= 0)) throw new Exception("Ugyldigt MedlemId (<=0).");
            if (overenskomster.Any(o => o.Id <= 0)) throw new Exception("Ugyldigt OverenskomstId (<=0).");

            if (medlemmer.Any(m => string.IsNullOrWhiteSpace(m.Navn))) throw new Exception("Medlem mangler navn.");
            if (arbejdssteder.Any(a => string.IsNullOrWhiteSpace(a.Navn))) throw new Exception("Arbejdssted mangler navn.");

            if (tillidsrepraesentanter.Any(tr => tr.DaekkedeArbejdssteder == null || tr.DaekkedeArbejdssteder.Count == 0))
                throw new Exception("TR mangler dækkede arbejdssteder.");

            if (tillidsrepraesentanter.Any(tr => tr.DaekkedeOverenskomster == null || tr.DaekkedeOverenskomster.Count == 0))
                throw new Exception("TR mangler dækkede overenskomster.");
        }

        // Fallback: Hardcoded testdata (bruges hvis JSON-indlæsning fejler)
        private static (
            List<Medlem> medlemmer,
            List<Arbejdssted> arbejdssteder,
            List<Overenskomst> overenskomster,
            List<Tillidsrepraesentant> tillidsrepraesentanter
        ) OpretTestdataHardcoded()
        {
            var arbejdssteder = new List<Arbejdssted>
            {
                new Arbejdssted(101, "Plejecenter Solsiden", "Solsiden 1", "Plejecenter", "København", 55.7065, 12.5616),
                new Arbejdssted(102, "Børnehuset Regnbuen", "Regnbuevej 12", "Dagtilbud", "København", 55.6572, 12.5940),
                new Arbejdssted(201, "Plejecenter Skovgården", "Skovvej 5", "Plejecenter", "Roskilde", 55.6415, 12.0878)
            };

            var overenskomster = new List<Overenskomst>
            {
                new Overenskomst(1, "FOA Overenskomst A"),
                new Overenskomst(2, "FOA Overenskomst B"),
                new Overenskomst(3, "FOA Overenskomst C")
            };

            var medlemmer = new List<Medlem>
            {
                new Medlem(1, "Anna Hansen", "København", 101, 1),
                new Medlem(2, "Bo Jensen", "København", 101, 1),
                new Medlem(3, "Cecilie Lund", "København", 101, 2),

                new Medlem(4, "David Madsen", "København", 102, 2),
                new Medlem(5, "Eva Holm", "København", 102, 2),

                new Medlem(6, "Frank Sørensen", "Roskilde", 201, 3),
                new Medlem(7, "Grethe Nielsen", "Roskilde", 201, 3)
            };

            var tillidsrepraesentanter = new List<Tillidsrepraesentant>
            {
                new Tillidsrepraesentant(
                    medlemId: 1,
                    fraDato: new DateTime(2025, 1, 1),
                    tilDato: new DateTime(2025, 12, 31),
                    daekkedeArbejdssteder: new List<int> { 101 },
                    daekkedeOverenskomster: new List<int> { 1 }),

                new Tillidsrepraesentant(
                    medlemId: 4,
                    fraDato: new DateTime(2025, 3, 1),
                    tilDato: null,
                    daekkedeArbejdssteder: new List<int> { 102 },
                    daekkedeOverenskomster: new List<int> { 2 })
            };

            return (medlemmer, arbejdssteder, overenskomster, tillidsrepraesentanter);
        }



        //  ## Main 
        static void Main(string[] args)
        {

            DateTime dato = new DateTime(2026, 1, 15);

            (
                List<Medlem> medlemmer,
                List<Arbejdssted> arbejdssteder,
                List<Overenskomst> overenskomster,
                List<Tillidsrepraesentant> tillidsrepraesentanter
            ) data;

            try
            {
                data = IndlaesTestdataFraJson();
                Console.WriteLine("Data indlæst fra JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kunne ikke indlæse JSON-data. Fallback til hardcoded testdata.");
                Console.WriteLine($"Årsag: {ex.Message}");
                data = OpretTestdataHardcoded();
            }

            var beregner = new AnalyseBeregner(
                data.medlemmer,
                data.arbejdssteder,
                data.overenskomster,
                data.tillidsrepraesentanter);


            // Opslag disctinaries til pæne udskrifter
            var arbejdsstederById = data.arbejdssteder.ToDictionary(a => a.Id);
            var overenskomsterById = data.overenskomster.ToDictionary(o => o.Id);

            Console.WriteLine();
            PrintMedlemsDaekning(beregner, dato);

            Console.WriteLine();
            PrintArbejdsstederDaekning(beregner, dato);

            Console.WriteLine();
            PrintValgfaellesskaber(beregner, dato, arbejdsstederById, overenskomsterById);

            Console.WriteLine("\nTryk en tast for at afslutte...");
            Console.ReadKey();
        }

        // Udskrift: medlemsdækning 
        private static void PrintMedlemsDaekning(AnalyseBeregner beregner, DateTime dato)
        {
            Console.WriteLine("## Medlemsdækning ");
            var r = beregner.BeregnMedlemsDaekning(dato);

            Console.WriteLine($"Total medlemmer: {r.Total}");
            Console.WriteLine($"Dækkede medlemmer: {r.Daekkede} ({r.AndelDaekkede:P1})");
            Console.WriteLine($"Ikke dækkede medlemmer: {r.IkkeDaekkede}");
            Console.WriteLine($"Ikke dækkede men TR på arbejdssted: {r.IkkeDaekkedeMedTRPaaArbejdssted}");
        }

        // Udskrift: arbejdsstedsdækning 
        private static void PrintArbejdsstederDaekning(AnalyseBeregner beregner, DateTime dato)
        {
            Console.WriteLine("## Arbejdsstedsdækning");

            var resultater = beregner.BeregnArbejdsstederDaekning(dato);

            foreach (var r in resultater.OrderBy(x => x.ArbejdsstedId))
            {
                Console.WriteLine(
                    $"{r.ArbejdsstedId} - {r.ArbejdsstedNavn} ({r.ArbejdsstedType}) | " +
                    $"Dækning: {r.Daekning} | Medlemmer: {r.AntalMedlemmer}");
            }
        }

        // Udskrift: Valgfællesskaber 
        private static void PrintValgfaellesskaber(
            AnalyseBeregner beregner,
            DateTime dato,
            Dictionary<int, Arbejdssted> arbejdsstederById,
            Dictionary<int, Overenskomst> overenskomsterById)
        {
            Console.WriteLine("## Valgfællesskaber");

            var kandidater = beregner.FindValgfaellesskaber(dato);

            if (kandidater.Count == 0)
            {
                Console.WriteLine("Ingen kandidater fundet.");
                return;
            }

            foreach (var k in kandidater)
            {
                string overenskomstNavn = overenskomsterById.ContainsKey(k.OverenskomstId)
                    ? overenskomsterById[k.OverenskomstId].Navn
                    : $"OverenskomstId={k.OverenskomstId}";

                var steder = k.ArbejdsstedIds
                    .Select(id => arbejdsstederById.ContainsKey(id) ? arbejdsstederById[id].Navn : $"ArbejdsstedId={id}");

                Console.WriteLine(
                    $"Overenskomst: {overenskomstNavn} | " +
                    $"Arbejdssteder: {string.Join(", ", steder)} | " +
                    $"Samlet medlemmer: {k.SamletAntalMedlemmer}");
            }
        }
    }
}