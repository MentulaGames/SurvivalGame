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
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using NCS = Lidgren.Network.NetConnectionStatus;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.SurvivalGame
{
    public class Main : Game
    {
        private const string Name = "Naxaras";

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private NetClient client;
        private SpriteFont font;

        private Camera cam;
        private FPS counter;
        private GameState state;

        private Player player;
        private Dictionary<string, Player> players;
        private Texture2D Playertexture;
        private TextureCollection textures;

        private IntVector2 oldPos;
        private double nextSend;
        private List<C_Tile> tiles;
        private List<C_Destrucible> dest;
        private List<C_Creature> creatures;

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
            creatures = new List<C_Creature>();
            players = new Dictionary<string, Player>();

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
            font = Content.Load<SpriteFont>("ConsoleFont");

            textures = new TextureCollection(Content, 11);
            textures[0] = Content.Load<Texture2D>("Tiles/Desert_Temp");
            textures[1] = Content.Load<Texture2D>("Tiles/Savana_Temp");
            textures[2] = Content.Load<Texture2D>("Tiles/Grassland_Temp");
            textures[3] = Content.Load<Texture2D>("Tiles/Forest_Temp");
            textures[4] = Content.Load<Texture2D>("Tiles/Tree_Temp");
            textures[5] = Content.Load<Texture2D>("Tiles/Water_Temp");
            textures[6] = Content.Load<Texture2D>("Actors/Rabbit_Temp");
            textures[7] = Content.Load<Texture2D>("Actors/Deer_Temp");
            textures[8] = Content.Load<Texture2D>("Actors/Wolf_Temp");
            textures[9] = Content.Load<Texture2D>("Utillities/Crosshair");
            textures[10] = Content.Load<Texture2D>("Utillities/DirectionArrow");
            Playertexture = Content.Load<Texture2D>("Actors/Player_Temp");

            player = new Player(Name, IntVector2.Zero, Vector2.Zero);
            oldPos = player.ChunkPos;
            cam.Update(player.ChunkPos, player.GetTilePos());
        }

        protected override void Update(GameTime gameTime)
        {
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

                if (inp != Vector2.Zero) player.Move(inp * delta * 5);
                cam.Update(player.ChunkPos, player.GetTilePos());

                if (oldPos != player.ChunkPos)
                {
                    NOM nom = client.CreateMessage();
                    nom.Write((byte)DataType.ChunkRequest);

                    nom.Write(player.ChunkPos);
                    nom.Write(oldPos);
                    client.SendMessage(nom, NetDeliveryMethod.ReliableUnordered);
                    oldPos = player.ChunkPos;
                }

            }
            else if (client.ConnectionStatus == NetConnectionStatus.Connected & this.state == GameState.Loading)
            {
                state = GameState.MainMenu;
                nextSend = NetTime.Now;
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.InitialMap);
                nom.Write(player.ChunkPos);
                client.SendMessage(nom, NetDeliveryMethod.ReliableUnordered);
            }

            double now = NetTime.Now;
            if (now > nextSend)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.PlayerUpdate);
                nom.Write(player.ChunkPos);
                nom.Write(player.GetTilePos());
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
                        nom.Write(player.Name);
                        client.Connect(msg.SenderEndPoint, nom);
                        break;
                    case (NIMT.StatusChanged):
                        NCS status = msg.ReadEnum<NCS>();
                        if (status == NCS.Disconnected)
                        {
                            string message = msg.ReadString();
                            DialogResult r = MessageBox.Show(message,
                                "The server closed the connection.",
                                MessageBoxButtons.AbortRetryIgnore);

                            switch (r)
                            {
                                case (DialogResult.Abort):
                                    this.Exit();
                                    break;
                                case (DialogResult.Retry):
                                    nom = client.CreateMessage();
                                    nom.Write(player.Name);
                                    client.Connect(msg.SenderEndPoint, nom);
                                    break;
                            }
                        }
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
                                    creatures.AddRange(msg.ReadCreatureArr());
                                }

                                state = GameState.Game;
                                break;
                            case (DataType.ChunkRequest):
                                length = msg.ReadInt16();

                                for (int i = 0; i < length; i++)
                                {
                                    tiles.AddRange(msg.ReadTileArr());
                                    dest.AddRange(msg.ReadDesArr());
                                    creatures.AddRange(msg.ReadCreatureArr());
                                }

                                Unload(player.ChunkPos);
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
                            case (DataType.PlayerRePosition):
                                msg.ReadReSetPlayer(ref player);
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
                Vector2 relPos;

                for (int i = 0; i < tiles.Count; i++)
                {
                    C_Tile t = tiles[i];

                    if (cam.TryGetRelativePosition(t.ChunkPos, t.Pos.ToVector2(), out relPos)) spriteBatch.Draw(textures[t.TextureId], relPos, Color.White, t.Layer);
                }

                for (int i = 0; i < dest.Count; i++)
                {
                    C_Destrucible d = dest[i];

                    if (cam.TryGetRelativePosition(d.ChunkPos, d.Pos.ToVector2(), out relPos)) spriteBatch.Draw(textures[d.TextureId], relPos, Color.White, d.Layer);
                }

                for (int i = 0; i < creatures.Count; i++)
                {
                    C_Creature c = creatures[i];

                    if (cam.TryGetRelativePosition(c.ChunkPos, c.Pos, out relPos)) spriteBatch.Draw(textures[c.TextureId], relPos, c.Color, 2);
                }

                for (int i = 0; i < players.Count; i++)
                {
                    Player p = players.ElementAt(i).Value;

                    if (cam.TryGetRelativePosition(p.ChunkPos, p.GetTilePos(), out relPos)) spriteBatch.Draw(Playertexture, relPos, Color.White);
                }

                spriteBatch.Draw(Playertexture, cam.GetRelativePosition(player.ChunkPos, player.GetTilePos()), Color.White);
                spriteBatch.DrawString(font, string.Format("Player Pos: {0}", player.GetTotalPos()), new Vector2(0, 48), Color.Red);
            }

            spriteBatch.DrawString(font, string.Format("Fps: {0}", counter.ToString()), Vector2.Zero, Color.Red);
            spriteBatch.DrawString(font, string.Format("Dest: {0}", dest.Count), new Vector2(0, 16), Color.Red);
            spriteBatch.DrawString(font, string.Format("Creatures: {0}", creatures.Count), new Vector2(0, 32), Color.Red);
            spriteBatch.Draw(textures[9], new Vector2(Mouse.GetState().X - 16, Mouse.GetState().Y - 16), Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Shutdown("bye");
            base.OnExiting(sender, args);
        }

        private void Unload(IntVector2 pos)
        {
            for (int i = 0; i < tiles.Count; )
            {
                if (Math.Abs(tiles[i].ChunkPos.X - pos.X) > 1 | Math.Abs(tiles[i].ChunkPos.Y - pos.Y) > 1) tiles.RemoveAt(i);
                else i++;
            }

            for (int i = 0; i < dest.Count; )
            {
                if (Math.Abs(dest[i].ChunkPos.X - pos.X) > 1 | Math.Abs(dest[i].ChunkPos.Y - pos.Y) > 1) dest.RemoveAt(i);
                else i++;
            }

            for (int i = 0; i < creatures.Count; )
            {
                if (Math.Abs(creatures[i].ChunkPos.X - pos.X) > 1 | Math.Abs(creatures[i].ChunkPos.Y - pos.Y) > 1) creatures.RemoveAt(i);
                else i++;
            }
        }
    }
}