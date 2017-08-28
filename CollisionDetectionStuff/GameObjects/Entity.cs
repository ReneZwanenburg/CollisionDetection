using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionDetectionStuff.GameObjects
{
    public class Entity
    {
        public Entity(Texture2D texture)
        {
            Texture = texture;
        }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Texture2D Texture { get; set; }

        public Rectangle Bounds => new Rectangle((int) Position.X, (int) Position.Y, Texture.Width, Texture.Height);

        public bool IsGrounded { get; set; }
    }
}