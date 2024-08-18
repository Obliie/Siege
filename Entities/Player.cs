using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Siege.Map;
using Siege.Map.Tiles;
using Siege.Core;

namespace Siege.Entities {

    /// <summary>
    /// The Game's main Player entity which can be controlled by the user.
    /// </summary>
    public class Player {
        // The map the player is active on.
        public MapBase map;

        // The Position of the Player on the Map.
        public Vector2 Position;

        // The integer X coordinate of the Players position on the map.
        public int X {
            get {
                return (int) Position.X;
            }
        }

        // The integer Y coordinate of the Players position on the map.
        public int Y {
            get {
                return (int) Position.Y;
            }
        }

        // The speed at which the Player travels on the map, in pixels per game update.
        public float BaseSpeed { get; set; }
        // The modifier for the base speed dependant upon map conditions.
        public float SpeedModifier { get; set; }
        // The actual speed accounting for the speed modifier.
        public float Speed { get { return BaseSpeed * SpeedModifier; } }

        // The Bounds of the Player calculated from its position and texture size.
        public Rectangle Bounds {
            get {
                return new Rectangle(X, Y, PlayerTexture.Width, PlayerTexture.Height);
            }
        }

        // The texture of the Player.
        public Texture2D PlayerTexture { get; set; }

        public Player(MapBase map) : this(map, new Vector2(0F, 0F)) { }

        public Player(MapBase map, Vector2 Position) {
            this.map = map;
            this.Position = Position;
            this.BaseSpeed = 2F;
            this.SpeedModifier = 1F;
        }

        /// <summary>
        /// Loads all Content for the player (Its textures and audio).
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public void LoadContent(ContentManager Content) {
            PlayerTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_PLAYER_TEXTURE);
        }

        /// <summary>
        /// Unloads all content for the player (Images and audio).
        /// </summary>
        public void UnloadContent() {
            // Remove the reference to the sprite sheet containing all Player textures.
            PlayerTexture = null;
        }

        /// <summary>
        /// Updates the Player based on the users input or conditions on the map. This is where
        /// movement and collision checks are conducted.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            // Make sure that the player does not go out of bounds, if it is out of bounds
            // set its position to the closest boundry.
            this.Position.X = MathHelper.Clamp(this.Position.X, 0, map.TileWidth * map.Width - PlayerTexture.Width);
            this.Position.Y = MathHelper.Clamp(this.Position.Y, 0, map.TileHeight * map.Height - PlayerTexture.Height);

            #region Calculate Players position on the Tile map
            // Calculate the coordinates of the current Tile the Player is on, priority is given to
            // to the upper left.
            int XOffset = this.X % map.TileWidth;
            int YOffset = this.Y % map.TileHeight;
            // Calculates the offset of the player away from the upper cornor of its current tile.
            int PlayerTileX = ((this.X - XOffset) / map.TileWidth) + 1;
            int PlayerTileY = ((this.Y - YOffset) / map.TileHeight) + 1;
            // If the X offset is more than half the tiles width and half the players width, the
            // user is primarily in the tile to the right so add one to the X.
            if (XOffset > (map.TileWidth / 2) + (PlayerTexture.Width / 2)) {
                PlayerTileX++;
            }
            // If the Y offset is more than half the tiles height and half the players height, the
            // user is primarily in the tile below so add one to the Y.
            if (YOffset > (map.TileHeight / 2) + (PlayerTexture.Height / 2)) {
                PlayerTileY++;
            }
            #endregion

            // Update the Players speed modifier based on its current tile.
            Tile CurrentTile = map.GetRowAtY(PlayerTileY).GetTileAtX(PlayerTileX);
            SpeedModifier = CurrentTile.Properties.SpeedModifier;

            #region Move player based on keyboard state
            // Get the current state of the keyboard.
            KeyboardState keyboardState = Keyboard.GetState();

            // If the left key is pressed move the Players position to the left by the number
            // of pixels determined by the speed.
            if (keyboardState.IsKeyDown(Keys.Left)) {
                this.Position.X -= Speed;
            }

            // If the right key is pressed move the Players position to the right by the number
            // of pixels determined by the speed.
            if (keyboardState.IsKeyDown(Keys.Right)) {
                this.Position.X += Speed;
            }

            // If the up key is pressed move the Players position up by the number
            // of pixels determined by the speed.
            if (keyboardState.IsKeyDown(Keys.Up)) {
                this.Position.Y -= Speed;
            }

            // If the down key is pressed move the Players position down by the number
            // of pixels determined by the speed.
            if (keyboardState.IsKeyDown(Keys.Down)) {
                this.Position.Y += Speed;
            }
            #endregion

            #region Collision checks for new position
            // Check if after movement the Player is colliding with a solid Tile below it. If
            // it is, move the player back in the direction it moved from.
            if (map.GetRowAtY(PlayerTileY + 1) != null) { // Only run if a row below the Player exists.
                Tile lowerTile = map.GetRowAtY(PlayerTileY + 1).GetTileAtX(PlayerTileX);
                if (lowerTile.Properties.Solid && lowerTile.Bounds.Intersects(this.Bounds)) {
                    this.Position.Y -= Speed;
                }
            }

            // Check if after movement the Player is colliding with a solid Tile above it. If
            // it is, move the player back in the direction it moved from.
            if (map.GetRowAtY(PlayerTileY - 1) != null) { // Only run if a row above the Player exists.
                Tile upperTile = map.GetRowAtY(PlayerTileY - 1).GetTileAtX(PlayerTileX);
                if (upperTile.Properties.Solid && upperTile.Bounds.Intersects(this.Bounds)) {
                    this.Position.Y += Speed;
                }
            }
            
            // Check if after movement the Player is colliding with a solid Tile right of it. If
            // it is, move the player back in the direction it moved from.
            if (map.GetRowAtY(PlayerTileY).GetTileAtX(PlayerTileX + 1) != null) { // Only run if a Tile to the right exists.
                Tile rightTile = map.GetRowAtY(PlayerTileY).GetTileAtX(PlayerTileX + 1);
                if (rightTile.Properties.Solid && rightTile.Bounds.Intersects(this.Bounds)) {
                    this.Position.X -= Speed;
                }
            }

            // Check if after movement the Player is colliding with a solid Tile left of it. If
            // it is, move the player back in the direction it moved from.
            if (map.GetRowAtY(PlayerTileY).GetTileAtX(PlayerTileX - 1) != null) { // Only run if a Tile to the left exists.
                Tile leftTile = map.GetRowAtY(PlayerTileY).GetTileAtX(PlayerTileX - 1);
                if (leftTile.Properties.Solid && leftTile.Bounds.Intersects(this.Bounds)) {
                    this.Position.X += Speed;
                }
            }
            #endregion
        }

        /// <summary>
        /// This is when the Player should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            if (PlayerTexture != null) {
                spriteBatch.Draw(PlayerTexture,
                    new Rectangle(X + map.XOffset, Y + map.YOffset, PlayerTexture.Width, PlayerTexture.Height),
                    Color.White);
            }

            spriteBatch.End();
        }
    }
}
