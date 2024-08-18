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
using Siege.Tower;
using Siege.Util;
using System.Globalization;
using Siege.TileModifiers;
using Siege.TileModifiers.Modifiers;

namespace Siege.Screens.Components {

    /// <summary>
    /// The screen component which is used to select Towers to place on the map.
    /// </summary>
    public class TileSelection : IScreenComponent {
        private Texture2D SelectionTexture;
        // A flag tracking if the selector has been initialized with the permitted towers for the map.
        private bool Initialized = false;
        // The map for which this tower selector is operational for.
        private MapBase map;
        // The users currently selected tower.
        public TowerType? SelectedTower { get; set; }
        // The users currently selected tile modifier
        public TileModiferTypes? SelectedTileModifier { get; set; }
        // A list of Buttons for each permitted TowerType on the map.
        private List<Button> Buttons = new List<Button>();

        // The Offsets from which the tower selector should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public TileSelection(MapBase map) {
            this.map = map;
        }

        /// <summary>
        /// Loads all assets for the tower selector, it will be called once per game at the very start.
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public void LoadContent(ContentManager Content) {
            this.SelectionTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_SELECTION_BORDER);
        }

        /// <summary>
        /// Unloads all assets for the tower selector, it will be called once per game at the very end.
        /// </summary>
        public void UnloadContent() {
            this.SelectionTexture = null;
        }

        /// <summary>
        /// Allows the tower selector to check for any logic. Clicks, hovers, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            // If the selector is not initialized, initialize it.
            if (!Initialized) {
                // Iterate through each permitted tower in the map and create a button for it which
                // sets the selected tower.
                foreach (TowerType tower in map.GetPermittedTowers()) {
                    // Create a Texture for the Button by cropping out the Towers texture.
                    Texture2D ButtonTexture = TextureHelper.Crop(map.TowerTextures,
                        new Rectangle((int)tower * map.TileWidth, 0, map.TileWidth, map.TileHeight));

                    TowerBase fakeTower = tower.CreateFakeTower();
                    Tooltip ButtonTooltip = new Tooltip(
                        tower.ToString() + " Tower",
                        "Damage: " + fakeTower.Damage,
                        "Damage Radius: " + fakeTower.DamageRadius,
                        "Rate of Fire: " + fakeTower.RateOfFire.ToString("sfff", CultureInfo.CreateSpecificCulture("en-gb")),
                        "Cost: " + fakeTower.Cost
                    );
                    ButtonTooltip.XOffset = 5;
                    ButtonTooltip.YOffset = 5;

                    // Create a new button with the texture which sets the currently selected tower
                    // to its own.
                    Button Button = new Button(ButtonTexture,
                        // The delegate executed when the mouse button is down on the button.
                        () => { },
                        // The delegate executed when the mouse button is released on the button.
                        () => {
                            this.SelectedTower = tower;
                            this.SelectedTileModifier = null;
                        }, ButtonTooltip);

                    // Sets the Offsets of the button to the right side of the map one tile away from the edges.
                    Button.XOffset = this.XOffset + map.TileWidth + 4;
                    Button.YOffset = this.YOffset + (int)tower * map.TileHeight + map.TileHeight + 4;
                    Button.Width = 24;
                    Button.Height = 24;

                    // Add the Button to this selector.
                    Buttons.Add(Button);
                }

                #region Lava TM
                // Create a Texture for the Button by cropping out the Lava texture.
                Texture2D LavaTexture = TextureHelper.Crop(map.TileTextures,
                    new Rectangle(1 * map.TileWidth, 0, map.TileWidth, map.TileHeight));

                Tooltip LavaTMTooltip = new Tooltip(
                    "Lava Tile Modifier",
                    "Length: 1 seconds",
                    "Cost: 40 Gold"
                );
                LavaTMTooltip.XOffset = 5;
                LavaTMTooltip.YOffset = 5;

                // Create a new button with the lava texture which sets the tile modifier to be lava
                Button LavaTMButton = new Button(LavaTexture,
                    // The delegate executed when the mouse button is down on the button.
                    () => { },
                    // The delegate executed when the mouse button is released on the button.
                    () => {
                        this.SelectedTower = null;
                        this.SelectedTileModifier = TileModiferTypes.LAVA;
                    }, LavaTMTooltip);

                // Sets the Offsets of the lava TM button to the right side of the map one tile away from the edges.
                LavaTMButton.XOffset = this.XOffset + map.TileWidth + 4;
                LavaTMButton.YOffset = this.YOffset + Buttons.Count * map.TileHeight + map.TileHeight + 4;
                LavaTMButton.Width = 24;
                LavaTMButton.Height = 24;

                // Add the Button to this selector.
                Buttons.Add(LavaTMButton);
                #endregion

                #region Water TM
                // Create a Texture for the Button by cropping out the Water texture.
                Texture2D WaterTexture = TextureHelper.Crop(map.TileTextures,
                    new Rectangle(3 * map.TileWidth, 0, map.TileWidth, map.TileHeight));

                Tooltip WaterTMTooltip = new Tooltip(
                    "Water Tile Modifier",
                    "Length: 2 seconds",
                    "Cost: 20 Gold"
                );
                WaterTMTooltip.XOffset = 5;
                WaterTMTooltip.YOffset = 5;

                // Create a new button with the water texture which sets the tile modifier to be water
                Button WaterTMButton = new Button(WaterTexture,
                    // The delegate executed when the mouse button is down on the button.
                    () => { },
                    // The delegate executed when the mouse button is released on the button.
                    () => {
                        this.SelectedTower = null;
                        this.SelectedTileModifier = TileModiferTypes.WATER;
                    }, WaterTMTooltip);

                // Sets the Offsets of the water TM button to the right side of the map one tile away from the edges.
                WaterTMButton.XOffset = this.XOffset + map.TileWidth + 4;
                WaterTMButton.YOffset = this.YOffset + Buttons.Count * map.TileHeight + map.TileHeight + 4;
                WaterTMButton.Width = 24;
                WaterTMButton.Height = 24;

                // Add the Button to this selector.
                Buttons.Add(WaterTMButton);
                #endregion

                #region Rock TM
                // Create a Texture for the Button by cropping out the Rock texture.
                Texture2D RockTexture = TextureHelper.Crop(map.TileTextures,
                    new Rectangle(5 * map.TileWidth, 0, map.TileWidth, map.TileHeight));

                Tooltip RockTMTooltip = new Tooltip(
                    "Rock Tile Modifier",
                    "Length: 5 seconds",
                    "Cost: 25 Gold"
                );
                RockTMTooltip.XOffset = 5;
                RockTMTooltip.YOffset = 5;

                // Create a new button with the rock texture which sets the tile modifier to be rock
                Button RockTMButton = new Button(RockTexture,
                    // The delegate executed when the mouse button is down on the button.
                    () => { },
                    // The delegate executed when the mouse button is released on the button.
                    () => {
                        this.SelectedTower = null;
                        this.SelectedTileModifier = TileModiferTypes.ROCK;
                    }, RockTMTooltip);

                // Sets the Offsets of the water TM button to the right side of the map one tile away from the edges.
                RockTMButton.XOffset = this.XOffset + map.TileWidth + 4;
                RockTMButton.YOffset = this.YOffset + Buttons.Count * map.TileHeight + map.TileHeight + 4;
                RockTMButton.Width = 24;
                RockTMButton.Height = 24;

                // Add the Button to this selector.
                Buttons.Add(RockTMButton);
                #endregion

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

            if (SelectedTower.HasValue) {
                spriteBatch.Draw(
                    SelectionTexture,
                    new Rectangle(this.XOffset + map.TileWidth, this.YOffset + ((int)SelectedTower.Value + 1) * map.TileHeight, map.TileWidth, map.TileHeight),
                    Color.White);
            } else if (SelectedTileModifier.HasValue) {
                spriteBatch.Draw(
                    SelectionTexture,
                    new Rectangle(this.XOffset + map.TileWidth, this.YOffset + ((int)SelectedTileModifier.Value + 1 + map.GetPermittedTowers().Count) * map.TileHeight, map.TileWidth, map.TileHeight),
                    Color.White);
            }

            spriteBatch.End();

            foreach (Button button in Buttons) {
                button.DrawTooltip(spriteBatch);
            }
        }
    }
}
