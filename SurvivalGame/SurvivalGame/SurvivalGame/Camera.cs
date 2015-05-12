using Mentula.General;
using Mentula.General.Res;
using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGame
{
    public class Camera
    {
        public Rectangle CameraWorld { get; private set; }

        private Vector2 Position;
        private Vector2 DesiredPosition;

        private Vector2 CameraOffset;
        private Rectangle GraphicsBounds;
        private int TS;
        private int CS;

        public Camera(Vector2 startPos, Rectangle graphicalBounds)
        {
            Position = startPos;
            DesiredPosition = startPos;

            CameraOffset = Vector2.Zero;
            TS = int.Parse(Resources.TileSize);
            CS = int.Parse(Resources.ChunkSize);

            GraphicsBounds = graphicalBounds;
            CameraWorld = graphicalBounds;
        }

        public void Move(Vector2 unitMove)
        {
            DesiredPosition = Position + unitMove;
        }

        public void Teleport(Vector2 newPos)
        {
            Position = new Vector2(newPos.X, newPos.Y);
            DesiredPosition = Position;
        }

        public void Update()
        {
            if (Position != DesiredPosition) Position = Vector2.SmoothStep(Position, DesiredPosition, 1f);

            CameraWorld = new Rectangle((int)Position.X, (int)Position.Y, (int)((GraphicsBounds.Width) + Position.X), (int)((GraphicsBounds.Height) + Position.Y));
            CameraOffset = new Vector2(CameraWorld.X, CameraWorld.Y);
        }

        public void Update(Vector2 lookAt, Rectangle mapBounds)
        {
            Vector2 relativeLookAt = new Vector2((lookAt.X - (GraphicsBounds.Width >> 1)) / TS, (lookAt.Y - (GraphicsBounds.Height >> 1)) / TS);

            if ((lookAt.X - (GraphicsBounds.Width >> 1)) >= 0 && (lookAt.X + (GraphicsBounds.Width >> 1)) <= mapBounds.Width * TS)
                Position = new Vector2(relativeLookAt.X, Position.Y);
            if ((lookAt.Y - (GraphicsBounds.Height >> 1)) >= 0 && (lookAt.Y + (GraphicsBounds.Height >> 1)) <= mapBounds.Height * TS)
                Position = new Vector2(Position.X, relativeLookAt.Y);

            CameraWorld = new Rectangle((int)Position.X, (int)Position.Y, (int)((GraphicsBounds.Width / TS) + Position.X), (int)((GraphicsBounds.Height / TS) + Position.Y));
            CameraOffset = new Vector2(CameraWorld.X * TS, CameraWorld.Y * TS);
        }

        public Vector2 GetRelativePosition(IntVector2 chunkPos, IntVector2 position)
        {
            Vector2 pos = (chunkPos * CS) * TS + (position * TS);
            return pos - CameraOffset;
        }
    }
}