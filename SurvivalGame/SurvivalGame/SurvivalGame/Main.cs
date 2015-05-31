#define LOCAL
#define COLLISION

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

                if (k_State.IsKeyDown(Keys.W)) inp.Y -= 1;
                if (k_State.IsKeyDown(Keys.S)) inp.Y += 1;
                if (k_State.IsKeyDown(Keys.A)) inp.X -= 1;
                if (k_State.IsKeyDown(Keys.D)) inp.X += 1;
                if (k_State.IsKeyDown(Keys.F)) { drawer.IsFullScreen = true; drawer.ApplyChanges(); }
                if (k_State.IsKeyDown(Keys.PrintScreen)) drawer.TakeScreenshot();

                if (inp != Vector2.Zero)
                {
                    inp = Vector2.Normalize(inp) * C_Player.Movement * delta;
                    Vector2 outp = Vector2.Zero;

                    player.Move(inp);
#if COLLISION
                    IntVector2 NW_CPos = player.ChunkPos;
                    IntVector2 NE_CPos = player.GetTilePos().X + C_Player.Diff >= 32 ? player.ChunkPos + IntVector2.UnitX : player.ChunkPos;
                    IntVector2 SW_CPos = player.GetTilePos().Y + C_Player.Diff >= 32 ? player.ChunkPos + IntVector2.UnitY : player.ChunkPos;
                    IntVector2 SE_CPos = new IntVector2(NE_CPos.X, SW_CPos.Y);

                    IntVector2 NW_TPos = new IntVector2(player.GetTilePos());
                    IntVector2 NE_TPos = new IntVector2(Actor.FormatPos(player.GetTilePos() + new Vector2(C_Player.Diff, 0)));
                    IntVector2 SW_TPos = new IntVector2(Actor.FormatPos(player.GetTilePos() + new Vector2(0, C_Player.Diff)));
                    IntVector2 SE_TPos = new IntVector2(Actor.FormatPos(player.GetTilePos() + new Vector2(C_Player.Diff)));

                    bool? NW_T = null;
                    bool? NE_T = null;
                    bool? SW_T = null;
                    bool? SE_T = null;

                    for (int i = 0; i < dest.Count; i++)
                    {
                        C_Destrucible d = dest[i];

                        if (!NW_T.HasValue & d.ChunkPos == NW_CPos & d.Pos == NW_TPos) NW_T = d;
                        if (!NE_T.HasValue & d.ChunkPos == NE_CPos & d.Pos == NE_TPos) NE_T = d;
                        if (!SW_T.HasValue & d.ChunkPos == SW_CPos & d.Pos == SW_TPos) SW_T = d;
                        if (!SE_T.HasValue & d.ChunkPos == SE_CPos & d.Pos == SE_TPos) SE_T = d;

                        if (NW_T.HasValue & NE_T.HasValue & SW_T.HasValue & SE_T.HasValue) break;
                    }

                    bool NW = NW_T.HasValue ? NW_T.Value : false;
                    bool NE = NE_T.HasValue ? NE_T.Value : false;
                    bool SW = SW_T.HasValue ? SW_T.Value : false;
                    bool SE = SE_T.HasValue ? SE_T.Value : false;

                    if ((NE | SE) & inp.X > 0 & inp.Y == 0) outp.X = -inp.X;        // Move right false
                    else if ((NW | SW) & inp.X < 0 & inp.Y == 0) outp.X = -inp.X;   // Move left false
                    else if ((SE | SW) & inp.Y > 0 & inp.X == 0) outp.Y = -inp.Y;   // Move down false
                    else if ((NE | NW) & inp.Y < 0 & inp.X == 0) outp.Y = -inp.Y;   // Move up false
                    else if (inp.X > 0 & inp.Y > 0)                                 // Move right, down
                    {
                        if (SE & !NE & !SW)
                        {
                            Vector2 dist = MEx.Abs(player.GetTotalPos() - (SE_CPos * MentulaExtensions.ChunkSize + SE_TPos).ToVector2());
                            if (dist.X > dist.Y) outp.X = -inp.X;
                            else if (dist.X < dist.Y) outp.Y = -inp.Y;
                            else outp = -inp;
                        }
                        else if ((SE | NE) & !SW) outp.X = -inp.X;                  // Move right false, down
                        else if ((SE | SW) & !NE) outp.Y = -inp.Y;                  // Move right, down false
                        else if (SE & SW & NE) outp = -inp;                         // Move right false, down false
                    }
                    else if (inp.X > 0 & inp.Y < 0)                                 // Move right, up
                    {
                        if (NE & !SE & !NW)
                        {
                            Vector2 dist = MEx.Abs(player.GetTotalPos() - (NE_CPos * MentulaExtensions.ChunkSize + NE_TPos).ToVector2());
                            if (dist.X > dist.Y) outp.X = -inp.X;
                            else if (dist.X < dist.Y) outp.Y = -inp.Y;
                            else outp = -inp;
                        }
                        else if ((NE | SE) & !NW) outp.X = -inp.X;                  // Move right false, up
                        else if ((NE | NW) & !SE) outp.Y = -inp.Y;                  // Move righ, up false
                        else if (NE & NW & SE) outp = -inp;                         // Move right false, up false
                    }
                    else if (inp.X < 0 & inp.Y > 0)                                 // Move left, down
                    {
                        if (SW & !NW & !SE)
                        {
                            Vector2 dist = MEx.Abs(player.GetTotalPos() - (SW_CPos * MentulaExtensions.ChunkSize + SW_TPos).ToVector2());
                            if (dist.X > dist.Y) outp.X = -inp.X;
                            else if (dist.X < dist.Y) outp.Y = -inp.Y;
                            else outp = -inp;
                        }
                        else if ((SW | NW) & !SE) outp.X = -inp.X;                  // Move left false, down
                        else if ((SE | SW) & !NW) outp.Y = -inp.Y;                  // Move left, down false
                        else if (SE & SW & NW) outp = -inp;                         // Move left false, down false
                    }
                    else if (inp.X < 0 & inp.Y < 0)                                 // Move left, up
                    {
                        if (NW & !SW & !NE)
                        {
                            Vector2 dist = MEx.Abs(player.GetTotalPos() - (NW_CPos * MentulaExtensions.ChunkSize + NW_TPos).ToVector2());
                            if (dist.X > dist.Y) outp.X = -inp.X;
                            else if (dist.X < dist.Y) outp.Y = -inp.Y;
                            else outp = -inp;
                        }
                        else if ((NW | SW) & !NE) outp.X = -inp.X;                  // Move left false, up
                        else if ((NE | NW) & !SW) outp.Y = -inp.Y;                  // Move left, up false
                        else if (NE & NW & SW) outp = -inp;                         // Move left false, up false
                    }

                    player.Move(outp);
#endif
                }

                if (Mouse.GetState().LeftButton == BtnSt.Pressed & now > attackTime)
                {
                    Vector2 posF = new Vector2(drawer.PreferredBackBufferWidth >> 1, drawer.PreferredBackBufferHeight >> 1);
                    Vector2 dir = Vector2.Normalize(MentulaExtensions.GetMousePos() - posF);
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

                                drawer.Creatures = creatures.ToArray();
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