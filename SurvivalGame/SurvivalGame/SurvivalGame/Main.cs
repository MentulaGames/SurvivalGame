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
using XnaGuiItems.Core;
using XnaGuiItems.Items;
using BtnSt = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using NCS = Lidgren.Network.NetConnectionStatus;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;
using MEx = Mentula.General.MathExtensions.Math;

namespace Mentula.SurvivalGame
{
    public class Main : Game
    {
        private SpriteDrawer drawer;
        private NetClient client;

        private GameState state;

        private C_Player player;
        private Dictionary<string, C_Player> players;
        private TextureCollection textures;

        private IntVector2 oldPos;
        private double nextSend;
        private double attackTime;
        private List<C_Tile> tiles;
        private List<C_Destrucible> dest;
        private List<C_Creature> creatures;

        public Main()
        {
            state = GameState.Constructing;

            drawer = new SpriteDrawer(this, false) { SynchronizeWithVerticalRetrace = false };
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";

            NPConf config = new NPConf(Resources.AppName);
            config.EnableMessageType(NIMT.DiscoveryResponse);
            client = new NetClient(config);
            client.Start();
        }

        protected override void Initialize()
        {
            state = GameState.Initializing;

            tiles = new List<C_Tile>();
            dest = new List<C_Destrucible>();
            creatures = new List<C_Creature>();
            players = new Dictionary<string, C_Player>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            state = GameState.Loading;

            textures = new TextureCollection(Content, 11);
            textures.LoadFromConfig("R/Textures");

            drawer.Load(Content, textures, "ConsoleFont", "Actors/Player_Temp", name =>
            {
                player.Name = name;
#if LOCAL
                client.DiscoverLocalPeers(Ips.PORT);
#endif
#if !LOCAL
                client.DiscoverKnownPeer(Ips.EndJo�ll);
#endif
            });

            player = new C_Player("NameLess", IntVector2.Zero, Vector2.Zero);
            oldPos = player.ChunkPos;

            state = GameState.MainMenu;
        }

        protected override void Update(GameTime gameTime)
        {
            double now = NetTime.Now;
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState k_State = Keyboard.GetState();

            if (state == GameState.Game & IsActive)
            {
                IsMouseVisible = false;
                Vector2 inp = Vector2.Zero;

                if (k_State.IsKeyDown(Keys.Escape)) this.Exit();

                if (k_State.IsKeyDown(Keys.W)) inp.Y = -1;
                else if (k_State.IsKeyDown(Keys.S)) inp.Y = 1;
                if (k_State.IsKeyDown(Keys.A)) inp.X = -1;
                else if (k_State.IsKeyDown(Keys.D)) inp.X = 1;

                if (inp != Vector2.Zero) player.Move(inp * delta * 5);

                if (Mouse.GetState().LeftButton == BtnSt.Pressed & now > attackTime)
                {
                    Vector2 mPos = MentulaExtensions.GetMousePos();
                    Vector2 posF = new Vector2(drawer.PreferredBackBufferWidth >> 1, drawer.PreferredBackBufferHeight >> 1);
                    Vector2 dir = (mPos - posF); dir.Normalize();
                    float rot = MEx.VectorToDegrees(dir);

                    NOM nom = client.CreateMessage();
                    nom.Write((byte)DataType.Attack_CSend);
                    nom.Write(rot);
                    client.SendMessage(nom, NetDeliveryMethod.Unreliable);
                    attackTime = NetTime.Now + .5f;
                }

                if (oldPos != player.ChunkPos)
                {
                    NOM nom = client.CreateMessage();
                    nom.Write((byte)DataType.ChunkRequest_Both);

                    nom.Write(player.ChunkPos);
                    nom.Write(oldPos);
                    client.SendMessage(nom, NetDeliveryMethod.ReliableUnordered);
                    oldPos = player.ChunkPos;
                }

            }
            else if (state == GameState.MainMenu & IsActive)
            {
                IsMouseVisible = true;
                if (client.ConnectionStatus == NCS.Connected)
                {
                    state = GameState.Loading;
                    nextSend = NetTime.Now;
                    NOM nom = client.CreateMessage();
                    nom.Write((byte)DataType.InitialMap_Both);
                    nom.Write(player.ChunkPos);
                    client.SendMessage(nom, NetDeliveryMethod.ReliableUnordered);
                }
            }

            if (now > nextSend)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.PlayerUpdate_Both);
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
                        switch (status)
                        {
                            case (NCS.Connected):
                                player.ChunkPos = IntVector2.Zero;
                                player.SetTilePos(Vector2.Zero);
                                break;
                            case (NCS.Disconnected):
                                string message = msg.ReadString();
                                System.Windows.Forms.DialogResult r = System.Windows.Forms.MessageBox.Show(message,
                                    "The server closed the connection.",
                                    System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);

                                switch (r)
                                {
                                    case (System.Windows.Forms.DialogResult.Abort):
                                        this.Exit();
                                        break;
                                    case (System.Windows.Forms.DialogResult.Retry):
                                        nom = client.CreateMessage();
                                        nom.Write(player.Name);
                                        client.Connect(msg.SenderEndPoint, nom);
                                        break;
                                }
                                break;
                        }
                        break;
                    case (NIMT.Data):
                        switch (msg.ReadEnum<DataType>())
                        {
                            case (DataType.InitialMap_Both):
                                int length = msg.ReadInt16();

                                for (int i = 0; i < length; i++)
                                {
                                    tiles.AddRange(msg.ReadTileArr());
                                    dest.AddRange(msg.ReadDesArr());
                                    creatures.AddRange(msg.ReadCreatureArr());
                                }

                                drawer.Tiles = tiles;
                                drawer.Dest = dest;
                                drawer.Creatures = creatures;
                                state = GameState.Game;
                                break;
                            case (DataType.ChunkRequest_Both):
                                length = msg.ReadInt16();

                                for (int i = 0; i < length; i++)
                                {
                                    tiles.AddRange(msg.ReadTileArr());
                                    dest.AddRange(msg.ReadDesArr());
                                    creatures.AddRange(msg.ReadCreatureArr());
                                }

                                Unload(player.ChunkPos);
                                drawer.Tiles = tiles;
                                drawer.Dest = dest;
                                drawer.Creatures = creatures;
                                break;
                            case (DataType.PlayerUpdate_Both):
                                players.Clear();
                                C_Player[] p_A = msg.ReadPlayers();

                                for (int i = 0; i < p_A.Length; i++)
                                {
                                    C_Player p_C = p_A[i];
                                    players.Add(p_C.Name, p_C);
                                }

                                drawer.Players = players;
                                break;
                            case (DataType.PlayerRePosition_SSend):
                                msg.ReadReSetPlayer(ref player);
                                break;
                            case (DataType.CreatureChange_SSend):
                                IntVector2 cPos = msg.ReadVector();
                                Vector2 pos = msg.ReadVector2();
                                float health = msg.ReadFloat();

                                if (health <= 0) creatures.RemoveAll(c => c.ChunkPos == cPos & c.Pos == pos);
                                else creatures.Find(c => c.ChunkPos == cPos & c.Pos == pos).Health = health;

                                drawer.Creatures = creatures;
                                break;
                        }
                        break;
                }
            }

            drawer.Update(delta, Mouse.GetState(), k_State, player, state);
            Thread.Sleep(1);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            drawer.Draw((float)gameTime.ElapsedGameTime.TotalSeconds, state);
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