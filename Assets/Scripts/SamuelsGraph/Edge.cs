public class Edge {
    // Vertex from
    private IVertex _source;
    // Vertex to
    private IVertex _destination;
    // The "cost" to "travel" from source to destination
    private float _weight;

    public IVertex Source => _source;
    public IVertex Destination => _destination;
    public float Weight => _weight;
    
    public Edge(IVertex source, IVertex destination, float weight = 1) {
        _source = source;
        _destination = destination;
        _weight = weight;
    }
}
