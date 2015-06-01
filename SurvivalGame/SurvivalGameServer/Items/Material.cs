namespace Mentula.SurvivalGameServer.Items
{
    public abstract class Material
    {
        public readonly int Id;
        public readonly string Name;

        public readonly float Ultimate_Tensile_Strength;
        public readonly float Tensile_Strain_At_Yield;
        public readonly float Density;

        public Material()
        {
            Id = -1;
            Name = "Unobtanium";
            Ultimate_Tensile_Strength = float.PositiveInfinity;
            Tensile_Strain_At_Yield = float.PositiveInfinity;
            Density = float.PositiveInfinity;
        }

        public Material(int id, string name, float UTS, float TSAY, float density)
        {
            Id = id;
            Name = name;
            Ultimate_Tensile_Strength = UTS;
            Tensile_Strain_At_Yield = TSAY;
            Density = density;
        }
    }
}