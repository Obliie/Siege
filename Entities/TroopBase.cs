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
using Siege.Events;
using Siege.Threding.Tasks;

namespace Siege.Entities {

    /// <summary>
    /// An AI controlled troop.
    /// </summary>
    public abstract class TroopBase {
        // The Map the Troop is active on.
        public MapBase map;
        // The Position of the Troop on the Map.
        public Vector2 Position;
        // The integer X coordinate of the Troops position on the map.
        public int X {
            get {
                return (int) Position.X;
            }
        }
        // The integer Y coordinate of the Troops position on the map.
        public int Y {
            get {
                return (int) Position.Y;
            }
        }
        // The X and Y coordinates of the Tile the Troop is currently travelling on.
        public int TileX { get; set; }
        public int TileY { get; set; }
        // The Bounds of the Troop calculated from its width and height.
        public Rectangle Bounds {
            get {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        // The position this Troop is attemptng to reach.
        public Vector2 TargetPosition;
        // A flag to store if this is the Players troop or an enemy troop.
        public bool Friendly;

        // The cost in Gold of deploying this Troop.
        public int Cost { get; set; }
        
        // The Width of this Troop.
        public int Width { get; set; }
        // The Height of this Troop.
        public int Height { get; set; }

        // The texture ID of the Troop.
        public int? TextureID { get; private set; }

        // The base speed at which the Troop travels on the map, in pixels per game update.
        public float BaseSpeed { get; set; }
        // The modifier for the base speed dependant upon map conditions.
        public float TileSpeedModifier { get; set; }
        // The modifier for the base speed from other sources.
        public float SpeedModifier { get; set; }
        // The actual speed accounting for the speed modifiers.
        public float Speed { get { return BaseSpeed * TileSpeedModifier * SpeedModifier; } }
        // The Direction in which the Troop travels.
        public Vector2 TDirection { get; set; }
        // The Troops Velocity calculated from its speed and direction.
        public Vector2 Velocity {
            get {
                return new Vector2(TDirection.X * Speed, TDirection.Y * Speed);
            }
        }
        // The PathfindingTask which handles updating this Troops direction to follow a path to its TargetPosition
        public PathfindingTask Pathfinder;

        // The Troops maximum health
        public int MaxHealth;
        // The Troops current health
        public float Health { get; set; }
        // If the Troop is dead (No health)
        public bool Dead {
            get {
                return Health <= 0;
            }
        }

        // The damage this Troop deals to a castle.
        public int Damage { get; set; }

        public TroopBase(MapBase map, int? TextureID, float Speed, bool Friendly, int Cost, int MaxHealth, int Damage, float? Health) 
            : this(map, TextureID, Speed, Friendly, Cost, MaxHealth, new Vector2(0F, 0F), Damage, Health) { }

        public TroopBase(MapBase map, int? TextureID, float Speed, bool Friendly, int Cost, int MaxHealth, Vector2 Position, int Damage, float? Health = null) {
            this.map = map;
            this.TextureID = TextureID;
            this.BaseSpeed = Speed / 2;
            this.TileSpeedModifier = 1;
            this.SpeedModifier = 1;
            this.Friendly = Friendly;
            this.Cost = Cost;
            this.MaxHealth = MaxHealth;
            if (Health.HasValue) {
                this.Health = Health.Value;
            } else {
                this.Health = MaxHealth;
            }
            this.Position = Position;
            this.Damage = Damage;

            Height = 16;
            Width = 16;

            if (map == null) return;
            if (Friendly) {
                TargetPosition = new Vector2(map.EnemyTroopSpawnPosition.X, map.EnemyTroopSpawnPosition.Y);
            } else {
                TargetPosition = new Vector2(map.PlayerTroopSpawnPosition.X, map.PlayerTroopSpawnPosition.Y);
            }

            // Set the OnMapChange method to listen for any map changes.
            map.MapChange += OnMapChange;
        }

        /// <summary>
        /// Gets the unique TroopType of an implementation of this base.
        /// </summary>
        /// <returns>The Troops's TroopType</returns>
        public abstract TroopType GetTroopType();

        /// <summary>
        /// Updates the Troop based on conditions on the map. This is where
        /// movement and collision checks are conducted.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            // Make sure that the troop does not go out of bounds, if it is out of bounds
            // set its position to the closest boundry.
            this.Position.X = MathHelper.Clamp(this.Position.X, 0, map.TileWidth * map.Width - Width);
            this.Position.Y = MathHelper.Clamp(this.Position.Y, 0, map.TileHeight * map.Height - Height);

            if (Dead) return;

            #region Calculate the Troops position on the Tile map
            // Calculate the coordinates of the current Tile the Troop is on, priority is given to
            // to the upper left.
            int XOffset = this.X % map.TileWidth;
            int YOffset = this.Y % map.TileHeight;
            // Calculates the offset of the Troop away from the upper cornor of its current tile.
            int TroopTileX = ((this.X - XOffset) / map.TileWidth) + 1;
            int TroopTileY = ((this.Y - YOffset) / map.TileHeight) + 1;
            // If the X offset is more than half the tiles width and half the Troop width, the
            // user is primarily in the tile to the right so add one to the X.
            if (XOffset > (map.TileWidth / 2) + (Width / 2)) {
                TroopTileX++;
            }
            // If the Y offset is more than half the tiles height and half the Troop height, the
            // user is primarily in the tile below so add one to the Y.
            if (YOffset > (map.TileHeight / 2) + (Height / 2)) {
                TroopTileY++;
            }
            this.TileX = TroopTileX;
            this.TileY = TroopTileY;
            #endregion

            #region Move the Troop based on its Velocity and update its Velocity for the next update.
            if (TroopTileX < 0 || TroopTileY < 0) return;

            // Update the Troops speed modifer based on the Tile its on and apply any damage ticks from the Tile
            // it is standing on.
            Tile CurrentTile = map.GetRowAtY(TroopTileY).GetTileAtX(TroopTileX);
            TileSpeedModifier = CurrentTile.Properties.SpeedModifier;
            Health -= CurrentTile.Properties.DamageTick;

            // If the pathfinder doesent exist create it and update the Troop with a route to its TargetPosition.
            if (Pathfinder == null) {
                List<Tile> Path = PathFinder.FindRoute(map, new Vector2(TroopTileX, TroopTileY), TargetPosition);
                Pathfinder = new PathfindingTask(this, Path);
            }

            // Whilst the Troop hasn't reached its TargetPosition run the Pathfinder, if it has reached it then set
            // it to have no direction.
            if (!HasReachedDistination()) {
                Pathfinder.Run();
            } else {
                this.TDirection = Direction.NONE;
            }

            // Move the Troop by its velocity
            Position += Velocity;

            #endregion

            #region Collision checks and velocity updates for new position
            // Check if after movement the Troop is colliding with a solid Tile below it. If
            // it is, move the Troop back in the direction it moved from.
            if (map.GetRowAtY(TroopTileY + 1) != null) { // Only run if a row below the Troop exists.
                Tile lowerTile = map.GetRowAtY(TroopTileY + 1).GetTileAtX(TroopTileX);
                if (lowerTile != null) {
                    if (lowerTile.Properties.Solid && lowerTile.Bounds.Intersects(this.Bounds)) {
                        this.Position.Y -= Speed;
                    }
                }
            }

            // Check if after movement the Troop is colliding with a solid Tile above it. If
            // it is, move the Troop back in the direction it moved from.
            if (map.GetRowAtY(TroopTileY - 1) != null) { // Only run if a row above the Troop exists.
                Tile upperTile = map.GetRowAtY(TroopTileY - 1).GetTileAtX(TroopTileX);
                if (upperTile != null) {
                    if (upperTile.Properties.Solid && upperTile.Bounds.Intersects(this.Bounds)) {
                        this.Position.Y += Speed;
                    }
                }
            }

            // Check if after movement the Troop is colliding with a solid Tile right of it. If
            // it is, move the Troop back in the direction it moved from.
            if (map.GetRowAtY(TroopTileY).GetTileAtX(TroopTileX + 1) != null) { // Only run if a Tile to the right exists.
                Tile rightTile = map.GetRowAtY(TroopTileY).GetTileAtX(TroopTileX + 1);
                if (rightTile != null) {
                    if (rightTile.Properties.Solid && rightTile.Bounds.Intersects(this.Bounds)) {
                        this.Position.X -= Speed;
                    }
                }
            }

            // Check if after movement the Troop is colliding with a solid Tile left of it. If
            // it is, move the Troop back in the direction it moved from.
            if (map.GetRowAtY(TroopTileY).GetTileAtX(TroopTileX - 1) != null) { // Only run if a Tile to the left exists.
                Tile leftTile = map.GetRowAtY(TroopTileY).GetTileAtX(TroopTileX - 1);
                if (leftTile != null) {
                    if (leftTile.Properties.Solid && leftTile.Bounds.Intersects(this.Bounds)) {
                        this.Position.X += Speed;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Checks if the Troop has reached its TargetPosition.
        /// </summary>
        /// <returns>True if it has, else False.</returns>
        public bool HasReachedDistination() {
            return (this.TileX == this.TargetPosition.X) && (this.TileY == this.TargetPosition.Y);
        }

        /// <summary>
        /// Runs any logic (Update the pathfinders route) when something changes on the map.
        /// </summary>
        public void OnMapChange(object sender, MapChangeEventArgs args) {
            List<Tile> NewPath = PathFinder.FindRoute(map, new Vector2(TileX, TileY), TargetPosition);
            if (Pathfinder != null) {
                Pathfinder.UpdatePath(NewPath);
            }

            if (Dead) {
                map.MapChange -= OnMapChange;
            }
        }

        /// <summary>
        /// When the Troop is removed, disconnect the event listener.
        /// </summary>
        public void Remove() {
            map.MapChange -= OnMapChange;
        }

        /// <summary>
        /// This is when the Player should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            // If the Troop has a texture, draw the Troop at its position on the map.
            if (TextureID.HasValue) {
                Color TroopColor = Color.Red;
                if (Friendly) {
                    TroopColor = Color.Green;
                }

                spriteBatch.Draw(
                    map.TroopTextures,
                    new Rectangle(map.XOffset + X, map.YOffset + Y, 16, 16),
                    new Rectangle((TextureID.Value * (map.TileWidth / 2)), 0, 16, 16),
                    TroopColor);
            }

            spriteBatch.End();
        }
    }
}
