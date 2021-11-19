using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class Grid : MonoBehaviour {
        [SerializeField]
        private LayerMask unwalkableMask;
        [SerializeField]
        private Vector2Int gridWorldSize;
        
        private Node[,] _grid;
        public int GridSizeX => gridWorldSize.x;
        public int GridSizeY => gridWorldSize.y;
        public int MaxSize => GridSizeX * GridSizeY;

        private void Awake() {
            CreateGrid();
        }

        private void CreateGrid() {
            _grid = new Node[GridSizeX, GridSizeY];

            for (int x = 0; x < GridSizeX; x++) {
                for (int y = 0; y < GridSizeY; y++) {
                    Collider2D[] colliders = new Collider2D[1];
                    Physics2D.OverlapBoxNonAlloc(new Vector2(x, y), new Vector2(0.9f, 0.9f), 0, colliders, unwalkableMask);

                    _grid[x, y] = new Node(colliders[0]?.gameObject?.GetComponent<ICollisionable>(), new Vector2Int(x, y), 0);
                }   
            }
        }

        public void CheckCollision(Vector2Int position, out ICollisionable collisionNode)
        {
            Node node = _grid[position.x, position.y];
            collisionNode = node.CollisionGameObject;
        }

        public void MoveNode(Vector2Int from, Vector2Int to)
        {
            _grid[to.x, to.y].CollisionGameObject = _grid[from.x, from.y].CollisionGameObject;
            _grid[from.x, from.y] = new Node(null, from, 0);
        }

        public List<Node> GetNeigbors(Node node) {
            List<Node> neigbors = new List<Node>();
            Vector2Int[] directions =
            {
                node.WorldPosition + new Vector2Int(-1, 0), // left
                node.WorldPosition + new Vector2Int(1, 0), // right
                node.WorldPosition + new Vector2Int(0, 1), // up
                node.WorldPosition + new Vector2Int(0, -1) // down
            };

            foreach (Vector2Int direction in directions)
            {
                bool outsideX = direction.x < 0 || direction.x > GridSizeX;
                bool outsideY = direction.y < 0 || direction.y > GridSizeY;

                if (outsideX || outsideY)
                {
                    continue;
                }
                
                neigbors.Add(_grid[direction.x, direction.y]);
            }

            return neigbors;
        }
        
        public Vector2Int? GetRandomWalkableNodePosition()
        {
            Vector2Int[] emptyNOdes = GetWalkableNodes();

            if (emptyNOdes.Length == 0) {
                return null;
            }

            return emptyNOdes[Random.Range(0, emptyNOdes.Length)];
        }

        /// <summary>
        /// Goes through the entire grid (except the edges) to see if there's any empty slots
        /// </summary>
        /// <returns></returns>
        private Vector2Int[] GetWalkableNodes() {
            List<Vector2Int> emptyNodes = new List<Vector2Int>();

            for (int x = 1; x < _grid.GetLength(0) - 1; x++) {
                for (int y = 1; y < _grid.GetLength(1) - 1; y++) {
                    if (_grid[x, y] != null && _grid[x, y].Walkable) {
                        emptyNodes.Add(new Vector2Int(x, y));
                    }
                }
            }

            return emptyNodes.ToArray();
        }

        public void AddNode(Vector2Int worldPosition, ICollisionable collisionNode)
        {
            _grid[worldPosition.x, worldPosition.y] = new Node(collisionNode, worldPosition, 0);
        }

        public Node NodeFromWorldPoint(Vector2Int worldPosition) {
            return _grid[worldPosition.x, worldPosition.y];
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));

            if (_grid != null) {
                foreach (Node node in _grid) {
                    Gizmos.color = node.Walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(new Vector3(node.X, node.Y, 0f), Vector3.one);
                }
            }
        }
    }
}