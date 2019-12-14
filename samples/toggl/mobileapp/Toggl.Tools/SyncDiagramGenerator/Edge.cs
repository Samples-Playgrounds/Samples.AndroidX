namespace SyncDiagramGenerator
{
    internal sealed class Edge : ILabeled
    {
        public string Label { get; set; }
        public Node From { get; set; }
        public Node To { get; set; }
    }
}
