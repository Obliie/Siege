using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Siege.Map;
using Siege.Screens;
using Siege.Map.Maps;
using Siege.IO;
using System.Collections.Generic;
using Siege.IO.Serializers;
using Siege.Map.Tiles;
using Siege.Core;

namespace Siege {
    /// <summary>
    /// The main instance of the game.
    /// </summary>
    public class Siege : Game {
        // The static instance of this game.
        public static Siege INSTANCE;

        // Variables used to manage Graphics within MonoGame.
        public GraphicsDeviceManager graphics { get; private set; }
        private SpriteBatch spriteBatch;

        public SpriteFont Font_14_Regular { get; private set; }
        public SpriteFont Font_14_Bold { get; private set; }

        public static Texture2D PlayerTexture;

        // The ScreenService used to manage what 'Screen' the player is viewing.
        public ScreenService ScreenService;

        // The MapService used to manage the games Maps.
        public MapService MapService;

        public FileLoader FileLoader { get; private set; }

        public Siege() {
            // Sets the static instance of the game to this Object.
            Siege.INSTANCE = this;

            // Create the graphics manager and set the content directory.
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create the MapService and initialize it with a Tile size of 32x32.
            MapService = new MapService(32, 32);

            // Create the ScreenService and initialize it with the Game and MainMenu screens.
            GameScreen gameScreen = new GameScreen(MapService, MapType.HELL);
            MainMenuScreen mainMenuScreen = new MainMenuScreen();
            PlayScreen playScreen = new PlayScreen();
            LoadScreen loadScreen = new LoadScreen();
            PauseScreen pauseScreen = new PauseScreen();
            ScreenService = new ScreenService(spriteBatch, gameScreen, mainMenuScreen, playScreen, loadScreen, pauseScreen);

            // Register Serializers
            SerializerService.RegisterSerializer(new MapSerializer());
            SerializerService.RegisterSerializer(new Vector2Serializer());
            SerializerService.RegisterSerializer(new TileSerializer());
            SerializerService.RegisterSerializer(new TileRowSerializer());
            SerializerService.RegisterSerializer(new RectangleSerializer());
            SerializerService.RegisterSerializer(new TileEntitySerializer());
            SerializerService.RegisterSerializer(new TowerSerializer());
            SerializerService.RegisterSerializer(new TroopSerializer());
            SerializerService.RegisterSerializer(new PlayerSerializer());

            this.Window.Title = "Siege";
            this.Window.AllowUserResizing = true;
            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            this.Font_14_Regular = Content.Load<SpriteFont>(SiegeConstants.ContentPaths.FONT_14_REGULAR);
            this.Font_14_Bold = Content.Load<SpriteFont>(SiegeConstants.ContentPaths.FONT_14_BOLD);
            Siege.PlayerTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_PLAYER_TEXTURE);
            // Load the Content for all screens registered to the ScreenService.
            ScreenService.LoadAllContent(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            this.Font_14_Regular = null;
            this.Font_14_Bold = null;
            // Unload the Content for all screens registered to the ScreenService.
            ScreenService.UnloadAllContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // If the user clicks the escape key, show the main menu.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                if (ScreenService.CurrentScreen.GetScreenType().Equals(ScreenType.GAME_SCREEN)) {
                    ScreenService.DisplayScreen(ScreenType.PAUSE_SCREEN);
                } else if (ScreenService.CurrentScreen.GetScreenType().Equals(ScreenType.LOAD_SCREEN)) {
                    ScreenService.DisplayScreen(ScreenType.PLAY_SCREEN);
                } else if (ScreenService.CurrentScreen.GetScreenType().Equals(ScreenType.PLAY_SCREEN)) {
                    ScreenService.DisplayScreen(ScreenType.MAIN_MENU_SCREEN);
                }
            }

            // Allows the current screen to update its logic if it exists
            if (ScreenService.CurrentScreen != null) {
                ScreenService.CurrentScreen.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            // Set the mouse to be visible in the game and set the canvas to be ghost white.
            GraphicsDevice.Clear(Color.GhostWhite);

            // Draw the current screen or draw the default one if it does not exist.
            ScreenService.DrawActiveScreenOrDefault();

            base.Draw(gameTime);
        }
    }
}


