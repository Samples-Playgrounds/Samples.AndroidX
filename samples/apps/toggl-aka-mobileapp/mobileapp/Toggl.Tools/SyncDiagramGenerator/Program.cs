using System;
using System.IO;

namespace SyncDiagramGenerator
{
    internal static class Program
    {
        private const string OUTDIRVARIABLE = "SYNC_GRAPH_OUT_DIRECTORY";
        private const string filename = "sync-graph.gv";

        public static void Main(string[] args)
        {
            var (nodes, edges) = new SyncDiagramGenerator().GenerateGraph();

            var outPath = getOutPath();

            new DotFileWriter().WriteToFile(outPath, nodes, edges);

            Console.WriteLine("Done.");
        }

        private static string getOutPath()
        {
            var outDir = Environment.GetEnvironmentVariable(OUTDIRVARIABLE) ?? "./";
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            var outPath = Path.Combine(outDir, filename);
            return outPath;
        }
    }
}
