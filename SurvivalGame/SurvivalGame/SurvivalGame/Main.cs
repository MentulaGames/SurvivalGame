#define LOCAL

using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.General;
using Mentula.General.Res;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.SurvivalGame
{
    public class Main : Game
    {
        private const string Name = "Arzana";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private NetClient client;
        private SpriteFont font;

        private Camera cam;
        private FPS counter;
        private GameState state;

        private Dictionary<string, Player> players;
        private Texture2D Playertexture;
        private Texture2D[] textures;

        private IntVector2 oldPos;
        private List<C_Tile> tiles;
        private List<C_Destrucible> dest;
        private double nextSend;

        public Main()
        {
            state = GameState.Constructing;

            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720, SynchronizeWithVerticalRetrace = false };
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            Content.RootDirectory = "Content";

            NPConf config = new NPConf(Resources.AppName);
            config.EnableMessageType(NIMT.DiscoveryResponse);
            client = new NetClient(config);
            client.Start();
        }

        protected override void Initialize()
        {
            state = GameState.Initializing;

            counter = new FPS();
            cam = new Camera(GraphicsDevice, IntVector2.Zero, Vector2.Zero);
            tiles = new List<C_Tile>();
            dest = new List<C_Destrucible>();
            players = new Dictionary<string, Player>();

#if LOCAL
            client.DiscoverLocalPeers(Ips.PORT);
#endif
#if !LOCAL
            client.DiscoverKnownPeer(Ips.EndJo�ll);
#endif

            base.Initialize();
        }

        protected override void LoadContent()
        {
            state = GameState.Loading;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Playertexture = new Texture2D(GraphicsDevice, 32, 32);
            font = Content.Load<SpriteFont>("ConsoleFont");

            textures = new Texture2D[6];
            textures[0] = Content.Load<Texture2D>("Tiles/Desert_Temp");
            textures[1] = Content.Load<Texture2D>("Tiles/Savana_Temp");
            textures[2] = Content.Load<Texture2D>("Tiles/Grassland_Temp");
            textures[3] = Content.Load<Texture2D>("Tiles/Forest_Temp");
            textures[4] = Content.Load<Texture2D>("Tiles/Tree_Temp");
            textures[5] = Content.Load<Texture2D>("Tiles/Water_Temp");

            Color[] data = new Color[Playertexture.Height * Playertexture.Width];
            for (int y = 0; y < Playertexture.Height; y++)
            {
                for (int x = 0; x < Playertexture.Width; x++)
                {
                    data[x + (y * Playertexture.Height)] = Color.Red;
                }
            }
            Playertexture.SetData<Color>(data);

            IntVector2 chunkPos = IntVector2.Zero;
            Vector2 pos = Vector2.Zero;
            players.Add(Name, new Player(Name, chunkPos, pos));
            oldPos = chunkPos;
            cam.Update(chunkPos, pos);
        }

        protected override void Update(GameTime gameTime)
        {
            Player p = players[Name];

            if (state == GameState.Game & IsActive)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 inp = Vector2.Zero;
                KeyboardState k_State = Keyboard.GetState();

                if (k_State.IsKeyDown(Keys.Escape)) this.Exit();

                if (k_State.IsKeyDown(Keys.W)) inp.Y = -1;
                else if (k_State.IsKeyDown(Keys.S)) inp.Y = 1;
                if (k_State.IsKeyDown(Keys.A)) inp.X = -1;
                else if (k_State.IsKeyDown(Keys.D)) inp.X = 1;

                if (inp != Vector2.Zero) p.Move(inp * delta * 5);
                cam.Update(p.ChunkPos, p.GetTilePos());

                if (oldPos != p.ChunkPos)
                {
                    NOM nom = client.CreateMessage();
                    nom.Write((byte)DataType.ChunkRequest);

                    nom.Write(p.ChunkPos);
                    nom.Write(oldPos);
                    client.SendMessage(nom, NetDeliveryMethod.Unreliable);
                    oldPos = p.ChunkPos;
                }

            }
            else if (client.ConnectionStatus == NetConnectionStatus.Connected & this.state == GameState.Loading)
            {
                this.state = GameState.MainMenu;
                nextSend = NetTime.Now;
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.InitialMap);
                nom.Write(p.ChunkPos);
                client.SendMessage(nom, NetDeliveryMethod.Unreliable);
            }

            double now = NetTime.Now;
            if (client.ConnectionStatus == NetConnectionStatus.Disconnected & state == GameState.Game) Exit();
            else if (now > nextSend)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.PlayerUpdate);
                nom.Write(p.ChunkPos);
                nom.Write(p.GetTilePos());
                client.SendMessage(nom, NetDeliveryMethod.Unreliable);
                nextSend += (1f / 30f);
            }

            NIM msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case (NIMT.DiscoveryResponse):
                        NOM nom = client.CreateMessage();
                        nom.Write(p.Name);
                        client.Connect(msg.SenderEndPoint, nom);
                        break;
                    case (NIMT.Data):
                        switch (msg.ReadEnum<DataType>())
                        {
                            case (DataType.InitialMap):
                                int length = msg.ReadInt16();

                                for (int i = 0; i < length; i++)
                                {
                                    tiles.AddRange(msg.ReadTileArr());
                                    dest.AddRange(msg.ReadDesArr());
                                }

                                state = GameState.Game;
                                break;
                            case (DataType.ChunkRequest):
                                length = msg.ReadInt16();

                                for (int i = 0; i < length; i++)
                                {
                                    tiles.AddRange(msg.ReadTileArr());
                                    dest.AddRange(msg.ReadDesArr());
                                }

                                UnloadCTiles(p.ChunkPos);
                                UnLoadCDest(p.ChunkPos);
                                break;
                            case (DataType.PlayerUpdate):
                                players.Clear();
                                Player[] p_A = msg.ReadPlayers();

                                for (int i = 0; i < p_A.Length; i++)
                                {
                                    Player p_C = p_A[i];
                                    players.Add(p_C.Name, p_C);
                                }
                                break;
                        }
                        break;
                }
            }

            Thread.Sleep(1);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            counter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (state == GameState.Game)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    C_Tile t = tiles[i];
                    Vector2 relPos;

                    if (cam.TryGetRelativePosition(t.ChunkPos, t.Pos.ToVector2(), out relPos)) spriteBatch.Draw(textures[t.TextureId], relPos, Color.White, t.Layer);
                }

                for (int i = 0; i < dest.Count; i++)
                {
                    C_Destrucible d = dest[i];
                    Vector2 relPos;

                    if (cam.TryGetRelativePosition(d.ChunkPos, d.Pos.ToVector2(), out relPos)) spriteBatch.Draw(textures[d.TextureId], relPos, Color.White, d.Layer);
                }
            }

            for (int i = 0; i < players.Count; i++)
            {
                Player p = players.ElementAt(i).Value;
                spriteBatch.Draw(Playertexture, cam.GetRelativePosition(p.ChunkPos, p.GetTilePos()), Color.White);
                spriteBatch.DrawString(font, string.Format("Pos({0}): {1}", p.Name, p.GetTotalPos()), new Vector2(0, 48 + (i * 16)), Color.Red);
            }

            spriteBatch.DrawString(font, string.Format("State: {0}", state), Vector2.Zero, Color.Red);
            spriteBatch.DrawString(font, string.Format("Fps: {0}", counter.ToString()), new Vector2(0, 16), Color.Red);
            spriteBatch.DrawString(font, string.Format("Dest: {0}", dest.Count), new Vector2(0, 32), Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Shutdown("bye");
            base.OnExiting(sender, args);
        }

        private void UnloadCTiles(IntVector2 pos)
        {
            for (int i = 0; i < tiles.Count; )
            {
                if (Math.Abs(tiles[i].ChunkPos.X - pos.X) > 1 | Math.Abs(tiles[i].ChunkPos.Y - pos.Y) > 1) tiles.RemoveAt(i);
                else i++;
            }
        }

        private void UnLoadCDest(IntVector2 pos)
        {
            for (int i = 0; i < dest.Count; )
            {
                if (Math.Abs(dest[i].ChunkPos.X - pos.X) > 1 | Math.Abs(dest[i].ChunkPos.Y - pos.Y) > 1) dest.RemoveAt(i);
                else i++;
            }
        }
    }
}