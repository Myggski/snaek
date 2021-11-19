using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PogGraph {
    private List<IVertex> _vertices;
    private HashSet<Edge> _edges;

    public int Order => _vertices.Count;
    public int Size => _edges.Count;
    public IVertex[] Vertices => _vertices.ToArray();
    public Edge[] Edges => _edges.ToArray();

    public void DFS(IVertex v = null) {
        HashSet<IVertex> visited = new HashSet<IVertex>();
        Stack<IVertex> S = new Stack<IVertex>();

        if (v == null) {
            v = _vertices[0];
        }
        
        S.Push(v);

        while (S.Count > 0) {
            IVertex current = S.Pop();
            
            if (!visited.Contains(current)) {
                continue;
            }
            
            visited.Add(current);

            foreach (Edge edge in current.Edges) {
                if (!visited.Contains(edge.Destination)) {
                    S.Push(edge.Destination);
                }
            }
        }
    }

    public void BFS(IVertex v = null) {
        HashSet<IVertex> visited = new HashSet<IVertex>();
        Queue<IVertex> Q = new Queue<IVertex>();

        if (v == null) {
            v = _vertices[0];
        }
        
        Q.Enqueue(v);

        while (Q.Count > 0) {
            IVertex current = Q.Dequeue();
            
            if (!visited.Contains(current)) {
                continue;
            }
            
            visited.Add(current);

            foreach (Edge edge in current.Edges) {
                if (!visited.Contains(edge.Destination)) {
                    Q.Enqueue(edge.Destination);
                }
            }
        }
    }

    public static float[][] FloydWarshall(PogGraph graph) {
        float[][] distances = new float[graph.Order][];

        for (int distanceIndex = 0; distanceIndex < graph.Order; distanceIndex++) {
            distances[distanceIndex] = Enumerable.Repeat(float.MaxValue, graph.Order).ToArray();
            distances[distanceIndex][distanceIndex] = 0f;
        }

        IVertex[] vertices = graph.Vertices;
        for (int vertexIndex = 0; vertexIndex < graph.Order; vertexIndex++) {
            foreach (Edge edge in vertices[vertexIndex].Edges) {
                distances[vertexIndex][edge.Destination.IndexInGraph] = edge.Weight;
            }
        }

        for (int k = 0; k < graph.Order; k++) {
            for (int i = 0; i < graph.Order; i++) {
                for (int j = 0; j < graph.Order; j++) {
                    if (distances[i][j] > distances[i][k] + distances[k][j]) {
                        distances[i][j] = distances[i][k] + distances[k][j];
                    }
                }
            }
        }

        return distances;
    }

    public (Dictionary<IVertex, float>, Dictionary<IVertex, IVertex>) Dijkstra(PogGraph graph, IVertex startVertex) {
        HashSet<IVertex> unvisited = new HashSet<IVertex>(graph.Vertices);
        Dictionary<IVertex, float> distances = new Dictionary<IVertex, float>();
        Dictionary<IVertex, IVertex> previous = new Dictionary<IVertex, IVertex>();

        foreach (IVertex vertex in graph.Vertices) {
            distances[vertex] = float.MaxValue;
            previous[vertex] = null;
        }

        while (unvisited.Count > 0) {
            IVertex current = null;
            float minDistance = float.MaxValue;

            foreach (KeyValuePair<IVertex, float> distDict in distances) {
                if (unvisited.Contains(distDict.Key) && distDict.Value < minDistance) {
                    minDistance = distDict.Value;
                    current = distDict.Key;
                }
            }

            if (minDistance == float.MaxValue) {
                break;
            }

            unvisited.Remove(current);
            float dist = distances[current];

            foreach (Edge edge in current.Edges) {
                float temp = dist + edge.Weight;

                if (temp < distances[edge.Destination]) {
                    distances[edge.Destination] = temp;
                    previous[edge.Destination] = current;
                }
            }
        }

        distances[startVertex] = 0;

        return (distances, previous);
    }

    public void AddVertex(IVertex vertex) {
        _vertices.Add(vertex);
    }

    public void AddEdge(IVertex v1, IVertex v2, float weight = 1f) {
        _edges.Add(v1.AddEdge(v2, weight));
        _edges.Add(v2.AddEdge(v1, weight));
    }

    public PogGraph() {
        _vertices = new List<IVertex>();
        _edges = new HashSet<Edge>();
    }
}
