using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mentula.SurvivalGameServer
{
    public class TissueLayer:MaterialLayer
    {
        public bool essential;
        public bool influencesEffectiveness;
        public TissueLayer(MaterialLayer matter, bool essential, bool i)
            :base(matter)
        {
            this.essential = essential;
            influencesEffectiveness = i;
        }
    }
}
