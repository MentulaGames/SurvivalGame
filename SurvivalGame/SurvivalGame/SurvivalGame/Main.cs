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
        private Texture2D texture;
        private List<CTile> tiles;
        private bool first;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

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
            texture = new Texture2D(GraphicsDevice, 32, 32);

            Color[] data = new Color[texture.Height * texture.Width];
            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    data[x + (y * texture.Height)] = Color.Red;
                }
            }
            texture.SetData<Color>(data);

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
                        tiles = msg.ReadTileArr().ToList();
                        break;
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