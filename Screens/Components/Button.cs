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

namespace Siege.Screens.Components {

    // A delegate representing a method which is called when an action related to the
    // button occurs.
    public delegate void ButtonAction();

    /// <summary>
    /// A Button is a range of pixels on the map in which actions are executed if the range is clicked
    /// or hovred on.
    /// </summary>
    public class Button : IScreenComponent {
        // The state of the Mouse in the previous update cycle.
        public MouseState PreviousState;

        // The Texture of the Button.
        public Texture2D Texture;

        // If the Button is currently being hovered on.
        public bool IsHoveredOn = false;
        public Tooltip ButtonTooltip { get; private set; }

        // Delegates which are invoked when the mouse button is clicked and released.
        public ButtonAction OnDown;
        public ButtonAction OnRelease;

        // The Offsets from which the Button should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        // The Width of this Button.
        public int Width { get; set; }
        // The Height of this Button.
        public int Height { get; set; }

        public Button(Texture2D Texture, ButtonAction OnDown, ButtonAction OnRelease)
            : this(Texture, OnDown, OnRelease, null) { }

        public Button(Texture2D Texture, ButtonAction OnDown, ButtonAction OnRelease, Tooltip tooltip) {
            this.Texture = Texture;

            if (Texture != null) {
                this.Width = Texture.Width;
                this.Height = Texture.Height;
            } else {
                this.Width = 1;
                this.Height = 1;
            }

            this.OnDown = OnDown;
            this.OnRelease = OnRelease;

            this.ButtonTooltip = tooltip;
        }

        /// <summary>
        /// Loads all assets for the Button, it will be called once per game at the very start.
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public void LoadContent(ContentManager Content) { }

        /// <summary>
        /// Unloads all assets for the Button, it will be called once per game at the very end.
        /// </summary>
        public void UnloadContent() {
            // Removes the reference to the buttons texture.
            this.Texture = null;
        }

        /// <summary>
        /// Allows the current Button to check for any logic. Clicks, hovers, etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime) {
            MouseState state = Mouse.GetState();

            // Checks if the mouses position is within the Buttons range of pixels.
            if (state.Position.X > XOffset && state.Position.X < XOffset + Width &&
                state.Position.Y > YOffset && state.Position.Y < YOffset + Height) {
                // If the user is not clicking any buttons set the hover flag to true allowing any
                // logic to be executed.
                if (state.LeftButton != ButtonState.Pressed && state.RightButton != ButtonState.Pressed) {
                    IsHoveredOn = true;
                }

                // If the left button is pressed invoke the OnDown delegate.
                if (state.LeftButton == ButtonState.Pressed) {
                    OnDown();
                }

                // If the left button is released and it was previously pressed invoke the OnRelease delegate.
                if (state.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed) {
                    OnRelease();
                }
            } else {
                // If the mouse is not within this Buttons range of pixels set the hover flag to false.
                IsHoveredOn = false;
            }

            // Execute any logic related to hovering on the button.
            if (IsHoveredOn) {
                //TODO: Tooltip thangs
            }
            
            // Set the previous state of the mouse to the state at the end of this cycle.
            PreviousState = state;
        }

        /// <summary>
        /// This method is called when the Button should draw itself.
        /// </summary>
        /// <param name="spriteBatch">The games SpriteBatch instance.</param>
        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Begin();

            spriteBatch.Draw(
                Texture,
                new Rectangle(XOffset, YOffset, Width, Height),
                Color.White);

            spriteBatch.End();
        }

        public void DrawTooltip(SpriteBatch spriteBatch) {
            // If the button is being hovered on and it has a Tooltip, draw it.
            if (IsHoveredOn && ButtonTooltip != null) {
                ButtonTooltip.Draw(spriteBatch);
            }
        }
    }
}
