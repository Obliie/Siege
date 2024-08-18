using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Siege.Entities;
using Siege.Map.Maps;

namespace Siege.Map {

    /// <summary>
    /// The MapService which manages the currently active map and allows it to be
    /// updated and drawn.
    /// </summary>
    public class MapService {
        // A collection of all Maps with a key of their unique MapType.
        public Dictionary<MapType, MapBase> Maps = new Dictionary<MapType, MapBase>();

        // The current Map.
        public MapBase CurrentMap;

        // The Width of a single Tile on this Map.
        public int TileWidth { get; private set; }

        // The Height of a single Tile on this Map.
        public int TileHeight { get; private set; }

        public MapService(int TileWidth, int TileHeight) {
            this.TileWidth = TileWidth;
            this.TileHeight = TileHeight;

            // Add all Maps to the collection tracking them.
            Maps.Add(MapType.HELL, new Hell(TileWidth, TileHeight));
        }

        /// <summary>
        /// Loads the map of the provided type.
        /// </summary>
        /// <param name="mapType">The MapType of the map to load.</param>
        public MapBase LoadMap(MapType mapType) {
            Maps.TryGetValue(mapType, out CurrentMap);

            // Initialize the map.
            CurrentMap.Initialize();
            return CurrentMap;
        }

        /// <summary>
        /// Loads content for all Maps. This needs to be done as content is only loaded at the start of the game.
        /// </summary>
        /// <param name="Content">MonoGame's content manager</param>
        public void LoadAllContent(ContentManager Content) {
            foreach (MapBase map in Maps.Values) {
                map.LoadContent(Content);
            }
        }

        /// <summary>
        /// Unloads content for all Maps. This needs to be done as contentis only unloaded at the end of the game.
        /// </summary>
        public void UnloadAllContent() {
            foreach (MapBase map in Maps.Values) {
                map.UnloadContent();
            }
        }
    }
}
