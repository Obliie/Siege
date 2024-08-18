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
using Siege.Events;

namespace Siege.Entities {

    /// <summary>
    /// A simple Projectile which can follow a velocity.
    /// </summary>
    public class Projectile {
        // The map on which this Projectile is travelling.
        private MapBase Map;

        // The TextureID of this projectile.
        private int TextureID;

        // The Position of the Projectile.
        public Vector2 Position { get; set; }
        // The Speed at which the Projectile travels.
        public float Speed { get; set; }
        // The Direction in which the Projectile travels.
        public Vector2 Direction { get; set; }
        // The Projectiles Velocity calculated from its speed and direction.
        public Vector2 Velocity {
            get {
                return new Vector2(Direction.X * Speed, Direction.Y * Speed);
            }
        }

        // The integer X coordinate of the Projectiles position on the map.
        public int X {
            get {
                return (int)Position.X;
            }
        }
        // The integer Y coordinate of the Projectiles position on the map.
        public int Y {
            get {
                return (int)Position.Y;
            }
        }
        // The Bounds of the Projectiles calculated from its position and texture size.
        public Rectangle Bounds {
            get {
                return new Rectangle(X, Y, 8, 8);
            }
        }

        // The visual offsets of this projectile based on the maps offset.
        public int XOffset { get { return Map.XOffset; } }
        public int YOffset { get { return Map.YOffset; } }

        // If this projectile was fired from a friendly Tower.
        public bool Friendly { get; set; }

        // The damage of the Projectile when it detonates.
        public float Damage { get; set; }
        // The radius to which the damage is applied
        public float DamageRadius { get; set; }
        // The Projectiles potential Hitbox if it collided at this instant.
        public Rectangle DamageRadiusBounds {
            get {
                return new Rectangle(
                    X - ((int)Math.Floor(Map.TileWidth * DamageRadius) / 2), 
                    Y - ((int)Math.Floor(Map.TileHeight * DamageRadius) / 2), 
                    (int)Math.Floor(Map.TileWidth * DamageRadius), 
                    (int)Math.Floor(Map.TileHeight * DamageRadius));
            }
        }

        // Possible targets the Projectile may damage if it collided at this instant.
        private List<TroopBase> PossibleTargets = new List<TroopBase>();

        // The time at which the Projectile was created.
        public TimeSpan CreationTime { get; set; }
        // The amount of time the Projectile should live until it despawns.
        public TimeSpan Lifetime { get; set; }
        // If the Projectile has despawned due to death or exceeding its lifetime.
        public bool Despawned = false;

        // The Action the Projectile applies to all Troops in its damage radius when it collides.
        public Action<List<TroopBase>> HitAction { get; set; }

        public Projectile(int TextureID, MapBase Map, bool Friendly, Vector2 StartingPosition, Vector2 StartingDirection, float Damage, float DamageRadius, float Speed, TimeSpan Lifetime, Action<List<TroopBase>> HitAction = null) {
            this.TextureID = TextureID;
            this.Map = Map;
            this.Friendly = Friendly;
            this.Position = StartingPosition;
            this.Direction = StartingDirection;
            this.Damage = Damage;
            this.DamageRadius = DamageRadius;
            this.Speed = Speed;
            this.Lifetime = Lifetime;
            this.HitAction = HitAction;
        }

        /// <summary>
        /// Allows the current Projectile to check for any logic. Collisions, input, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            if (Despawned) return;
            // If the creation time is not set assume the Projectile was just created and set its creation time to the current instant.
            if (CreationTime.TotalMilliseconds == 0) {
                CreationTime = TimeSpan.FromMilliseconds(gameTime.TotalGameTime.TotalMilliseconds);
            }

            // Update the Projectiles position based on its velocity.
            Position += Velocity;

            // Clear the possible targets it may have as it has moved.
            PossibleTargets.Clear();
            // Iterate through all Troops on the map.
            foreach (TroopBase Troop in Map.GetTroops()) {
                // Only proceed if the Projectile and Troop have opposite 'Friendly' values, otherwise
                // the enemy or user would damage themselves.
                if (!(this.Friendly ^ Troop.Friendly)) {
                    continue;
                }

                // If the Projectiles damage radius is more than 0, add the Troop to the PossibleTargets list if
                // it collides with its potential HitBox.
                if (DamageRadius > 0 && this.DamageRadiusBounds.Intersects(Troop.Bounds)) {
                    PossibleTargets.Add(Troop);
                }

                // If this Projectiles HitBox doesen't intersect the Troops move onto the next iteration.
                if (!this.Bounds.Intersects(Troop.Bounds)) {
                    continue;
                }

                // The Projectile has collided with a Troop, damage it and all other targets in its radius.
                Troop.Health -= this.Damage;
                foreach (TroopBase Target in PossibleTargets) {
                    Target.Health -= this.Damage;
                }

                // If the Projectile has a HitAction, apply it to the hit Troops.
                if (HitAction != null) {
                    if (DamageRadius <= 0) PossibleTargets.Add(Troop);
                    HitAction.Invoke(PossibleTargets);
                }

                // As the Projectile has collided, despawn it.
                this.Despawned = true;
                break;
            }

            // Check if the projectile has exceeded its lifetime, if it has make it despawn.
            if (gameTime.TotalGameTime.TotalMilliseconds > CreationTime.TotalMilliseconds + Lifetime.TotalMilliseconds) {
                Despawned = true;
            }
        }

        /// <summary>
        /// This method is called when the Projectile should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public void Draw(SpriteBatch spriteBatch) {
            if (Despawned) return;
            spriteBatch.Begin();

            spriteBatch.Draw(Map.ProjectileTextures,
                new Rectangle(Bounds.X + XOffset, Bounds.Y + YOffset, 8, 8),
                new Rectangle((TextureID * 8), 0, 8, 8),
                Color.White);

            spriteBatch.End();
        }
    }
}
