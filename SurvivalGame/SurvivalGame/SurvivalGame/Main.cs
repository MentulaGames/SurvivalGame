#define LOCAL

using Lidgren.Network;
using Mentula.General;
using Mentula.General.Res;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
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

        private Player player;
        private Texture2D Playertexture;
        private Texture2D[] textures;
        private Actor drawPos;
        private Camera camera;

        private List<CTile> tiles;
        private List<CTile> tilesToDraw;
        private bool first;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720 };
            camera = new Camera(new IntVector2(0, 0), new Vector2(0, 0), new IntVector2(1280, 720));
            Content.RootDirectory = "Content";
            drawPos = new Actor(IntVector2.Zero, Vector2.Zero);
            NPConf config = new NPConf(Resources.AppName);
            config.EnableMessageType(NIMT.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();
        }

        protected override void Initialize()
        {
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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Playertexture = new Texture2D(GraphicsDevice, 32, 32);

            textures = new Texture2D[4];
            textures[0] = Content.Load<Texture2D>("Tiles/Desert_Temp");
            textures[1] = Content.Load<Texture2D>("Tiles/Forest_Temp");
            textures[2] = Content.Load<Texture2D>("Tiles/Grassland_Temp");
            textures[3] = Content.Load<Texture2D>("Tiles/Savana_Temp");

            Color[] data = new Color[Playertexture.Height * Playertexture.Width];
            for (int y = 0; y < Playertexture.Height; y++)
            {
                for (int x = 0; x < Playertexture.Width; x++)
                {
                    data[x + (y * Playertexture.Height)] = Color.Red;
                }
            }
            Playertexture.SetData<Color>(data);

            player = new Player("Arzana", new IntVector2(), new Vector2(10, 10));
        }

        protected override void Update(GameTime gameTime)
        {
            Vector2 inp = new Vector2();
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape)) this.Exit();

            if (state.IsKeyDown(Keys.W)) inp.Y = -1;
            else if (state.IsKeyDown(Keys.S)) inp.Y = 1;
            if (state.IsKeyDown(Keys.A)) inp.X = -1;
            else if (state.IsKeyDown(Keys.D)) inp.X = 1;

            //if (inp.X != 0 || inp.Y != 0)
            //{
            //    inp.Normalize();
            //    NOM nom = client.CreateMessage();
            //    nom.Write(inp);
            //    client.SendMessage(nom, NetDeliveryMethod.Unreliable);
            //}

            if (client.ConnectionStatus == NetConnectionStatus.Connected && first == false)
            {
                first = true;
                NOM nom = client.CreateMessage();
                nom.Write(player.ChunkPos);
                client.SendMessage(nom, NetDeliveryMethod.Unreliable);
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
                    case (NIMT.Data):
                        int length = msg.ReadInt32();

                        for (int i = 0; i < length; i++)
                        {
                            tiles.AddRange(msg.ReadTileArr());
                        }
                        break;
                }
            }
            IntVector2 cameratilepos = new IntVector2(camera.GetTilePos());
            IntVector2 drawtilepos = new IntVector2(drawPos.GetTilePos());
            if (camera.ChunkPos != drawPos.ChunkPos | new IntVector2(camera.GetTilePos()) != new IntVector2(drawPos.GetTilePos()) | tilesToDraw.Count == 0) ;
            {
                int cSize = int.Parse(Resources.ChunkSize);
                tilesToDraw = new List<CTile>();
                for (int i = 0; i < tiles.Count; i++)
                {
                    Vector2 tilepos = tiles[i].ChunkPos * cSize + tiles[i].Pos;
                    Vector2 campos = camera.ChunkPos.ToVector2() * cSize + camera.GetTilePos();
                    if (tilepos.X >= campos.X - 1 && tilepos.Y >= campos.Y - 1 && tilepos.X <= campos.X + camera.Bounds.X / 32 + 1 && tilepos.Y <= campos.Y + camera.Bounds.Y / 32 + 1)
                    {
                        tilesToDraw.Add(tiles[i]);
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
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