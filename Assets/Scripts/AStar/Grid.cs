using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace AStar {
    public class Grid : MonoBehaviour {
        [SerializeField]
        private LayerMask unwalkableMask;
        [SerializeField]
        private Vector2Int gridWorldSize;

        [SerializeField]
        private int tileSize = 1;
        
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
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    Collider2D[] colliders = new Collider2D[1];
                    Physics2D.OverlapBoxNonAlloc(new Vector2(x, y), new Vector2(0.9f, 0.9f), 0, colliders, unwalkableMask);

                    _grid[x, y] = new Node(colliders[0]?.gameObject?.GetComponent<ICollisionable>(), gridPosition, FromGridToWorld(gridPosition), 0);
                }   
            }
        }
        
        public Vector2Int FromWorldToGrid(Vector3 worldPosition)
        {
            return Vector2Int.RoundToInt(worldPosition / tileSize);
        }
        
        public Vector2 FromGridToWorld(Vector2Int gridPos) {
            return (gridPos * tileSize);
        }

        public void CheckCollision(Vector2 worldPosition, out ICollisionable collisionNode) {
            Vector2Int gridPosition = FromWorldToGrid(worldPosition);

            Node node = _grid[gridPosition.x, gridPosition.y];
            collisionNode = node.CollisionGameObject;
        }

        public Vector2Int GetNextGridPosition(Vector3 position, Vector2Int lookAtDirection) {
            return Vector2Int.FloorToInt(position) + lookAtDirection * tileSize;
        }

        public void MoveNode(Vector2 fromWorld, Vector2 toWorld) {
            Vector2Int from = FromWorldToGrid(fromWorld);
            Vector2Int to = FromWorldToGrid(toWorld);

            _grid[to.x, to.y].CollisionGameObject = _grid[from.x, from.y].CollisionGameObject;
            _grid[from.x, from.y] = new Node( null, from, FromGridToWorld(from), 0);
        }

        public List<Node> GetNeigbors(Node node) {
            List<Node> neigbors = new List<Node>();
            Vector2Int[] directions =
            {
                node.GridPosition + Vector2Int.left,
                node.GridPosition + Vector2Int.right,
                node.GridPosition + Vector2Int.up,
                node.GridPosition + Vector2Int.down
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
        
        public Vector2? GetRandomWalkableNodePosition()
        {
            Vector2Int[] emptyNodes = GetWalkableNodes();

            if (emptyNodes.Length == 0) {
                return null;
            }

            return FromGridToWorld(emptyNodes[Random.Range(0, emptyNodes.Length)]);
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

        public void AddNode(Vector2 worldPosition, ICollisionable collisionNode) {
            Vector2Int gridPosition = FromWorldToGrid(worldPosition);
            _grid[gridPosition.x, gridPosition.y] = new Node(collisionNode, gridPosition, worldPosition, 0);
        }

        public Node NodeFromWorldPoint(Vector2Int worldPosition) {
            return _grid[worldPosition.x, worldPosition.y];
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));

            if (_grid != null) {
                foreach (Node node in _grid) {
                    Gizmos.color = node.Walkable ? Color.white : Color.red;
                    Gizmos.DrawCube(new Vector3(node.WorldPosition.x, node.WorldPosition.y, 0f), Vector3.one);
                }
            }
        }
    }
}