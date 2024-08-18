using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Siege.Screens {
    public class ScreenService {
        // The Game's SpriteBatch instance.
        private SpriteBatch spriteBatch;

        // A collection of all registered Screen's with a key of their unique ScreenType.
        private Dictionary<ScreenType, IScreen> screens = new Dictionary<ScreenType, IScreen>();
        
        // The current screen the user is viewing.
        public IScreen CurrentScreen { get; private set; }

        public ScreenService(SpriteBatch spriteBatch, params IScreen[] screens) {
            this.spriteBatch = spriteBatch;

            // Registers each of the provided screens to this service.
            foreach (IScreen screen in screens) {
                this.screens.Add(screen.GetScreenType(), screen);
            }
        }

        /// <summary>
        /// A method which draws the active screen if one exists or otherwise draws the default screen.
        /// </summary>
        public void DrawActiveScreenOrDefault() {
            // If there is no current screen, load the default screen.
            if (CurrentScreen == null) {
                DisplayScreen(GetDefaultScreen());
            }

            CurrentScreen.Draw(spriteBatch);
        }

        /// <summary>
        /// Displays the screen with the provided ScreenType.
        /// </summary>
        /// <param name="Screen">The ScreenType of the screen to display.</param>
        public void DisplayScreen(ScreenType Screen) {
            IScreen screen;

            this.screens.TryGetValue(Screen, out screen);

            if (screen != null) {
                // Sets the current screen to the screen loaded from the provided ScreenType and updates the
                // windows width and height.
                CurrentScreen = screen;
                Siege.INSTANCE.graphics.PreferredBackBufferWidth = screen.WindowWidth;
                Siege.INSTANCE.graphics.PreferredBackBufferHeight = screen.WindowHeight;
                Siege.INSTANCE.graphics.ApplyChanges();

                // Draw the screen.
                screen.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Loads content for all registered screens.
        /// </summary>
        /// <param name="Content">MonoGame's content manager.</param>
        public void LoadAllContent(ContentManager Content) {
            foreach (IScreen screen in screens.Values) {
                screen.LoadContent(Content);
            }
        }

        /// <summary>
        /// Unloads content for all registered screens.
        /// </summary>
        public void UnloadAllContent() {
            foreach (IScreen screen in screens.Values) {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Gets the default Screen which should be displayed when the game starts.
        /// </summary>
        /// <returns>The ScreenType of the defualt screen.</returns>
        public ScreenType GetDefaultScreen() {
            return ScreenType.MAIN_MENU_SCREEN;
        }
    }
}
