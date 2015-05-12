using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class Camera : Actor
    {
        public IntVector2 Bounds;
        public Camera()
            : base()
        {
            this.Bounds = new IntVector2();
        }
        public Camera(IntVector2 ChunkPos, Vector2 tilePos, IntVector2 Bounds)
            : base(ChunkPos, tilePos)
        {
            this.Bounds = Bounds;
        }
    }
}
