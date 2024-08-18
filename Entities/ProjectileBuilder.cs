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

namespace Siege.Entities {

    /// <summary>
    /// A Builder for Projectiles allowing them to be easily created in batches if they
    /// possess similar properties.
    /// </summary>
    public class ProjectileBuilder : IBuilder<Projectile> {
        // The map this Projectile is travelling in.
        private MapBase map;
        // If the Projectile was fired by a friendly Tower.
        private bool friendly;
        // The Projectiles texture.
        private int ProjectileTextureID;
        // The starting Position of the Projectile.
        private Vector2 StartingPosition;
        // The direction in which the Projectile is travelling.
        private Vector2 StartingDirection;
        // The damage which the Projectile inflicts upon collision.
        private float? ProjectileDamage;
        // The radius to which the Projectiles damage is applied.
        private float? ProjectileDamageRadius;
        // The speed at which the Projectile travels.
        private float? ProjectileSpeed;
        // The TimeSpan representing how long this Projectile lives before it despawns.
        private TimeSpan Lifetime;
        // The optional Action applied to all Troops in the damage radius when the Projectile collides/
        private Action<List<TroopBase>> ProjectileHitAction = null;

        /// <summary>
        /// Creates an instance of this Builder.
        /// </summary>
        /// <returns>A new instance of this Builder.</returns>
        public static ProjectileBuilder Builder() {
            return new ProjectileBuilder();
        }

        /// <summary>
        /// Sets the Map of the Projectile to build.
        /// </summary>
        /// <param name="map">The Projectiles map.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder Map(MapBase map) {
            this.map = map;

            return this;
        }

        /// <summary>
        /// Sets if this Projectile was fired from a friendly Tower.
        /// </summary>
        /// <param name="friendly">If the Projectile is friendly.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder Friendly(bool friendly) {
            this.friendly = friendly;

            return this;
        }

        /// <summary>
        /// Sets the Texture of the Projectile to build.
        /// </summary>
        /// <param name="ProjectileTexture">The Projectiles texture.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder TextureID(int ProjectileTextureID) {
            this.ProjectileTextureID = ProjectileTextureID;

            return this;
        }

        /// <summary>
        /// Sets the position of the Projectile to build.
        /// </summary>
        /// <param name="StartingPosition">The Projectiles position.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder Position(Vector2 StartingPosition) {
            this.StartingPosition = StartingPosition;

            return this;
        }

        /// <summary>
        /// Sets the direction of the Projectile to build.
        /// </summary>
        /// <param name="StartingDirection">The Projectiles direction.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder Direction(Vector2 StartingDirection) {
            this.StartingDirection = StartingDirection;

            return this;
        }

        /// <summary>
        /// Sets the damage of the Projectile to build.
        /// </summary>
        /// <param name="ProjectileDamage">The Projectiles damage.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder Damage(float ProjectileDamage) {
            this.ProjectileDamage = ProjectileDamage;

            return this;
        }

        /// <summary>
        /// Sets the radius of damage of the Projectile to build.
        /// </summary>
        /// <param name="ProjectileDamageRadius">The Projectiles damage radius.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder DamageRadius(float ProjectileDamageRadius) {
            this.ProjectileDamageRadius = ProjectileDamageRadius;

            return this;
        }

        /// <summary>
        /// Sets the speed at which the Projectile to build should travel.
        /// </summary>
        /// <param name="ProjectileSpeed">The Projectiles speed.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder Speed(float ProjectileSpeed) {
            this.ProjectileSpeed = ProjectileSpeed;

            return this;
        }

        /// <summary>
        /// Sets the lifetime of the Projectile to build.
        /// </summary>
        /// <param name="Lifetime">The Projectiles lifetime.</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder LifeTime(TimeSpan Lifetime) {
            this.Lifetime = Lifetime;

            return this;
        }

        /// <summary>
        /// Sets the HitAction of the Projectile to build.
        /// </summary>
        /// <param name="ProjectileHitAction">The Projectiles HitAction</param>
        /// <returns>This builder.</returns>
        public ProjectileBuilder HitAction(Action<List<TroopBase>> ProjectileHitAction) {
            this.ProjectileHitAction = ProjectileHitAction;

            return this;
        }

        /// <summary>
        /// Validates the data provided to the Builder.
        /// </summary>
        /// <returns>The Builder but if data is missing a BuilderExceptions.MissingData exception will be thrown.</returns>
        public IBuilder<Projectile> Validate() {
            if (map == null || ProjectileTextureID == null || StartingPosition == null || StartingDirection == null || !ProjectileDamage.HasValue || !ProjectileDamageRadius.HasValue || !ProjectileSpeed.HasValue || Lifetime == null) {
                throw new BuilderExceptions.MissingData();
            }

            return this;
        }

        /// <summary>
        /// Constructs the Projectile based on the data provided to the Builder.
        /// </summary>
        /// <returns>The newly constructed Projectile.</returns>
        public Projectile Build() {
            Validate();

            return new Projectile(ProjectileTextureID, map, friendly, StartingPosition, StartingDirection, ProjectileDamage.Value, ProjectileDamageRadius.Value, ProjectileSpeed.Value, Lifetime, ProjectileHitAction);
        }
    }
}
