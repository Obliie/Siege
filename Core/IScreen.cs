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
    /// A screen is a certain unique visible page to a user.
    /// </summary>
    public interface IScreen : IScreenComponent {

        /// <summary>
        /// The window width and hight when the user is viewing this Screen.
        /// </summary>
        int WindowWidth { get; }
        int WindowHeight { get; }

        /// <summary>
        /// Gets the unique ScreenType identified by the enumeration.
        /// </summary>
        /// <returns>The ScreenType</returns>
        ScreenType GetScreenType();
    }
}
