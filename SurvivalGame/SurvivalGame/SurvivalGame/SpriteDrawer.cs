using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mentula.General;
using Microsoft.Xna.Framework.Content;
using XnaGuiItems.Core;
using Microsoft.Xna.Framework.Input;
using XnaGuiItems.Items;

namespace Mentula.SurvivalGame
{
    public class SpriteDrawer : GraphicsDeviceManager
    {
        public List<C_Tile> Tiles;
        public List<C_Destrucible> Dest;
        public List<C_Creature> Creatures;
        public Dictionary<string, C_Player> Players;

        private SpriteBatch batch;
        private Camera cam;
        private FPS counter;
        private bool debug;

        private TextureCollection texC;
        private Texture2D pTexture;
        private C_Player player;

        private Vector2 relPlayerPos;
        private Vector2 arrowPos;
        private float arrowRot;

        private SpriteFont nameF;
        private SpriteFont menuF;
        private SpriteFont debugF;
        private GuiItem main;

        public SpriteDrawer(Game game, bool debug)
            : base(game)
        {
            Tiles = new List<C_Tile>();
            Dest = new List<C_Destrucible>();
            Creatures = new List<C_Creature>();
            Players = new Dictionary<string, C_Player>();

            counter = new FPS();
            this.debug = debug;
        }

        public void Load(ContentManager content, ref C_Player player, string textures, string debugFont, string menuFont, string nameFont, string pTexture, Action<string> onDiscovery, Action onVSync)
        {
            batch = new SpriteBatch(GraphicsDevice);
            cam = new Camera(GraphicsDevice, IntVector2.Zero, Vector2.Zero);

            texC = new TextureCollection(content, 11);
            texC.LoadFromConfig(textures);
            nameF = content.Load<SpriteFont>(nameFont);
            menuF = content.Load<SpriteFont>(menuFont);
            debugF = content.Load<SpriteFont>(debugFont);

            this.pTexture = content.Load<Texture2D>(pTexture);
            this.player = player;

            InitMain(onDiscovery, onVSync);
        }

        public void UpdateMain(float delta, MouseState m_S, ref KeyboardState k_S)
        {
            main.Update(m_S);

            for (int i = 0; i < main.Controls.Count; i++)
            {
                GuiItem g = main.Controls[i];

                if (g is TextBox) (g as TextBox).Update(m_S, k_S, delta);
                else if (g is Button) (g as Button).Update(m_S, delta);
            }
        }

        public void UpdateGame(float delta, ref C_Player player)
        {
            this.player = player;

            cam.Update(player.ChunkPos, player.GetTilePos());
            relPlayerPos = cam.GetRelativePosition(player.ChunkPos, player.GetTilePos());

            Vector2 camb = new Vector2(relPlayerPos.X + 16, relPlayerPos.Y + 16);
            Vector2 dir = Vector2.Normalize(MentulaExtensions.GetMousePos() - camb);
            arrowRot = (float)Math.Atan2(dir.X, dir.Y);
            arrowPos = camb + dir * 24 + new Vector2(-dir.Y * 8, dir.X * 8);
        }

        public void Draw(float delta, GameState state)
        {
            counter.Update(delta);

            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            switch (state)
            {
                case (GameState.MainMenu):
                    main.Draw(batch);
                    break;
                case (GameState.Game):
                    Vector2 relPos;

                    for (int i = 0; i < Tiles.Count; i++)
                    {
                        C_Tile t = Tiles[i];

                        if (cam.TryGetRelativePosition(t.ChunkPos, t.Pos.ToVector2(), out relPos)) batch.Draw(texC[t.TextureId], relPos, Color.White, t.Layer);
                    }

                    for (int i = 0; i < Dest.Count; i++)
                    {
                        C_Destrucible d = Dest[i];

                        if (cam.TryGetRelativePosition(d.ChunkPos, d.Pos.ToVector2(), out relPos)) batch.Draw(texC[d.TextureId], relPos, Color.White, d.Layer);
                    }

                    for (int i = 0; i < Creatures.Count; i++)
                    {
                        C_Creature c = Creatures[i];

                        if (cam.TryGetRelativePosition(c.ChunkPos, c.Pos, out relPos)) batch.Draw(texC[c.TextureId], relPos, c.Color, 2);
                    }

                    for (int i = 0; i < Players.Count; i++)
                    {
                        C_Player p = Players.ElementAt(i).Value;

                        if (cam.TryGetRelativePosition(p.ChunkPos, p.GetTilePos(), out relPos))
                        {
                            batch.Draw(pTexture, relPos, Color.White);
                            batch.DrawString(nameF, p.Name, relPos - new Vector2(0, 25), Color.Black);
                        }
                    }

                    batch.Draw(pTexture, relPlayerPos, Color.White);
                    batch.DrawString(nameF, player.Name, relPlayerPos - new Vector2(0, 25), Color.Black);
                    batch.Draw(texC[9], MentulaExtensions.GetMousePos() - new Vector2(8, 8), Color.Red);
                    batch.Draw(texC[10], arrowPos, Color.Red, -arrowRot);

                    if (debug)
                    {
                        batch.DrawString(debugF, string.Format("Fps: {0}", counter.ToString()), Vector2.Zero, Color.Red);
                        batch.DrawString(debugF, string.Format("Dest: {0}", Dest.Count), new Vector2(0, 16), Color.Red);
                        batch.DrawString(debugF, string.Format("Creatures: {0}", Creatures.Count), new Vector2(0, 32), Color.Red);
                        batch.DrawString(debugF, string.Format("Player Pos: {0}", player.GetTotalPos()), new Vector2(0, 48), Color.Red);
                    }
                    break;
            }
            batch.End();
        }

        private void InitMain(Action<string> onDisc, Action onVSync)
        {
            int scrW = GraphicsDevice.Viewport.Width;
            int scrH = GraphicsDevice.Viewport.Height;

            main = new GuiItem(GraphicsDevice, GraphicsDevice.Viewport.Bounds) { BackColor = Color.LawnGreen };

            Label lblName = new Label(GraphicsDevice, main, new Rectangle(scrW >> 1, scrH / 3, 150, 21), menuF) { AutoSize = true, BackColor = main.BackColor, Text = "UserName:" };
            Label lblScrH = new Label(GraphicsDevice, main, new Rectangle(scrW >> 2, scrH / 3, 150, 21), menuF) { AutoSize = true, BackColor = main.BackColor, Text = "Screen height :" };
            Label lblScrW = new Label(GraphicsDevice, main, new Rectangle(scrW >> 2, scrH / 3 + 25, 150, 21), menuF) { AutoSize = true, BackColor = main.BackColor, Text = "Screen width  :" };

            MouseEventHandler me = (sender, e) =>
            {
                for (int i = 0; i < main.Controls.Count; i++)
                {
                    GuiItem g = main.Controls[i];

                    if (g is TextBox)
                    {
                        (g as TextBox).Focused = false;
                    }
                }

                (sender as TextBox).Focused = true;
            };

            TextBox txtName = new TextBox(GraphicsDevice, main, new Rectangle((scrW >> 1) + 100, scrH / 3, 150, 21), menuF) { AllowDrop = true, FlickerStyle = FlickerStyle.Fast };
            TextBox txtScrH = new TextBox(GraphicsDevice, main, new Rectangle((scrW >> 2) + 140, scrH / 3, 50, 21), menuF) { FlickerStyle = FlickerStyle.Fast, Text = scrH.ToString() };
            TextBox txtScrW = new TextBox(GraphicsDevice, main, new Rectangle((scrW >> 2) + 140, scrH / 3 + 25, 50, 21), menuF) { FlickerStyle = FlickerStyle.Fast, Text = scrW.ToString() };
            txtName.Click += me;
            txtScrH.Click += me;
            txtScrW.Click += me;

            Button btnDisc = new Button(GraphicsDevice, main, new Rectangle(scrW >> 1, scrH / 3 + 50, 250, 21), menuF) { Text = "Discover server" };
            Button btnScrA = new Button(GraphicsDevice, main, new Rectangle(scrW >> 2, scrH / 3 + 50, 200, 21), menuF) { Text = "Resize window" };
            Button btnVSync = new Button(GraphicsDevice, main, new Rectangle(scrW >> 1, scrH / 3 + 25, 250, 21), menuF) { Text = "Toggle VSync: off" };
            btnDisc.LeftClick += (sender, e) => onDisc(txtName.Text);
            btnScrA.LeftClick += (sender, e) =>
            {
                if (int.TryParse(txtScrH.Text, out scrH) & int.TryParse(txtScrW.Text, out scrW) & scrH > 0 & scrW > 0)
                {
                    PreferredBackBufferHeight = scrH;
                    PreferredBackBufferWidth = scrW;
                    ApplyChanges();
                    cam = new Camera(GraphicsDevice, player.ChunkPos, player.GetTilePos());
                    InitMain(onDisc, onVSync);
                }
                else System.Windows.Forms.MessageBox.Show(string.Format("The screen {0}({1}) is an invalid value!", scrH < 1 ? "height" : "width", scrH < 1 ? scrH : scrW), "Invalid dimentions!");
            };
            btnVSync.LeftClick += (sender, e) =>
                {
                    btnVSync.Text = btnVSync.Text == "Toggle VSync: off" ? "Toggle VSync: on" : "Toggle VSync: off";
                    onVSync();
                };
        }
    }
}