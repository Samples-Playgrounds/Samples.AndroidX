using System.Collections.Generic;

namespace SyncDiagramGenerator
{
    internal sealed class SubGraph
    {
        public Node SourceNode { get; set; }

        public List<Node> Nodes { get; } = new List<Node>();
        public List<Edge> Edges { get; } = new List<Edge>();
    }
}
