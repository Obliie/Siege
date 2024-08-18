using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Siege.Screens.Components;
using Siege.Util;

namespace Siege.Screens {
    public class PlayScreen : IScreen {
        private bool Initialized = false;

        private List<Button> MapButtons = new List<Button>();
        private Button LoadButton;

        private Texture2D LoadButtonTexture;

        // The Offsets from which the PlayScreen should be drawn.
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int WindowWidth {
            get {
                return 800;
            }
        }

        public int WindowHeight {
            get {
                return 480;
            }
        }

        public int ButtonsPerRow { 
            get { 
                return 4; 
            } 
        }

        public ScreenType GetScreenType() {
            return ScreenType.PLAY_SCREEN;
        }

        public void LoadContent(ContentManager Content) {
            LoadButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.PLAY_LOAD_BUTTTON_TEXTURE);
        }

        public void UnloadContent() {
            this.LoadButtonTexture = null;
        }

        public void Update(GameTime gameTime) {
            if (!Initialized) {
                int TotalButtons = Siege.INSTANCE.MapService.Maps.Count;
                int Rows = (int) Math.Ceiling((decimal)(TotalButtons / ButtonsPerRow));
                Siege.INSTANCE.MapService.Maps.Values.ToList().ForEach(Map => {
                    Texture2D Rect = TextureHelper.GenerateRectangleTexture(Color.Blue);
                    TextButton Button = new TextButton(
                        () => { },
                        () => {
                            Siege.INSTANCE.MapService.LoadMap(Map.GetMapType());
                            Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.GAME_SCREEN);
                        }, Map.GetMapType().ToString(), Color.Red, Color.Yellow);
                    Button.Width = 128;
                    Button.Height = 50;
                    Button.XOffset = 80 + (MapButtons.Count * (Button.Width + 10));
                    Button.YOffset = 120 + ((int)Math.Ceiling((decimal)(MapButtons.Count / ButtonsPerRow)) * (Button.Height + 20));

                    MapButtons.Add(Button);
                });

                LoadButton = new Button(LoadButtonTexture,
                    () => { },
                    () => {
                        Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.LOAD_SCREEN);
                    });
                LoadButton.Width = 128;
                LoadButton.Height = 50;
                LoadButton.XOffset = 336;
                LoadButton.YOffset = 360;

                Initialized = true;
            }

            MapButtons.ForEach(Button => Button.Update(gameTime));
            LoadButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) {
            MapButtons.ForEach(Button => Button.Draw(spriteBatch));
            if (LoadButton != null) {
                LoadButton.Draw(spriteBatch);
            }
        }
    }
}
