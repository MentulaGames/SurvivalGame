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
        private IntVector2 oldPos;
        private double nextSend;
        private double attackTime;

        private List<C_Tile> tiles;
        private List<C_Destrucible> dest;
        private List<C_Creature> creatures;
        private Dictionary<string, C_Player> players;

        public Main()
        {
            state = GameState.Constructing;
            drawer = new SpriteDrawer(this, true) { SynchronizeWithVerticalRetrace = false };
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
            player = new C_Player();
            oldPos = player.ChunkPos;

            drawer.Load(Content, ref player, "R/Textures", "Fonts/ConsoleFont", "Fonts/MenuFont", "Fonts/NameFont", "Actors/Player_Temp", name =>
            {
                player.Name = name;
#if LOCAL
                client.DiscoverLocalPeers(Ips.PORT);
#endif
#if !LOCAL
                client.DiscoverKnownPeer(Ips.EndJoëll);
#endif
            });

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

                if (inp != Vector2.Zero)
                {
                    inp = Vector2.Normalize(inp) * 10f * delta;
                    IntVector2 old = new IntVector2(player.GetTilePos());
                    IntVector2 @new = new IntVector2(Actor.FormatPos(player.GetTilePos() + inp));
                    player.Move(inp);

                    if ((@new - old).Length > 0)
                    {
                        C_Destrucible c_D_LU = dest.FirstOrDefault(d => d.ChunkPos == player.ChunkPos & d.Pos == @new);
                        C_Destrucible c_D_RU = dest.FirstOrDefault(d => d.ChunkPos == player.ChunkPos & d.Pos == new IntVector2(@new.X + 1, @new.Y));
                        C_Destrucible c_D_LD = dest.FirstOrDefault(d => d.ChunkPos == player.ChunkPos & d.Pos == new IntVector2(@new.X, @new.Y + 1));
                        C_Destrucible c_D_RD = dest.FirstOrDefault(d => d.ChunkPos == player.ChunkPos & d.Pos == new IntVector2(@new.X + 1, @new.Y + 1));

                        if ((c_D_LU != null && !c_D_LU.Walkable) | (c_D_RU != null && !c_D_RU.Walkable) | (c_D_LD != null && !c_D_LD.Walkable) | (c_D_RD != null && !c_D_RD.Walkable)) player.Move(-inp);
                    }
                }

                if (Mouse.GetState().LeftButton == BtnSt.Pressed & now > attackTime)
                {
                    Vector2 mPos = MentulaExtensions.GetMousePos();
                    Vector2 posF = new Vector2(drawer.PreferredBackBufferWidth >> 1, drawer.PreferredBackBufferHeight >> 1);
                    Vector2 dir = Vector2.Normalize(mPos - posF);
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
                    client.SendMessage(nom, NetDeliveryMethod.Unreliable);
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
                    client.SendMessage(nom, NetDeliveryMethod.ReliableUnordered);
                }
            }

            if (now > nextSend)
            {
                NOM nom = client.CreateMessage();
                nom.Write((byte)DataType.PlayerUpdate_Both);
                nom.Write(player.ChunkPos);
                nom.Write(player.GetTilePos());
                client.SendMessage(nom, NetDeliveryMethod.UnreliableSequenced);
                nextSend += (1f / 30f);
            }

            NIM msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case (NIMT.DiscoveryResponse):
                        NOM nom = client.CreateMessage(player.Name);
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
                                        nom = client.CreateMessage(player.Name);
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

                                drawer.SetData(ref tiles, ref dest, ref creatures);
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
                                drawer.SetData(ref tiles, ref dest, ref creatures);
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

            switch (state)
            {
                case (GameState.MainMenu):
                    drawer.UpdateMain(delta, Mouse.GetState(), ref k_State);
                    break;
                case (GameState.Game):
                    drawer.UpdateGame(delta, ref player);
                    break;
            }
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