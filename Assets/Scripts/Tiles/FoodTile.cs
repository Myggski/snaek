using UnityEngine;

public class FoodTile : MonoBehaviour, ICollisionable {
    public TileType TileType => TileType.Food;

    /// <summary>
    /// Makes snake grow
    /// </summary>
    /// <param name="snakeHead">The snake that's eating</param>
    public void Collide(SnakeHead snakeHead)
    {
        snakeHead.GrowSnake();
        Destroy(gameObject);
    }
}
