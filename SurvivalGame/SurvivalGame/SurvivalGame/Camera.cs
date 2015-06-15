using Mentula.General;
using Mentula.General.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Mentula.SurvivalGame
{
    public class Camera
    {
        public Rectangle CameraViewPort;

        private Vector2 Position;
        private Vector2 DesiredPosition;

        private Vector2 CameraOffset;
        private Point LookAtOffset;

        public Camera(GraphicsDevice device, IntVector2 startChunk, Vector2 startPos)
        {
            Position = (startChunk * Res.ChunkSize).ToVector2() * Res.TileSize + startPos * Res.TileSize;
            DesiredPosition = Position;

            CameraOffset = Position;
            CameraViewPort = new Rectangle(-Res.TileSize, -Res.TileSize, device.Viewport.Width + Res.TileSize, device.Viewport.Height + Res.TileSize);
            LookAtOffset = new Point(CameraViewPort.Width >> 1, CameraViewPort.Height >> 1);
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
            DesiredPosition = (chunk * Res.ChunkSize).ToVector2() * Res.TileSize + pos * Res.TileSize;
        }

        public void Update()
        {
            if (Position == DesiredPosition) return;

            Position = Vector2.SmoothStep(Position, DesiredPosition, .5f);
            CameraOffset = Position;
        }

        public void Update(IntVector2 cPos, Vector2 pos)
        {
            pos = (cPos * Res.ChunkSize).ToVector2() * Res.TileSize + pos * Res.TileSize;
            DesiredPosition = new Vector2(pos.X - LookAtOffset.X, pos.Y - LookAtOffset.Y);

            if (Position == DesiredPosition) return;

            Position = Vector2.SmoothStep(Position, DesiredPosition, .5f);
            CameraOffset = Position;
        }

        public Vector2 GetRelativePosition(IntVector2 chunkPos, Vector2 position)
        {
            Vector2 pos = (chunkPos * Res.ChunkSize).ToVector2() * Res.TileSize + position * Res.TileSize;
            return pos - CameraOffset;
        }

        public bool TryGetRelativePosition(IntVector2 chunkPos, Vector2 position, out Vector2 relative)
        {
            Vector2 pos = (chunkPos * Res.ChunkSize).ToVector2() * Res.TileSize + position * Res.TileSize;
            relative = pos - CameraOffset;
            return CameraViewPort.Contains((int)relative.X, (int)relative.Y);
        }
    }
}