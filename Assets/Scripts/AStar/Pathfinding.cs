using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    [RequireComponent(typeof(Grid), typeof(PathRequestManager))]
    public class Pathfinding : MonoBehaviour
    {
        private Grid _grid;
        private PathRequestManager _pathRequestManager;

        private void Setup()
        {
            _grid = GetComponent<Grid>();
            _pathRequestManager = GetComponent<PathRequestManager>();
        }

        public void StartFindPath(Vector2Int startPosition, Vector2Int targetPosition)
        {
            StartCoroutine(FindPath(startPosition, targetPosition));
        }

        private IEnumerator FindPath(Vector2Int startPosition, Vector2Int targetPosition)
        {
            Vector2Int[] waypoints = new Vector2Int[0];
            bool pathSuccess = false;
            Node startNode = _grid.NodeFromWorldPoint(startPosition);
            Node targetNode = _grid.NodeFromWorldPoint(targetPosition);

            if (startNode.Walkable && targetNode.Walkable)
            {
                Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;

                        break;
                    }

                    foreach (Node neighbor in _grid.GetNeigbors(currentNode))
                    {
                        if (!neighbor.Walkable || closedSet.Contains(neighbor))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor) +
                                                        neighbor.MovementPenelty;

                        if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains((neighbor)))
                        {
                            neighbor.GCost = newMovementCostToNeighbor;
                            neighbor.HCost = GetDistance(neighbor, targetNode);
                            neighbor.Parent = currentNode;

                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                            }
                            else
                            {
                                openSet.UpdateItem(neighbor);
                            }
                        }
                    }
                }
            }

            yield return null;

            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
            }

            _pathRequestManager.FinishedProcessPath(waypoints, pathSuccess);
        }

        private Vector2Int[] RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            Vector2Int[] simplifiedPath = SimplifyPath(path);
            Array.Reverse(simplifiedPath);
            
            return simplifiedPath;
        }

        private Vector2Int[] SimplifyPath(List<Node> path)
        {
            List<Vector2Int> waypoints = new List<Vector2Int>();
            Vector2Int directionOld = Vector2Int.zero;

            if (path.Count > 0)
            {
                waypoints.Add(path[0].WorldPosition);
                
                for (int i = 1; i < path.Count; i++)
                {
                    Vector2Int directionNew =
                        new Vector2Int(path[i - 1].X - path[i].X, path[i - 1].Y - path[i].Y);
                    if (directionNew != directionOld)
                    {
                        waypoints.Add(path[i - 1].WorldPosition);
                    }

                    directionOld = directionNew;
                }
            }

            return waypoints.ToArray();
        }

        private int GetDistance(Node from, Node to)
        {
            int distanceX = Mathf.Abs(from.X - to.X);
            int distanceY = Mathf.Abs(from.Y - to.Y);

            if (distanceX > distanceY)
            {
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }

            return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        private void Awake()
        {
            Setup();
        }
    }
}