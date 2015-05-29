using Mentula.General.Res;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Mentula.General
{
    [DebuggerDisplay("CPos={ChunkPos}, TPos={tilePos}")]
    public class Actor
    {
        public IntVector2 ChunkPos;

        protected Vector2 tilePos;

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
            return this.ChunkPos.ToVector2() * MentulaExtensions.ChunkSize + tilePos;
        }

        public Vector2 GetTilePos()
        {
            return tilePos;
        }

        private void FormatPos()
        {
            while (tilePos.X < 0 | tilePos.Y < 0 | tilePos.X > MentulaExtensions.ChunkSize | tilePos.Y > MentulaExtensions.ChunkSize)
            {
                if (tilePos.X < 0)
                {
                    tilePos.X += MentulaExtensions.ChunkSize;
                    ChunkPos.X--;
                }
                else if (tilePos.X > MentulaExtensions.ChunkSize)
                {
                    tilePos.X -= MentulaExtensions.ChunkSize;
                    ChunkPos.X++;
                }

                if (tilePos.Y < 0)
                {
                    tilePos.Y += MentulaExtensions.ChunkSize;
                    ChunkPos.Y--;
                }
                else if (tilePos.Y > MentulaExtensions.ChunkSize)
                {
                    tilePos.Y -= MentulaExtensions.ChunkSize;
                    ChunkPos.Y++;
                }
            }
        }

        public static Vector2 FormatPos(Vector2 tilePos)
        {
            while (tilePos.X < 0 | tilePos.Y < 0 | tilePos.X > MentulaExtensions.ChunkSize | tilePos.Y > MentulaExtensions.ChunkSize)
            {
                if (tilePos.X < 0) tilePos.X += MentulaExtensions.ChunkSize;
                else if (tilePos.X > MentulaExtensions.ChunkSize) tilePos.X -= MentulaExtensions.ChunkSize;

                if (tilePos.Y < 0) tilePos.Y += MentulaExtensions.ChunkSize;
                else if (tilePos.Y > MentulaExtensions.ChunkSize) tilePos.Y -= MentulaExtensions.ChunkSize;
            }

            return tilePos;
        }
    }
}