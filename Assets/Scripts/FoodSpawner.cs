using System;
using UnityEngine;

public class FoodSpawner : MonoBehaviour {
    [SerializeField]
    private GameObject foodPrefab;

    /// <summary>
    /// Spawning food as soon as the game starts
    /// </summary>
    private void Setup() {
        TrySpawnFood();
        
        SnakeHead.OnSnakeScore += SpawnFoodEvent;
    }

    /// <summary>
    /// Remove itself from OnSnakeScore event
    /// </summary>
    private void ClearEvent()
    {
        SnakeHead.OnSnakeScore -= SpawnFoodEvent;
    }

    /// <summary>
    /// Creates a separate method that's specific to the OnSnakeScore event, else it wont clear the event properly on destroy
    /// </summary>
    /// <param name="_">Don't need total score in this method</param>
    private void SpawnFoodEvent(int _)
    {
        TrySpawnFood();
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

    private void OnDestroy()
    {
        ClearEvent();
    }
}