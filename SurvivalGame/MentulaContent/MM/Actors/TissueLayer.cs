namespace Mentula.Content
{
    public class TissueLayer : MaterialLayer
    {
        public readonly bool essential;
        public readonly bool influencesEffectiveness;

        internal TissueLayer()
        {
            essential = false;
            influencesEffectiveness = false;
        }

        internal TissueLayer(MaterialLayer matter, bool essential, bool i)
            : base(matter)
        {
            this.essential = essential;
            influencesEffectiveness = i;
        }
    }
}