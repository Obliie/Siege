using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Map;
using Siege.Map.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Siege.Entities;
using Siege.Map.Tiles;
using Siege.Tower;
using Siege.Events;
using Siege.TileModifiers;

namespace Siege.Core {

    /// <summary>
    /// A Map is a collection of Tiles in a two dimensional region. The Map is divided evenly by the specified Tile width and Tile Height.
    /// </summary>
    public abstract class MapBase : IScreenComponent {
        // The collection of TileRows which form this Map sorted with their Y coordinate.
        private Dictionary<int, TileRow> Map = new Dictionary<int, TileRow>();
        // A collection of all TileEntities on this map sorted to the Tile they are placed on.
        private Dictionary<Tile, ITileEntity> TileEntities = new Dictionary<Tile, ITileEntity>();
        // A collection of all Troops on this map.
        private List<TroopBase> Troops = new List<TroopBase>();
        // A collection of TowerTypes which are permitted to be placed on this map.
        private HashSet<TowerType> PermittedTowers = new HashSet<TowerType>();
        // A collection of TroopTypes which are permitted to be placed on this map.
        private HashSet<TroopType> PermittedTroops = new HashSet<TroopType>();

        // The Offsets from which point the map should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        // The Unique ID of this Maps instance
        public Guid UID { get; set; }

        // The Width of a single Tile on this Map.
        public int TileWidth { get; private set; }
        // The Height of a single Tile on this Map.
        public int TileHeight { get; private set; }
        // The Height of the map in Tiles.
        public int Height { get; set; }
        // The Width of the map in Tiles.
        public int Width { get; set; }

        // MonoGame's spritebatch instance.
        public SpriteBatch spriteBatch;
        // The sprite sheet containing textures for all Tiles on this Map.
        public Texture2D TileTextures;
        // The sprite sheet containing textures for all Towers.
        public Texture2D TowerTextures;
        // The sprite sheet containing textures for all Projectiles.
        public Texture2D ProjectileTextures;
        // The sprite sheet containing textures for all Troops.
        public Texture2D TroopTextures;
        private Texture2D CastleTextures;

        // The Player entity on the map.
        public Player player;

        // The location at which Player troops spawn on this map.
        public Vector2 PlayerTroopSpawnPosition;
        // The location at which Enemy troops spawn on this map.
        public Vector2 EnemyTroopSpawnPosition;

        // The positions of the Player and Enemy castles
        public Vector2 PlayerCastlePosition;
        public Vector2 EnemyCastlePosition;

        // An event handler posted when the map is changed in any way.
        public event MapChangeEventHandler MapChange;
        // An event handler posted when either the player or enemy castles take damage
        public event CastleDamageEventHandler CastleDamage;

        public MapBase(int TileWidth, int TileHeight, Vector2 PlayerStartPosition) {
            this.TileWidth = TileWidth;
            this.TileHeight = TileHeight;

            this.XOffset = 0;
            this.YOffset = TileHeight;

            this.player = new Player(this, PlayerStartPosition);
        }

        /// <summary>
        /// Gets the unique MapType identified by the enumeration.
        /// </summary>
        /// <returns>The MapType</returns>
        public abstract MapType GetMapType();

        /// <summary>
        /// Initializes the map, during this the all Tiles are generated and their
        /// properties are set. The properties should be set in the super class but the
        /// Tiles are generated here, in the base class.
        /// </summary>
        public virtual void Initialize() {
            // Reset the current data
            Map.Clear();
            TileEntities.Clear();
            Troops.Clear();
            UID = Guid.NewGuid();
            

            // Populate the map, ensure a TileRow exists for every Y-coordinate and a Tile exists for every X-coordinate in a TileRow.
            for (int y = 1; y <= Height; y++) {
                Map.Add(y, new TileRow(y, TileWidth, TileHeight));

                for (int x = 1; x <= Width; x++) {
                    Tile tile = GetRowAtY(y).CreateTileAtX(x);
                }
            }

            // Set the Player castle to be on the left side of the map.
            GetRowAtY((int)PlayerTroopSpawnPosition.Y).GetTileAtX((int)PlayerTroopSpawnPosition.X).Properties = TilePropertyPresets.CASTLE_ENTERANCE;
            for (int PlayerCastleY = (int)PlayerCastlePosition.Y - 2; PlayerCastleY <= (int)PlayerCastlePosition.Y + 2; PlayerCastleY++) {
                GetRowAtY(PlayerCastleY).GetTileAtX((int)PlayerCastlePosition.X).Properties = TilePropertyPresets.CASTLE;
            }

            // Set the Enemy castle to be on the right side of the map.
            GetRowAtY((int)EnemyTroopSpawnPosition.Y).GetTileAtX((int)EnemyTroopSpawnPosition.X).Properties = TilePropertyPresets.CASTLE_ENTERANCE;
            for (int EnemyCastleY = (int)EnemyCastlePosition.Y - 2; EnemyCastleY <= (int)EnemyCastlePosition.Y + 2; EnemyCastleY++) {
                GetRowAtY(EnemyCastleY).GetTileAtX((int)EnemyCastlePosition.X).Properties = TilePropertyPresets.CASTLE;
            }
        }

        /// <summary>
        /// Gets the TileRow at the specified Y coordinate of this Map.
        /// </summary>
        /// <param name="Y">The Y coordinate of which to retrieve a TileRow from.</param>
        /// <returns>The TileRow if it exists, else null.</returns>
        public TileRow GetRowAtY(int Y) {
            TileRow row;

            Map.TryGetValue(Y, out row);

            return row;
        }

        /// <summary>
        /// Gets the Neighbouring tiles of the provided Tile.
        /// </summary>
        /// <param name="tile">The Tile to get neighbouring Tiles for.</param>
        /// <returns>A set of the provided Tiles neighbours.</returns>
        public HashSet<Tile> GetTileNeighbours(Tile tile) {
            HashSet<Tile> Neighbours = new HashSet<Tile>();
            TileRow UpperRow = GetRowAtY(tile.Y - 1);
            TileRow CurrentRow = GetRowAtY(tile.Y);
            TileRow LowerRow = GetRowAtY(tile.Y + 1);

            // Get the upper Tile if it exists.
            if (UpperRow != null) {
                Tile upperTile = UpperRow.GetTileAtX(tile.X);
                if (upperTile != null) {
                    Neighbours.Add(upperTile);
                }
            }
            // Get the left Tile if it exists.
            if (CurrentRow != null) {
                Tile leftTile = CurrentRow.GetTileAtX(tile.X - 1);
                if (leftTile != null) {
                    Neighbours.Add(leftTile);
                }
            }
            // Get the lower Tile if it exists.
            if (LowerRow != null) {
                Tile lowerTile = LowerRow.GetTileAtX(tile.X);
                if (lowerTile != null) {
                    Neighbours.Add(lowerTile);
                }
            }
            // Get the right Tile if it exists.
            if (CurrentRow != null) {
                Tile rightTile = CurrentRow.GetTileAtX(tile.X + 1);
                if (rightTile != null) {
                    Neighbours.Add(rightTile);
                }
            }

            return Neighbours;
        }

        /// <summary>
        /// Gets the TileEntity at the provided position
        /// </summary>
        /// <param name="TileX">The X coordinate of the Tile</param>
        /// <param name="TileY">The Y coordinaete of the Tile</param>
        /// <returns>The TileEntity if it exists</returns>
        public ITileEntity GetTileEntityAtPosition(int TileX, int TileY) {
            return GetTileEntityAtPosition(GetRowAtY(TileY).GetTileAtX(TileX));
        }

        /// <summary>
        /// Gets the TileEntity at the provded Tile
        /// </summary>
        /// <param name="tile">The tile</param>
        /// <returns>The TileEntity if it exists</returns>
        public ITileEntity GetTileEntityAtPosition(Tile tile) {
            ITileEntity tileEntity;
            TileEntities.TryGetValue(tile, out tileEntity);

            return tileEntity;
        }
        /// <summary>
        /// Adds a TileEntity to the map if one doesn't exist in the position it is
        /// requesting to be placed in.
        /// </summary>
        /// <param name="tileEntity">The TileEntity which wants to be placed on this map</param>
        public bool AddTileEntity(ITileEntity tileEntity) {
            // Check if a Tile Entity can be placed on the Tile.
            if (!tileEntity.GetTilePosition().Properties.AcceptsTileEntity && !(tileEntity is TileModifierBase)) {
                return false;
            }

            // It can... so place it and run the TileEntites OnAcceptPlacemnt() method, also post a map change event.
            if (!TileEntities.ContainsKey(tileEntity.GetTilePosition())) {
                TileEntities.Add(tileEntity.GetTilePosition(), tileEntity);
                tileEntity.OnAcceptPlacement();

                MapChange(this, new MapChangeEventArgs(tileEntity.GetTilePosition()));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a TileEntity from a specified Tile on the map.
        /// </summary>
        /// <param name="tile">The Tile from which to remove the TileEntity.</param>
        public void RemoveTileEntityOnTile(Tile tile) {
            ITileEntity entity;
            TileEntities.TryGetValue(tile, out entity);

            // Check if the entity first exists before attempting to remove it.
            if (entity != null) {
                entity.Remove();
                TileEntities.Remove(tile);

                // Post a map change event.
                MapChange(this, new MapChangeEventArgs(tile));
            }
        }

        /// <summary>
        /// Gets a Dictionary of all TileEntities on this map.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Tile, ITileEntity> GetTileEntities() {
            return TileEntities;
        }

        /// <summary>
        /// Adds a Troop to this map.
        /// </summary>
        /// <param name="Troop">The Troop to add to the map.</param>
        public void DeployTroop(TroopBase Troop) {
            if (!Troops.Contains(Troop)) {
                Troops.Add(Troop);
            }
        }

        /// <summary>
        /// Gets a Dictionary of all Troops on this map.
        /// </summary>
        /// <returns></returns>
        public List<TroopBase> GetTroops() {
            return Troops;
        }

        /// <summary>
        /// Adds a TowerType to the set of permitted towers for this map.
        /// </summary>
        /// <param name="towerType">The type of the Tower to permit use for on this map.</param>
        public void AddPermittedTower(TowerType towerType) {
            PermittedTowers.Add(towerType);
        }

        /// <summary>
        /// Removes a TowerType from the set of permitted towers for this map.
        /// </summary>
        /// <param name="towerType">The type of the Tower to remove use for on this map.</param>
        public void RemovePermittedTower(TowerType towerType) {
            PermittedTowers.Remove(towerType);
        }

        /// <summary>
        /// Retrieves a set of the permitted towers for this map.
        /// </summary>
        /// <returns>A clone of the set of permitted towers for this map.</returns>
        public HashSet<TowerType> GetPermittedTowers() {
            return new HashSet<TowerType>(PermittedTowers);
        }

        /// <summary>
        /// Adds a TroopType to the set of permitted troops for this map.
        /// </summary>
        /// <param name="troopType">The type of the Troop to permit use for on this map.</param>
        public void AddPermittedTroop(TroopType troopType) {
            PermittedTroops.Add(troopType);
        }

        /// <summary>
        /// Removes a TroopType from the set of permitted troops for this map.
        /// </summary>
        /// <param name="troopType">The type of the Troop to remove use for on this map.</param>
        public void RemovePermittedTroop(TroopType troopType) {
            PermittedTroops.Remove(troopType);
        }

        /// <summary>
        /// Retrieves a set of the permitted troops for this map.
        /// </summary>
        /// <returns>A clone of the set of permitted troops for this map.</returns>
        public HashSet<TroopType> GetPermittedTroops() {
            return new HashSet<TroopType>(PermittedTroops);
        }

        /// <summary>
        /// Loads all assets for the Map, it will be called once per game at the very start.
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public virtual void LoadContent(ContentManager Content) {
            // Load the sprite sheet for all Tile textures.
            TileTextures = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_TILES_TEXTURES);

            // Load the sprite sheet for all Tower textures.
            TowerTextures = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_TOWER_TEXTURES);

            // Load the sprite sheet for all Projectile textures.
            ProjectileTextures = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_PROJECTILE_TEXTURES);

            // Load the sprite sheet for all Troop textures.
            TroopTextures = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_TROOP_TEXTURES);

            CastleTextures = Content.Load<Texture2D>(SiegeConstants.ContentPaths.GAME_CASTLES_TEXTURES);

            // Load the content for the player.
            player.LoadContent(Content);
        }

        /// <summary>
        /// Unloads all assets for the Map, it will be called once per game at the very end.
        /// </summary>
        public virtual void UnloadContent() {
            // Remove the reference to the sprite sheets containing all Tile, Tower and Projectile textures.
            TileTextures = null;
            TowerTextures = null;
            ProjectileTextures = null;
            TroopTextures = null;
            CastleTextures = null;
        }

        /// <summary>
        /// Allows the current Map to check for any logic. Collisions, input, etc.
        /// </summary>
        public virtual void Update(GameTime gameTime) {
            // Allow the Player to update.
            player.Update(gameTime);

            // Allow all TileEntities on this map to update. Check if they have despawned, if so remove them.
            List<ITileEntity> DespawnedTileEntities = new List<ITileEntity>();
            foreach (ITileEntity TileEntity in TileEntities.Values) {
                TileEntity.Update(gameTime);
                if (TileEntity is TileModifierBase) {
                    if ((TileEntity as TileModifierBase).Despawned) DespawnedTileEntities.Add(TileEntity);
                }
            }
            foreach (ITileEntity DespawnedTileEntity in DespawnedTileEntities) {
                RemoveTileEntityOnTile(DespawnedTileEntity.GetTilePosition());
            }

            // Allow all Troops on this map to update.
            foreach (TroopBase Troop in Troops) {
                Troop.Update(gameTime);
                if (Troop.HasReachedDistination()) {
                    CastleDamage(this, new CastleDamageEventArgs(Troop.Damage, Troop));
                }
            }
            Troops.RemoveAll(Troop => Troop.Dead);
        }

        /// <summary>
        /// This method is called when the Map should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            spriteBatch.Draw(
                this.CastleTextures,
                new Rectangle(XOffset + ((int)PlayerCastlePosition.X * TileWidth) - TileWidth, YOffset + ((int)(PlayerCastlePosition.Y - 2) * TileHeight) - TileHeight, TileWidth, TileHeight * 5),
                new Rectangle(0, 0, TileWidth, TileHeight * 5),
                Color.White);

            spriteBatch.Draw(
                this.CastleTextures,
                new Rectangle(XOffset + ((int)EnemyCastlePosition.X * TileWidth) - TileWidth, YOffset + ((int)(EnemyCastlePosition.Y - 2) * TileHeight) - TileHeight, TileWidth, TileHeight * 5),
                new Rectangle(TileWidth, 0, TileWidth, TileHeight * 5),
                Color.White);

            // Draw all Tiles on the map which have a texture.
            for (int y = 1; y <= Height; y++) {
                for (int x = 1; x <= Width; x++) {
                    Tile tile = GetRowAtY(y).GetTileAtX(x);

                    if (tile != null && tile.Properties.TextureID.HasValue) {
                        spriteBatch.Draw(
                            this.TileTextures,
                            new Rectangle(XOffset + (x * TileWidth) - TileWidth, YOffset + (y * TileHeight) - TileHeight, TileWidth, TileHeight),
                            new Rectangle(tile.Properties.TextureID.Value * TileWidth, 0, TileWidth, TileHeight),
                            Color.White);
                    }
                }
            }

            spriteBatch.End();

            // Draw the TileEntities
            foreach (ITileEntity TileEntity in TileEntities.Values) {
                TileEntity.Draw(spriteBatch);
            }

            // Draw the Troops
            foreach (TroopBase Troop in Troops) {
                Troop.Draw(spriteBatch);
            }

            // Draw the Player
            player.Draw(spriteBatch);
        }

        /// <summary>
        /// Gets the Dictionary containing the Tile map.
        /// </summary>
        public Dictionary<int, TileRow> GetMap() {
            return Map;
        }
    }
}
