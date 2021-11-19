using UnityEngine;

public class FoodSpawner : MonoBehaviour {
    [SerializeField]
    private GameObject foodPrefab;

    /// <summary>
    /// Spawning food as soon as the game starts
    /// </summary>
    private void Setup() {
        TrySpawnFood();
        
        SnakeHead.OnSnakeScore += _ => TrySpawnFood();
    }

    /// <summary>
    /// Spawns food at random empty tile in grid
    /// If there's no more tiles that's empty, it does nothing
    /// </summary>
    private void TrySpawnFood()
    {
        Vector2Int? emptyTilePosition = GameBoard.GetRandomWalkableNodePosition();

        if (!emptyTilePosition.HasValue)
        {
            return;
        }

        GameBoard.SpawnFood(foodPrefab, emptyTilePosition.Value);
    }

    private void Start() {
        Setup();
    }
}