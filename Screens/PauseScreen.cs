using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Siege.Core;
using Siege.IO;
using Siege.Screens.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Screens {
    public class PauseScreen : IScreen {
        private bool Initialized = false;

        // The Pause screen buttons
        private Button ResumeButton;
        private Button SaveButton;
        private Button MainMenuButton;
        private Button ExitButton;

        // Textures for the Pause screen buttons
        private Texture2D ResumeButtonTexture;
        private Texture2D SaveButtonTexture;
        private Texture2D MainMenuButtonTexture;
        private Texture2D ExitButtonTexture;

        // The Offsets from which the LoadScreen should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        // The width and height of the pause screen
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

        public ScreenType GetScreenType() {
            return ScreenType.PAUSE_SCREEN;
        }

        /// <summary>
        /// Loads all content for the buttons from MonoGame's ContentManager
        /// </summary>
        /// <param name="Content">MonoGame's ContentManager</param>
        public void LoadContent(ContentManager Content) {
            ResumeButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.PAUSE_RESUME_BUTTON_TEXTURE);
            SaveButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.PAUSE_SAVE_BUTTON_TEXTURE);
            MainMenuButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.PAUSE_MAINMENU_BUTTON_TEXTURE);
            ExitButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.PAUSE_EXIT_BUTTON_TEXTURE);
        }

        /// <summary>
        /// Unsets all the button textures so that the instances can be garbage collected.
        /// </summary>
        public void UnloadContent() {
            this.SaveButtonTexture = null;
            this.MainMenuButtonTexture = null;
            this.ExitButtonTexture = null;
        }

        public void Update(GameTime gameTime) {
            if (!Initialized) {
                ResumeButton = new Button(ResumeButtonTexture,
                    () => { },
                    () => {
                        Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.GAME_SCREEN);
                    });
                ResumeButton.Width = 128;
                ResumeButton.Height = 60;
                ResumeButton.XOffset = 336;
                ResumeButton.YOffset = 100;

                SaveButton = new Button(SaveButtonTexture,
                    () => { },
                    () => {
                        FileLoader FileLoader = new FileLoader("Saves/" + DateTime.Now.ToString("MM dd yyyy HH mm ss",
                                CultureInfo.InvariantCulture) + ".dat");
                        FileNode Save = new FileNode();
                        Save.SetValue<MapBase>("Map", Siege.INSTANCE.MapService.CurrentMap);
                        FileLoader.Save(Save);

                        Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.MAIN_MENU_SCREEN);
                    });
                SaveButton.Width = 128;
                SaveButton.Height = 60;
                SaveButton.XOffset = 336;
                SaveButton.YOffset = 180;

                MainMenuButton = new Button(MainMenuButtonTexture,
                    () => { },
                    () => {
                        Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.MAIN_MENU_SCREEN);
                    });
                MainMenuButton.Width = 128;
                MainMenuButton.Height = 60;
                MainMenuButton.XOffset = 336;
                MainMenuButton.YOffset = 260;

                ExitButton = new Button(ExitButtonTexture,
                    () => { },
                    () => {
                        Siege.INSTANCE.Exit();
                    });
                ExitButton.Width = 128;
                ExitButton.Height = 60;
                ExitButton.XOffset = 336;
                ExitButton.YOffset = 340;

                Initialized = true;
            }

            ResumeButton.Update(gameTime);
            SaveButton.Update(gameTime);
            MainMenuButton.Update(gameTime);
            ExitButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (ResumeButton != null) {
                ResumeButton.Draw(spriteBatch);
            }
            if (SaveButton != null) {
                SaveButton.Draw(spriteBatch);
            }
            if (MainMenuButton != null) {
                MainMenuButton.Draw(spriteBatch);
            }
            if (ExitButton != null) {
                ExitButton.Draw(spriteBatch);
            }
        }
    }
}
