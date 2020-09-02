using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Leo.ECS
{
    public static class Gizmos
    {
        #region Variables
        private static Texture2D Blank;

        public static Color Color;

        private static SpriteBatch batch;
        public static void SetSpriteBatch(SpriteBatch batch) => Gizmos.batch = batch;
        #endregion

        #region Initialize
        public static void Initialize(GraphicsDevice GraphicsDevice)
        {
            Blank = new Texture2D(GraphicsDevice, 1, 1);
            Blank.SetData(new Color[] { Color.White });
        }
        #endregion

        #region Tools
        public static void DrawLine(Vector2 start, Vector2 end, int width = 1)
        {
            Vector2 edge = end - start;

            float angle = (float)Math.Atan2(edge.Y, edge.X);

            batch.Draw(
                texture: Blank,
                destinationRectangle: new Rectangle(
                    (int) start.X,
                    (int) start.Y,
                    (int) edge.Length(),
                    width),
                sourceRectangle: null,
                color: Color,
                rotation: angle,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }

        public static void DrawRectangle(int x, int y, int width, int height, int lineWidth = 1)
        {
            Vector2 TopLeft = new Vector2(x, y);
            Vector2 BotRight = new Vector2(x + width, y + height);
            DrawLine(TopLeft, new Vector2(BotRight.X, TopLeft.Y), lineWidth);
            DrawLine(TopLeft, new Vector2(TopLeft.X, BotRight.Y), lineWidth);
            DrawLine(BotRight, new Vector2(BotRight.X, TopLeft.Y), lineWidth);
            DrawLine(BotRight, new Vector2(TopLeft.X, BotRight.Y), lineWidth);
        }

        public static void DrawRectangle(Vector2 Center, float halfWidth, float halfHeight, int width = 1)
        {
            Vector2 halfdims = new Vector2(halfWidth, halfHeight);
            Vector2 botLeft = Center - halfdims;
            Vector2 topRight = Center + halfdims;
            Vector2 topLeft = new Vector2(botLeft.X, topRight.Y);
            Vector2 botRight = new Vector2(topRight.X, botLeft.Y);
            DrawLine(topLeft, topRight, width);
            DrawLine(topRight, botRight, width);
            DrawLine(botRight, botLeft, width);
            DrawLine(botLeft, topLeft, width);
        }
        #endregion
    }
}
