namespace Mentula.Content
{
    public class Cheats
    {
        public static Metal Unobtanium { get; private set; }

        static Cheats()
        {
            Unobtanium = new Metal();
        }
    }
}