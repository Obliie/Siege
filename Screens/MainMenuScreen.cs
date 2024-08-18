using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Siege.Core;
using Siege.Screens.Components;
using Siege.Util;
using Siege.IO;

namespace Siege.Screens {
    public class MainMenuScreen : IScreen {
        private bool Initialized = false;

        // The Background Texture
        private Texture2D BackgroundTextures;
        // The Textures for all the Buttons.
        private Texture2D PlayButtonTexture;
        private Texture2D SettingsButtonTexture;
        private Texture2D ExitButtonTexture;

        // The Main Menu screen buttons
        private Button PlayButton;
        private Button SettingsButton;
        private Button ExitButton;

        // The Offsets from which the MainMenuScreen should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int WindowWidth {
            get {
                return 800;
            }
        }

        public int WindowHeight {
            get {
                return 480;
            }
        }

        /// <summary>
        /// Returns that this Screen is a ScreenType of GAME_SCREEN so that it can be identified
        /// by the ScreenService.
        /// </summary>
        /// <returns>ScreenType.GAME_SCREEN</returns>
        public ScreenType GetScreenType() {
            return ScreenType.MAIN_MENU_SCREEN;
        }

        /// <summary>
        /// Loads all content for the screen (Images and audio).
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public void LoadContent(ContentManager Content) {
            BackgroundTextures = Content.Load<Texture2D>(SiegeConstants.ContentPaths.MAINMENU_BACKGROUND_TEXTURE);
            PlayButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.MAINMENU_PLAY_BUTTON_TEXTURE);
            SettingsButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.MAINMENU_SETTINGS_BUTTON_TEXTURE);
            ExitButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.MAINMENU_EXIT_BUTTON_TEXTURE);
        }

        /// <summary>
        /// Unloads all content for the screen (Images and audio).
        /// </summary>
        public void UnloadContent() {
            BackgroundTextures = null;
            PlayButtonTexture = null;
            ExitButtonTexture = null;
        }

        /// <summary>
        /// Updates all components on the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime) {
            // If the Screen has not been initialized, create all the relavent buttons.
            if (!Initialized) {
                PlayButton = new Button(PlayButtonTexture,
                    () => { },
                    () => { 
                        Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.PLAY_SCREEN); 
                    });
                PlayButton.XOffset = 336;
                PlayButton.YOffset = 310;
                PlayButton.Width = 128;
                PlayButton.Height = 50;

                SettingsButton = new Button(SettingsButtonTexture,
                    () => { },
                    () => {
                        //TODO: Settings Screen.
                    });
                SettingsButton.XOffset = 321;
                SettingsButton.YOffset = 350;
                SettingsButton.Width = 158;
                SettingsButton.Height = 70;

                ExitButton = new Button(ExitButtonTexture,
                    () => { },
                    () => {
                        Siege.INSTANCE.Exit();
                    });
                ExitButton.XOffset = 336;
                ExitButton.YOffset = 410;
                ExitButton.Width = 128;
                ExitButton.Height = 50;

                Initialized = true;
            }

            // Update all the Buttons.
            PlayButton.Update(gameTime);
            SettingsButton.Update(gameTime);
            ExitButton.Update(gameTime);
        }

        /// <summary>
        /// This is when the Screen should draw its components.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            spriteBatch.Draw(this.BackgroundTextures, new Rectangle(0, 0, 800, 480), Color.White);

            spriteBatch.End();

            // Draw the Play button if it exists
            if (PlayButton != null) {
                PlayButton.Draw(spriteBatch);
            }

            // Draw the Settings button if it exists
            if (SettingsButton != null) {
                SettingsButton.Draw(spriteBatch);
            }

            // Draw the Exit button if it exists
            if (ExitButton != null) {
                ExitButton.Draw(spriteBatch);
            }
        }
    }
}
