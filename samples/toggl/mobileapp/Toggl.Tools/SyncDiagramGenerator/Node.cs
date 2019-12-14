namespace SyncDiagramGenerator
{
    internal sealed class Node : ILabeled
    {
        public enum NodeType
        {
            Regular = 0,
            EntryPoint = 1,
            LooseEnd = 2,
            InvalidTransitionState = 3,
            DeadEnd = 4,
            RetryLoop = 5,
            APIDelayReset = 6,
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public NodeType Type { get; set; }
        public SubGraph SubGraph { get; set; }
    }
}
