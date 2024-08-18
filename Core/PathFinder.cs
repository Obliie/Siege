using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Map.Tiles;
using Priority_Queue;
using Microsoft.Xna.Framework;

namespace Siege.Core {
    public class PathFinder {

        /// <summary>
        /// An implementation of the A* algorithm for pathfinding a route between two start and end Vector locations, the Vectors are converted to Tiles.
        /// </summary>
        /// <param name="Map">The map on which the route is for.</param>
        /// <param name="Start">The starting Vector.</param>
        /// <param name="End">The ending Vector.</param>
        /// <returns>An enumerable route from the start position to the end position.</returns>
        public static List<Tile> FindRoute(MapBase Map, Vector2 Start, Vector2 End) {
            if ((Start.X == 0 || Start.Y == 0) && (End.X > 0 && End.Y > 0)) {
                return new List<Tile> { Map.GetRowAtY((int)End.Y).GetTileAtX((int)End.X) };
            }

            return FindRoute(Map, Map.GetRowAtY((int)Start.Y).GetTileAtX((int)Start.X), Map.GetRowAtY((int)End.Y).GetTileAtX((int)End.X));
        }

        /// <summary>
        /// An implementation of the A* algorithm for pathfinding a route between two start and end Tile locations.
        /// </summary>
        /// <param name="Map">The map on which the route is for.</param>
        /// <param name="Start">The starting Tile.</param>
        /// <param name="End">The ending Tile.</param>
        /// <returns>A listed route from the start position to the end position.</returns>
        public static List<Tile> FindRoute(MapBase Map, Tile Start, Tile End) {
            // Create a priority queue and add the starting position to it.
            SimplePriorityQueue<Tile> SearchFrontier = new SimplePriorityQueue<Tile>();
            SearchFrontier.Enqueue(Start, 0);

            // Create a dictionary which stores where a Tile was reached from in order to backtrace a route.
            Dictionary<Tile, Tile> VisitedTilesFrom = new Dictionary<Tile, Tile>();
            VisitedTilesFrom.Add(Start, Start);

            // Create a dictionary which stores the cost of moving to a Tile from the starting position.
            Dictionary<Tile, float> WeightedCostSoFar = new Dictionary<Tile, float>();
            WeightedCostSoFar.Add(Start, 0);

            // Iterate until the search frontier is empty.
            while (SearchFrontier.Count > 0) {
                Tile CurrentTile = SearchFrontier.Dequeue();

                // If the current tile is the tile we aim to reach, end the iteration as we have reached our goal.
                if (CurrentTile == End) {
                    break;
                }

                // Get the neighbours of the dequeue'd tile from the map and iterate through them.
                foreach (Tile tile in Map.GetTileNeighbours(CurrentTile)) {
                    // Calculate the cost to reach the neighbouring tile.
                    float NewWeightedCost = WeightedCostSoFar[CurrentTile] + tile.Properties.EdgeWeight;

                    // If the tile is solid, skip it as it cant be travelled through.
                    if (tile.Properties.Solid && tile != Start && tile != End) {
                        continue;
                    }

                    // If the cost to reach this tile doesnt exist or if the cost is lower, update the cost and route directories and queue the tile.
                    if (!WeightedCostSoFar.ContainsKey(tile) || NewWeightedCost < WeightedCostSoFar[tile]) {
                        WeightedCostSoFar[tile] = NewWeightedCost;

                        // Set the queue priority of the tile to be the cost to reach it, plus the distance between it and the final tile.
                        float QueuePriority = NewWeightedCost + Distance(End, tile);
                        SearchFrontier.Enqueue(tile, QueuePriority);

                        VisitedTilesFrom[tile] = CurrentTile;
                    }
                }
            }

            // Backtrace the most efficient route from the directory.
            List<Tile> Path = new List<Tile>();
            Tile CurrentBacktraceTile = End;
            do {
                // If there is no backtrace tile, there is no possible path, so break out of the loop.
                if (CurrentBacktraceTile == null) {
                    break;
                }

                // Add the tile to the path set and get the tile that it was reached from, set this to the new backtrace tile.
                Path.Add(CurrentBacktraceTile);
                VisitedTilesFrom.TryGetValue(CurrentBacktraceTile, out CurrentBacktraceTile);
            } while (CurrentBacktraceTile != Start); // If the backtrace tile is the start, the route is found so end the iteration.
            Path.Add(Start);

            // Reverse the list as the path was calcualated backwards.
            Path.Reverse();

            // Return the path.
            return Path;
        }

        /// <summary>
        /// Calculates the distance (In number of tiles) between two tiles.
        /// </summary>
        /// <param name="a">The start Tile.</param>
        /// <param name="b">The end Tile.</param>
        /// <returns>The distance between the two Tiles.</returns>
        public static float Distance(Tile a, Tile b) {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
