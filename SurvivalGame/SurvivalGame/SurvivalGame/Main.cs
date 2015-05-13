#define LOCAL
//#define PLAYER

using Lidgren.Network;
using Mentula.General;
using Mentula.General.Res;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Lidgren.Network.Xna;
using System.Collections.Generic;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.SurvivalGame
{
    public class Main : Game
    {
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

        private List<CTile> tiles;
        private double nextSend;

#if PLAYER
        private const string Name = "Naxaras";
#endif
#if !PLAYER
        private const string Name = "Arzana";
#endif

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
            cam = new Camera(Vector2.Zero, GraphicsDevice.Viewport.Bounds);
            tiles = new List<CTile>();

#if LOCAL
            client.DiscoverLocalPeers(Ips.PORT);
#endif
#if !LOCAL
            client.DiscoverKnownPeer(Ips.EndJoëll);
#endif

            base.Initialize();
        }

        protected override void LoadContent()
        {
            state = GameState.Loading;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Playertexture = new Texture2D(GraphicsDevice, 32, 32);
            font = Content.Load<SpriteFont>("ConsoleFont");

            textures = new Texture2D[4];
            textures[0] = Content.Load<Texture2D>("Tiles/Desert_Temp");
            textures[1] = Content.Load<Texture2D>("Tiles/Savana_Temp");
            textures[2] = Content.Load<Texture2D>("Tiles/Grassland_Temp");
            textures[3] = Content.Load<Texture2D>("Tiles/Forest_Temp");

            Color[] data = new Color[Playertexture.Height * Playertexture.Width];
            for (int y = 0; y < Playertexture.Height; y++)
            {
                for (int x = 0; x < Playertexture.Width; x++)
                {
                    data[x + (y * Playertexture.Height)] = Color.Red;
                }
            }
            Playertexture.SetData<Color>(data);

            players = new Dictionary<string, Player>();
            players.Add(Name, new Player(Name, IntVector2.Zero, Vector2.Zero));
        }

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 inp = new Vector2();
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape)) this.Exit();

            if (state.IsKeyDown(Keys.W)) inp.Y = -1;
            else if (state.IsKeyDown(Keys.S)) inp.Y = 1;
            if (state.IsKeyDown(Keys.A)) inp.X = -1;
            else if (state.IsKeyDown(Keys.D)) inp.X = 1;
            cam.Move(inp);
            players[Name].SetTilePos(players[Name].GetTilePos() + inp);
            cam.Update();

            if (client.ConnectionStatus == NetConnectionStatus.Connected && this.state == GameState.Loading)
            {
                this.state = GameState.MainMenu;
                nextSend = NetTime.Now;
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.InitialMap);
                nom.Write(players[Name].ChunkPos);
                client.SendMessage(nom, NetDeliveryMethod.Unreliable);
            }

            double now = NetTime.Now;
            if (now > nextSend)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.PlayerUpdate);
                Player p = players[Name];
                nom.Write(p.ChunkPos);
                nom.Write(p.GetTilePos());
                client.SendMessage(nom, NetDeliveryMethod.Unreliable);
            }

            NIM msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case (NIMT.DiscoveryResponse):
                        NOM nom = client.CreateMessage();
                        nom.Write(players[Name].Name);
                        client.Connect(msg.SenderEndPoint, nom);
                        break;
                    case (NIMT.Data):
                        switch (msg.ReadEnum<DataType>())
                        {
                            case (DataType.InitialMap):
                                int length = msg.ReadInt32();

                                for (int i = 0; i < length; i++)
                                {
                                    tiles.AddRange(msg.ReadTileArr());
                                }
                                this.state = GameState.Game;
                                break;
                            case (DataType.PlayerUpdate):
                                string name = msg.ReadString();
                                if (players.FirstOrDefault(p => p.Key == name).Value != null) players[name].ReSet(msg.ReadVector(), msg.ReadVector2());
                                else players.Add(name, new Player(name, msg.ReadVector(), msg.ReadVector2()));
                                break;
                        }
                        break;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            counter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

            if (state == GameState.Game)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    CTile t = tiles[i];
                    spriteBatch.Draw(textures[t.TextureId], cam.GetRelativePosition(t.ChunkPos, t.Pos), Color.White);
                }
            }

            spriteBatch.DrawString(font, string.Format("State: {0}", state), Vector2.Zero, Color.Red);
            spriteBatch.DrawString(font, string.Format("Fps: {0}", counter.Avarage), new Vector2(0, 16), Color.Red);
            for (int i = 0; i < players.Count; i++)
            {
                spriteBatch.DrawString(font, string.Format("Pos({0}): {1}", players.ElementAt(i).Value.Name, players.ElementAt(i).Value.GetTotalPos()), new Vector2(0, 32 + (i * 16)), Color.Red);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Shutdown("bye");
            base.OnExiting(sender, args);
        }
    }
}