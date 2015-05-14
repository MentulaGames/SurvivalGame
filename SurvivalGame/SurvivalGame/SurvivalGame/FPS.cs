using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.SurvivalGame
{
    public class FPS
    {
        protected const int FRAME_BUFFER = 100;
        public float Current;
        public float Avarage;

        private Queue<float> buffer;

        public FPS()
        {
            Current = 0;
            Avarage = 0;
            buffer = new Queue<float>();
        }

        public void Update(float delta)
        {
            Current = 1 / delta;
            buffer.Enqueue(Current);

            if (buffer.Count > FRAME_BUFFER) buffer.Dequeue();

            Avarage = buffer.Average();
        }

        public override string ToString()
        {
            return ((int)Current).ToString();
        }
    }
}
