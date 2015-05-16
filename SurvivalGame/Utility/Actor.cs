using Mentula.General.Res;
using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class Actor
    {
        public IntVector2 ChunkPos;

        private Vector2 tilePos;

        public Actor()
        {
            ChunkPos = new IntVector2();
            tilePos = new Vector2();
        }

        public Actor(IntVector2 chunkPos, Vector2 tilePos)
        {
            ChunkPos = chunkPos;
            this.tilePos = tilePos;
            FormatPos();
        }

        public void SetTilePos(Vector2 tilePos)
        {
            this.tilePos = tilePos;
            FormatPos();
        }

        public void Move(Vector2 unit)
        {
            tilePos += unit;
            FormatPos();
        }

        public void ReSet(IntVector2 chunkPos, Vector2 tilePos)
        {
            ChunkPos = chunkPos;
            this.tilePos = tilePos;
        }

        public Vector2 GetTotalPos()
        {
            return this.ChunkPos.ToVector2() * int.Parse(Resources.ChunkSize) + tilePos;
        }

        public Vector2 GetTilePos()
        {
            return tilePos;
        }

        private void FormatPos()
        {
            int cSize = int.Parse(Resources.ChunkSize);
            while (tilePos.X < 0 | tilePos.Y < 0 | tilePos.X > cSize | tilePos.Y > cSize)
            {
                if (tilePos.X < 0)
                {
                    tilePos.X += cSize;
                    ChunkPos.X--;
                }
                else if (tilePos.X > cSize)
                {
                    tilePos.X -= cSize;
                    ChunkPos.X++;
                }

                if (tilePos.Y < 0)
                {
                    tilePos.Y += cSize;
                    ChunkPos.Y--;
                }
                else if (tilePos.Y > cSize)
                {
                    tilePos.Y -= cSize;
                    ChunkPos.Y++;
                }
            }
        }
    }
}