#define LOCAL
#define COLLISION

using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.General;
using Mentula.General.Resources;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BtnSt = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MEx = Mentula.General.MathExtensions.Math;
using NCS = Lidgren.Network.NetConnectionStatus;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NOM = Lidgren.Network.NetOutgoingMessage;
using NPConf = Lidgren.Network.NetPeerConfiguration;

namespace Mentula.SurvivalGame
{
    public class Main : Game
    {
        private SpriteDrawer drawer;
        private NetClient client;
        private GameState state;

        private string error;
        private Action onRetry;

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

            NPConf config = new NPConf(Res.AppName);
            config.EnableMessageType(NIMT.DiscoveryResponse);
            client = new NetClient(config);
        }

        protected override void Initialize()
        {
            state = GameState.Initializing;

            tiles = new List<C_Tile>();
            dest = new List<C_Destrucible>();
            creatures = new List<C_Creature>();
            players = new Dictionary<string, C_Player>();
            client.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            state = GameState.Loading;
            player = new C_Player();
            oldPos = player.ChunkPos;

            drawer.Load(Content, ref player, "R/Textures", "Fonts/ConsoleFont", "Fonts/MenuFont", "Fonts/NameFont", "Actors/Player_Temp");
            drawer.Initialize(name =>
            {
                player.Name = name;
#if LOCAL
                client.DiscoverKnownPeer("localhost", Ips.PORT);
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

            if (IsActive)
            {
                if (state == GameState.Game)
                {
                    IsMouseVisible = false;
                    Vector2 inp = default(Vector2);

                    if (k_State.IsKeyDown(Keys.Escape)) this.Exit();
                    if (k_State.IsKeyDown(Keys.PrintScreen))
                    {
                        bool result = drawer.TakeScreenshot();
                        if(!result)
                        {
                            state = GameState.Error;
                            error = "An error occured while taking a screenshot.";
                            drawer.UpdateError(0, new MouseState(), error, null, null, null, true);
                            onRetry = () => state = GameState.MainMenu;
                        }
                    }
                    if (k_State.IsKeyDown(Keys.F)) 
                    {
                        drawer.ToggleFullScreen(); 
                        drawer.ApplyChanges(); 
                    }

                    if (k_State.IsKeyDown(Keys.W)) inp.Y -= 1;
                    if (k_State.IsKeyDown(Keys.S)) inp.Y += 1;
                    if (k_State.IsKeyDown(Keys.A)) inp.X -= 1;
                    if (k_State.IsKeyDown(Keys.D)) inp.X += 1;

                    if (inp != default(Vector2))
                    {
                        inp = Vector2.Normalize(inp) * C_Player.MOVEMENT * delta;
                        Vector2 outp = default(Vector2);

                        player.Move(inp);
#if COLLISION
                        IntVector2 NW_CPos = player.ChunkPos;
                        IntVector2 NE_CPos = player.GetTilePos().X + C_Player.DIFF >= Res.ChunkSize ? player.ChunkPos + IntVector2.UnitX : player.ChunkPos;
                        IntVector2 SW_CPos = player.GetTilePos().Y + C_Player.DIFF >= Res.ChunkSize ? player.ChunkPos + IntVector2.UnitY : player.ChunkPos;
                        IntVector2 SE_CPos = new IntVector2(NE_CPos.X, SW_CPos.Y);

                        IntVector2 NW_TPos = new IntVector2(player.GetTilePos());
                        IntVector2 NE_TPos = new IntVector2(Actor.FormatPos(player.GetTilePos() + new Vector2(C_Player.DIFF, 0)));
                        IntVector2 SW_TPos = new IntVector2(Actor.FormatPos(player.GetTilePos() + new Vector2(0, C_Player.DIFF)));
                        IntVector2 SE_TPos = new IntVector2(Actor.FormatPos(player.GetTilePos() + new Vector2(C_Player.DIFF)));

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
                                Vector2 dist = MEx.Abs(player.GetTotalPos() - (SE_CPos * Res.ChunkSize + SE_TPos).ToVector2());
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
                                Vector2 dist = MEx.Abs(player.GetTotalPos() - (NE_CPos * Res.ChunkSize + NE_TPos).ToVector2());
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
                                Vector2 dist = MEx.Abs(player.GetTotalPos() - (SW_CPos * Res.ChunkSize + SW_TPos).ToVector2());
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
                                Vector2 dist = MEx.Abs(player.GetTotalPos() - (NW_CPos * Res.ChunkSize + NW_TPos).ToVector2());
                                if (dist.X > dist.Y) outp.X = -inp.X;
                                else if (dist.X < dist.Y) outp.Y = -inp.Y;
                                else outp = -inp;
                            }
                            else if ((NW | SW) & !NE) outp.X = -inp.X;                  // Move left false, up
                            else if ((NE | NW) & !SW) outp.Y = -inp.Y;                  // Move left, up false
                            else if (NE & NW & SW) outp = -inp;                         // Move left false, up false
                        }

                        if (outp != default(Vector2)) player.Move(outp);
#endif
                    }

                    if (Mouse.GetState().LeftButton == BtnSt.Pressed & now > attackTime)    // If the player tyd to attack and can attack.
                    {
                        Vector2 playerPos = new Vector2(drawer.PreferredBackBufferWidth >> 1, drawer.PreferredBackBufferHeight >> 1);
                        float rot = MEx.VectorToDegrees(MentulaExtensions.GetMousePos() - playerPos);

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

                    if (now > nextSend)
                    {
                        NOM nom = client.CreateMessage();
                        nom.Write((byte)DataType.PlayerUpdate_Both);
                        nom.Write(player.ChunkPos);
                        nom.Write(player.GetTilePos());
                        client.SendMessage(nom, NetDeliveryMethod.UnreliableSequenced);
                        nextSend += .033f;
                    }
                }
                else if (state == GameState.MainMenu)    // If the player is in the main menu.
                {
                    IsMouseVisible = true;

                    if (client.ConnectionStatus == NCS.Connected)   // If the client has connected to the server.
                    {
                        state = GameState.Loading;
                        nextSend = NetTime.Now;
                        NOM nom = client.CreateMessage();
                        nom.Write((byte)DataType.InitialMap_Both);
                        client.SendMessage(nom, NetDeliveryMethod.ReliableUnordered);
                    }
                }
                else if (state == GameState.Error)
                {
                    IsMouseVisible = true;
                }
            }

            NIM msg = null;
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
                            case (NCS.Connected):       // The client succesfully connected to the server.
                                player.SetTilePos(Vector2.Zero);
                                break;
                            case (NCS.Disconnected):    // The client has been disconected from the server.
                                error = msg.ReadString();
                                state = GameState.Error;
                                drawer.UpdateError(0, new MouseState(), error, null, null, null, true);

                                onRetry = () =>
                                    {
                                        if (client.ServerConnection != null)
                                        {
                                            nom = client.CreateMessage(player.Name);    // Resent a connection request.
                                            client.Connect(msg.SenderEndPoint, nom);
                                        }
                                        else state = GameState.MainMenu;
                                    };
                                break;
                        }
                        break;
                    case (NIMT.Data):
                        switch (msg.ReadEnum<DataType>())
                        {
                            case (DataType.InitialMap_Both):        // Initialze the map.
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
                            case (DataType.ChunkRequest_Both):      // new chunks for the player.
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
                            case (DataType.PlayerUpdate_Both):      // Update the players.
                                players.Clear();
                                C_Player[] a_Players = msg.ReadPlayers();

                                for (int i = 0; i < a_Players.Length; i++)
                                {
                                    C_Player curr = a_Players[i];
                                    players.Add(curr.Name, curr);
                                }

                                drawer.Players = players;
                                break;
                            case (DataType.PlayerRePosition_SSend): // The players movement has been denied.
                                msg.ReadReSetPlayer(ref player);
                                break;
                            case (DataType.CreatureChange_SSend):   // A single creature has been changed by the server.
                                C_Creature c = msg.ReadCreature();
                                Vector2 totalPos = (c.ChunkPos.ToVector2() * Res.ChunkSize) + c.Pos;

                                if (player.GetTotalPos() == totalPos)
                                {
                                    player = new C_Player(player.Name, c.ChunkPos, c.Pos, c.State);
                                }
                                else
                                {
                                    C_Player p = players.Values.FirstOrDefault(v => v.GetTotalPos() == totalPos);
                                    if (p != null)
                                    {
                                        players[p.Name] = new C_Player(p.Name, c.ChunkPos, c.Pos, c.State);

                                        drawer.Players = players;
                                    }
                                    else
                                    {
                                        C_Creature cr = creatures.FirstOrDefault(v => v.ChunkPos == c.ChunkPos && v.Pos == c.Pos);

                                        if (cr != null)
                                        {
                                            creatures[creatures.IndexOf(cr)] = c;

                                            drawer.Creatures = creatures;
                                        }
                                    }
                                }
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
                case (GameState.Error):
                    drawer.UpdateError(delta, Mouse.GetState(), error, () => state = GameState.MainMenu, () => Exit(), onRetry);
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
            // Unload all chunks where the player is not present or are not ajason to the player chunk.
            const int RADIUS = 1;

            for (int i = 0; i < tiles.Count; )
            {
                if (Math.Abs(tiles[i].ChunkPos.X - pos.X) > RADIUS | Math.Abs(tiles[i].ChunkPos.Y - pos.Y) > RADIUS) tiles.RemoveAt(i);
                else i++;
            }

            for (int i = 0; i < dest.Count; )
            {
                if (Math.Abs(dest[i].ChunkPos.X - pos.X) > RADIUS | Math.Abs(dest[i].ChunkPos.Y - pos.Y) > RADIUS) dest.RemoveAt(i);
                else i++;
            }

            for (int i = 0; i < creatures.Count; )
            {
                if (Math.Abs(creatures[i].ChunkPos.X - pos.X) > RADIUS | Math.Abs(creatures[i].ChunkPos.Y - pos.Y) > RADIUS) creatures.RemoveAt(i);
                else i++;
            }
        }
    }
}