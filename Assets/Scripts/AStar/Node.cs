using UnityEngine;

namespace AStar
{
    public class Node : IHeapItem<Node>
    {
        private ICollisionable _collisionableGameObject;
        private int _x;
        private int _y;
        private Vector2Int _gridPosition;
        private Vector2 _worldPosition;
        private int _gCost;
        private int _hCost;
        private Node _parent;
        private int _heapIndex;
        private int _movementPenelty;

        public int HeapIndex
        {
            get => _heapIndex;
            set => _heapIndex = value;
        }
        public int GCost
        {
            get => _gCost;
            set => _gCost = value;
        }
        public int HCost
        {
            get => _hCost;
            set => _hCost = value;
        }
        public Node Parent
        {
            get => _parent;
            set => _parent = value;
        }
        public ICollisionable CollisionGameObject
        {
            get => _collisionableGameObject;
            set => _collisionableGameObject = value;
        }
        public int FCost => _gCost + _hCost;
        public int MovementPenelty => _movementPenelty;
        public bool Walkable => ReferenceEquals(_collisionableGameObject, null) || _collisionableGameObject.TileType != TileType.Obstacle;
        public Vector2Int GridPosition => _gridPosition;
        public Vector2 WorldPosition => _worldPosition;

        public Node(ICollisionable collisionableGameObject, Vector2Int gridPosition, Vector2 worldPosition, int movementPenelty) {
            _gridPosition = gridPosition;
            _collisionableGameObject = collisionableGameObject;
            _worldPosition = worldPosition;
            _movementPenelty = movementPenelty;
        }

        public int CompareTo(Node nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
            {
                compare = _hCost.CompareTo(nodeToCompare.HCost);
            }

            return -compare;
        }
    }
}