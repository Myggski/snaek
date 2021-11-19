using UnityEngine;

namespace Tiles
{
    public class ObstacleTile : MonoBehaviour, ICollisionable
    {
        public TileType TileType => TileType.Obstacle;
        /// <summary>
        /// Kills the snake
        /// </summary>
        /// <param name="snakeHead">The snake that is colliding</param>
        public void Collide(SnakeHead snakeHead)
        {
            snakeHead.Kill();
        }
    }
}