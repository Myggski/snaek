using System;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    [RequireComponent(typeof(Pathfinding))]
    public class PathRequestManager : SingletonBase<PathRequestManager> {
        private Queue<PathRequest> _pathRequests = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;
        private Pathfinding _pathfinding;
        private bool _isProcessingPath;

        private void Setup()
        {
            base.Awake();

            _pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Vector2Int startPosition, Vector2Int targetPosition,
            Action<Vector2Int[], bool> callback) {
            PathRequest newRequest = new PathRequest(startPosition, targetPosition, callback);
            instance._pathRequests.Enqueue(newRequest);
            instance.TryProcessNext();
        }
        
        public void FinishedProcessPath(Vector2Int[] path, bool success) {
            _currentPathRequest.Callback(path, success);
            _isProcessingPath = false;
            TryProcessNext();
        }

        private void TryProcessNext() {
            if (!_isProcessingPath && _pathRequests.Count > 0) {
                _currentPathRequest = _pathRequests.Dequeue();
                _isProcessingPath = true;
                _pathfinding.StartFindPath(_currentPathRequest.StartPosition, _currentPathRequest.TargetPosition);
            } 
        }

        private struct PathRequest {
            public Vector2Int StartPosition;
            public Vector2Int TargetPosition;
            public Action<Vector2Int[], bool> Callback;

            public PathRequest(Vector2Int startPosition, Vector2Int targetPosition,
                Action<Vector2Int[], bool> callback) {
                StartPosition = startPosition;
                TargetPosition = targetPosition;
                Callback = callback;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            Setup();
        }
    }
}