using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Siege.Core;
using Siege.Screens.Components;
using Siege.IO;

namespace Siege.Screens {
    public class LoadScreen : IScreen {
        private bool Initialized = false;

        private Button LoadButton;
        private Button DeleteButton;
        private List<Button> LoadSaveButtons = new List<Button>();

        private Texture2D LoadButtonTexture;
        private Texture2D DeleteButtonTexture;

        public string SelectedSave { get; set; }

        private TimeSpan LastSavesUpdate = TimeSpan.FromSeconds(0);

        // The Offsets from which the LoadScreen should be drawn.
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

        public ScreenType GetScreenType() {
            return ScreenType.LOAD_SCREEN;
        }

        public void LoadContent(ContentManager Content) {
            LoadButtonTexture = Content.Load<Texture2D>(SiegeConstants.ContentPaths.LOAD_LOAD_BUTTON_TEXTURE);
            DeleteButtonTexture = Content.Load <Texture2D>(SiegeConstants.ContentPaths.LOAD_DELETE_BUTTON_TEXTURE);
        }

        public void UnloadContent() {
            this.LoadButtonTexture = null;
            this.DeleteButtonTexture = null;
        }

        public void Update(GameTime gameTime) {
            if (!Initialized) {
                LoadButton = new Button(LoadButtonTexture,
                    () => { },
                    () => {
                        if (string.IsNullOrEmpty(SelectedSave)) return;

                        FileLoader FileLoader = new FileLoader(SelectedSave);
                        FileNode Save = FileLoader.Load();
                        Save.GetValue<MapBase>("Map");

                        Siege.INSTANCE.ScreenService.DisplayScreen(ScreenType.GAME_SCREEN);
                    });
                LoadButton.Width = 64;
                LoadButton.Height = 30;
                LoadButton.XOffset = 304;
                LoadButton.YOffset = 360;

                DeleteButton = new Button(DeleteButtonTexture,
                    () => { },
                    () => {
                        if (string.IsNullOrEmpty(SelectedSave)) return;

                        if (File.Exists(SelectedSave)) {
                            File.Delete(SelectedSave);

                            LoadSaveButtons.RemoveAll(Save => (Save as TextButton).Text.Contains(SelectedSave));
                        }
                    });
                DeleteButton.Width = 64;
                DeleteButton.Height = 30;
                DeleteButton.XOffset = 432;
                DeleteButton.YOffset = 360;

                Initialized = true;
            }

            if ((gameTime.TotalGameTime.TotalMilliseconds > LastSavesUpdate.TotalMilliseconds + TimeSpan.FromSeconds(10).TotalMilliseconds) || LastSavesUpdate.TotalMilliseconds == 0) {
                LoadSaveButtons.Clear();
                LastSavesUpdate = TimeSpan.FromMilliseconds(gameTime.TotalGameTime.TotalMilliseconds);

                string[] Saves = Directory.GetFileSystemEntries("Saves/", "*.dat");
                int SaveNumber = 1;
                foreach (string Save in Saves) {
                    TextButton LoadSaveButton = new TextButton(
                        () => { },
                        () => {
                            string ASave = Save;
                            this.SelectedSave = ASave;
                        },
                        Save.Split('/')[1].Split('.')[0], Color.Black, Color.White);
                    LoadSaveButton.XOffset = 304;
                    LoadSaveButton.YOffset = 100 + (35 * SaveNumber);
                    LoadSaveButton.Width = 192;
                    LoadSaveButton.Height = 30;

                    LoadSaveButtons.Add(LoadSaveButton);

                    SaveNumber++;
                }
            }

            LoadSaveButtons.ForEach(Button => Button.Update(gameTime));
            LoadButton.Update(gameTime);
            DeleteButton.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) {
            LoadSaveButtons.ForEach(Button => Button.Draw(spriteBatch));
            if (LoadButton != null) {
                LoadButton.Draw(spriteBatch);
            }
            if (DeleteButton != null) {
                DeleteButton.Draw(spriteBatch);
            }
        }
    }
}
