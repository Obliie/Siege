using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Screens;
using Siege.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Siege.Map.Maps;
using Siege.Screens.Components;
using Siege.Map.Tiles;
using Siege.Tower;
using Siege.Tower.Towers;
using Siege.Entities;
using Siege.Util;
using Siege.TileModifiers;
using Siege.TileModifiers.Modifiers;

namespace Siege.Screens {

    /// <summary>
    /// The main GameScreen displayed when the user is playing the game.
    /// </summary>
    public class GameScreen : IScreen {
        // The Mouse's state for the previous update cycle.
        private MouseState PreviousMouseState;
        // The map service for the game.
        private MapService mapService;

        // The active selector for this game, troop or tower.
        private IScreenComponent activeSelector;
        // The TowerSelector for this game.
        private TileSelection towerSelector;
        // The TroopSelector for this game.
        private TroopSelection troopSelector;
        private TowerDataDisplay towerDataDisplay;

        private CastleHealth CastleHealth;

        // The texture for the buttons to switch selectors.
        private Texture2D SelectorButtonTextures;
        // The button to open the Tower selector.
        private Button TowerSelectorButton;
        // The button to open the Troop selector.
        private Button TroopSelectorButton;

        // The time since the enemy last spawned a set of troops.
        public TimeSpan PreviousEnemyTroopSpawn;
        // The time between when the enemy should spawn troops.
        public TimeSpan EnemyTroopSpawnRate;

        private TimeSpan LastGoldUpdate = TimeSpan.FromSeconds(0);
        public int GoldPerSecond { get; set; }
        public int PlayerGold { get; set; }
        public int PlayerDiamonds { get; set; }

        private string MessageToFlash { get; set; }
        private TimeSpan FlashDuration = TimeSpan.FromSeconds(3);
        private TimeSpan FlashStart = TimeSpan.FromSeconds(0);


        // The Offsets from which the GameScreen should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int WindowWidth {
            get {
                return 1000;
            }
        }

        public int WindowHeight {
            get {
                return 500;
            }
        }

        private bool GameOver = false;
        private TextButton GameOverButton;

        public GameScreen(MapService mapService, MapType? map) {
            this.mapService = mapService;

            // If a default map is provided, load it.
            if (map.HasValue) {
                mapService.LoadMap(map.Value);
            }

            // If the tower selector does not exist, create it for the current map.
            if (towerSelector == null) {
                towerSelector = new TileSelection(mapService.CurrentMap);
                // Set the offsets of the tower selector to be right of the map.
                towerSelector.XOffset = mapService.CurrentMap.Width * mapService.CurrentMap.TileWidth;
                towerSelector.YOffset = 0;
            }

            // If the troop selector does not exist, create it for the current map.
            if (troopSelector == null) {
                troopSelector = new TroopSelection(mapService.CurrentMap);
                // Set the offsets of the troop selector to be right of the map.
                troopSelector.XOffset = mapService.CurrentMap.Width * mapService.CurrentMap.TileWidth;
                troopSelector.YOffset = 0;
            }

            if (towerDataDisplay == null) {
                towerDataDisplay = new TowerDataDisplay(mapService.CurrentMap);

                towerDataDisplay.YOffset = mapService.CurrentMap.Height * mapService.CurrentMap.TileHeight;
            }

            if (CastleHealth == null) {
                CastleHealth = new CastleHealth(mapService.CurrentMap, 5000, 10000);
            }

            this.EnemyTroopSpawnRate = TimeSpan.FromMilliseconds(500);
            // Set the active selector to the Tower selector by default.
            activeSelector = towerSelector;

            this.MessageToFlash = "";
            this.PlayerGold = 250;
            this.PlayerDiamonds = 0;
            this.GoldPerSecond = 10;
        }

        /// <summary>
        /// Returns that this Screen is a ScreenType of GAME_SCREEN so that it can be identified
        /// by the ScreenService.
        /// </summary>
        /// <returns>ScreenType.GAME_SCREEN</returns>
        public ScreenType GetScreenType() {
            return ScreenType.GAME_SCREEN;
        }

        /// <summary>
        /// Loads all content for the screen (Images and audio).
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public void LoadContent(ContentManager Content) {
            // Allow the map service to load its content.
            mapService.LoadAllContent(Content);

            towerSelector.LoadContent(Content);
            troopSelector.LoadContent(Content);
            towerDataDisplay.LoadContent(Content);
            CastleHealth.LoadContent(Content);

            // Load the Textures for the buttons that switch selectors.
            SelectorButtonTextures = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_SELECTOR_BUTTON_TEXTURES);
        }

        /// <summary>
        /// Unloads all content for the screen (Images and audio).
        /// </summary>
        public void UnloadContent() {
            // Allow the map service to unload its content
            mapService.UnloadAllContent();
            towerSelector.UnloadContent();
            troopSelector.UnloadContent();
            towerDataDisplay.UnloadContent();
            CastleHealth.UnloadContent();

            // Remove the reference to the texture containing the selector buttons.
            SelectorButtonTextures = null;
        }

        /// <summary>
        /// Updates all components on the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime) {
            // If there is no currently loaded map return as there is no Game to play.
            if (mapService.CurrentMap == null) {
                return;
            }

            if (CastleHealth.PlayerCastleHealth <= 0 || CastleHealth.EnemyCastleHealth <= 0) {
                if (!GameOver) {
                    string text = CastleHealth.PlayerCastleHealth <= 0 ? "You have lost, click to return to main menu" : "You have won, click to return to main menu";
                    GameOverButton = new TextButton(() => { }, 
                        () => {
                            Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.MAIN_MENU_SCREEN);
                        }, 
                        text, 
                        Color.LightGray, 
                        Color.Black);
                    GameOverButton.XOffset = 0;
                    GameOverButton.YOffset = 0;
                    GameOverButton.Width = WindowWidth;
                    GameOverButton.Height = WindowHeight;
                    GameOver = true;
                }
                if (GameOverButton != null) {
                    GameOverButton.Update(gameTime);
                }
                return;
            }

            // If either of the selector buttons are null, create them.
            if (TowerSelectorButton == null) {
                TowerSelectorButton = new Button(TextureHelper.Crop(SelectorButtonTextures, new Rectangle(0, 0, 96, 32)),
                    () => { },
                    () => {
                        activeSelector = towerSelector;
                    });
                TowerSelectorButton.XOffset = mapService.CurrentMap.Width * mapService.CurrentMap.TileWidth;
                TowerSelectorButton.YOffset = mapService.CurrentMap.Height * mapService.CurrentMap.TileHeight;
                TowerSelectorButton.Width = 96;
                TowerSelectorButton.Height = 32;
            }
            if (TroopSelectorButton == null) {
                TroopSelectorButton = new Button(TextureHelper.Crop(SelectorButtonTextures, new Rectangle(0, 32, 96, 32)),
                    () => { },
                    () => {
                        activeSelector = troopSelector;
                    });
                TroopSelectorButton.XOffset = mapService.CurrentMap.Width * mapService.CurrentMap.TileWidth;
                TroopSelectorButton.YOffset = mapService.CurrentMap.Height * mapService.CurrentMap.TileHeight;
                TroopSelectorButton.Width = 96;
                TroopSelectorButton.Height = 32;
            }

            if (gameTime.TotalGameTime.TotalMilliseconds > LastGoldUpdate.TotalMilliseconds + TimeSpan.FromSeconds(1).TotalMilliseconds) {
                LastGoldUpdate = TimeSpan.FromMilliseconds(gameTime.TotalGameTime.TotalMilliseconds);
                PlayerGold += GoldPerSecond;
                PlayerDiamonds += 1;
            }

            // Update the current map.
            mapService.CurrentMap.Update(gameTime);
            // Update the active selector for the map.
            activeSelector.Update(gameTime);
            towerDataDisplay.Update(gameTime);
            CastleHealth.Update(gameTime);

            // Update the button to switch to the Troop selector if the Tower selector is active.
            if (activeSelector is TileSelection && TroopSelectorButton != null) {
                TroopSelectorButton.Update(gameTime);
            // Update the button to switch to the Tower selector if the Troop selector is active.
            } else if (activeSelector is TroopSelection && TowerSelectorButton != null) {
                TowerSelectorButton.Update(gameTime);
            }

            if (!string.IsNullOrEmpty(MessageToFlash) && FlashStart.TotalMilliseconds == 0) {
                FlashStart = TimeSpan.FromMilliseconds(gameTime.TotalGameTime.TotalMilliseconds);
            }
            if (gameTime.TotalGameTime.TotalMilliseconds > FlashStart.TotalMilliseconds + FlashDuration.TotalMilliseconds) {
                MessageToFlash = "";
                FlashStart = TimeSpan.FromSeconds(0);
            }

            // Gets the current mouse state and checks if it is released within the bounds of the map.
            MouseState CurrentMouseState = Mouse.GetState();
            if (CurrentMouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed) {
                if (CurrentMouseState.Position.X < (mapService.CurrentMap.TileWidth * mapService.CurrentMap.Width) + mapService.CurrentMap.XOffset &&
                        CurrentMouseState.Position.Y < (mapService.CurrentMap.TileHeight * mapService.CurrentMap.Height) + mapService.CurrentMap.YOffset &&
                        CurrentMouseState.Position.X > mapService.CurrentMap.XOffset && CurrentMouseState.Position.Y > mapService.CurrentMap.YOffset) {


                    // If the active selector is the Tower selector then create a Tower at the clicked position.
                    if (activeSelector is TileSelection) {
                        // Get the Tile the mouse button was released on by calculating the X and Y coordinates of the Tile.
                        int TileX = ((CurrentMouseState.Position.X - (CurrentMouseState.Position.X % mapService.CurrentMap.TileWidth)) / mapService.CurrentMap.TileWidth) + 1 - (mapService.CurrentMap.XOffset / mapService.TileWidth);
                        int TileY = ((CurrentMouseState.Position.Y - (CurrentMouseState.Position.Y % mapService.CurrentMap.TileHeight)) / mapService.CurrentMap.TileHeight) + 1 - (mapService.CurrentMap.YOffset / mapService.TileHeight);
                        Tile tile = mapService.CurrentMap.GetRowAtY(TileY).GetTileAtX(TileX);
                        TileSelection tileSelector = activeSelector as TileSelection;

                        if (tileSelector.SelectedTower.HasValue) {
                            // Create a Tower for the pressed Tile on the map based on the currently selected Tower.
                            if (!(PlayerGold >= TowerPropertyPresets.GetPropertiesForTower(towerSelector.SelectedTower.Value).Cost)) {
                                MessageToFlash = "Not enough Gold to purchase " + TowerPropertyPresets.GetPropertiesForTower(towerSelector.SelectedTower.Value).Name;
                                return;
                            } else {
                                PlayerGold -= TowerPropertyPresets.GetPropertiesForTower(towerSelector.SelectedTower.Value).Cost;
                            }

                            TowerBase Tower = TowerBuilder.Builder()
                                .Type(towerSelector.SelectedTower.Value)
                                .Map(mapService.CurrentMap)
                                .Friendly(true)
                                .Position(tile)
                                .Build();

                            // Adds the created Tower to the TileEntities on the current map.
                            if (!mapService.CurrentMap.AddTileEntity(Tower)) {
                                ITileEntity tileEntity = mapService.CurrentMap.GetTileEntityAtPosition(TileX, TileY);
                                if (tileEntity is TowerBase) {
                                    towerDataDisplay.SelectedTower = tileEntity as TowerBase;
                                } else {
                                    towerDataDisplay.SelectedTower = null;
                                }
                            } else {
                                towerDataDisplay.SelectedTower = null;
                            }
                        } else if (tileSelector.SelectedTileModifier.HasValue) {
                            //Create the Tile Modifier
                            TileModifierBase TM;
                            switch (tileSelector.SelectedTileModifier.Value) {
                                case TileModiferTypes.LAVA:
                                    TM = new LavaTileModifier(mapService.CurrentMap, tile);
                                    break;
                                case TileModiferTypes.WATER:
                                    TM = new WaterTileModifier(mapService.CurrentMap, tile);
                                    break;
                                case TileModiferTypes.ROCK:
                                    TM = new RockTileModifier(mapService.CurrentMap, tile);
                                    break;
                                default:
                                    return;
                            }

                            if (!(PlayerGold >= TM.Cost)) {
                                MessageToFlash = "Not enough Gold to purchase " + TM.Name + " Troop";
                                return;
                            } else {
                                PlayerGold -= TM.Cost;
                            }

                            mapService.CurrentMap.AddTileEntity(TM);
                        }
                    }

                    // If the active selector is the Troop selector then create a Troop at the clicked position.
                    if (activeSelector is TroopSelection) {
                        // Create a Troop for the pressed position on the map based on the currently selected Troop.
                        TroopBase Troop = TroopBuilder.Builder()
                            .Type(troopSelector.GetSelection())
                            .Map(mapService.CurrentMap)
                            .Position(new Vector2(CurrentMouseState.Position.X - mapService.CurrentMap.XOffset, CurrentMouseState.Position.Y - mapService.CurrentMap.YOffset))
                            .Friendly(true)
                            .Build();

                        if (!(PlayerGold >= Troop.Cost)) {
                            MessageToFlash = "Not enough Gold to purchase " + troopSelector.GetSelection().ToString();
                            return;
                        } else {
                            PlayerGold -= Troop.Cost;
                        }

                        // Deploys the created Troop on the current map.
                        mapService.CurrentMap.DeployTroop(Troop);
                    }
                }
            }

            // If the last time Troops were spawned on this map is more than the rate of spawn, spawn a new set of enemy troops
            // and update the time the troops were last spawned to the current instant.
            if (gameTime.TotalGameTime.TotalMilliseconds > PreviousEnemyTroopSpawn.TotalMilliseconds + EnemyTroopSpawnRate.TotalMilliseconds) {
                PreviousEnemyTroopSpawn = TimeSpan.FromMilliseconds(gameTime.TotalGameTime.TotalMilliseconds);

                TroopBuilder Troop = TroopBuilder.Builder()
                        .Map(mapService.CurrentMap)
                        .Position(new Vector2((mapService.CurrentMap.EnemyTroopSpawnPosition.X - 1) * mapService.CurrentMap.TileWidth, (mapService.CurrentMap.EnemyTroopSpawnPosition.Y - 1) * mapService.CurrentMap.TileHeight))
                        .Friendly(false);

                if (CastleHealth.EnemyCastleHealth >= 8000) {
                    Troop.Type(TroopType.BASIC);
                } else if (CastleHealth.EnemyCastleHealth >= 4000 && CastleHealth.EnemyCastleHealth <= 8000) {
                    if (!(EnemyTroopSpawnRate.TotalMilliseconds == 400)) EnemyTroopSpawnRate = TimeSpan.FromMilliseconds(400);
                    Troop.Type((TroopType) Enum.GetValues(typeof(TroopType)).GetValue(new Random().Next(0, 2)));
                } else if (CastleHealth.EnemyCastleHealth >= 2000 && CastleHealth.EnemyCastleHealth <= 4000) {
                    if (!(EnemyTroopSpawnRate.TotalMilliseconds == 300)) EnemyTroopSpawnRate = TimeSpan.FromMilliseconds(400);
                    Troop.Type((TroopType)Enum.GetValues(typeof(TroopType)).GetValue(new Random().Next(0, 3)));
                } else if (CastleHealth.EnemyCastleHealth >= 2000 && CastleHealth.EnemyCastleHealth <= 4000) {
                    if (!(EnemyTroopSpawnRate.TotalMilliseconds == 250)) EnemyTroopSpawnRate = TimeSpan.FromMilliseconds(400);
                    Troop.Type((TroopType)Enum.GetValues(typeof(TroopType)).GetValue(new Random().Next(1, 3)));
                } else if (CastleHealth.EnemyCastleHealth <= 2000) {
                    if (!(EnemyTroopSpawnRate.TotalMilliseconds == 200)) EnemyTroopSpawnRate = TimeSpan.FromMilliseconds(400);
                    Troop.Type((TroopType)Enum.GetValues(typeof(TroopType)).GetValue(new Random().Next(0, 3)));
                }

                mapService.CurrentMap.DeployTroop(Troop.Build());
            }

            // Set the previous state of the mouse to the state at the end of this cycle.
            this.PreviousMouseState = CurrentMouseState;
        }

        /// <summary>
        /// This is when the Screen should draw its components.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            // If there is no currently loaded map return as there is no game to play.
            if (mapService.CurrentMap == null) {
                return;
            }
            if (GameOverButton != null && GameOver) {
                GameOverButton.Draw(spriteBatch);
                return;
            }

            // Draw the current map.
            mapService.CurrentMap.Draw(spriteBatch);
            // Draw the active selector.
            activeSelector.Draw(spriteBatch);

            towerDataDisplay.Draw(spriteBatch);
            CastleHealth.Draw(spriteBatch);

            // Draw the button to switch to the Troop selector if the Tower selector is active.
            if (activeSelector is TileSelection && TroopSelectorButton != null) {
                TroopSelectorButton.Draw(spriteBatch);
            // Draw the button to switch to the Tower selector if the Troop selector is active.
            } else if (activeSelector is TroopSelection && TowerSelectorButton != null) {
                TowerSelectorButton.Draw(spriteBatch);
            }

            spriteBatch.Begin();

            spriteBatch.DrawString(Siege.INSTANCE.Font_14_Bold, "Gold: " + PlayerGold,
                new Vector2(mapService.CurrentMap.XOffset + 20, (mapService.CurrentMap.Height * mapService.CurrentMap.TileHeight) + mapService.CurrentMap.YOffset),
                Color.Black);
            spriteBatch.DrawString(Siege.INSTANCE.Font_14_Bold, "Diamonds: " + PlayerDiamonds,
                new Vector2(mapService.CurrentMap.XOffset + 110, (mapService.CurrentMap.Height * mapService.CurrentMap.TileHeight) + mapService.CurrentMap.YOffset),
                Color.Black);
            if (!string.IsNullOrEmpty(MessageToFlash)) {
                spriteBatch.DrawString(Siege.INSTANCE.Font_14_Bold, MessageToFlash,
                    new Vector2(mapService.CurrentMap.XOffset + 220, (mapService.CurrentMap.Height * mapService.CurrentMap.TileHeight) + mapService.CurrentMap.YOffset),
                    Color.Black);
            }

            spriteBatch.End();
        }
    }
}
