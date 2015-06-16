using System.Diagnostics;
using System.Linq;

namespace Mentula.Content
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    public abstract class Material
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public float Ultimate_Tensile_Strength { get; private set; }
        public float Tensile_Strain_At_Yield { get; private set; }
        public float Density { get; private set; }

        internal Material()
        {
            Id = -1;
            Name = "Unobtanium";
            Ultimate_Tensile_Strength = float.PositiveInfinity;
            Tensile_Strain_At_Yield = float.PositiveInfinity;
            Density = float.PositiveInfinity;
        }

        internal Material(int id, string name, float UTS, float TSAY, float density)
        {
            Id = id;
            Name = name;
            Ultimate_Tensile_Strength = UTS;
            Tensile_Strain_At_Yield = TSAY;
            Density = density;
        }

        internal Material(Material m)
        {
            Id = m.Id;
            Name = m.Name;
            Ultimate_Tensile_Strength = m.Ultimate_Tensile_Strength;
            Tensile_Strain_At_Yield = m.Tensile_Strain_At_Yield;
            Density = m.Density;
        }

        public void InitRefrence(Material[] dataset)
        {
            Material dataEntry = dataset.FirstOrDefault(m => m.Id == Id);

            if (dataEntry == null) return;

            Name = dataEntry.Name;
            Ultimate_Tensile_Strength = dataEntry.Ultimate_Tensile_Strength;
            Tensile_Strain_At_Yield = dataEntry.Tensile_Strain_At_Yield;
            Density = dataEntry.Density;
        }

        public override string ToString()
        {
            return "Id=" + Id.ToString() + " Name=" + Name + " UTS=" + Ultimate_Tensile_Strength.ToString() + " TSAY=" + Tensile_Strain_At_Yield.ToString() + " Dens=" + Density.ToString();
        }
    }
}