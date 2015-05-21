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
using MEx = Mentula.MathExtensions.Math;

namespace Mentula.SurvivalGame
{
    public class Main : Game
    {
        private int scrW = 1280;
        private int scrH = 720;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private NetClient client;
        private SpriteFont font;

        private Label lblName;
        private Label lblScrH;
        private Label lblScrW;
        private TextBox txtName;
        private TextBox txtScrH;
        private TextBox txtScrW;
        private Button btnDisc;
        private Button btnScrA;

        private Camera cam;
        private FPS counter;
        private GameState state;

        private C_Player player;
        private Dictionary<string, C_Player> players;
        private Texture2D Playertexture;
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

            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = scrW, PreferredBackBufferHeight = scrH, SynchronizeWithVerticalRetrace = false };
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

            counter = new FPS();
            cam = new Camera(GraphicsDevice, IntVector2.Zero, Vector2.Zero);
            tiles = new List<C_Tile>();
            dest = new List<C_Destrucible>();
            creatures = new List<C_Creature>();
            players = new Dictionary<string, C_Player>();

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

            player = new C_Player("Arzana", IntVector2.Zero, Vector2.Zero);
            oldPos = player.ChunkPos;
            cam.Update(player.ChunkPos, player.GetTilePos());

            lblName = new Label(GraphicsDevice, new Rectangle(scrW >> 1, scrH / 3, 150, 21), font) { AutoSize = true, BackColor = Color.MediumPurple, Text = "UserName:" };
            lblScrH = new Label(GraphicsDevice, new Rectangle(scrW >> 2, scrH / 3, 150, 21), font) { AutoSize = true, BackColor = Color.MediumPurple, Text = "Screen height :" };
            lblScrW = new Label(GraphicsDevice, new Rectangle(scrW >> 2, scrH / 3 + 25, 150, 21), font) { AutoSize = true, BackColor = Color.MediumPurple, Text = "Screen width  :" };

            txtName = new TextBox(GraphicsDevice, new Rectangle((scrW >> 1) + 100, scrH / 3, 150, 21), font) { AllowDrop = true, FlickerStyle = FlickerStyle.Fast };
            txtName.Click += (sender, e) =>
                {
                    sender.AllowDrop = true;
                    txtScrH.AllowDrop = false;
                    txtScrW.AllowDrop = false;
                };

            txtScrH = new TextBox(GraphicsDevice, new Rectangle((scrW >> 2) + 140, scrH / 3, 50, 21), font) { FlickerStyle = FlickerStyle.Fast, Text = scrH.ToString() };
            txtScrH.Click += (sender, e) =>
                {
                    sender.AllowDrop = true;
                    txtScrW.AllowDrop = false;
                    txtName.AllowDrop = false;
                };

            txtScrW = new TextBox(GraphicsDevice, new Rectangle((scrW >> 2) + 140, scrH / 3 + 25, 50, 21), font) { FlickerStyle = FlickerStyle.Fast, Text = scrW.ToString() };
            txtScrW.Click += (sender, e) =>
                {
                    sender.AllowDrop = true;
                    txtScrH.AllowDrop = false;
                    txtName.AllowDrop = false;
                };

            btnDisc = new Button(GraphicsDevice, new Rectangle(scrW >> 1, scrH / 3 + 50, 250, 21), font) { Text = "Discover server" };
            btnDisc.LeftClick += (sender, e) =>
                {
                    player.Name = txtName.Text;
#if LOCAL
                    client.DiscoverLocalPeers(Ips.PORT);
#endif
#if !LOCAL
                    client.DiscoverKnownPeer(Ips.EndShitPc);
#endif
                };

            btnScrA = new Button(GraphicsDevice, new Rectangle(scrW >> 2, scrH / 3 + 50, 200, 21), font) { Text = "Resize window" };
            btnScrA.LeftClick += (sender, e) =>
                {
                    int oldH = scrH;
                    int oldW = scrW;

                    if (int.TryParse(txtScrH.Text, out scrH) & int.TryParse(txtScrW.Text, out scrW) & scrH > 0 & scrW > 0)
                    {
                        graphics.PreferredBackBufferHeight = scrH;
                        graphics.PreferredBackBufferWidth = scrW;
                        graphics.ApplyChanges();
                        cam = new Camera(GraphicsDevice, player.ChunkPos, player.GetTilePos());
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(string.Format("The screen {0}({1}) is an invalid value!", scrH < 1 ? "height" : "width", scrH < 1 ? scrH : scrW), "Invalid dimentions!");

                        scrH = oldH;
                        scrW = oldW;
                    }
                };

            state = GameState.MainMenu;
        }

        protected override void Update(GameTime gameTime)
        {
            double now = NetTime.Now;
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state == GameState.Game & IsActive)
            {
                Vector2 inp = Vector2.Zero;
                KeyboardState k_State = Keyboard.GetState();

                if (k_State.IsKeyDown(Keys.Escape)) this.Exit();

                if (k_State.IsKeyDown(Keys.W)) inp.Y = -1;
                else if (k_State.IsKeyDown(Keys.S)) inp.Y = 1;
                if (k_State.IsKeyDown(Keys.A)) inp.X = -1;
                else if (k_State.IsKeyDown(Keys.D)) inp.X = 1;

                if (inp != Vector2.Zero) player.Move(inp * delta * 5);
                cam.Update(player.ChunkPos, player.GetTilePos());

                if (Mouse.GetState().LeftButton == BtnSt.Pressed & now > attackTime)
                {
                    Vector2 mPos = MentulaExtensions.GetMousePos();
                    Vector2 posF = new Vector2(scrW >> 1, scrH >> 1);
                    Vector2 dir = (mPos - posF); dir.Normalize();
                    float rot = MEx.VectorToDegrees(dir);

                    NOM nom = client.CreateMessage();
                    nom.Write((byte)DataType.Attack_CSend);
                    nom.Write(rot);
                    client.SendMessage(nom, NetDeliveryMethod.Unreliable);
                    attackTime = NetTime.Now + 500;
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
                MouseState m_State = Mouse.GetState();
                KeyboardState k_State = Keyboard.GetState();

                lblName.Update(m_State);
                lblScrH.Update(m_State);
                lblScrW.Update(m_State);

                btnDisc.Update(m_State, delta);
                btnScrA.Update(m_State, delta);

                if (txtName.AllowDrop) txtName.Update(m_State, k_State, delta);
                else ((GuiItem)txtName).Update(m_State);

                if (txtScrH.AllowDrop) txtScrH.Update(m_State, k_State, delta);
                else ((GuiItem)txtScrH).Update(m_State);

                if (txtScrW.AllowDrop) txtScrW.Update(m_State, k_State, delta);
                else ((GuiItem)txtScrW).Update(m_State);

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
                        if (status == NCS.Disconnected)
                        {
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
                                break;
                            case (DataType.PlayerUpdate_Both):
                                players.Clear();
                                C_Player[] p_A = msg.ReadPlayers();

                                for (int i = 0; i < p_A.Length; i++)
                                {
                                    C_Player p_C = p_A[i];
                                    players.Add(p_C.Name, p_C);
                                }
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
                    C_Player p = players.ElementAt(i).Value;

                    if (cam.TryGetRelativePosition(p.ChunkPos, p.GetTilePos(), out relPos)) spriteBatch.Draw(Playertexture, relPos, Color.White);
                }

                spriteBatch.Draw(Playertexture, cam.GetRelativePosition(player.ChunkPos, player.GetTilePos()), Color.White);
                spriteBatch.DrawString(font, string.Format("Player Pos: {0}", player.GetTotalPos()), new Vector2(0, 48), Color.Red);
                Vector2 camb = new Vector2(scrW / 2 + 32, scrH / 2 + 32);
                Vector2 dir = (MentulaExtensions.GetMousePos() - camb); dir.Normalize();
                float rot = (float)Math.Atan2(dir.X, dir.Y);
                spriteBatch.Draw(textures[10], camb + dir * 24 + new Vector2(-dir.Y * 8, dir.X * 8), Color.Red, -rot);

                spriteBatch.DrawString(font, string.Format("Dest: {0}", dest.Count), new Vector2(0, 16), Color.Red);
                spriteBatch.DrawString(font, string.Format("Creatures: {0}", creatures.Count), new Vector2(0, 32), Color.Red);
            }
            else if (state == GameState.MainMenu)
            {
                GraphicsDevice.Clear(Color.MediumPurple);
                lblName.Draw(spriteBatch);
                lblScrH.Draw(spriteBatch);
                lblScrW.Draw(spriteBatch);
                txtName.Draw(spriteBatch);
                txtScrH.Draw(spriteBatch);
                txtScrW.Draw(spriteBatch);
                btnDisc.Draw(spriteBatch);
                btnScrA.Draw(spriteBatch);
            }

            spriteBatch.Draw(textures[9], MentulaExtensions.GetMousePos() - new Vector2(8, 8), Color.Red);
            spriteBatch.DrawString(font, string.Format("Fps: {0}", counter.ToString()), Vector2.Zero, Color.Red);
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