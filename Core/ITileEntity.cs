using System.Text;
using System.Threading.Tasks;
using Siege.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Siege.Map.Tiles;

namespace Siege.Core {

    /// <summary>
    /// A Entity which can be placed on a Tile which needs to be updated.
    /// </summary>
    public interface ITileEntity {

        /// <summary>
        /// Gets the Tile on which this TileEntity is placed.
        /// </summary>
        Tile GetTilePosition();

        /// <summary>
        /// Runs any logic which should be executed when this TileEntity is successfully placed.
        /// </summary>
        void OnAcceptPlacement();

        /// <summary>
        /// Run any logic which should be executed when this TileEntity is removed.
        /// </summary>
        void Remove();

        /// <summary>
        /// Allows the current TileEntity to check for any logic. Collisions, input, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// This method is called when the TileEntity should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        void Draw(SpriteBatch spriteBatch);
    }
}
