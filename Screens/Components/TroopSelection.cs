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
using Siege.Util;
using Siege.Entities;

namespace Siege.Screens.Components {

    /// <summary>
    /// The screen component which is used to select Troops to deploy on the map.
    /// </summary>
    public class TroopSelection : IScreenComponent {
        private Texture2D SelectionTexture;
        // A flag tracking if the selector has been initialized with the permitted troops for the map.
        private bool Initialized = false;
        // The map for which this troop selector is operational for.
        private MapBase map;
        // The users currently selected troop.
        private TroopType SelectedTroop;
        // A list of Buttons for each permitted TroopType on the map.
        private List<Button> Buttons = new List<Button>();

        // The Offsets from which the troop selector should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public TroopSelection(MapBase map) {
            this.map = map;
        }

        /// <summary>
        /// Loads all assets for the troop selector, it will be called once per game at the very start.
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public void LoadContent(ContentManager Content) {
            this.SelectionTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_SELECTION_BORDER);
        }

        /// <summary>
        /// Unloads all assets for the troop selector, it will be called once per game at the very end.
        /// </summary>
        public void UnloadContent() {
            this.SelectionTexture = null;
        }

        /// <summary>
        /// Allows the troop selector to check for any logic. Clicks, hovers, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            // If the selector is not initialized, initialize it.
            if (!Initialized) {
                // Iterate through each permitted troop in the map and create a button for it which
                // sets the selected troop.
                foreach (TroopType troop in map.GetPermittedTroops()) {
                    // Create a Texture for the Button by cropping out the Troops texture.
                    Texture2D ButtonTexture = TextureHelper.Crop(map.TroopTextures,
                        new Rectangle((int)troop * (map.TileWidth / 2), 0, (map.TileWidth / 2), (map.TileHeight / 2)));

                    TroopBase FakeTroop = troop.CreateFakeTroop();
                    Tooltip ButtonTooltip = new Tooltip(
                        troop.ToString() + " Troop",
                        "Damage: " + FakeTroop.Damage,
                        "Speed: " + FakeTroop.BaseSpeed,
                        "Health: " + FakeTroop.MaxHealth,
                        "Cost: " + FakeTroop.Cost
                        );
                    ButtonTooltip.XOffset = 5;
                    ButtonTooltip.YOffset = 5;

                    // Create a new button with the texture which sets the currently selected troop
                    // to its own.
                    Button Button = new Button(ButtonTexture,
                        // The delegate executed when the mouse button is down on the button.
                        () => { },
                        // The delegate executed when the mouse button is released on the button.
                        () => {
                            this.SelectedTroop = troop;
                        }, ButtonTooltip);

                    // Sets the Offsets of the button to the right side of the map one tile away from the edges.
                    Button.XOffset = this.XOffset + map.TileWidth + 4;
                    Button.YOffset = this.YOffset + (int)troop * map.TileHeight + map.TileHeight + 4;
                    Button.Width = 24;
                    Button.Height = 24;

                    // Add the Button to this selector.
                    Buttons.Add(Button);
                }

                // Set the selectors state to initialized.
                Initialized = true;
            }

            // Allow all Buttons to be updated.
            foreach (Button button in Buttons) {
                button.Update(gameTime);
            }
        }

        /// <summary>
        /// This method is called when the screen component should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            // Draw all the Buttons on this selector.
            foreach (Button button in Buttons) {
                button.Draw(spriteBatch);
            }

            spriteBatch.Begin();

            spriteBatch.Draw(
                SelectionTexture,
                new Rectangle(this.XOffset + map.TileWidth, this.YOffset + ((int)SelectedTroop + 1) * map.TileHeight, map.TileWidth, map.TileHeight),
                Color.White);

            spriteBatch.End();

            foreach (Button button in Buttons) {
                button.DrawTooltip(spriteBatch);
            }
        }

        /// <summary>
        /// Gets the currently selected Troop.
        /// </summary>
        /// <returns>The TroopType of the currently selected Troop.</returns>
        public TroopType GetSelection() {
            return SelectedTroop;
        }
    }
}
