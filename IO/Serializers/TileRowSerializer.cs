using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Map.Tiles;

namespace Siege.IO.Serializers {
    public class TileRowSerializer : Serializable<TileRow> {
        public FileNode Serialize(TileRow obj) {
            FileNode Node = new FileNode();

            Node.SetValue<int>("Y", obj.Y);
            Node.SetValue<int>("TileWidth", obj.TileWidth);
            Node.SetValue<int>("TileHeight", obj.TileHeight);

            Node.SetDictionary<int, Tile>("Tiles", obj.Row);

            return Node;
        }

        public TileRow Deserialize(FileNode node) {
            int Y = node.GetValue<int>("Y");
            int TileWidth = node.GetValue<int>("TileWidth");
            int TileHeight = node.GetValue<int>("TileHeight");

            TileRow Row = new TileRow(Y, TileWidth, TileHeight);
            Row.Row = node.GetDictionary<int, Tile>("Tiles");

            return Row;
        }
    }
}
