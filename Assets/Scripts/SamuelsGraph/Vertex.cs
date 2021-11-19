using System;
using System.Collections.Generic;

public class Vertex<T> : IVertex {
    private T _value;
    private Type _type;
    private HashSet<Edge> _edges;
    private int _indexInGraph;

    public Type Type => _type;
    public dynamic Value => _value;
    public HashSet<Edge> Edges => _edges;
    public int IndexInGraph => _indexInGraph;

    public Edge AddEdge(IVertex target, float weight = 1f) {
        Edge edge = new Edge(this, target, weight);
        _edges.Add(edge);

        return edge;
    }
    
    public Vertex(T value) {
        _type = typeof(T);
        _value = value;
        _edges = new HashSet<Edge>();
    }

    public Vertex(T value, PogGraph pogGraph) {
        _type = typeof(T);
        _value = value;
        _edges = new HashSet<Edge>();

        _indexInGraph = pogGraph.Order;
        pogGraph.AddVertex(this);
    }
}
