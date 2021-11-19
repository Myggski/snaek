using UnityEngine;

public interface ICollisionable
{
    public void Collide(SnakeHead snakeHead);
    public TileType TileType { get; }
    
}