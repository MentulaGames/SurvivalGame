using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;

namespace Mentula.SurvivalGameServer
{
    public class MegaChunk
    {
        public const int MEGACHUNKSIZE = 128;
        public ChunkData[] ChunkData;
        public IntVector2 Pos;
        public MegaChunk(IntVector2 pos)
        {
            Pos = pos;
            ChunkData = MegaChunkGenerator.GenerateMegaChunk(pos);
        }
    }
}
