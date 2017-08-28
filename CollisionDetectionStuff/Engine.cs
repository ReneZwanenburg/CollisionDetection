using System.Collections.Generic;
using CollisionDetectionStuff.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CollisionDetectionStuff
{
    public class Engine : Game
    {
        private readonly UserInputHandler uih = new UserInputHandler();
        private GraphicsDeviceManager graphics;
        private List<Entity> otherTiles;
        private SpriteBatch spriteBatch;
        private Entity entity;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            var texture = new Texture2D(GraphicsDevice, 32, 32);
            var pixels = new Color[32 * 32];
            for (var i = 0; i < pixels.Length; ++i) pixels[i] = Color.Black;
            texture.SetData(pixels);

            entity = new Entity(texture);

            {
                var texture2 = new Texture2D(GraphicsDevice, 32, 32);
                var pixels2 = new Color[32 * 32];
                for (var i = 0; i < pixels.Length; ++i) pixels2[i] = Color.White;
                texture2.SetData(pixels2);

                otherTiles = new List<Entity>();
                var tile2 = new Entity(texture2)
                {
                    Position = new Vector2(256, 256)
                };
                var tile3 = new Entity(texture2)
                {
                    Position = new Vector2(288, 256)
                };
                var tile4 = new Entity(texture2)
                {
                    Position = new Vector2(320, 256)
                };
                var tile5 = new Entity(texture2)
                {
                    Position = new Vector2(352, 256)
                };
                var tile6 = new Entity(texture2)
                {
                    Position = new Vector2(256, 288)
                };
                var tile7 = new Entity(texture2)
                {
                    Position = new Vector2(256, 320)
                };
                var tile8 = new Entity(texture2)
                {
                    Position = new Vector2(256, 352)
                };
                
                otherTiles.Add(entity);
                otherTiles.Add(tile2);
                otherTiles.Add(tile3);
                otherTiles.Add(tile4);
                otherTiles.Add(tile5);
                otherTiles.Add(tile6);
                otherTiles.Add(tile7);
                otherTiles.Add(tile8);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            uih.Handle(entity, otherTiles, Keyboard.GetState());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (var otherTile in otherTiles)
                spriteBatch.Draw(otherTile.Texture, otherTile.Position, Color.White);

            spriteBatch.Draw(entity.Texture, entity.Position, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}