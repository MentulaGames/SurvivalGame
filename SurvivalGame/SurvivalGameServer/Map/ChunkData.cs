using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;

namespace Mentula.SurvivalGameServer
{
    public class ChunkData
    {
        public IntVector2 ChunkPos;
        public int ChunkType;
        public ChunkData(IntVector2 chunkPos)
        {
            ChunkPos = chunkPos;
            ChunkType = 0;
        }

        public ChunkData(IntVector2 chunkPos,int chunkType)
        {
            ChunkPos = chunkPos;
            ChunkType = chunkType;
        }
    }
}
