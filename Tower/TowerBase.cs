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
using Siege.Map.Tiles;
using Siege.Entities;

namespace Siege.Tower {

    /// <summary>
    /// A Tower is a component which can be placed on a Tile. It provides the user with defences against oncoming entities.
    /// </summary>
    public abstract class TowerBase : ITileEntity {
        // The Tower's status
        private TowerStatus Status = TowerStatus.ACTIVE;

        // The Map the Tower is placed on.
        public MapBase map { get; private set; }
        // The Tower's position
        public Tile Position { get; private set; }

        // The Tower's texture ID
        public int? TextureID { get; private set; }
        // The human readable Tower name
        public string Name { get; set; }
        // Stores if the Tower is friendly (Placed by the user) or enemy.
        public bool Friendly { get; set; }

        // The damage of this Tiles projectiles
        public int Damage { get; set; }
        // The radius to which the Towers projectiles applies its damage
        public int DamageRadius { get; set; }
        // The speed at which the Towers projectiles travel
        public int Speed { get; set; }
        // The lifetime of the Projectile
        public int Lifetime { get; set; }
        // Generates a ProjectileBuilder from the Towers existing properties, the position is set to the center
        // of the Tower.
        public ProjectileBuilder PBuilderBase { 
            get {
                return ProjectileBuilder.Builder()
                .Map(map)
                .Friendly(Friendly)
                .Position(new Vector2(this.Position.X * map.TileWidth - 20, this.Position.Y * map.TileHeight - 20))
                .Damage(Damage)
                .DamageRadius(DamageRadius)
                .Speed(Speed)
                .LifeTime(TimeSpan.FromMilliseconds(Lifetime));
            } 
        }

        // The cost of placing this Tower in Gold
        public int Cost { get; set; }

        // The visual offsets of the Tower, based on the maps offset.
        public int XOffset { get { return map.XOffset; } }
        public int YOffset { get { return map.YOffset; } }

        // The TimeSpan since the last time the Tower fired projectiles.
        private TimeSpan PreviousFire;
        // The TimeSpan between the rate at which projectiles should be fired.
        public TimeSpan RateOfFire { get; set; }
        // A collection of projectiles produced by this Tower.
        protected List<Projectile> Projectiles = new List<Projectile>();

        public TowerBase(MapBase map, Tile Position, TowerProperties towerProperties, bool Friendly) {
            this.map = map;
            this.Position = Position;
            this.TextureID = towerProperties.TextureID;
            this.Name = towerProperties.Name;
            this.Friendly = Friendly;
            this.Cost = towerProperties.Cost;
            this.RateOfFire = towerProperties.RateOfFire;
            this.Damage = towerProperties.Damage;
            this.DamageRadius = towerProperties.DamageRadius;
            this.Speed = towerProperties.Speed;
            this.Lifetime = towerProperties.Lifetime;
        }

        /// <summary>
        /// Gets the unique TowerType of an implementation of this base.
        /// </summary>
        /// <returns>The Tower's TowerType</returns>
        public abstract TowerType GetTowerType();

        /// <summary>
        /// Fires a set of projectiles. This methods call timing is based on the TimeSpan of the RateOfFire variable.
        /// </summary>
        public abstract void FireProjectiles();

        /// <summary>
        /// Updates any other objects when this Tower is successfully placed.
        /// </summary>
        public void OnAcceptPlacement() {
            // Sets the solid property of this Tower's position to true to prevent clipping.
            Position.Properties.Solid = true;
        }

        /// <summary>
        /// Logic that should be done when this Tower is removed.
        /// </summary>
        public void Remove() {
            // Sets the Solid property of the position this Tower was placed on to false so that it can be travelled through again.
            Position.Properties.Solid = false;
        }

        /// <summary>
        /// Gets the Tile position of this Tower.
        /// </summary>
        /// <returns>The Tile this tower is positioned on.</returns>
        public Tile GetTilePosition() {
            return Position;
        }

        /// <summary>
        /// Allows the Tower to check for any logic. Firing projectiles, status, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            // If the Tower is inactive don't update it and return.
            if (Status == TowerStatus.INACTIVE) {
                return;
            }

            // If the last time Projectiles were fired from this tower is more than the rate of fire, fire a new set of projectiles
            // and update the time the projectiles were fired to the current instant.
            if (gameTime.TotalGameTime.TotalMilliseconds > PreviousFire.TotalMilliseconds + RateOfFire.TotalMilliseconds) {
                PreviousFire = TimeSpan.FromMilliseconds(gameTime.TotalGameTime.TotalMilliseconds);
                FireProjectiles();
            }

            // Iterate through each Projectile fired by this Tower and update it.
            foreach (Projectile projectile in Projectiles) {
                projectile.Update(gameTime);

                // If the Projectile is out of the maps bounds, despawn it.
                if (projectile.X > map.TileWidth * map.Width || projectile.Y > map.TileHeight * map.Height || projectile.Y < 0 || projectile.X < 0) {
                    projectile.Despawned = true;
                }
            }
            // Remove all Projectiles that are flagged as despawned.
            Projectiles.RemoveAll(projectile => projectile.Despawned);
        }

        /// <summary>
        /// This method is called when the Tower should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            // If the Tower has a texture, draw the Tower at its position on the map.
            if (TextureID.HasValue) {
                spriteBatch.Draw(
                    map.TowerTextures,
                    new Rectangle(XOffset + (Position.X * map.TileWidth) - map.TileWidth, YOffset + (Position.Y * map.TileHeight) - map.TileHeight, map.TileWidth, map.TileHeight),
                    new Rectangle(TextureID.Value * map.TileWidth, 0, map.TileWidth, map.TileHeight),
                    Color.White);
            }

            spriteBatch.End();

            // Draw all the projectiles which have been fired by this Tower.
            foreach (Projectile projectile in Projectiles) {
                projectile.Draw(spriteBatch);
            }
        }
    }
}
