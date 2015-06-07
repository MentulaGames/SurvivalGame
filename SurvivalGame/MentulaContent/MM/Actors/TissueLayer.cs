using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mentula.Content
{
    public class TissueLayer : MaterialLayer
    {
        public bool essential;
        public bool influencesEffectiveness;

        public TissueLayer()
        {
            essential = false;
            influencesEffectiveness = false;
        }

        public TissueLayer(MaterialLayer matter, bool essential, bool i)
            : base(matter)
        {
            this.essential = essential;
            influencesEffectiveness = i;
        }
    }
}
