using Mentula.General;
using Mentula.General.Res;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mentula.SurvivalGame
{
    public class Camera
    {
        public Rectangle CameraWorld;

        private Vector2 Position;
        private Vector2 DesiredPosition;

        private Vector2 CameraOffset;
        private Point LookAtOffset;

        private readonly int TS;
        private readonly int CS;
        private readonly Func<IntVector2, Vector2, Vector2> GetTotalPos;

        public Camera(GraphicsDevice device, IntVector2 startChunk, Vector2 startPos)
        {
            CS = int.Parse(Resources.ChunkSize);
            TS = int.Parse(Resources.TileSize);
            GetTotalPos = (IntVector2 v1, Vector2 v2) => (v1 * CS).ToVector2() * TS + v2 * TS;

            Position = GetTotalPos(startChunk, startPos);
            DesiredPosition = Position;

            CameraOffset = Position;
            CameraWorld = new Rectangle(-TS, -TS, device.Viewport.Width + TS, device.Viewport.Height + TS);
            LookAtOffset = new Point(CameraWorld.Width >> 1, CameraWorld.Height >> 1);
        }

        public void Move(Vector2 unitMove)
        {
            DesiredPosition += unitMove;
        }

        public void Teleport(Vector2 unitMove)
        {
            Position += unitMove;
            DesiredPosition = Position;
        }

        public void Teleport(IntVector2 chunk, Vector2 pos)
        {
            DesiredPosition = GetTotalPos(chunk, pos);
        }

        public void Update()
        {
            if (Position == DesiredPosition) return;

            Position = Vector2.SmoothStep(Position, DesiredPosition, .5f);
            CameraOffset = Position;
        }

        public void Update(IntVector2 cPos, Vector2 pos)
        {
            pos = GetTotalPos(cPos, pos);
            DesiredPosition = new Vector2(pos.X - LookAtOffset.X, pos.Y - LookAtOffset.Y);

            if (Position == DesiredPosition) return;

            Position = Vector2.SmoothStep(Position, DesiredPosition, .5f);
            CameraOffset = Position;
        }

        public Vector2 GetRelativePosition(IntVector2 chunkPos, Vector2 position)
        {
            Vector2 pos = GetTotalPos(chunkPos, position);
            return pos - CameraOffset;
        }

        public bool TryGetRelativePosition(IntVector2 chunkPos, Vector2 position, out Vector2 relative)
        {
            Vector2 pos = (chunkPos * CS).ToVector2() * TS + position * TS;
            relative = pos - CameraOffset;
            return CameraWorld.Contains((int)relative.X, (int)relative.Y);
        }
    }
}