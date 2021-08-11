using Proteomics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Proteomics.ProteolyticDigestion;
using UsefulProteomicsDatabases;

namespace FlashIBAQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var uniprotPtms = GetUniProtMods(Environment.CurrentDirectory);
            string sourceXmlPath = @"F:\ProjectsActive\MannData\U2OSAnalysis\2020-04-08-12-20-16NoMetalOccupancyFixV305\Task2-GPTMDTask\human.proteinGPTMD.xml";
            var proteins = ProteinDbLoader.LoadProteinXML(sourceXmlPath, true, DecoyType.None, uniprotPtms, false, null, out var un);
            DigestionParams digestionParams = new DigestionParams(maxMissedCleavages: 0, maxModsForPeptides: 0);
            ProteinDigestion digestion = new ProteinDigestion(digestionParams, null, null);
            List<int> numPeptides = proteins.Select(p => digestion.Digestion(p).Count()).ToList();
            List<string> iBaqProteinDigestionTable = Enumerable.Range(0, proteins.Count).Select(p => $"{proteins[p].Accession}\t{proteins[p].Length}\t{numPeptides[p]}").ToList();
            File.WriteAllLines(@"C:\Users\Anthony\Dropbox\Projects\Nucleoli\ProteinDigestionTable.txt", iBaqProteinDigestionTable);
        }

        /// <summary>
        /// Gets UniProt ptmlist
        /// </summary>
        /// <param name="spritzDirectory"></param>
        /// <returns></returns>
        public static List<Modification> GetUniProtMods(string spritzDirectory)
        {
            Loaders.LoadElements();
            var psiModDeserialized = Loaders.LoadPsiMod(Path.Combine(spritzDirectory, "PSI-MOD.obo.xml"));
            return Loaders.LoadUniprot(Path.Combine(spritzDirectory, "ptmlist.txt"), Loaders.GetFormalChargesDictionary(psiModDeserialized)).ToList();
        }
    }
}
