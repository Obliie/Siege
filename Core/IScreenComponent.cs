using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Siege.Core {

    /// <summary>
    /// A single Component which can be drawn and updated on a Screen.
    /// </summary>
    public interface IScreenComponent {
        // The X Offset from which to draw this component
        int XOffset { get; set; }
        // The Y Offset from which to draw this component
        int YOffset { get; set; }

        /// <summary>
        /// Loads all assets for the screen component, it will be called once per game at the very start.
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        void LoadContent(ContentManager Content);

        /// <summary>
        /// Unloads all assets for the screen component, it will be called once per game at the very end.
        /// </summary>
        void UnloadContent();

        /// <summary>
        /// Allows the current screen component to check for any logic. Collisions, input, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// This method is called when the screen component should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        void Draw(SpriteBatch spriteBatch);
    }
}
