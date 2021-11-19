using System;
using AStar;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeHead : MonoBehaviour {
    [SerializeField]
    private GameObject snakeBodyPrefab;

    private PogList<GameObject> _snakeBody;
    private int _totalScore;
    private Vector2Int _lookAtDirection = Vector2Int.right;
    private Vector2Int[] _path;

    private Vector2Int SnakeHeadPosition => Vector2Int.FloorToInt(_snakeBody[0].transform.position);
    private Vector2Int NextPosition => SnakeHeadPosition + _lookAtDirection;

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

        RythmHandler.RythmEvent += ChangePosition;
        GameBoard.GameOverEvent += ClearEvent;
    }

    /// <summary>
    /// Clears the event on destroy
    /// </summary>
    private void ClearEvent() {
        OnSnakeScore = null;
    }

    /// <summary>
    /// This is called by Unitys Input System It change direction if moving-keys is being pressed
    /// </summary>
    /// <param name="value">A Vector2 that contains what direction the snake head is facing</param>
    public void ChangeDirection(InputAction.CallbackContext value) {
        // Preventing pause-meta gaming, the player can't pause then change direction
        if (GameBoard.IsGamePaused)
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
        return _lookAtDirection == checkDirection * -1;
    }

    /// <summary>
    /// This is called by the rythmActivated event that's in RythmHandler
    /// It checks the grid for collision, and emit events depending on the result
    /// </summary>
    private void ChangePosition()
    {
        GameBoard.CheckCollision(NextPosition, out ICollisionable collisionNode);

        if (collisionNode != null)
        {
            collisionNode.Collide(this);
        }
        else
        {
            MoveSnake();
        }
        
        PathRequestManager.RequestPath(
            Vector2Int.FloorToInt(transform.position),
            Vector2Int.FloorToInt(GameBoard.FoodPosition),
            OnPathFound
        );
    }

    private void OnPathFound(Vector2Int[] path, bool pathFound) {
        _path = pathFound ? path : null;
    }

    /// <summary>
    /// Kills the snake and ends the game
    /// </summary>
    public void Kill()
    {
        GameBoard.SetGameOver();
        ClearEvent();
    }
    
    /// <summary>
    /// Moves only the head to the new position, and adds a new part of the snake heads old position
    /// </summary>
    public void GrowSnake()
    {
        Vector2Int currentSnakeHeadPosition = SnakeHeadPosition;
        UpdateSnakeBodyPartPosition(0, 0, NextPosition);
        
        GameObject newBodyPart = GameBoard.InstantiateNode(snakeBodyPrefab, currentSnakeHeadPosition);

        if (_snakeBody.Count == 1) {
            _snakeBody.Add(newBodyPart);
        } else {
            _snakeBody.Insert(1, newBodyPart);
        }

        _totalScore += 10;
        OnSnakeScore?.Invoke(_totalScore);
    }

    /// <summary>
    /// Changes the position of the snake
    /// </summary>
    private void MoveSnake() {
        UpdateSnakeBodyPartPosition(0, _snakeBody.Count - 1, NextPosition);
    }
    
    /// <summary>
    /// It's a recursive method that changes the position of every body part of the snake, including the head
    /// </summary>
    /// <param name="index">Index of the snake part (0 = snake head)</param>
    /// <param name="maxNumberOfInteractions">How many times the method will be called</param>
    /// <param name="nextPosition">The new position that the part will be moving to</param>
    private void UpdateSnakeBodyPartPosition(int index, int maxNumberOfInteractions, Vector2Int nextPosition) {
        Vector2Int tilePosition = Vector2Int.FloorToInt(_snakeBody[index].transform.position);
        _snakeBody[index].transform.position = new Vector3(nextPosition.x, nextPosition.y, 0);
        GameBoard.MoveNode(tilePosition, nextPosition);

        if (index < maxNumberOfInteractions) {
            UpdateSnakeBodyPartPosition(index + 1, maxNumberOfInteractions, tilePosition);
        }
    }

    private void Start() {
        Setup();
    }

    private void OnDestroy() {
        ClearEvent();
    }

    private void OnDrawGizmos() {
        if (_path != null) {
            Gizmos.color = Color.magenta;
            foreach (Vector2Int point in _path) {
                Gizmos.DrawCube(new Vector3(point.x, point.y, 0), Vector3.one);
            }
        }
    }
}
