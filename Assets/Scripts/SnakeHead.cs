using System;
using System.Collections;
using AStar;
using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SnakeHead : MonoBehaviour {
    [SerializeField]
    private GameObject snakeBodyPrefab;

    private PogList<GameObject> _snakeBody;
    private int _totalScore;
    private Vector2Int _lookAtDirection = Vector2Int.right;
    private Vector2Int[] _autopilotPath;
    private int _autopilotPathIndex = 0;
    private Vector2Int _nextPositionToMove;

    private Vector2Int SnakeHeadPosition => Vector2Int.FloorToInt(_snakeBody[0].transform.position);
    
    public static event Action<int> OnSnakeScore;

    /// <summary>
    /// Get the snake tile from head and adds it to the grid
    /// Creates a linked list (PogList) of all the snake parts
    /// Also listens to the game rythm to update the snakes new position
    /// </summary>
    private void Setup() {
        _snakeBody = new PogList<GameObject>();
        _snakeBody.Add(gameObject);
        _totalScore = 0;

        RythmHandler.OnRythmPlay += ChangePosition;
        GameBoard.OnGameOver += ClearEvents;
        GameBoard.OnFoodSpawned += SearchForPath;
        GameBoard.OnSetAutopilot += SearchForPath;
    }

    /// <summary>
    /// Clears the methods from the events
    /// </summary>
    private void ClearEvents()
    {
        RythmHandler.OnRythmPlay -= ChangePosition;
        GameBoard.OnGameOver -= ClearEvents;
        GameBoard.OnFoodSpawned -= SearchForPath;
        GameBoard.OnSetAutopilot -= SearchForPath;
    }

    /// <summary>
    /// This is called by Unitys Input System It change direction if moving-keys is being pressed
    /// </summary>
    /// <param name="value">A Vector2 that contains what direction the snake head is facing</param>
    public void ChangeDirection(InputAction.CallbackContext value) {
        // Preventing pause-meta gaming, the player can't pause then change direction
        if (GameBoard.IsGamePaused || GameBoard.IsAutopilot)
        {
            return;
        }

        if (value.started) {
            TryChangeMovement(Vector2Int.FloorToInt(value.ReadValue<Vector2>()));
        }
    }

    /// <summary>
    /// If the new direction is the opposite of current, it's being ignored, else a new direction will be set
    /// TODO: There's a bug that can make the player move backwards. If the direction is changed, but hasn't moved yet, and then the direction can be changed again.
    /// </summary>
    /// <param name="newDirection">The new wanted direction to look at</param>
    private void TryChangeMovement(Vector2Int newDirection) {
        if (!IsOppositeDirection(newDirection)) {
            _lookAtDirection = newDirection;
        }
    }

    /// <summary>
    /// Checks if the direction is the opposite of the current direction
    /// </summary>
    /// <param name="checkDirection">A direction that the snake wants to move to</param>
    /// <returns></returns>
    private bool IsOppositeDirection(Vector2 checkDirection) {
        if (_snakeBody.Count > 1)
        {
            return checkDirection * -1 == Vector2Int.FloorToInt(_snakeBody[0].transform.position - _snakeBody[1].transform.position);    
        }
        
        return checkDirection * -1 == _lookAtDirection;
    }

    /// <summary>
    /// This is called by the rythmActivated event that's in RythmHandler
    /// It checks the grid for collision, and emit events depending on the result
    /// </summary>
    private void ChangePosition() {
        _nextPositionToMove = GameBoard.GetNextGridPosition(_snakeBody[0].transform.position, _lookAtDirection);
        GameBoard.CheckCollision(_nextPositionToMove, out ICollisionable collisionNode);

        if (collisionNode != null)
        {
            collisionNode.Collide(this);
        }
        else
        {
            MoveSnake();

            if (GameBoard.IsAutopilot && !ReferenceEquals(_autopilotPath, null))
            {
                AutoPilotDirectionHandling();
            }
        }
    }

    private void OnSearchFinished(Vector2Int[] path, bool pathSuccessful) {
        if (pathSuccessful) {
            _autopilotPath = path;
        }
        
        _autopilotPathIndex = 0;
    }

    private void AutoPilotDirectionHandling() {
        Vector2Int currentWaypoint = _autopilotPath[_autopilotPathIndex];

        if (SnakeHeadPosition == currentWaypoint) {
            _autopilotPathIndex++;

            if (_autopilotPathIndex >= _autopilotPath.Length)
            {
                return;
            }
        }
        
        currentWaypoint = _autopilotPath[_autopilotPathIndex];
        Vector2Int direction = (currentWaypoint - SnakeHeadPosition).Normalize();

        if (IsOppositeDirection(direction))
        {
            direction = GetRandomDirection(direction);
        }

        _lookAtDirection = direction;
    }
    
    /// <summary>
    /// Gets random direction
    /// </summary>
    /// <returns></returns>
    private Vector2Int GetRandomDirection(Vector2Int currentDirection)
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };

        // Remove opposite direction and the direction that is the same as current _lookAtDirection
        Array.FindAll(directions,direction => direction != currentDirection && _lookAtDirection != direction && IsOppositeDirection(direction));

        return directions[Random.Range(0, directions.Length)];
    }

    /// <summary>
    /// Kills the snake and ends the game
    /// </summary>
    public void Kill()
    {
        GameBoard.SetGameOver();
        ClearEvents();
    }
    
    /// <summary>
    /// Moves only the head to the new position, and adds a new part of the snake heads old position
    /// </summary>
    public void GrowSnake()
    {
        Vector2Int currentSnakeHeadPosition = SnakeHeadPosition;
        UpdateSnakeBodyPartPosition(0, _nextPositionToMove);
        
        GameObject newBodyPart = GameBoard.InstantiateNode(snakeBodyPrefab, currentSnakeHeadPosition);

        if (_snakeBody.Count == 1) {
            _snakeBody.Add(newBodyPart);
        } else {
            _snakeBody.Insert(1, newBodyPart);
        }

        _totalScore += 10;
        OnSnakeScore?.Invoke(_totalScore);
    }

    private void SearchForPath()
    {
        PathRequestManager.RequestPath(
            Vector2Int.FloorToInt(_nextPositionToMove),
            Vector2Int.FloorToInt(GameBoard.FoodPosition),
            OnSearchFinished
        );
    }

    /// <summary>
    /// Changes the position of the snake
    /// </summary>
    private void MoveSnake()
    {
        Vector2Int moveToPosition = _nextPositionToMove;

        for (int index = 0; index < _snakeBody.Count; index++)
        {
            moveToPosition = UpdateSnakeBodyPartPosition(index, moveToPosition);    
        }
    }
    
    /// <summary>
    /// It's a recursive method that changes the position of every body part of the snake, including the head
    /// </summary>
    /// <param name="index">Index of the snake part (0 = snake head)</param>
    /// <param name="moveToPosition">The new position that the part will be moving to</param>
    private Vector2Int UpdateSnakeBodyPartPosition(int index, Vector2Int moveToPosition)
    {
        Vector2Int tilePosition = Vector2Int.FloorToInt(_snakeBody[index].transform.position);
        _snakeBody[index].transform.position = new Vector3(moveToPosition.x, moveToPosition.y);
        GameBoard.MoveNode(tilePosition, moveToPosition);

        return tilePosition;
    }

    private void Start() {
        Setup();
    }

    private void OnDrawGizmos() {
        if (_autopilotPath != null && _autopilotPath.Length > 0)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(_nextPositionToMove.ToVector2(), new Vector3(_autopilotPath[_autopilotPathIndex].x, _autopilotPath[_autopilotPathIndex].y));
            
            for (int i = 0; i < _autopilotPath.Length; i++) {
                if (i != _autopilotPath.Length - 1)
                {
                    Gizmos.DrawLine(new Vector3(_autopilotPath[i].x, _autopilotPath[i].y), new Vector3(_autopilotPath[i + 1].x, _autopilotPath[i + 1].y));
                }
            }
        }
    }
}
