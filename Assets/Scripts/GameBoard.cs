using System;
using UnityEngine;
using Grid = AStar.Grid;

[RequireComponent(typeof(Grid))]
public class GameBoard : SingletonBase<GameBoard>
{
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject snakeHeadPrefab;

    private Grid _grid;
    private static bool _isGamePaused;
    private static bool _isGameOver;
    private static Vector2Int _foodPosition;
    
    public static event Action GameOverEvent;

    public static bool IsGamePaused => _isGamePaused;
    public static bool IsGameOver => _isGameOver;
    public static Vector2Int FoodPosition => _foodPosition; 

    /// <summary>
    /// Gets the grid and generates level and spawns snake
    /// </summary>
    private void Setup()
    {
        _grid = GetComponent<Grid>();
        SetupOuterWalls();
        SpawnSnakeHead();
    }

    /// <summary>
    /// Sets the event to null when destroyed
    /// </summary>
    private void ClearEvent() {
        GameOverEvent = null;
    }

    /// <summary>
    /// Gets a random walkable node from grid
    /// </summary>
    /// <returns></returns>
    public static Vector2Int? GetRandomWalkableNodePosition()
    {
        return instance._grid.GetRandomWalkableNodePosition();
    }

    /// <summary>
    /// Checks if the new position will collide with something
    /// </summary>
    /// <param name="nextPosition">The next position for the snake</param>
    /// <param name="collisionNode">A GameObject that can be collided with</param>
    public static void CheckCollision(Vector2Int nextPosition, out ICollisionable collisionNode)
    {
        instance._grid.CheckCollision(nextPosition, out collisionNode);
    } 

    /// <summary>
    /// Triggers the Game over event
    /// </summary>
    public static void SetGameOver()
    {
        _isGameOver = true;
        GameOverEvent?.Invoke();
    }

    /// <summary>
    /// Toggles in-game pause, when pressing the Pause/Resume-button
    /// </summary>
    public static void TogglePause()
    {
        _isGamePaused = !_isGamePaused;

        Time.timeScale = _isGamePaused ? 0 : 1;
    }

    /// <summary>
    /// Closes the application, when pressing on the Quit-button
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    public static void SpawnFood(GameObject foodPrefab, Vector2Int worldPosition) {
        _foodPosition = worldPosition;
        InstantiateNode(foodPrefab, worldPosition);
    }

    /// <summary>
    /// Instantiate the tile into the world, and adds it to the grid
    /// </summary>
    /// <param name="prefab">Tile prefab</param>
    /// <param name="worldPosition">Where in the world it should be instantiated</param>
    /// <returns></returns>
    public static GameObject InstantiateNode(GameObject prefab, Vector2Int worldPosition)
    {
        GameObject node = Instantiate(prefab, new Vector2(worldPosition.x, worldPosition.y), instance.transform.rotation, instance.transform);

        instance._grid.AddNode(worldPosition, node.GetComponent<ICollisionable>());

        return node;
    }

    /// <summary>
    /// Moves a node in the grid
    /// </summary>
    /// <param name="from">Where the node is currently at in the grid</param>
    /// <param name="to">The new location where the node will be placed</param>
    public static void MoveNode(Vector2Int from, Vector2Int to)
    {
        instance._grid.MoveNode(from, to);
    }

    /// <summary>
    /// Spawns snake into the game
    /// </summary>
    private void SpawnSnakeHead()
    {
        InstantiateNode(snakeHeadPrefab, Vector2Int.RoundToInt(snakeHeadPrefab.transform.position));
    }

    /// <summary>
    /// Adds walls to the edges of the level, so the snake can't escape
    /// </summary>
    private void SetupOuterWalls()
    {
        for (int x = 0; x < _grid.GridSizeX; x++)
        {
            InstantiateNode(wallPrefab, new Vector2Int(x, 0));
            InstantiateNode(wallPrefab, new Vector2Int(x, _grid.GridSizeY - 1));
        }

        for (int y = 0; y < _grid.GridSizeY; y++)
        {
            InstantiateNode(wallPrefab, new Vector2Int(0, y));
            InstantiateNode(wallPrefab, new Vector2Int(_grid.GridSizeX - 1, y));
        }
    }

    private void Start()
    {
        Setup();
    }
    
    protected override void OnDestroy() {
        base.OnDestroy();

        ClearEvent();
    }
}
