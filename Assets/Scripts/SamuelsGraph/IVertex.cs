using System.Collections.Generic;

public interface IVertex {
    public dynamic Value { get; }
    public HashSet<Edge> Edges { get; }
    public int IndexInGraph { get; }
    public Edge AddEdge(IVertex vertex, float weight = 1f);
}
