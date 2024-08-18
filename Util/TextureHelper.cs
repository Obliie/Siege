using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Siege.Util {

    /// <summary>
    /// A Helper class for texture related operations.
    /// </summary>
    public static class TextureHelper {

        /// <summary>
        /// Crops a region from a texture.
        /// </summary>
        /// <param name="OriginalTexture">The original texture to crop from.</param>
        /// <param name="Region">The region on the original texture to crop.</param>
        /// <returns>The newly cropped texture.</returns>
        public static Texture2D Crop(Texture2D OriginalTexture, Rectangle Region) {
            // Create a new Texture with data based on the cropped regions with and height.
            Texture2D CropedTexture = new Texture2D(Siege.INSTANCE.GraphicsDevice, (int)Region.Width, (int)Region.Height);
            Color[] Data = new Color[Region.Width * Region.Height];

            // Set the newly created Textures data to that of the cropped region.
            OriginalTexture.GetData(0, Region, Data, 0, Data.Length);
            CropedTexture.SetData(Data);

            return CropedTexture;
        }

        //will be 1x1 stretch.
        public static Texture2D GenerateRectangleTexture(Color color) {
            Texture2D rect = new Texture2D(Siege.INSTANCE.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            return rect;
        }
    }
}
