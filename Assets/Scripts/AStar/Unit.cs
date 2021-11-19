using System.Collections;
using UnityEngine;

namespace AStar {
    public class Unit : MonoBehaviour {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private float speed = 20f;
        private Vector2Int[] _path;
        private int _targetIndex;

        private void Start() {
            PathRequestManager.RequestPath(
                Vector2Int.FloorToInt(transform.position),
                Vector2Int.FloorToInt(target.position),
                OnPathFound
            );
        }

        private void OnPathFound(Vector2Int[] path, bool pathSuccessful) {
            if (pathSuccessful) {
                _path = path;
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        private IEnumerator FollowPath() {
            Vector2Int currentWaypoint = _path[0];

            while (true) {
                if (Vector2Int.FloorToInt(transform.position) == currentWaypoint) {
                    _targetIndex++;
                    if (_targetIndex >= _path.Length) {
                        yield break;
                    }

                    currentWaypoint = _path[_targetIndex];
                }
                
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWaypoint.x, currentWaypoint.y), speed * Time.deltaTime);
                yield return null;
            }
        }

       /* public void OnDrawGizmos() {
            if (_path != null) {
                for (int i = _targetIndex; i < _path.Length; i++) {
                    Gizmos.color = Color.black;
                    Vector3 pathPosition = new Vector3(_path[i].x, _path[i].y);
                    Gizmos.DrawCube(pathPosition, Vector3.one);

                    if (i == _targetIndex) {
                        Gizmos.DrawLine(transform.position, pathPosition);
                    } else {
                        Gizmos.DrawLine(new Vector3(_path[i - 1].x, _path[i - 1].y), pathPosition);
                    }
                }   
            }
        }*/
    }
}