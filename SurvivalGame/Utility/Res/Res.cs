namespace Mentula.General.Resources
{
    public static class Res
    {
        public static string AppName;

        public static int ChunkSize;
        public static int TileSize;

        static Res()
        {
            AppName = Resources.AppName;
            ChunkSize = int.Parse(Resources.ChunkSize);
            TileSize = int.Parse(Resources.TileSize);
        }
    }
}